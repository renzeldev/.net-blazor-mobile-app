using System.ComponentModel.DataAnnotations;

namespace UmfaApp.Models.UmfaApiModels.RequestModels
{
    public class AddDeviceRequest
    {
        [Required]
        public string? DeviceId { get; set; }

        [Required]
        public int? UserId { get; set; }
    }
}
