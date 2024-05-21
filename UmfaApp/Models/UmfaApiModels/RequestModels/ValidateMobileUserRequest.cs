using System.ComponentModel.DataAnnotations;

namespace UmfaApp.Models.UmfaApiModels.RequestModels
{
    public class ValidateMobileUserRequest
    {
        [Required, MinLength(1)]
        public string Username { get; set; }

        [Required, MinLength(1)]
        public string Password { get; set; }

        [Required, MinLength(1)]
        public string DeviceId { get; set; }
    }
}
