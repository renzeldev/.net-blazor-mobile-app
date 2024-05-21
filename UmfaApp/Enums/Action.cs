using System.ComponentModel.DataAnnotations;

namespace UmfaApp.Enums
{
    public enum Action
    {
        [Display(Name = "Upload Media")] UploadMedia,
        [Display(Name = "Upload Image")] UploadImage,
        [Display(Name = "Upload Voice Note")] UploadVoiceNote,
        [Display(Name = "Upload Readings")] UploadReadings,
        [Display(Name = "Get Latest Period")] GetLatestPeriod,
        [Display(Name = "Create Request")]  CreateReadingList,
        [Display(Name = "Delete Image")] DeleteImage,
        [Display(Name = "Delete Reading List Request")] DeleteReadingListRequest,
    }
}
