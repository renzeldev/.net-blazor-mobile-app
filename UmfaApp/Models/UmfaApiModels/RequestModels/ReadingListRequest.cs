using Microsoft.Maui.Devices.Sensors;

namespace UmfaApp.Models.UmfaApiModels.RequestModels
{
    public class ReadingListRequest
    {
        public string BuildingIds { get; set; }
        public string Locations { get; set; }

        public ReadingListRequest(List<int> buildingIds, List<string> locations) 
        {
            BuildingIds = string.Join(",", buildingIds);
            Locations = string.Join(",", locations);
        }

        public ReadingListRequest(string buildingIds, string locations)
        {
            BuildingIds = buildingIds;
            Locations = locations;
        }
    }
}
