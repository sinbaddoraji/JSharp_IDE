using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JdbWrapper
{
    internal class TextEditor : FastColoredTextBox
    {
        int CurrentLine => Selection.Start.iLine;

        public List<int> BookmarkedLines = new List<int>();

        DebuggerGUI parent;

        public string shortName;

        public TextEditor(DebuggerGUI parent, string filename)
        {
            Dock = DockStyle.Fill;
            AutoCompleteBrackets = true;
            Language = Language.CSharp;
            BookmarkColor = Color.Red;
            ReadOnly = true;

            OpenFile(filename);
            shortName = Path.GetFileName(filename);
            shortName = shortName.Substring(0, shortName.Length - 5);

            this.parent = parent;
            InitalizeContextMenu();

            TextChanged += TextEditor_TextChanged;
        }

        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.SelectionStart = TextLength;
            
        }

        private void InitalizeContextMenu()
        {
            MenuItem copy = new MenuItem("Copy");
            copy.Click += Copy_Click;

            MenuItem setBreakpoint = new MenuItem("Set Breakpoint");
            setBreakpoint.Click += SetBreakpoint_Click;

            MenuItem removeBreakpoint = new MenuItem("Remove Breakpoint");
            setBreakpoint.Click += RemoveBreakpoint_Click1;

            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.AddRange(new[] { copy, setBreakpoint, removeBreakpoint });
        }

        private void RemoveBreakpoint_Click1(object sender, EventArgs e)
        {
            if(BookmarkedLines.Contains(CurrentLine))
            {
                BookmarkedLines.Remove(CurrentLine);
                UnbookmarkLine(CurrentLine);

                parent.consoleAppManager.WriteLine($"clear {shortName}:{CurrentLine}");
            }
        }

        private void SetBreakpoint_Click(object sender, EventArgs e)
        {
            if (!BookmarkedLines.Contains(CurrentLine))
            {
                BookmarkedLines.Add(CurrentLine);
                BookmarkLine(CurrentLine);

                parent.consoleAppManager.WriteLine($"stop at {shortName}:{CurrentLine}");
            }
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            Copy();
        }
    }
}
