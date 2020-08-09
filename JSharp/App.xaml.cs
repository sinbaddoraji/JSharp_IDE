using System;
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
            if(!File.Exists("debugger.jar"))
                File.WriteAllBytes("debugger.jar", JSharp.Properties.Resources.debugger);

            if (!Directory.Exists("lib"))
                Directory.CreateDirectory("lib");

            if (!File.Exists("lib\\tools.jar"))
                File.WriteAllBytes("lib\\tools.jar", JSharp.Properties.Resources.tools);

            if (!File.Exists($@"{JSharp.Properties.Settings.Default.JdkPath}\jre\lib\classlist"))
            {
                System.Windows.Forms.MessageBox.Show(@"JDK path currently empty");
                new Settings().ShowDialog();
            }
            else
            {
                //Initialize editor settings
                TextEditor.TextEditor.InitalizeCompletionData();
            }
        }
    }
}