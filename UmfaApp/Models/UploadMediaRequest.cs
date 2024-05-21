
using UmfaApp.Data.Tables;

namespace UmfaApp.Models
{
    [Serializable]
    public class UploadMediaRequest
    {
        public int UserId { get; set; }
        public string DeviceId { get; set; }
        public ReadingListRequest Request { get; set; }
    }
}
