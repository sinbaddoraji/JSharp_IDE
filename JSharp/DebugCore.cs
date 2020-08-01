using JSharp.PluginCore;
using JSharp.Windows;
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

        public static string EntryFile = null;

        private static Process process;

        public static string JavaConsole;
        public static string JavaCompiler;
        public static string JavaJar;
        public static string ProgramLocation;

        public static System.Windows.Controls.TextBox OutputTextbox;

        static DebugCore()
        {
            JdkPath = Properties.Settings.Default.JdkPath;
            JavaConsole = $"\"{JdkPath}\\bin\\java.exe\"";
            JavaCompiler = $"\"{JdkPath}\\bin\\javac.exe\"";
            JavaJar = $"\"{JdkPath}\\bin\\jar.exe\"";
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

        public static string GetProjectDirectory()
        {
            if (EntryFile != null)
                return GetParentDir(EntryFile);
            else
                return Directory.GetParent(PluginHolder.Instance.ParentWindow.GetSelectedFile(false)).Name;
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
            if (File.Exists(JavaCompiler))
            {
                process = new Process();
                process.StartInfo.FileName = JavaCompiler;
                process.StartInfo.Arguments = FileName;
                process.StartInfo.WorkingDirectory = WorkingDirectory;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.ErrorDialog = false;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                processStarted = process.Start();

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

        public static string GetProjectClasses()
        {
            string output = "";
            string dir = GetProjectDirectory();
            foreach (var item in Directory.GetFiles(dir, "*.class"))
            {
                output += item.Substring(dir.Length + 1) + " ";
            }

            return output.Trim();
        }
        public static void CreatePackage()
        {
            CompileProject(false);

            string manifestfile = GetProjectDirectory() + "\\Manifest.mf";
            GenerateProjectManifest(manifestfile);

            string classes = GetProjectClasses();
            const string jarfilename = "output.jar";

            string processCode = $"/K  cd/   &&  cd  {GetProjectDirectory()} &&  {JavaJar}  cmf   Manifest.mf  {jarfilename}   {classes}";
            ProcessStartInfo ProcessInfo = new ProcessStartInfo("cmd.exe", processCode)
            {
                CreateNoWindow = true,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            process = Process.Start(ProcessInfo);
        }

        private static void GenerateProjectManifest(string manifestfile)
        {
            string mainClass = Path.GetFileNameWithoutExtension(EntryFile);
            File.WriteAllText(manifestfile, "Manifest-Version: 1.0"
                       + "\nAnt-Version: Apache Ant 1.9.4"
                       + "\nCreated-By: 1.8.0_25-b18 (Oracle Corporation)"
                       + "\nClass-Path: "
                       + "\nX-COMMENT: Main-Class will be added automatically by build"
                       + "\nMain-Class: " + mainClass
                       + "\nMain-Class: " + mainClass);
        }
    }
}
