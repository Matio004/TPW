using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace TP.ConcurrentProgramming.Data
{
    internal class Logger : ILogger, IDisposable
    {
        private static readonly Lazy<Logger> _singletonInstance = new Lazy<Logger>(() => new Logger());

        private readonly ConcurrentQueue<BallLogEntry> _logQueue;
        private readonly string _logFilePath;
        private readonly Thread _loggingThread;
        private readonly AutoResetEvent _logSignal;
        private readonly SemaphoreSlim _queueLimiter;
        private const int MaxQueueSize = 10000;
        private bool _isLoggingActive;
        private readonly StreamWriter _logWriter;

        private Logger()
        {
            _logSignal = new AutoResetEvent(false);
            _logQueue = new ConcurrentQueue<BallLogEntry>();
            _queueLimiter = new SemaphoreSlim(MaxQueueSize);

            string? repoRoot = FindRepoRoot(AppDomain.CurrentDomain.BaseDirectory);
            string logsDir = Path.Combine(repoRoot ?? AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(logsDir);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            _logFilePath = Path.Combine(logsDir, $"logs_{timestamp}.json");

            _logWriter = new StreamWriter(_logFilePath, append: true, encoding: Encoding.UTF8);
            _isLoggingActive = true;

            _loggingThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true
            };
            _loggingThread.Start();
        }

        private void ProcessLogQueue()
        {
            while (_isLoggingActive || !_logQueue.IsEmpty)
            {
                _logSignal.WaitOne();

                while (_logQueue.TryDequeue(out var entry))
                {
                    try
                    {
                        string json = JsonSerializer.Serialize(entry);
                        _logWriter.WriteLine(json);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error writing log entry: {ex.Message}");
                    }
                    finally
                    {
                        _queueLimiter.Release();
                    }
                }
                _logWriter.Flush();
            }
        }

        public static Logger LoggerInstance => _singletonInstance.Value;

        private static string? FindRepoRoot(string startDir)
        {
            DirectoryInfo? dir = new DirectoryInfo(startDir);
            while (dir != null && !dir.GetFiles("*.sln").Any())
            {
                dir = dir.Parent;
            }
            return dir?.FullName;
        }

        public void Log(string message, int threadId, IVector position, IVector velocity)
        {
            if (!_isLoggingActive)
                return;

            if (_queueLimiter.Wait(0))
            {
                var logEntry = new BallLogEntry(DateTime.Now, message, threadId, position, velocity);
                _logQueue.Enqueue(logEntry);
                _logSignal.Set();
            }
        }

        public void Dispose()
        {
            if (!_isLoggingActive) return;
            _isLoggingActive = false;
            _logSignal.Set();
            _loggingThread.Join();
            _logWriter.Dispose();
        }

        internal class BallLogEntry
        {
            public DateTime Timestamp { get; set; }
            public string Message { get; set; }
            public int ThreadId { get; set; }
            public IVector Position { get; set; }
            public IVector Velocity { get; set; }

            internal BallLogEntry(DateTime timestamp, string message, int threadId, IVector position, IVector velocity)
            {
                Timestamp = timestamp;
                Message = message;
                ThreadId = threadId;
                Position = position;
                Velocity = velocity;
            }
        }
    }
}
