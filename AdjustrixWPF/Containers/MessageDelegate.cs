using System;
using System.Threading;
using System.Threading.Tasks;

namespace AdjustrixWPF.Containers
{
    public class MessageDelegate
    {
        private string message = "";
        private readonly TimeSpan defaultTimeSpan = TimeSpan.FromSeconds(3);
        private CancellationTokenSource? cancellationTokenSource; // Cancellation token source for task cancellation

        private readonly object lockObj = new();

        public event Action<string> MessageChanged;

        private void ChangeMessage(string message = "")
        {
            lock (lockObj)
            {
                this.message = message;
                MessageChanged?.Invoke(this.message);
            }
        }

        public void ChangeMessage(string message, TimeSpan timeSpan = default)
        {
            if (timeSpan == default)
            {
                timeSpan = defaultTimeSpan;
            }
            CancellationToken token;
            lock (lockObj)
            {
                // Cancel the previous task if it's still running
                cancellationTokenSource?.Cancel();
                // Create a new CancellationTokenSource for the new task
                cancellationTokenSource = new CancellationTokenSource();
                token = cancellationTokenSource.Token;
            }

            ChangeMessage(message);

            // Run the new task
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(timeSpan, token); // Pass the cancellation token to Task.Delay
                    if (!token.IsCancellationRequested)
                    {
                        ChangeMessage(); // Clear the message if not canceled
                    }
                }
                catch (TaskCanceledException)
                {
                    // Task was canceled, just exit without clearing the message
                }
            });
        }
    }
}
