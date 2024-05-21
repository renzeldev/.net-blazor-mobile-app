namespace UmfaApp.Models
{
    public class ReadingListEntry
    {
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
        public DateTime PreviousReadingdate { get; set; }
        public double PreviousReading { get; set; }
        public DateTime ReadingDate { get; set; }
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
    }
}
