using UmfaApp.Settings;

namespace UmfaApp.Helpers
{
    public static class OnlineHelper
    {
        public static bool IsOnline => AppSettings.IsLoggedInOnline && AppSettings.IsConnectedToInternet;
    }
}
