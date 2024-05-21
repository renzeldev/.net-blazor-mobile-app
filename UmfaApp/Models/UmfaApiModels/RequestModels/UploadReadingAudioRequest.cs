using System.ComponentModel.DataAnnotations;

namespace UmfaApp.Models.UmfaApiModels.RequestModels
{
    public class UploadReadingAudioRequest
    {
        [Required]
        public int? UserID { get; set; }

        [Required]
        public string? DeviceId { get; set; }

        [Required]
        public string? CommsDate { get; set; }

        [Required]
        public ReadingAudio? JSONReadingData { get; set; }
    }

    public class ReadingAudio
    {
        [Required]
        public int? BuildingId { get; set; }

        [Required]
        public int? PeriodId { get; set; }

        [Required]
        public int? BuildingServiceID { get; set; }

        [Required]
        public string AudioDTM { get; set; }

        [Required]
        public byte[] Audio { get; set; }
    }
}
