using System.ComponentModel.DataAnnotations;
using UmfaApp.Data.Tables;

namespace UmfaApp.Models.UmfaApiModels.RequestModels
{
    public class UploadReadingsRequest
    {
        public int UserID { get; set; }
        public string DeviceId { get; set; }
        public string CommsDate { get; set; }

        public List<ApiReading> Readings { get; set; }

        public UploadReadingsRequest(int userId, string deviceId, List<Reading> readings)
        {
            CommsDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");
            UserID = userId;
            DeviceId = deviceId;
            Readings = readings.Where(r => r.IsCaptured).Select(r => new ApiReading(r)).ToList();
        }
    }

    public class ApiReading
    {
        public int ReadingID { get; set; } = 0;

        [Required]
        public int? BuildingId { get; set; }

        [Required]
        public int? PeriodId { get; set; }

        [Required]
        public int? BuildingServiceID { get; set; }

        [Required]
        public string? PreviousReadingDate { get; set; }

        [Required]
        public decimal? PreviousReading { get; set; }

        [Required]
        public decimal? ExpectedReading { get; set; }

        [Required]
        public decimal? ActualReading { get; set; }

        [Required]
        public decimal? Usage { get; set; }

        [Required]
        public bool? RollOver { get; set; }

        [Required]
        public bool? Calculated { get; set; }

        [Required]
        public bool? Active { get; set; }

        [Required]
        public decimal? OffSetPerc { get; set; }

        [Required]
        public bool? HasAbnormally { get; set; }

        [Required]
        public decimal? ReadingOffSet { get; set; }

        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }

        public string? Notes { get; set; }

        public ApiReading(Reading reading)
        {
            BuildingId = reading.BuildingId;
            PeriodId = reading.PeriodId;
            BuildingServiceID = reading.BuildingServiceId;
            PreviousReadingDate = reading.PreviousReadingdate;
            PreviousReading = (decimal)reading.PreviousReading;
            ExpectedReading = (decimal)reading.ExpectedReading;
            ActualReading = reading.ActualReading != null ? (decimal)reading.ActualReading : null;
            Usage = reading.Usage != null ? (decimal)reading.Usage : null;
            RollOver = reading.RollOver;
            Calculated = reading.Calculated;
            Active = true; 
            OffSetPerc = (decimal)reading.MultFactor;
            HasAbnormally = reading.HasAbnormality;
            ReadingOffSet = (decimal)reading.ReadingOffset;
            Latitude = reading.GpsLat != null ? (decimal)reading.GpsLat : null;
            Longitude = reading.GpsLng != null ? (decimal)reading.GpsLng : null;
            Notes = reading.NewNote;
        }
    }
}
