using JSharp.PluginCore;
using JSharp.Windows.MainWindow;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace JSharp
{
    internal static class DebugCore
    {
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
        /// Java Debugger
        /// </summary>
        private static readonly string JavaJdb;

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
        private static string _mainClassFile;

        /// <summary>
        /// Debug process (process for compiler, console, ..)
        /// </summary>
        private static Process _process;

        /// <summary>
        /// Text-box with process output
        /// </summary>
        public static readonly System.Windows.Controls.TextBox OutputTextbox;

        /// <summary>
        /// Text-box with process output
        /// </summary>
        private static string GetProjectDirectory()
        {
            return _mainClassFile != null ? GetParentDir(_mainClassFile) : Directory.GetParent(PluginHolder.Instance.ParentWindow.GetSelectedFile(false)).Name;
        }

        /// <summary>
        /// Get file name without directory path or class name
        /// </summary>
        private static string GetName(string filename)
        {
            var fname = filename.Remove(filename.Length - 5);
            if (!File.Exists(fname + ".class")) Compile(filename);

            return fname.Substring(filename.LastIndexOf("\\", StringComparison.Ordinal) + 1);
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
        private static string GetProjectClasses()
        {
            var dir = GetProjectDirectory();

            var classFiles = Directory.GetFiles(dir, "*.class");
            var output = classFiles.Aggregate("", (current, t) => current + (t.Substring(dir.Length + 1) + " "));

            return output.Trim();
        }

        /// <summary>
        /// Core for JSharp debugging and compiling processes
        /// </summary>
        static DebugCore()
        {
            var jdkPath = Properties.Settings.Default.JdkPath;
            JavaConsole = $"\"{jdkPath}\\bin\\java.exe\"";
            JavaCompiler = $"\"{jdkPath}\\bin\\javac.exe\"";
            JavaJar = $"\"{jdkPath}\\bin\\jar.exe\"";
            JavaJdb = $"\"{jdkPath}\\bin\\jdb.exe\"";

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
            var projectFolder = GetParentDir(PluginHolder.Instance.ParentWindow.GetSelectedFile(false));
            _mainClassFile = null;

            foreach (var javaFile in Directory.GetFiles(projectFolder, "*.java"))
            {
                if (_mainClassFile == null && File.ReadAllText(javaFile).Contains("static void main"))
                {
                    _mainClassFile = javaFile;
                }

                Compile(ProjectFolder, javaFile);
            }

            if (run) RunProject();
        }

        /// <summary>
        /// Run JSharp project
        /// </summary>
        public static void RunProject()
        {
            if(_mainClassFile != null)
            {
                Run(_mainClassFile);
            }
            else
            {
                MessageBox.Show(@"Main class can not be found in project folder");
            }
        }

        /// <summary>
        /// Run Java program in debugger
        /// </summary>
        public static void RunInDebugger()
        {
            CompileProject(false);
            _process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "JdbWrapper.exe", Arguments = $"{JavaJdb} \"{ProjectFolder}\""
                }
            };
            OutputTextbox.Text += "Debugger Starting...\n";
            OutputTextbox.ScrollToEnd();

            _process.Start();
        }

        /// <summary>
        /// Close debug process if any
        /// </summary>
         public static void CloseProcess()
         {
            if (_process != null)
                _process.Close();
         }

        /// <summary>
        /// Run Java program
        /// </summary>
        public static void Run(string filename)
        {
            if (filename.Contains(".java"))
            {
                var fName = GetName(filename);
                var processCode = $"/K  cd/   &&  cd {GetParentDir(filename)}  &&  {JavaConsole}  {fName} && title {fName}.java";

                _process?.Close();
                _process = new Process()
                {
                    StartInfo = new ProcessStartInfo("cmd.exe", processCode)
                    {
                        CreateNoWindow = true, UseShellExecute = true
                    }
                };

                _process.Start();
                OutputTextbox.Text += $"Running {filename}\n";
                OutputTextbox.ScrollToEnd();
            }
            else
            {
                MessageBox.Show(@"This is not a valid Java file");
            }
        }

        /// <summary>
        /// Compile file
        /// </summary>
        public static void Compile(string fileName) => Compile(ProjectFolder, fileName);

        /// <summary>
        /// Compile file in working directory
        /// </summary>
        private static void Compile(string workingDirectory, string fileName)
        {
            if (File.Exists(JavaCompiler.Replace("\"","")))
            {
                _process?.Close();

                _process = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = JavaCompiler, Arguments = $"-g \"{fileName}\"",
                        WorkingDirectory = workingDirectory,
                        CreateNoWindow = true, ErrorDialog = false, UseShellExecute = false,
                        RedirectStandardError = true, RedirectStandardOutput = true
                    }
                };
                _process.Start();

                var output = _process.StandardError.ReadToEnd();
                OutputTextbox.Text += output.Length > 0 ? $"{output}\n" : $"Successfully built {fileName}\n";
                OutputTextbox.ScrollToEnd();

                _process.WaitForExit(20000);
            }
            else
            {
                MessageBox.Show(@"Unable to compile " + fileName);
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

            var classes = GetProjectClasses();
            const string jarfilename = "output.jar";

            var processCode = $"/K  cd/   &&  cd  {GetProjectDirectory()} &&  {JavaJar}  cmf   Manifest.mf  {jarfilename}   {classes}";

            _process?.Close();
            _process = new Process()
            {
                StartInfo = new ProcessStartInfo("cmd.exe", processCode)
                {
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            _process.Start();
        }

        /// <summary>
        /// Generate Manifest file form project
        /// </summary>
        private static void GenerateProjectManifest(string manifestfile)
        {
            var mainClass = Path.GetFileNameWithoutExtension(_mainClassFile);
            File.WriteAllText(manifestfile,@"Manifest-Version: 1.0"
                       + "\nAnt-Version: Apache Ant 1.9.4"
                       + "\nCreated-By: 1.8.0_25-b18 (Oracle Corporation)"
                       + "\nClass-Path: "
                       + "\nX-COMMENT: Main-Class will be added automatically by build"
                       + $"\nMain-Class: {mainClass}"
                       + $"\nMain-Class: {mainClass}");
        }
    }
}
