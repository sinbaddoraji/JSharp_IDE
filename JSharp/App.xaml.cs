using JSharp.TextEditor;
using System;
using System.IO;
using System.Windows;

namespace JSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Initialize editor settings
            new Editor();

            if (!File.Exists($@"{JSharp.Properties.Settings.Default.JdkPath}\jre\lib\classlist"))
            {
                string javaPath = GetJavaInstallationPath();
                if (Directory.Exists(javaPath))
                {
                    JSharp.Properties.Settings.Default.JdkPath = javaPath;
                    JSharp.Properties.Settings.Default.Save();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("JDK path currently empty");
                    new JSharp.MainWindow.Settings().ShowDialog();
                }

            }

            if (!File.Exists("jni4net.j-0.8.8.0.jar"))
                File.WriteAllBytes("jni4net.j-0.8.8.0.jar", JSharp.Properties.Resources.jni4net_j_0_8_8_0);
        }

        private string GetJavaInstallationPath()
        {
            string environmentPath = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(environmentPath)) return environmentPath;

            const string javaKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment\\";
            using (Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(javaKey))
            {
                using (Microsoft.Win32.RegistryKey key = rk.OpenSubKey(rk.GetValue("CurrentVersion").ToString()))
                    return key.GetValue("JavaHome").ToString();
            }
        }
    }
}