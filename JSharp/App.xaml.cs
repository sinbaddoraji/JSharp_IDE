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
            if(!File.Exists("debugger.jar"))
                File.WriteAllBytes("debugger.jar", JSharp.Properties.Resources.debugger);

            if (!Directory.Exists("lib"))
                Directory.CreateDirectory("lib");

            if (!File.Exists("lib\\tools.jar"))
                File.WriteAllBytes("lib\\tools.jar", JSharp.Properties.Resources.tools);

            if (!File.Exists($@"{JSharp.Properties.Settings.Default.JdkPath}\jre\lib\classlist"))
            {
                System.Windows.Forms.MessageBox.Show("JDK path currently empty");
                new MainWindow.Settings().ShowDialog();
            }
            else
            {
                //Initialize editor settings
                new Editor();
            }
        }
    }
}