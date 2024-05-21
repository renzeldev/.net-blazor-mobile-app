using System.ComponentModel.DataAnnotations;

namespace UmfaApp.Models.UmfaApiModels.RequestModels
{
    public class UploadReadingImageRequest
    {
        [Required]
        public int? UserID { get; set; }

        [Required]
        public string? DeviceId { get; set; }

        [Required]
        public string? CommsDate { get; set; }

        [Required]
        public ReadingImage? JSONReadingData { get; set; }
    }

    public class ReadingImage
    {
        [Required]
        public int? BuildingId { get; set; }

        [Required]
        public int? PeriodId { get; set; }

        [Required]
        public int? BuildingServiceID { get; set; }

        [Required]
        public string ImageDTM { get; set; }

        [Required]
        public byte[] Image { get; set; }
    }
}
