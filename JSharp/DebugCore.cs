using JSharp.PluginCore;
using JSharp.Windows.MainWindow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSharp
{
    class DebugCore
    {
        static string JdkPath;

        static Main PatentWindow => PluginHolder.Instance.ParentWindow;
        static string ProjectFolder => PatentWindow.ProjectFolder;

        static string EntryFile = null;

        static Process process;

        static DebugCore()
        {
            JdkPath = JSharp.Properties.Settings.Default.JdkPath;
        }

        public static void CompileProject(bool run)
        {
            foreach (var item in Directory.GetFiles(ProjectFolder, "*.java"))
            {
                if(EntryFile != null && File.ReadAllText(item).Contains("main"))
                {
                    EntryFile = item;
                }

                Compile(ProjectFolder, item);
            }

            RunProject();
        }

        public static void RunProject()
        {
            RunFile(EntryFile);
        }
        
        public static void RunFile(string filename)
        {
            if (filename == null) return;

            if (filename.Contains(".java"))
            {
                String ffname = filename.Remove(filename.Length - 5);
                ffname = ffname + ".class";

                if (File.Exists(ffname))
                {
                    ProcessStartInfo ProcessInfo;
                    //Process process;
                    String javapath = "\"" + JdkPath + "\\bin\\java.exe" + "\"";
                    String getfilename = filename.Substring(filename.LastIndexOf("\\") + 1);
                    String fname = "";
                    if (getfilename.Contains(".java"))
                    {
                        fname = getfilename.Remove(getfilename.Length - 5);
                    }
                    ProcessInfo = new ProcessStartInfo("cmd.exe", "/K" + "  cd/   &&  cd " + ProjectFolder + "  &&  " + javapath + "  " + fname);
                    ProcessInfo.CreateNoWindow = true;
                    ProcessInfo.UseShellExecute = true;
                    Process.Start(ProcessInfo);
                }
            }
        }

        public static bool Compile(string fileName)
        {
            return Compile(ProjectFolder, fileName);
        }

        public static bool Compile(string WorkingDirectory, string FileName)
        {
            bool processStarted = false;
            string javac = JdkPath + "\\bin\\javac.exe";
            if (File.Exists(javac))
            {
                process = new Process();
                process.StartInfo.FileName = javac;
                process.StartInfo.Arguments = FileName;
                process.StartInfo.WorkingDirectory = WorkingDirectory;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.ErrorDialog = false;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                processStarted = process.Start();
            }
            else
            {
                MessageBox.Show("Unable to compile " + FileName);
            }
            return processStarted;
        }
    }
}
