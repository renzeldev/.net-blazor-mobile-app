using UmfaApp.Enums;

namespace UmfaApp.Models.UmfaApiModels.ResponseModels
{
    public class ValidateMobileUserResponse
    {
        public bool IsValidated { get; set; }
        public NotValidatedReason? NotValidatedReason { get; set; }
        public int? UserId { get; set; }
    }
}
