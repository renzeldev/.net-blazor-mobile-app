using Android.App;
using Android.Content;
using Android.OS;
using System.Text.Json;
using UmfaApp.Helpers;
using UmfaApp.Models;
using UmfaApp.Services;

namespace UmfaApp.Platforms.Android
{
    [Service]
    public class UploadMediaServiceAndroid : Service
    {
        private readonly Queue<Func<Task>> _jobQueue = new Queue<Func<Task>>();
        private bool _isJobRunning = false;
        private string NOTIFICATION_CHANNEL_ID = "6969";
        private int NOTIFICATION_ID = 1;
        private string NOTIFICATION_CHANNEL_NAME = "notification";
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // Create a notification channel and manager
            var notificationChannel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, NOTIFICATION_CHANNEL_NAME, NotificationImportance.Default);
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(notificationChannel);

            // Build the notification
            var notification = new Notification.Builder(this, NOTIFICATION_CHANNEL_ID)
                .SetContentTitle("Foreground Service")
                .SetContentText("This is a foreground service running.")
                .SetSmallIcon(Resource.Drawable.material_ic_menu_arrow_up_black_24dp)
                .Build();

            // Start the service in the foreground
            StartForeground(NOTIFICATION_ID, notification);

            string serializedObject = intent.GetStringExtra("request");
            var request = JsonSerializer.Deserialize<UploadMediaRequest>(serializedObject);

            var readingListService = ServiceLocator.GetService<IReadingListService>();

            // Add a new job to the queue
            AddJob(async () =>
            {
                await readingListService.UploadMediaAsync(request.UserId, request.DeviceId, request.Request);
            });

            return StartCommandResult.Sticky;
        }

        private async Task AddJob(Func<Task> jobFunc)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                _jobQueue.Enqueue(jobFunc);
                if (!_isJobRunning)
                {
                    _isJobRunning = true;
                    ProcessNextJob();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task ProcessNextJob()
        {
            while (true)
            {
                Func<Task> nextJob = null;

                await _semaphoreSlim.WaitAsync();
                try
                {
                    if (_jobQueue.TryDequeue(out nextJob))
                    {
                        // No need to set _isJobRunning here, it's already true
                    }
                    else
                    {
                        _isJobRunning = false;
                        // Stop the service if no more jobs
                        // StopForeground(true);
                        StopSelf();
                        break;
                    }
                }
                finally
                {
                    _semaphoreSlim.Release();
                }

                if (nextJob != null)
                {
                    try
                    {
                        await nextJob.Invoke();
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception, log it, etc.
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            _semaphoreSlim?.Dispose();
            base.OnDestroy();
        }
    }
}
