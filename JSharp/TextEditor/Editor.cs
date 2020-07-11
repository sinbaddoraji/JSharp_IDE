using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using JSharp.CodeFolding;
using JSharp.Code_Completion;
using JSharp.Highlighting;
using JSharp.PluginCore;
using JSharp.Properties;
using ContextMenu = System.Windows.Controls.ContextMenu;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;

namespace JSharp.TextEditor
{
    public class Editor : ICSharpCode.AvalonEdit.TextEditor
    {
        private readonly SaveFileDialog _saveFileDialog;
        private EditorCompletionWindow _completionWindow;

        public static string FilterOptions { get; set; }

        public string OpenedDocument { get; private set; } = "Untitled";

        private object _foldingStrategy;
        private FoldingManager _foldingManager;

        private Stream _fileStream;

        public static readonly ContextMenu EditorContntextMenu = new ContextMenu();

        public bool IsUnoccupied() => OpenedDocument == null;

        public static Brush TextForeground = null;
        public static Brush TextBackground = null;

        private readonly Dictionary<string, string> _brackets;

        private readonly InnerHighlightingManager _highlightingManager;

        private static IList<ICompletionData> CompletionData => EditorCompletionWindow.CompletionList.CompletionData;

        private string GetClosedWordToCursor(int from)
        {
            var caretOffset = from - 1;

            while(caretOffset > -1 && !char.IsWhiteSpace(Document.GetCharAt(caretOffset)))
            {
                caretOffset--;
            }

            caretOffset++;
            var start = caretOffset;
            
            while (caretOffset < Document.TextLength && !char.IsWhiteSpace(Document.GetCharAt(caretOffset)))
            {
                caretOffset++;
            }

            var end = caretOffset;

            return Document.GetText(start, end-start).Trim();
        }

        private string GetPreviousWord(int from)
        {
            var caretOffset = from - 1;

            while (caretOffset > -1 && !char.IsWhiteSpace(Document.GetCharAt(caretOffset)))
            {
                caretOffset--;
            }
            return GetPreviousWord(from);
        }

        public Editor()
        {
            //Initialize objects
            _saveFileDialog = new SaveFileDialog {Filter = FilterOptions};
            _foldingManager = FoldingManager.Install(TextArea);
            _foldingStrategy = new BraceFoldingStrategy();

            //Initialize default properties
            ShowLineNumbers = true;
            FontSize = 16;
            AllowDrop = true;

            //Initialize event handlers
            KeyDown += Editor_KeyDown;
            TextArea.TextEntering += TextEditor_TextArea_TextEntering;
            TextArea.TextEntered += TextEditor_TextArea_TextEntered;
            TextArea.TextCopied += TextArea_TextCopied;
            TextArea.Document.TextChanged += Document_TextChanged;
            Drop += Editor_Drop;

            //Setup other elements
            InitalizeContextMenu();

            if (TextForeground != null) Foreground = TextForeground;
            if (TextBackground != null) Background = TextBackground;

            _brackets = new Dictionary<string, string>
            {
                { "(", ")" },
                { "{", "}" },
                { "[", "]" },
                { "'", "'" },
                { "\"", "\"" }
            };

            _highlightingManager = new InnerHighlightingManager();
            ShowLineNumbers = true;

            EditorCompletionWindow.InitalizeCompletionData();
        }

        

        private void InitalizeContextMenu()
        {
            //Initialize menu item objects
            var cut = new MenuItem { Header = "Cut" };
            var copy = new MenuItem { Header = "Copy" };
            var paste = new MenuItem { Header = "Paste" };
            var selectAll = new MenuItem { Header = "Select All" };

            var setBreakPoint = new MenuItem { Header = "Set Breakpoint" };

            //Initialize menu item events
            cut.Click += delegate { Cut(); };
            copy.Click += delegate { Copy(); };
            paste.Click += delegate { Paste(); };
            selectAll.Click += delegate { SelectAll(); };
            setBreakPoint.Click += SetBreakPoint_Click;
            //Add menu items to context menu
            EditorContntextMenu.Items.Add(cut);
            EditorContntextMenu.Items.Add(copy);
            EditorContntextMenu.Items.Add(paste);
            EditorContntextMenu.Items.Add(selectAll);
            EditorContntextMenu.Items.Add(setBreakPoint);

            //Initialize context menu
            ContextMenu = EditorContntextMenu;
        }

