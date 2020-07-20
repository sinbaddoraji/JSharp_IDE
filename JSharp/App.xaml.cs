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

            if (!File.Exists($@"{JSharp.Properties.Settings.Default.JdkPath}\jre\lib\classlist"))
            {
                System.Windows.Forms.MessageBox.Show("JDK path currently empty");
                new MainWindow.Settings().ShowDialog();
            }

            if (!File.Exists("jni4net.j-0.8.8.0.jar"))
                File.WriteAllBytes("jni4net.j-0.8.8.0.jar", JSharp.Properties.Resources.jni4net_j_0_8_8_0);

            //Initialize editor settings
            new Editor();
        }
    }
}