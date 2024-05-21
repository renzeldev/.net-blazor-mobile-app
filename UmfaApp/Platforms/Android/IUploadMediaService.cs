using System.Text.Json;
using UmfaApp.Data.Tables;
#if ANDROID
using Android.Content;
using Android.App;
#endif

namespace UmfaApp.Platforms.Android
{
    public interface IUploadMediaService
    {
        // Task UploadMedia(ReadingListRequest request);
    }

    public class AndroidUploadMediaService : IUploadMediaService
    {
        
    }

}