        private void SetBreakPoint_Click(object sender, RoutedEventArgs e)
        {
            var lineOffset = (Document.GetLineByOffset(CaretOffset).Offset);
            //var line = Document.GetLineByOffset(Document.GetLineByOffset(CaretOffset).Offset);
            //Select(line.Offset, line.Length);

            TextArea.TextView.LineTransformers.Add(new BreakPointColourizer(lineOffset));
        }

        private static void Editor_Drop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            PluginHolder.Instance.ParentWindow.OpenDocuments(files);
        }

        private void Document_TextChanged(object sender, EventArgs e)
        {
            UpdateFoldings();
        }

        private static void TextArea_TextCopied(object sender, TextEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Space) return;
            var wordContext = GetClosedWordToCursor(CaretOffset);

            if (!CompletionData.Any(x => x.Text.StartsWith(wordContext)) && _completionWindow == null && wordContext.Length > 0)
            {
                var com = new MyCompletionData(wordContext);
                CompletionData.Add(com);
            }
        }
        private void TextEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if(_brackets.ContainsKey(e.Text))
            {
                Document.Insert(CaretOffset, _brackets[e.Text]);
                CaretOffset--;
            }

            var wordContext = GetClosedWordToCursor(CaretOffset);

            try
            {
                if (CompletionData.Any(x => x.Text.StartsWith(wordContext)))
                {
                    if (_completionWindow != null) return;
                    if (wordContext.Length < 1) return;
                    // Open code completion after the user has pressed dot:
                    _completionWindow = new EditorCompletionWindow(TextArea);

                    _completionWindow.Show();
                    _completionWindow.Closed += delegate
                    {
                        _completionWindow = null;
                    };
                }
                else
                {
                    if (!char.IsLetterOrDigit(e.Text[0]))
                    {
                        
                    }
                }

            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void TextEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 1 && _completionWindow != null)
            {
                string wordContext = GetClosedWordToCursor(CaretOffset);

                
                if (!char.IsLetterOrDigit(e.Text[0]) && wordContext != "")
                {
                    if (!CompletionData.Any(x => x.Text.StartsWith(wordContext)) && _completionWindow == null)
                    {
                        var com = new MyCompletionData(wordContext);
                        CompletionData.Add(com);
                    }
                    else if(_completionWindow == null)
                    {
                        _completionWindow.Close();
                    }
                    else
                    {
                        if(wordContext.Length > 0)
                        {
                            EditorCompletionWindow.CompletionList.RequestInsertion(e);
                        }
                    }
                    
                }
            }
            //e.Handled = true;
        }

        public void OpenDocument(string filename)
        {
            if (filename == "Untitled") return;

            //Initialize document properties
            OpenedDocument = filename;
            _fileStream = File.Open(filename, FileMode.Open);
            Load(_fileStream);
            SelectHighlighting(filename);
            InitializeFolding();
        }

        private void SelectHighlighting(string filename)
        {
            //Setup syntax highlighting
            SyntaxHighlighting = _highlightingManager.GetHighlightingFromExtension(new FileInfo(filename).Extension);
            //Set up folding strategy
            var highlighting = HighlightingManager.Instance.HighlightingDefinitions.Where(x => x.Name.Contains("ml"));
            _foldingStrategy = highlighting.Contains(SyntaxHighlighting) ? new XmlFoldingStrategy() : (object)new BraceFoldingStrategy();
        }

        public void SaveDocument()
        {
            if (IsUnoccupied()) return;
            try
            {
                Save(_fileStream);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void SaveAs(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            _fileStream.Close();
            fileName = _saveFileDialog.FileName;
            _fileStream = File.Open(fileName, FileMode.Open);

            if (Settings.Default.RecentFiles == null)
            {
                Settings.Default.RecentFiles = new StringCollection();
            }
            if (!Settings.Default.RecentFiles.Contains(fileName))
            {
                Settings.Default.RecentFiles.Add(fileName);
                Settings.Default.Save();
            }
        }

        public void SaveAs()
        {
            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
                SaveAs(_saveFileDialog.FileName);
        }

        private void InitializeFolding()
        {
            if (_foldingManager == null)
                _foldingManager = FoldingManager.Install(TextArea);
            UpdateFoldings();
        }

        private void UpdateFoldings()
        {
            switch (_foldingStrategy)
            {
                case BraceFoldingStrategy _:
                    BraceFoldingStrategy.UpdateFoldings(_foldingManager, Document);
                    break;
                case XmlFoldingStrategy xmlFoldingStrategy:
                    xmlFoldingStrategy.UpdateFoldings(_foldingManager, Document);
                    break;
            }
        }
    }
}