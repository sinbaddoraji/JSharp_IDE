using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using JSharp.Code_Completion;
using JSharp.Highlighting;
using JSharp.PluginCore;
using JSharp.Properties;
using System.Threading;
using System.Threading.Tasks;
using ContextMenu = System.Windows.Controls.ContextMenu;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using java.lang.reflect;

namespace JSharp.TextEditor
{
    public class Editor : ICSharpCode.AvalonEdit.TextEditor
    {
        private readonly SaveFileDialog _saveFileDialog;
        private EditorCompletionWindow _completionWindow;

        public static string FilterOptions { get; set; }


        public string OpenedDocument { get; private set; }
        public string OpenedDocumentShortName;

        private object _foldingStrategy;
        private FoldingManager _foldingManager;

        public static readonly ContextMenu EditorContntextMenu = new ContextMenu();

        public bool IsUnoccupied() => OpenedDocument == null;

        public static Brush TextForeground = null;
        public static Brush TextBackground = null;

        private readonly Dictionary<string, string> _brackets;

        private readonly InnerHighlightingManager _highlightingManager;

        private static EditorCompletionList CompletionList => EditorCompletionWindow.CompletionList;

        public static IEnumerable<string> _classList;

        private string GetClosedWordToCursor(int from)
        {
            var caretOffset = from - 1;

            while(caretOffset > -1 && !char.IsWhiteSpace(Document.GetCharAt(caretOffset)))
                caretOffset--;

            caretOffset++;
            var start = caretOffset;
            
            while (caretOffset < Document.TextLength && !char.IsWhiteSpace(Document.GetCharAt(caretOffset)))
                caretOffset++;

            return Document.GetText(start, caretOffset - start).Trim();
        }
        

        public Editor()
        {
            //Initialize objects
            _saveFileDialog = new SaveFileDialog {Filter = FilterOptions};
            _foldingManager = FoldingManager.Install(TextArea);

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

            SyntaxHighlighting = _highlightingManager.GetHighlightingFromExtension(".java");
           
            if(!EditorCompletionWindow.InitalizeCompletionData())
            {

                _classList = File.ReadAllLines($@"{Settings.Default.JdkPath}\jre\lib\classlist").Select(x => x.Replace("/", ".")).Where(x => !x.Contains("$"));
                foreach (string item in _classList)
                {
                    try
                    {
                        if (item.Contains("lang")) AddCompletionData(item);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            
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
            if (SyntaxHighlighting != _highlightingManager.GetHighlightingFromExtension(".java")) return;
            var wordContext = GetClosedWordToCursor(CaretOffset);

            if (!CompletionList.CompletionData.Any(x => x.Text.StartsWith(wordContext)) && _completionWindow == null && wordContext.Length > 0)
            {
                var com = new MyCompletionData(wordContext);
                CompletionList.CompletionData.Add(com);
            }
        }
        private void TextEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if(_brackets.ContainsKey(e.Text))
            {
                Document.Insert(CaretOffset, _brackets[e.Text]);
                CaretOffset--;
            }
            if (SyntaxHighlighting != _highlightingManager.GetHighlightingFromExtension(".java")) return;

            var wordContext = GetClosedWordToCursor(CaretOffset);

            if (CompletionList.CompletionData.Any(x => x.Text.StartsWith(wordContext)))
                InitializeCompletionWindow(wordContext);
            else
                _completionWindow?.Close();
        }

        private void InitializeCompletionWindow(string wordContext)
        {
            if (_completionWindow != null || (wordContext.Length < 1)) { return; }

            //Initialize code completion window
            _completionWindow = new EditorCompletionWindow(TextArea);

            _completionWindow.Show();
            _completionWindow.Closed += delegate
            {
                _completionWindow = null;
                EditorCompletionWindow.CompletionList?.SelectItem(string.Empty);
            };
        }

        private void TextEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (SyntaxHighlighting != _highlightingManager.GetHighlightingFromExtension(".java")) return;

            string wordContext = GetClosedWordToCursor(CaretOffset);

            if(EditorCompletionWindow.CompletionList.SelectedItem == null && _completionWindow != null)
            {
                _completionWindow.Close();
            }
            else if(e.Text[0] == '.' && EditorCompletionWindow.CompletionList.SelectedItem.Text.Length < 1)
                InitializeCompletionWindow(wordContext);

            if (e.Text[0] == ';')
            {
                if(_classList.Contains(wordContext.Replace(".","/")))
                    AddCompletionData(wordContext);

                EditorCompletionWindow.CompletionList.RequestInsertion(e);
            }
            else if(e.Text[0] == '\n' && _completionWindow != null)
            {
                _completionWindow.Close();
            }
            else if (e.Text.Length > 1 && _completionWindow != null && !char.IsLetterOrDigit(e.Text[0]) && wordContext != "")
            {
                if (!CompletionList.Contains(wordContext) && _completionWindow == null)
                    AddCompletionData(wordContext);
                else if (wordContext.Length > 0 && !CompletionList.Contains(wordContext))
                    EditorCompletionWindow.CompletionList.RequestInsertion(e);
            }
        }

        private void AddCompletion_Data(string data)
        {
            if (CompletionList.Contains(data)) return;
            if (data.Length < 1) return;

            CompletionList.Add(data);
        }

        private void AddCompletionData(string data)
        {
            AddCompletion_Data(data);

            java.lang.Class c = java.lang.Class.forName(data);

            if (c == null) return;
            AddCompletion_Data(c.getSimpleName());
            

            Parallel.ForEach(c.getDeclaredFields(), field =>
            {
                lock (CompletionList)
                {
                    if(Modifier.isPublic(field.getModifiers()))
                    {
                        AddCompletion_Data(c.getSimpleName() + "." + field.getName());
                    }
                }
            });
            Parallel.ForEach(c.getDeclaredMethods(), method =>
            {
                lock (CompletionList)
                {
                    if (Modifier.isPublic(method.getModifiers()))
                    {
                        AddCompletion_Data(c.getSimpleName() + "." + method.getName() + "(");
                    }
                }
            });
        }

        public void OpenDocument(string filename)
        {
            if (filename == null) return;

            //Initialize document properties
            OpenedDocument = filename;
            OpenedDocumentShortName = new FileInfo(OpenedDocument).Name;
            Load(filename);
            SelectHighlighting(filename);
            InitializeFolding();
        }

        private void SelectHighlighting(string filename)
        {
            //Setup syntax highlighting
            SyntaxHighlighting = _highlightingManager.GetHighlightingFromExtension(new FileInfo(filename).Extension);
            //Set up folding strategy
            var highlighting = HighlightingManager.Instance.HighlightingDefinitions.Where(x => x.Name.Contains("ml"));
            _foldingStrategy = highlighting.Contains(SyntaxHighlighting) ? new XmlFoldingStrategy() : null;
        }

        public void SaveDocument()
        {
            if (IsUnoccupied())
                return;

            try
            {
                File.WriteAllText(OpenedDocument, this.Text);
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

            fileName = _saveFileDialog.FileName;
            OpenedDocument = fileName;
            OpenedDocumentShortName = new FileInfo(OpenedDocument).Name;

            File.WriteAllText(fileName, Text);
            Load(fileName);

            if (Settings.Default.RecentFiles == null)
                Settings.Default.RecentFiles = new StringCollection();

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
                default:
                    BraceFoldingStrategy.UpdateFoldings(_foldingManager, Document);
                    break;
                case XmlFoldingStrategy xmlFoldingStrategy:
                    xmlFoldingStrategy.UpdateFoldings(_foldingManager, Document);
                    break;
            }
        }
    }
}