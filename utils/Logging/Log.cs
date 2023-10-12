using System.Diagnostics;
using System.Text;

namespace utils.Logging
{
    public class Log : IDisposable
    {
        #region IDisposable implementation

        static bool disposed;

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    if (logFileStream != null)
                    {
                        logFileStream.Dispose();
                        logFileStream = null;
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        Stream? logFileStream;

        static Log? instance;

        public static void Init(string loggingDirectory, IFileAccess fileAccess)
        {
            instance = new Log();
            instance.logFileStream = fileAccess.CreateForWriting(
                Path.Combine(
                    loggingDirectory,
                    DateTime.Now.ToString("O").Replace(":", ".") + ".txt"
                )
            );
        }

        static void LogMessage(LogLevel level, string message, params string[] args)
        {
            var formatted = string.Format(message, args);
            formatted = level.ToString() + ": " + formatted;

            Debug.WriteLine(formatted);
            var data = Encoding.UTF8.GetBytes(formatted);
            instance?.logFileStream?.WriteAsync(data, 0, data.Length);
        }

        public static void LogWarning(string message, params string[] args)
        {
            LogMessage(LogLevel.Warning, message, args);
        }

        public static void LogError(string message, params string[] args)
        {
            LogMessage(LogLevel.Error, message, args);
        }

        public static void LogDebug(string message, params string[] args)
        {
            LogMessage(LogLevel.Debug, message, args);
        }
    }
}
