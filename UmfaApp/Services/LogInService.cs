using System.Text.Json;
using UmfaApp.Enums;
using UmfaApp.Models;
using UmfaApp.Models.UmfaApiModels.RequestModels;
using UmfaApp.Models.UmfaApiModels.ResponseModels;
using UmfaApp.Settings;

namespace UmfaApp.Services
{
    public interface ILogInService
    {
        public Task<ValidateMobileUserResponse> AuthenticateUserOnline(LoginModel loginRequest);
        public Task<(bool Authenticated, int? UserId)> AuthenticateUserOffline(LoginModel loginRequest);
    }
    public class LogInService : ILogInService
    {
        private readonly IUmfaService _umfaService;
        public LogInService(IUmfaService umfaService) 
        {
            _umfaService = umfaService;
        }

        public async Task<ValidateMobileUserResponse> AuthenticateUserOnline(LoginModel loginRequest)
        {
            var deviceId = await GetOrCreateDeviceId();

            var response = await ValidateOnline(loginRequest, deviceId);

            if(response == null) { return null; }

            // add device if not found
            if(!response.IsValidated && response.NotValidatedReason.Equals(NotValidatedReason.DeviceNotFound))
            {
                var created = await _umfaService.AddDeviceAsync(new AddDeviceRequest { DeviceId = deviceId, UserId = response.UserId });

                if(created)
                {
                    return new ValidateMobileUserResponse { IsValidated = false, NotValidatedReason = NotValidatedReason.DeviceNotApproved};
                }
                else
                {
                    return new ValidateMobileUserResponse { IsValidated = false, NotValidatedReason = NotValidatedReason.DeviceNotFound };
                }
            }

            // create local entry for newly validated user
            if (response.IsValidated)
            {
                await SetUserAuthenticated(loginRequest, (int)response.UserId!);
            }

            return response;
        }

        public async Task<(bool Authenticated, int? UserId)> AuthenticateUserOffline(LoginModel loginRequest)
        {
            var deviceId = await GetOrCreateDeviceId();
            AppSettings.DeviceId = deviceId;
            var userString = await SecureStorage.Default.GetAsync(loginRequest.Email);

            if(string.IsNullOrWhiteSpace(userString))
            {
                return (false, null);
            }

            var user = JsonSerializer.Deserialize<UserStorage>(userString);

            if(user.Password.Equals(loginRequest.Password))
            {
                return (true, user.Id);
            }

            return (false, null);
        }

        private async Task<ValidateMobileUserResponse> ValidateOnline(LoginModel loginRequest, string deviceId)
        {
            var request = new ValidateMobileUserRequest
            {
                DeviceId = deviceId,
                Password = loginRequest.Password,
                Username = loginRequest.Email,
            };

            return await _umfaService.ValidateMobileUserAsync(request);
        }

        private async Task SetUserAuthenticated(LoginModel loginRequest, int userId)
        {
            var user = new UserStorage
            {
                Password = loginRequest.Password,
                Id = userId,
            };

            await SecureStorage.Default.SetAsync(loginRequest.Email, JsonSerializer.Serialize(user));
        }

        private async Task<UserStorage> GetUserStorage(string email)
        {
            var userString = await SecureStorage.Default.GetAsync(email);
            
            if (string.IsNullOrWhiteSpace(userString))
            {
                return null;
            }

            return JsonSerializer.Deserialize<UserStorage>(userString);
        }

        private async Task<string> GetOrCreateDeviceId()
        {
            var deviceId = await SecureStorage.Default.GetAsync("DeviceId");

            if(string.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
                await SecureStorage.Default.SetAsync("DeviceId", deviceId);
            }

            AppSettings.DeviceId = deviceId;
            return deviceId;
        }
    }
}
