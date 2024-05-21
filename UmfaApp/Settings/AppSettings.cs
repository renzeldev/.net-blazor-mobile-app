using UmfaApp.Models;

namespace UmfaApp.Settings
{
    internal static class AppSettings
    {
        public static bool IsLoggedInOnline { get; set; } = false;
        public static bool IsLoggedIn { get; set; } = false;
        public static int? UserId { get; set; }
        
        public static string? DeviceId { get; set; }

        public static bool DarkMode { get; set; } = false;

        
        public static string CurrentTitle { get; set; }
        public static Partner ActivePartner { get; set; }

        public static string PhotoDirectory => Path.Combine(FileSystem.AppDataDirectory, "photos");
        public static string AudioDirectory => Path.Combine(FileSystem.AppDataDirectory, "audio");

        private static NetworkAccess _networkAccess => Connectivity.Current.NetworkAccess;
        private static IEnumerable<ConnectionProfile> _profiles => Connectivity.Current.ConnectionProfiles;
        public static bool IsConnectedToInternet => _networkAccess == NetworkAccess.Internet;
        public static bool IsConnectedToWifi => _profiles.Contains(ConnectionProfile.WiFi);

        public static void ResetAppSettings()
        {
            IsLoggedIn = false;
            IsLoggedInOnline = false;
            UserId = null;
            ActivePartner = null;
        }
    }
}
