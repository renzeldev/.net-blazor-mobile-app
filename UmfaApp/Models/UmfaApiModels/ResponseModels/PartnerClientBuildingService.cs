namespace UmfaApp.Models.UmfaApiModels.ResponseModels
{
    public class PartnerClientBuildingService
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }
        public string Location { get; set; }
        public int NoOfMeters { get; set; }
    }
}
