using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Folding;
using JSharp.PluginCore;
using JSharp.Properties;
using System.Threading.Tasks;
using ContextMenu = System.Windows.Controls.ContextMenu;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using java.lang.reflect;
using JSharp.TextEditor.Completion;
using JSharp.TextEditor.Folding;
using JSharp.TextEditor.Highlighting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace JSharp.TextEditor
{
    /// <inheritdoc />
    /// <summary>
    /// This file manages the properties of the text editor
    /// Note: Some properties are left public for easy plugin access
    /// </summary>
    public class TextEditor : ICSharpCode.AvalonEdit.TextEditor
    {
        #region Static properties

        /// <summary>
        /// Save file dialog for Text editor
        /// </summary>
        private static readonly SaveFileDialog SaveFileDialog = new SaveFileDialog { Filter = FilterOptions};

        /// <summary>
        /// Filter options for saveFileDialog or any additional dialog connected to textEditor
        /// </summary>
        public static string FilterOptions { get; } = "Java Files (*.java)|*.java|Other Files (*.*)|*.*";

        /// <summary>
        /// Abstract list holding auto complete information for all text editors
        /// </summary>
        private static EditorCompletionList CompletionList => EditorCompletionWindow._editorCompletionList;

        /// <summary>
        /// Class list extracted from JDK (A list of classes used by Java programs)
        /// </summary>
        private static IEnumerable<string> _classList;

        /// <summary>
        /// Foreground colour for all document text editors running on the JSharp process
        /// </summary>
        public static Brush TextForeground;

        /// <summary>
        /// Background colour for all document text editors running on the JSharp process
        /// </summary>
        public static Brush TextBackground;

        /// <summary>
        /// Brackets to be auto-completed by JSharp
        /// </summary>
        private static readonly Dictionary<string, string> Brackets
            = new Dictionary<string, string> { { "(", ")" }, { "{", "}" }, { "[", "]" }, { "'", "'" }, { "\"", "\"" } };

        /// <summary>
        /// Class used to manage syntax highlighting for all documents open on the JSharp process
        /// </summary>
        private static readonly InnerHighlightingManager HighlightingManager = new InnerHighlightingManager();

        #endregion

        #region Instance-related Properties

        /// <summary>
        /// The pop-up that makes use of CompletionList to provide hints
        /// </summary>
        private EditorCompletionWindow _completionWindow;

        /// <summary>
        /// Currently opened document
        /// </summary>
        public string OpenedDocument { get; private set; }

        /// <summary>
        /// Name of currently opened document
        /// </summary>
        public string OpenedDocumentShortName;

        /// <summary>
        /// Folding strategy currently being used
        /// </summary>
        private object _foldingStrategy;

        /// <summary>
        /// Object that handles all folding behaviour for current textEditor instance
        /// </summary>
        private FoldingManager _foldingManager;

        /// <summary>
        /// Context menu for text editor instance
        /// </summary>
        private ContextMenu _editorContntextMenu;

        /// <summary>
        /// Context menu for text editor instance
        /// </summary>
        public static readonly ContextMenu GlobalEditorContntextMenu = new ContextMenu();

        /// <summary>
        /// Current instance of text editor is currently unoccupied
        /// </summary>
        public bool IsUnoccupied() => OpenedDocument == null;

        /// <summary>
        /// Offset location of the closest word to cursor (Beginning of the word)
        /// </summary>
        public int ClosestWordOffset;

        #endregion

        #region TextEditor Core

        /// <summary>
        /// JSharp document text editor
        /// </summary>
        public TextEditor()
        {
            InitalizeEventHandlers();
            InitalizeProperties();
            InitalizeContextMenu();
        }

        private void InitalizeProperties()
        {
            FontSize = 16;
            AllowDrop = true;
            ShowLineNumbers = true;
            _foldingManager = FoldingManager.Install(TextArea);
            if (TextForeground != null) Foreground = TextForeground;
            if (TextBackground != null) Background = TextBackground;
            SyntaxHighlighting = HighlightingManager.GetHighlightingFromExtension(".java");

        }

        /// <summary>
        /// Initialize context menu for text editor instance
        /// </summary>
        private void InitalizeContextMenu()
        {
            //Initialize menu item objects
            var cut = new MenuItem { Header = "Cut" };
            var copy = new MenuItem { Header = "Copy" };
            var paste = new MenuItem { Header = "Paste" };
            var selectAll = new MenuItem { Header = "Select All" };

            //Initialize menu item events
            cut.Click += delegate { Cut(); };
            copy.Click += delegate { Copy(); };
            paste.Click += delegate { Paste(); };
            selectAll.Click += delegate { SelectAll(); };
            //Add menu items to context menu
            _editorContntextMenu = new ContextMenu();
            _editorContntextMenu.Items.Add(cut);
            _editorContntextMenu.Items.Add(copy);
            _editorContntextMenu.Items.Add(paste);
            _editorContntextMenu.Items.Add(selectAll);

            //Add global items to context menu
            foreach (var item in GlobalEditorContntextMenu.Items)
            {
                _editorContntextMenu.Items.Add(item);
            }

            //Initialize context menu
            ContextMenu = _editorContntextMenu;
        }

        /// <summary>
        /// Initialize completion window for this instance of the text editor
        /// </summary>
        private void InitializeCompletionWindow(string wordContext)
        {
            if (_completionWindow != null || (wordContext.Length < 1)) { return; }

            //Initialize code completion window
            _completionWindow = new EditorCompletionWindow(this);
            if (EditorCompletionWindow._editorCompletionList.Contains(wordContext))
            {
                EditorCompletionWindow._editorCompletionList?.SelectItem(wordContext);
            }
            var firstMatch = CompletionList.CompletionData.First(x => x.Text.StartsWith(wordContext));
            if (firstMatch != null && EditorCompletionWindow._editorCompletionList.ListBox != null)
            {
                int index = EditorCompletionWindow._editorCompletionList.ListBox.Items.IndexOf(firstMatch);
                EditorCompletionWindow._editorCompletionList.ListBox.SelectIndex(index);
            }

            _completionWindow.Show();
            _completionWindow.Closed += delegate
            {
                _completionWindow = null;
                EditorCompletionWindow._editorCompletionList?.SelectItem(string.Empty);
            };
        }

        /// <summary>
        /// Initialize event handlers for text editor
        /// </summary>
        private void InitalizeEventHandlers()
        {
            //Initialize event handlers
            KeyDown += Editor_KeyDown;
            TextArea.TextEntering += TextEditor_TextArea_TextEntering;
            TextArea.TextEntered += TextEditor_TextArea_TextEntered;
            TextArea.Document.TextChanged += Document_TextChanged;
            Drop += Editor_Drop;
        }

        #endregion

        #region Code completion

        static TextEditor()
        {
            SaveFileDialog.Filter = FilterOptions;
        }

        /// <summary>
        /// Get the closest word to user's cursor
        /// </summary>
        private string GetClosedWordToCursor(int from)
        {
            var caretOffset = from - 1;

            while (caretOffset > -1 && !char.IsWhiteSpace(Document.GetCharAt(caretOffset)))
                caretOffset--;

            caretOffset++;
            var start = ClosestWordOffset = caretOffset;

            while (caretOffset < Document.TextLength && !char.IsWhiteSpace(Document.GetCharAt(caretOffset)))
                caretOffset++;

            return Document.GetText(start, caretOffset - start).Trim();
        }


        /// <summary>
        /// Add completion data without searching through the Java library first
        /// </summary>
        private static void AddCompletion_Data(string data)
        {
            if (CompletionList.Contains(data)) return;
            if (data.Length < 1) return;

            CompletionList.Add(data);
        }

        /// <summary>
        /// Initialize Code completion data
        /// </summary>
        public static void InitalizeCompletionData()
        {
            if (!EditorCompletionWindow.InitalizeCompletionData())
            {
                if (Settings.Default.invalidClasses == null)
                    Settings.Default.invalidClasses = new StringCollection();

                bool invalidFound = false;

                _classList = File.ReadAllLines($@"{Settings.Default.JdkPath}\jre\lib\classlist").Select(x => x.Replace("/", ".")).Where(x => !x.Contains("$") && !x.Contains("#") && !Settings.Default.invalidClasses.Contains(x));
                foreach (string item in _classList)
                {
                    try
                    {
                        if (item.Contains("lang")) AddCompletionData(item);
                        else AddCompletion_Data(item);
                    }
                    catch
                    {
                        Settings.Default.invalidClasses.Add(item);
                        invalidFound = true;
                    }
                }

                if(invalidFound)
                    Settings.Default.Save();
            }
        }

        /// <summary>
        /// Add completion data and search through the Java library for more related data
        /// </summary>
        private static void AddCompletionData(string data)
        {
            AddCompletion_Data(data);

            java.lang.Class c = java.lang.Class.forName(data);
            if (c == null) return;
            AddCompletion_Data(c.getSimpleName());


            Parallel.ForEach(c.getDeclaredFields(), field =>
            {
                lock (CompletionList)
                {
                    if (Modifier.isPublic(field.getModifiers()))
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

        #endregion

        #region Event Handlers

        private static void Editor_Drop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            PluginHolder.Instance.ParentWindow.OpenDocuments(files);
        }

        private void Document_TextChanged(object sender, EventArgs e)
        {
            UpdateFoldings();
        }

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Space) return;
            if (SyntaxHighlighting != HighlightingManager.GetHighlightingFromExtension(".java")) return;
            var wordContext = GetClosedWordToCursor(CaretOffset);

            if (!CompletionList.CompletionData.Any(x => x.Text.StartsWith(wordContext)) && _completionWindow == null && wordContext.Length > 0)
            {
                var com = new EditorCompletionData(wordContext);
                CompletionList.CompletionData.Add(com);
            }
        }

        private void TextEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (Brackets.ContainsKey(e.Text))
            {
                Document.Insert(CaretOffset, Brackets[e.Text]);
                CaretOffset--;
            }
            if (SyntaxHighlighting != HighlightingManager.GetHighlightingFromExtension(".java")) return;

            var wordContext = GetClosedWordToCursor(CaretOffset);

            if (CompletionList.CompletionData.Any(x => x.Text.StartsWith(wordContext)))
                InitializeCompletionWindow(wordContext);
            else
                _completionWindow?.Close();
        }

        private void TextEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (SyntaxHighlighting != HighlightingManager.GetHighlightingFromExtension(".java")) return;

            string wordContext = GetClosedWordToCursor(CaretOffset);

            if (EditorCompletionWindow._editorCompletionList.SelectedItem == null && _completionWindow != null)
            {
                _completionWindow.Close();
            }
            else if (e.Text[0] == '.' && EditorCompletionWindow._editorCompletionList.SelectedItem?.Text.Length < 1)
                InitializeCompletionWindow(wordContext);

            if (e.Text[0] == ';')
            {
                if (_classList.Contains(wordContext.Replace(".", "/")))
                    AddCompletionData(wordContext);

                EditorCompletionWindow._editorCompletionList.RequestInsertion(e);
            }
            else if (e.Text[0] == '\n' && _completionWindow != null)
            {
                _completionWindow.Close();
            }
            else if (e.Text.Length > 1 && _completionWindow != null && !char.IsLetterOrDigit(e.Text[0]) && wordContext != "")
            {
                if (!CompletionList.Contains(wordContext) && _completionWindow == null)
                    AddCompletionData(wordContext);
                else if (wordContext.Length > 0 && !CompletionList.Contains(wordContext))
                    EditorCompletionWindow._editorCompletionList.RequestInsertion(e);
            }
        }

        #endregion

        #region File Handling

        /// <summary>
        /// Open file as document
        /// </summary>
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

        /// <summary>
        /// Save document to file
        /// </summary>
        public void SaveDocument()
        {
            if (IsUnoccupied())
                return;

            try
            {
                File.WriteAllText(OpenedDocument, Text);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Save document as specified filename
        /// </summary>
        private void SaveAs(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            fileName = SaveFileDialog.FileName;
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

        /// <summary>
        /// Save document as something else
        /// </summary>
        public void SaveAs()
        {
            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
                SaveAs(SaveFileDialog.FileName);
        }

        #endregion

        #region Syntax Highlighting and Code folding

        private void SelectHighlighting(string filename)
        {
            //Set up syntax highlighting
            SyntaxHighlighting = HighlightingManager.GetHighlightingFromExtension(new FileInfo(filename).Extension);
            //Set up folding strategy
            var highlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.HighlightingDefinitions.Where(x => x.Name.Contains("ml"));
            _foldingStrategy = highlighting.Contains(SyntaxHighlighting) ? new XmlFoldingStrategy() : null;
        }

        /// <summary>
        /// Using a folding manager and a folding strategy, update all folds in the active document
        /// </summary>
        private void InitializeFolding()
        {
            if (_foldingManager == null)
                _foldingManager = FoldingManager.Install(TextArea);
            UpdateFoldings();
        }

        /// <summary>
        /// Using a folding manager and a folding strategy, update all folds in the active document
        /// </summary>
        private void UpdateFoldings()
        {
            switch (_foldingStrategy)
            {
                default:
                    JavaFoldingStrategy.UpdateFoldings(_foldingManager, Document);
                    break;
                case XmlFoldingStrategy xmlFoldingStrategy:
                    xmlFoldingStrategy.UpdateFoldings(_foldingManager, Document);
                    break;
            }
        }

        #endregion

        public EditorCompletionWindow EditorCompletionWindow
        {
            get => default;
            set
            {
            }
        }

        internal InnerHighlightingManager InnerHighlightingManager
        {
            get => default;
            set
            {
            }
        }
    }
}