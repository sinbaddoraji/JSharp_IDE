using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JdbWrapper
{
    public partial class DebuggerGUI : Form
    {
        public ConsoleAppManager consoleAppManager;
        FolderBrowserDialog f = new FolderBrowserDialog();
        string mainFile;

        string jdbPath = "";

        TabPage mainTab;

        public DebuggerGUI(string jdbPath)
        {
            InitializeComponent();
            this.jdbPath = jdbPath;
        }

        public DebuggerGUI(string jdbPath, string openingPath) : this(jdbPath)
        {
            OpenProjectFolder(openingPath);
        }

        private void Open_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Clear();
            if (f.ShowDialog() == DialogResult.OK)
            {
                OpenProjectFolder(f.SelectedPath);
            }
        }

        private void OpenProjectFolder(string folder)
        {
            string[] files = System.IO.Directory.GetFiles(folder, "*.java");
            Directory.SetCurrentDirectory(folder);

            foreach (var item in files)
            {
                Open(item);
            }

            tabControl1.SelectedTab = mainTab;
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
                consoleAppManager = new ConsoleAppManager(jdbPath);
                consoleAppManager.ErrorTextReceived += ConsoleAppManager_ErrorTextReceived;
                consoleAppManager.StandartTextReceived += ConsoleAppManager_StandartTextReceived;
                consoleAppManager.ProcessExited += ConsoleAppManager_ProcessExited;

                consoleAppManager.ExecuteAsync(new[] { mainFile });
                consoleAppManager.WriteLine($"stop in {mainFile}.main");
            }
        }

        private void Run_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine($"run");
        }

        private void ConsoleAppManager_ProcessExited(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void ConsoleAppManager_StandartTextReceived(object sender, string e)
        {
            string recieved = e.Trim();
            outputTextbox.Text += e;
        }

        private void ConsoleAppManager_ErrorTextReceived(object sender, string e)
        {
            outputTextbox.Text += e;
        }

        private void Open(string file)
        {
            TabPage t = new TabPage
            {
                Text = Path.GetFileName(file)
            };

            TextEditor textEditor = new TextEditor(this, file);
            if(textEditor.Text.Contains("static void main(String[] args)"))
            {
                mainFile = textEditor.shortName;
                mainTab = t;
            }

            t.Controls.Add(textEditor);

            tabControl1.TabPages.Add(t);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("cont");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("step");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("next");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("stepi");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("suspend");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("resume");
        }

        private void tabControl2_Selecting(object sender, TabControlCancelEventArgs e)
        {

        }

        private void tabControl2_Selected(object sender, TabControlEventArgs e)
        {
            consoleAppManager.WriteLine("locals");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("help");
        }

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("classes");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("fields");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("methods");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("threads");
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine("locals");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            consoleAppManager.WriteLine(textBox1.Text);
        }
    }
}
