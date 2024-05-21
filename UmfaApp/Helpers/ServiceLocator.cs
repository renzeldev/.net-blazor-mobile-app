﻿
namespace UmfaApp.Helpers
{
    public static class ServiceLocator
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static T GetService<T>() => ServiceProvider.GetService<T>();
    }
}
