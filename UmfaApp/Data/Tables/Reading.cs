using SQLite;
using UmfaApp.Models;

namespace UmfaApp.Data.Tables
{
    public class Reading : TableBase
    {
        public Guid ReadingListRequestId { get; set; }
        public long ReadingId { get; set; }
        public int PeriodId { get; set; }
        public string PeriodName { get; set; }
        public string PeriodShort { get; set; }
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }
        public int BuildingServiceId { get; set; }
        public string BuildingServiceName { get; set; }
        public string MeterNo { get; set; }
        public string Tenant { get; set; }
        public string NoteListed { get; set; }
        public string Location { get; set; }
        public int SequenceNo { get; set; }
        public double MultFactor { get; set; }
        public string PreviousReadingdate { get; set; }
        public double PreviousReading { get; set; }
        public string? ReadingDate { get; set; }
        public double AverageUsage { get; set; }
        public double ExpectedReading { get; set; }
        public double? ActualReading { get; set; }
        public double? Usage { get; set; }
        public bool Calculated { get; set; }
        public bool CalcUnits { get; set; }
        public bool RollOver { get; set; }
        public bool PrevIsActual { get; set; }
        public bool CurrIsActual { get; set; }
        public double ReadingOffset { get; set; }
        public int Phases { get; set; }
        public double CBSize { get; set; }

        //  newly added (not from response)
        public bool IsCaptured => ActualReading is not null;

        public string NewNote { get; set; }
        public string Photos { get; set; }

        public string UploadedPhotos { get; set; }
        public string FailedUploadedPhotos { get; set; }

        public double? GpsLat { get; set; }

        public double? GpsLng { get; set; }

        public double? GpsAge { get; set; }

        public bool HasAbnormality { get; set; } = false;

        public string VoiceNote { get; set; }
        public bool VoiceNoteUploaded { get; set; } = false;

        [IgnoreAttribute]
        public List<string>? PhotosList => Photos?.Split("|").ToList();

        public Reading() {}

        public Reading(Guid readingListRequestId, ReadingListEntry entry)
        {
            ReadingListRequestId = readingListRequestId;
            ReadingId = entry.ReadingId;
            PeriodId = entry.PeriodId;
            PeriodName = entry.PeriodName;
            PeriodShort = entry.PeriodShort;
            BuildingId = entry.BuildingId;
            BuildingName = entry.BuildingName;
            BuildingServiceId = entry.BuildingServiceId;
            BuildingServiceName = entry.BuildingServiceName;
            MeterNo = entry.MeterNo;
            Tenant = entry.Tenant;
            NoteListed = entry.NoteListed;
            Location = entry.Location;
            SequenceNo = entry.SequenceNo;
            MultFactor = entry.MultFactor;
            PreviousReadingdate = entry.PreviousReadingdate.ToString("yyyy-MM-dd HH:mm");
            PreviousReading = entry.PreviousReading;
            ReadingDate = entry.ReadingDate.ToString("yyyy-MM-dd HH:mm");
            AverageUsage = entry.AverageUsage;
            ExpectedReading = entry.ExpectedReading;
            ActualReading = entry.ActualReading;
            Usage = entry.Usage;
            Calculated = entry.Calculated;
            CalcUnits = entry.CalcUnits;
            RollOver = entry.RollOver;
            PrevIsActual = entry.PrevIsActual;
            CurrIsActual = entry.CurrIsActual;
            ReadingOffset = entry.ReadingOffset;
            Phases = entry.Phases;
            CBSize = entry.CBSize;
        }
    }
}
