using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
    internal class DiagnosticsLogger : IDisposable
    {
        private readonly BlockingCollection<string> _logBuffer = new BlockingCollection<string>(1000);
        private readonly Task _writerTask;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly string _filePath;

        public DiagnosticsLogger(string filePath = "diagnostics.json")
        {
            _filePath = filePath;
            _writerTask = Task.Run(WriteToFileAsync);
        }

        public void LogBallState(IBall ball)
        {
            if (_logBuffer.IsAddingCompleted) return;

            var data = new
            {
                Timestamp = DateTime.Now.ToString("O"),
                BallId = ball.GetHashCode(),
                PosX = ball.Position.x,
                PosY = ball.Position.y,
                Vx = ball.Velocity.x,
                Vy = ball.Velocity.y
            };

            string serializedLog = JsonSerializer.Serialize(data);

            _logBuffer.TryAdd(serializedLog);
        }

        private async Task WriteToFileAsync()
        {
            using StreamWriter sw = new StreamWriter(_filePath, append: false, Encoding.ASCII);
            try
            {
                foreach (var log in _logBuffer.GetConsumingEnumerable())
                {
                    await sw.WriteLineAsync(log);
                }
            }
            catch (Exception)
            {
               
            }
         
        }

        public void Dispose()
        {
           
            _logBuffer.CompleteAdding();

            try
            {
               
                _writerTask.Wait();
            }
            catch (AggregateException) { }

            _logBuffer.Dispose();
            _cts.Dispose();
        }
    }
}