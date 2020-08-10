using System;
using System.Diagnostics;
using System.IO;
using JSharp.Windows;

namespace JSharp
{
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            Directory.SetCurrentDirectory(Directory.GetParent(Environment.GetCommandLineArgs()[0]).FullName);

            if (!File.Exists($@"{JSharp.Properties.Settings.Default.JdkPath}\jre\lib\classlist"))
            {
                string defaultJDK = FindJavaPath();
                if(defaultJDK != null)
                {
                    JSharp.Properties.Settings.Default.JdkPath = defaultJDK;
                    JSharp.Properties.Settings.Default.Save();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(@"JDK path currently empty");
                    new Settings().ShowDialog();
                }
            }
            else
            {
                //Initialize editor settings
                TextEditor.TextEditor.InitalizeCompletionData();
            }
        }

        private string FindJavaPath()
        {
            Process p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "where.exe",
                    Arguments = "jdb",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            p.Start();
            p.WaitForExit(2000);

            return p.StandardOutput.ReadToEnd();
        }
    }
}