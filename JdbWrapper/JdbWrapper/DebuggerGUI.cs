using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace JdbWrapper
{
    public partial class DebuggerGUI : Form
    {
        public ConsoleProcessManger consoleAppManager;
        private readonly FolderBrowserDialog f = new FolderBrowserDialog();
        private string mainFile;
        private readonly string jdbPath = "";

        public DebuggerGUI(string jdbPath, string openingPath) : this(jdbPath)
        {
            OpenProjectFolder(openingPath);
        }

        public DebuggerGUI(string jdbPath) : this()
        {
            this.jdbPath = jdbPath.Trim();
        }

        public DebuggerGUI()
        {
            InitializeComponent();
            jdbPath = FindJavaPath();

            AutoScaleDimensions = new SizeF(96F, 96F);

            AutoScaleMode = AutoScaleMode.Dpi;
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
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            p.Start();
            p.WaitForExit(2000);

            return p.StandardOutput.ReadToEnd();
        }

        private void OpenProjectFolder(string folder)
        {
            string[] files = Directory.GetFiles(folder, "*.java");
            Directory.SetCurrentDirectory(folder);

            foreach (string item in files)
            {
                Open(item);
            }


            StartDebugging();
        }

        private void StartDebugging()
        {
            if (mainFile == null)
            {
                MessageBox.Show("There is no entry file. The application can not be debugged");
            }
            else
            {
                //Start Debugging process
                consoleAppManager = new ConsoleProcessManger(jdbPath);
                consoleAppManager.ErrorTextReceived += ConsoleAppManager_ErrorTextReceived;
                consoleAppManager.StandartTextReceived += ConsoleAppManager_StandartTextReceived;

                consoleAppManager.ExecuteAsync(new[] { mainFile });
                consoleAppManager.WriteLine($"stop in {mainFile}.main");
            }
        }



        private void Open(string file)
        {
            TabPage t = new TabPage
            {
                Text = Path.GetFileName(file)
            };

            TextEditor textEditor = new TextEditor(this, file);
            if (textEditor.Text.Contains("static void main(String[] args)"))
            {
                mainFile = textEditor.shortName;
                tabControl1.SelectedTab = t;
            }

            t.Controls.Add(textEditor);

            tabControl1.TabPages.Add(t);
        }

        private void DebuggerGUI_SizeChanged(object sender, EventArgs e)
        {
            panel1.Left = (Width - panel1.Width) / 2;
        }

        private void DirectCommand_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine(textBox1.Text);
        }

        private void Suspend_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("suspend");
        }

        private void Classes_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("classes");
        }

        private void Methods_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("methods");
        }

        private void Threads_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("threads");
        }

        private void StepOver_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("stepi");
        }

        private void Fields_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("fields");
        }

        private void Locals_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("locals");
        }

        private void Resume_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("resume");
        }

        private void Continue_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("cont");
        }

        private void Step_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("step");
        }

        private void Next_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("next");
        }

        private void Help_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("help");
        }

        private void Run_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine($"run");
        }

        private void ConsoleAppManager_StandartTextReceived(object sender, string e)
        {
            outputTextbox.Text += e;
        }

        private void ConsoleAppManager_ErrorTextReceived(object sender, string e)
        {
            outputTextbox.Text += e;
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                consoleAppManager.WriteLine(textBox1.Text);
            }
        }

        private void Open_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Clear();
            if (f.ShowDialog() == DialogResult.OK)
            {
                OpenProjectFolder(f.SelectedPath);
            }
        }

        private void DebuggerGUI_Load(object sender, EventArgs e)
        {

        }

        private void DebuggerGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            consoleAppManager.Close();

            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                TextEditor textEditor = (TextEditor)tabPage.Controls[0];
                textEditor.SaveToFile(textEditor.filename,System.Text.Encoding.UTF8);
            }
        }
    }
}
