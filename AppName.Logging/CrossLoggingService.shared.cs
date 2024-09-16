using System;

namespace AppName.Logging
{
    /// <summary>
    /// Cross CrossLoggingService
    /// </summary>
    public static class CrossLoggingService
    {
        static Lazy<ILoggingService> implementation = new Lazy<ILoggingService>(() => CreateNLogSampleLogging(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets if the plugin is supported on the current platform.
        /// </summary>
        public static bool IsSupported => implementation.Value == null ? false : true;

        /// <summary>
        /// Current plugin implementation to use
        /// </summary>
        public static ILoggingService Current
        {
            get
            {
                ILoggingService ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static ILoggingService CreateNLogSampleLogging()
        {
            return new LoggingService();

        }

        internal static Exception NotImplementedInReferenceAssembly() =>
        new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");

    }
}
