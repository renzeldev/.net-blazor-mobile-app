using UmfaApp.Enums;

namespace UmfaApp.Data.Tables
{
    [Serializable]
    public class ReadingListRequest : TableBase
    {
        public Guid ReadingListId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string Period { get; set;}

        public bool ReadingsUploaded { get; set; }

        public bool Touched { get; set; } = false;

        public UploadStatus MediaUploadStatus { get; set; } = UploadStatus.Pending;
    }
}
