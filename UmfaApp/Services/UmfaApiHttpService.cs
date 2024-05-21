using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using UmfaApp.Models;
using UmfaApp.Models.UmfaApiModels.RequestModels;
using UmfaApp.Models.UmfaApiModels.ResponseModels;
using UmfaApp.Settings;

namespace UmfaApp.Services
{
    public interface IUmfaService
    {
        public Task<ValidateMobileUserResponse> ValidateMobileUserAsync(ValidateMobileUserRequest request);
        public Task<bool> AddDeviceAsync(AddDeviceRequest request);
        public Task<List<PartnerClientBuildingService>> GetPartnerClientBuildingServicesAsync(PartnerClientBuildingServicesRequest request);
        public Task<List<Partner>> GetPartnersAsync(int userId);
        public Task<List<ReadingListEntry>> GetReadingList(List<int> buildingIds, List<string> locations);
        public Task<List<ReadingListEntry>> GetReadingList(string buildingIds, string locations);

        public Task<(int Error, int RequestId, List<int>? BuildingIds)> UploadReadingsAsync(UploadReadingsRequest request);

        public Task<bool> UploadReadingImageAsync(UploadReadingImageRequest request);
        public Task<bool> UploadReadingAudioAsync(UploadReadingAudioRequest request);

    }
    public class UmfaApiHttpService : HttpService, IUmfaService
    {
        private readonly ILogger<UmfaApiHttpService> _logger;

        public UmfaApiHttpService(IConfiguration config, ILogger<UmfaApiHttpService> logger) : base(config.GetRequiredSection(nameof(UmfaApiSettings)).Get<UmfaApiSettings>().BaseUrl)
        {
            _logger = logger;
        }

        public async Task<bool> AddDeviceAsync(AddDeviceRequest request)
        {
            try
            {
                var response = await PostAsync("devices", request);

                return response is not null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return false;
            }
        }

        public async Task<List<PartnerClientBuildingService>> GetPartnerClientBuildingServicesAsync(PartnerClientBuildingServicesRequest request)
        {
            try
            {
                var response = await GetAsync("meters/meters-locations", request);
                return JsonSerializer.Deserialize<List<PartnerClientBuildingService>>(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return null;
            }
        }

        public async Task<List<Partner>> GetPartnersAsync(int userId)
        {
            try
            {
                var response = await GetAsync("partners", new GetPartnersRequest { UserId = userId});
                return JsonSerializer.Deserialize<List<Partner>>(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return null;
            }
        }

        public async Task<List<ReadingListEntry>> GetReadingList(List<int> buildingIds, List<string> locations)
        {
            try
            {
                var request = new ReadingListRequest(buildingIds, locations);

                var response = await GetAsync("readings/reading-list", request);
                return JsonSerializer.Deserialize<List<ReadingListEntry>>(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return null;
            }
        }

        public async Task<List<ReadingListEntry>> GetReadingList(string buildingIds, string locations)
        {
            try
            {
                var request = new ReadingListRequest(buildingIds, locations);

                var response = await GetAsync("readings/reading-list", request);
                return JsonSerializer.Deserialize<List<ReadingListEntry>>(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return null;
            }
        }

        public async Task<bool> UploadReadingAudioAsync(UploadReadingAudioRequest request)
        {
            try
            {
                await PostAsync("readings/mobile/audio", request);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        public async Task<bool> UploadReadingImageAsync(UploadReadingImageRequest request)
        {
            try
            {
                await PostAsync("readings/mobile/image", request);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        public async Task<(int Error, int RequestId, List<int>? BuildingIds)> UploadReadingsAsync(UploadReadingsRequest request)
        {
            try
            {
                var response = await PostWithResponseAsync("readings/mobile", request);

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (0, JsonSerializer.Deserialize<int>(content), null);
                }

                if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return (-1, 0, JsonSerializer.Deserialize<List<int>>(content));
                }

                return (-2, int.Parse(JsonSerializer.Deserialize<ErrorModel>(content).Detail), null);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return (-3, 0, null);
            }
        }

        public async Task<ValidateMobileUserResponse> ValidateMobileUserAsync(ValidateMobileUserRequest request)
        {
            try
            {
                var response = await PutAsync("users/validate-user", request);
                return JsonSerializer.Deserialize<ValidateMobileUserResponse>(response);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);

                return null;
            }
        }
    }
}
