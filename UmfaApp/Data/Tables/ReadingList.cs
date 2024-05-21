namespace UmfaApp.Data.Tables
{
    public class ReadingList : TableBase
    {
        public string Name { get; set; }
        public int UserId { get; set; }
        public int PartnerId { get; set; }
        public string BuildingIds { get; set; }
        public string Locations { get; set; }
    }
}
