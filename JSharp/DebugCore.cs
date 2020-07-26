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
using System.Windows.Media;

namespace JSharp
{
    internal static class DebugCore
    {
        private static readonly string JdkPath;

        private static Main PatentWindow => PluginHolder.Instance.ParentWindow;
        private static string ProjectFolder => PatentWindow.ProjectFolder;

        private static string EntryFile = null;

        private static Process process;

        public static string JavaConsole;
        public static string ProgramLocation;

        public static System.Windows.Controls.TextBox OutputTextbox;

        static DebugCore()
        {
            JdkPath = Properties.Settings.Default.JdkPath;
            JavaConsole = $"\"{JdkPath}\\bin\\java.exe\"";
            ProgramLocation = Directory.GetParent(Environment.GetCommandLineArgs()[0]).ToString();
            OutputTextbox = new System.Windows.Controls.TextBox
            {
                TextWrapping = System.Windows.TextWrapping.Wrap,
                IsReadOnly = true
            };
        }

        public static void CompileProject(bool run)
        {
            string projectFolder = GetParentDir(PluginHolder.Instance.ParentWindow.GetSelectedFile(false));
            EntryFile = null;

            foreach (var item in Directory.GetFiles(projectFolder, "*.java"))
            {
                if(EntryFile == null && File.ReadAllText(item).Contains("static void main"))
                {
                    EntryFile = item;
                }

                Compile(ProjectFolder, item);
            }

            if(run) RunProject();
        }

        public static void RunProject()
        {
            if(EntryFile != null)
            {
                RunFile(EntryFile);
            }
            else
            {
                MessageBox.Show("Main class can not be found in project folder");
            }
        }

        private static string GetName(string filename)
        {
            string fname = filename.Remove(filename.Length - 5);
            if (!File.Exists(fname + ".class")) Compile(filename);

            return fname.Substring(filename.LastIndexOf("\\") + 1);
        }

        private static string GetParentDir(string filename)
        {
            string root = Directory.GetParent(filename).ToString();
            if (ProjectFolder != null && root == ProjectFolder)
            {
                return ProjectFolder;
            }
            else
            {
                return root;
            }
        }

        public static void RunInDebugger(string filename)
        {
            string debuggerDir = ProgramLocation + "\\debugger.jar";
            string processCode = $"/K  cd/   &&  cd {GetParentDir(filename)}  &&  {JavaConsole} -jar {debuggerDir} {GetName(filename)}";

            process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = processCode;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            OutputTextbox.Clear();

            OutputTextbox.Text += "Debugger Starting...\n";
            process.Start();
           
        }

        public static void RunFile(string filename)
        {
            if (filename.Contains(".java") && filename != null)
            {
                string fName = GetName(filename);
                string processCode = $"/K  cd/   &&  cd {GetParentDir(filename)}  &&  {JavaConsole}  {fName} && title {fName}.java";
                ProcessStartInfo ProcessInfo = new ProcessStartInfo("cmd.exe", processCode)
                {
                    CreateNoWindow = true, UseShellExecute = true
                };
                Process.Start(ProcessInfo);
                OutputTextbox.Text += $"Running {filename}\n";
            }
            else
            {
                MessageBox.Show("This is not a valid Java file");
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

                OutputTextbox.Clear();
                processStarted = process.Start();

                OutputTextbox.Text = process.StandardOutput.ReadToEnd();

                using (StreamReader s = process.StandardError)
                {
                    string output = s.ReadToEnd();
                    if (output.Length > 0)
                    {
                        OutputTextbox.Text += output + "\n";
                    }
                    else
                    {
                        OutputTextbox.Text += $"Successfully built {FileName}\n";
                    }

                    process.WaitForExit(20000);
                }
            }
            else
            {
                MessageBox.Show("Unable to compile " + FileName);
            }
            return processStarted;
        }
    }
}
