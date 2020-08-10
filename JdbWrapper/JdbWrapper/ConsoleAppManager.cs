using System.Collections.Generic;

namespace JdbWrapper
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class ConsoleAppManager
    {
        private readonly string appName;
        private readonly Process process = new Process();
        private readonly object theLock = new object();
        private SynchronizationContext context;
        private string pendingWriteData;

        public ConsoleAppManager(string appName)
        {
            this.appName = appName;

            process.StartInfo.FileName = this.appName;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.UseShellExecute = false;

            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;

            process.Exited += ProcessOnExited;
        }

        public event EventHandler<string> ErrorTextReceived;
        public event EventHandler ProcessExited;
        public event EventHandler<string> StandartTextReceived;

        public int ExitCode => process.ExitCode;

        public bool Running
        {
            get; private set;
        }

        public void ExecuteAsync(params string[] args)
        {
            if (Running)
            {
                throw new InvalidOperationException(
                    "Process is still Running. Please wait for the process to complete.");
            }

            string arguments = string.Join(" ", args);

            process.StartInfo.Arguments = arguments;

            context = SynchronizationContext.Current;

            process.Start();
            Running = true;

            new Task(ReadOutputAsync).Start();
            new Task(WriteInputTask).Start();
            new Task(ReadOutputErrorAsync).Start();
        }

        public void Write(string data)
        {
            if (data == null)
            {
                return;
            }

            lock (theLock)
            {
                pendingWriteData = data;
            }
        }

        public void WriteLine(string data)
        {
            Write(data + Environment.NewLine);
        }

        protected virtual void OnErrorTextReceived(string e)
        {
            EventHandler<string> handler = ErrorTextReceived;

            if (handler != null)
            {
                if (context != null)
                {
                    context.Post(delegate { handler(this, e); }, null);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        protected virtual void OnProcessExited()
        {
            EventHandler handler = ProcessExited;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnStandartTextReceived(string e)
        {
            EventHandler<string> handler = StandartTextReceived;

            if (handler != null)
            {
                if (context != null)
                {
                    context.Post(delegate { handler(this, e); }, null);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        private void ProcessOnExited(object sender, EventArgs eventArgs)
        {
            OnProcessExited();
        }

        private async void ReadOutputAsync()
        {
            StringBuilder standart = new StringBuilder();
            char[] buff = new char[1024];
            int length;

            while (process.HasExited == false)
            {
                standart.Clear();

                length = await process.StandardOutput.ReadAsync(buff, 0, buff.Length);
                standart.Append(buff.SubArray(0, length));
                OnStandartTextReceived(standart.ToString());
                Thread.Sleep(1);
            }

            Running = false;
        }

        private async void ReadOutputErrorAsync()
        {
            StringBuilder sb = new StringBuilder();

            do
            {
                sb.Clear();
                char[] buff = new char[1024];
                int length = await process.StandardError.ReadAsync(buff, 0, buff.Length);
                sb.Append(buff.SubArray(0, length));
                OnErrorTextReceived(sb.ToString());
                Thread.Sleep(1);
            }
            while (process.HasExited == false);
        }

        private async void WriteInputTask()
        {
            while (process.HasExited == false)
            {
                Thread.Sleep(1);

                if (pendingWriteData != null)
                {
                    await process.StandardInput.WriteLineAsync(pendingWriteData);
                    await process.StandardInput.FlushAsync();

                    lock (theLock)
                    {
                        pendingWriteData = null;
                    }
                }
            }
        }
    }


    public static class CharArrayExtensions
    {
        public static char[] SubArray(this char[] input, int startIndex, int length)
        {
            List<char> result = new List<char>();
            for (int i = startIndex; i < length; i++)
            {
                result.Add(input[i]);
            }

            return result.ToArray();
        }
    }
}
