using System.ComponentModel.DataAnnotations;

namespace UmfaApp.Models.UmfaApiModels.RequestModels
{
    public class PartnerClientBuildingServicesRequest
    {
        [Required]
        public int? PartnerId { get; set; }

        [Required]
        public int? UserId { get; set; }
    }
}
