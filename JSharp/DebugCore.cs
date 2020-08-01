using JSharp.PluginCore;
using JSharp.Windows.MainWindow;
using sun.reflect.generics.tree;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace JSharp
{
    internal static class DebugCore
    {
        /// <summary>
        /// JDK path
        /// </summary>
        private static readonly string JdkPath;

        /// <summary>
        /// Path to Java console (java.exe)
        /// </summary>
        private static readonly string JavaConsole;

        /// <summary>
        /// Path to Java compiler (javac.exe)
        /// </summary>
        private static readonly string JavaCompiler;

        /// <summary>
        /// Path to Java jar (jar.exe)
        /// </summary>
        private static readonly string JavaJar;

        /// <summary>
        /// Location Path of JSharp
        /// </summary>
        private static readonly string ProgramLocation;

        /// <summary>
        /// JSharp Main-Window
        /// </summary>
        private static Main MainWindow => PluginHolder.Instance.ParentWindow;

        /// <summary>
        /// JSharp project folder
        /// </summary>
        private static string ProjectFolder => MainWindow.ProjectFolder;

        /// <summary>
        /// Java class containing Main(String[] args)
        /// </summary>
        public static string MainClassFile;

        /// <summary>
        /// Debug process (process for compiler, console, ..)
        /// </summary>
        private static Process process;

        /// <summary>
        /// Text-box with process output
        /// </summary>
        public static System.Windows.Controls.TextBox OutputTextbox;

        /// <summary>
        /// Text-box with process output
        /// </summary>
        public static string GetProjectDirectory()
        {
            if (MainClassFile != null)
                return GetParentDir(MainClassFile);
            else
                return Directory.GetParent(PluginHolder.Instance.ParentWindow.GetSelectedFile(false)).Name;
        }

        /// <summary>
        /// Get file name without directory path or class name
        /// </summary>
        private static string GetName(string filename)
        {
            string fname = filename.Remove(filename.Length - 5);
            if (!File.Exists(fname + ".class")) Compile(filename);

            return fname.Substring(filename.LastIndexOf("\\") + 1);
        }

        /// <summary>
        /// Get project folder for file
        /// </summary>
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

        /// <summary>
        /// Get class names from project folder
        /// </summary>
        public static string GetProjectClasses()
        {
            string output = "";
            string dir = GetProjectDirectory();

            var classFiles = Directory.GetFiles(dir, "*.class");
            for (int i = 0; i < classFiles.Length; i++)
                output += classFiles[i].Substring(dir.Length + 1) + " ";

            return output.Trim();
        }

        /// <summary>
        /// Core for JSharp debugging and compiling processes
        /// </summary>
        static DebugCore()
        {
            JdkPath = Properties.Settings.Default.JdkPath;
            JavaConsole = $"\"{JdkPath}\\bin\\java.exe\"";
            JavaCompiler = $"{JdkPath}\\bin\\javac.exe";
            JavaJar = $"\"{JdkPath}\\bin\\jar.exe\"";

            ProgramLocation = Directory.GetParent(Environment.GetCommandLineArgs()[0]).ToString();

            OutputTextbox = new System.Windows.Controls.TextBox
            {
                TextWrapping = System.Windows.TextWrapping.Wrap,
                IsReadOnly = true
            };
        }

        /// <summary>
        /// Compile current JSharp Project or current Java file
        /// </summary>
        public static void CompileProject(bool run)
        {
            string projectFolder = GetParentDir(PluginHolder.Instance.ParentWindow.GetSelectedFile(false));
            MainClassFile = null;

            var javaFiles = Directory.GetFiles(projectFolder, "*.java");
            for (int i = 0; i < javaFiles.Length; i++)
            {
                if (MainClassFile == null && File.ReadAllText(javaFiles[i]).Contains("static void main"))
                {
                    MainClassFile = javaFiles[i];
                }

                Compile(ProjectFolder, javaFiles[i]);
            }

            if (run) RunProject();
        }

        /// <summary>
        /// Run JSharp project
        /// </summary>
        public static void RunProject()
        {
            if(MainClassFile != null)
            {
                Run(MainClassFile);
            }
            else
            {
                MessageBox.Show("Main class can not be found in project folder");
            }
        }

        /// <summary>
        /// Run Java program in debugger
        /// </summary>
        public static void RunInDebugger(string filename)
        {
            string debuggerDir = ProgramLocation + "\\debugger.jar";
            string processCode = $"/K  cd/   &&  cd {GetParentDir(filename)}  &&  {JavaConsole} -jar {debuggerDir} {GetName(filename)}";

            process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe", Arguments = processCode,
                    CreateNoWindow = true, ErrorDialog = true, UseShellExecute = false,
                    RedirectStandardError = true, RedirectStandardOutput = true
                }
            };
            OutputTextbox.Text += "Debugger Starting...\n";
            process.Start();
        }

        /// <summary>
        /// Run Java program
        /// </summary>
        public static void Run(string filename)
        {
            if (filename.Contains(".java") && filename != null)
            {
                string fName = GetName(filename);
                string processCode = $"/K  cd/   &&  cd {GetParentDir(filename)}  &&  {JavaConsole}  {fName} && title {fName}.java";

                process?.Close();
                process = new Process()
                {
                    StartInfo = new ProcessStartInfo("cmd.exe", processCode)
                    {
                        CreateNoWindow = true, UseShellExecute = true
                    }
                };

                process.Start();
                OutputTextbox.Text += $"Running {filename}\n";
            }
            else
            {
                MessageBox.Show("This is not a valid Java file");
            }
        }

        /// <summary>
        /// Compile file
        /// </summary>
        public static void Compile(string fileName) => Compile(ProjectFolder, fileName);

        /// <summary>
        /// Compile file in working directory
        /// </summary>
        public static void Compile(string WorkingDirectory, string FileName)
        {
            if (File.Exists(JavaCompiler))
            {
                process?.Close();

                process = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = JavaCompiler, Arguments = FileName,
                        WorkingDirectory = WorkingDirectory,
                        CreateNoWindow = true, ErrorDialog = false, UseShellExecute = false,
                        RedirectStandardError = true, RedirectStandardOutput = true
                    }
                };
                process.Start();

                string output = process.StandardError.ReadToEnd();
                OutputTextbox.Text += output.Length > 0 ? $"{output}\n" : $"Successfully built {FileName}\n";

                process.WaitForExit(20000);
            }
            else
            {
                MessageBox.Show("Unable to compile " + FileName);
            }
        }

        /// <summary>
        /// Compile Java project into Jar file
        /// </summary>
        public static void CreatePackage()
        {
            CompileProject(false);

            string manifestfile = GetProjectDirectory() + "\\Manifest.mf";
            GenerateProjectManifest(manifestfile);

            string classes = GetProjectClasses();
            const string jarfilename = "output.jar";

            string processCode = $"/K  cd/   &&  cd  {GetProjectDirectory()} &&  {JavaJar}  cmf   Manifest.mf  {jarfilename}   {classes}";

            process?.Close();
            process = new Process()
            {
                StartInfo = new ProcessStartInfo("cmd.exe", processCode)
                {
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            process.Start();
        }

        /// <summary>
        /// Generate Manifest file form project
        /// </summary>
        private static void GenerateProjectManifest(string manifestfile)
        {
            string mainClass = Path.GetFileNameWithoutExtension(MainClassFile);
            File.WriteAllText(manifestfile,"Manifest-Version: 1.0"
                       + "\nAnt-Version: Apache Ant 1.9.4"
                       + "\nCreated-By: 1.8.0_25-b18 (Oracle Corporation)"
                       + "\nClass-Path: "
                       + "\nX-COMMENT: Main-Class will be added automatically by build"
                       + $"\nMain-Class: {mainClass}"
                       + $"\nMain-Class: {mainClass}");
        }
    }
}
