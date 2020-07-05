using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using JSharp.Code_Completion;
using JSharp.Highlighting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace JSharp
{
    public class Editor : TextEditor
    {
        private readonly System.Windows.Forms.SaveFileDialog saveFileDialog;
        private EditorCompletionWindow completionWindow;

        public static string FilterOptions { get; set; }

        public string OpenedDocument { get; private set; } = "Untitled";

        private object foldingStrategy;
        private FoldingManager foldingManager;

        private Stream fileStream;

        public static ContextMenu contextMenu = new ContextMenu();

        public bool IsUnoccupied() => OpenedDocument == null;

        public static Brush TextForeground = null;
        public static Brush TextBackground = null;

        IList<ICompletionData> data;

        private Dictionary<string, string> brackets;

        private InnerHighlightingManager highlightingManager;

        private IList<ICompletionData> CompletionData => EditorCompletionWindow.CompletionList.CompletionData;

        public string GetClosedWordToCursor()
        {
            int caretOffset = CaretOffset - 1;

            int start; int end;

            while(caretOffset > -1 && !char.IsWhiteSpace(Document.GetCharAt(caretOffset)))
            {
                caretOffset--;
            }

            caretOffset++;
            start = caretOffset;
            
            while (caretOffset < Document.TextLength && !char.IsWhiteSpace(Document.GetCharAt(caretOffset)))
            {
                caretOffset++;
            }

            end = caretOffset;

            return Document.GetText(start, end-start);
        }

        public Editor()
        {
            //Initialize objects
            saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = FilterOptions;
            foldingManager = FoldingManager.Install(TextArea);
            foldingStrategy = new BraceFoldingStrategy();

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

            brackets = new Dictionary<string, string>
            {
                { "(", ")" },
                { "{", "}" },
                { "[", "]" },
                { "'", "'" },
                { "\"", "\"" }
            };

            highlightingManager = new InnerHighlightingManager();
            this.ShowLineNumbers = true;

            EditorCompletionWindow.InitalizeCompletionData();
        }

        

        private void InitalizeContextMenu()
        {
            //Initialize menu item objects
            MenuItem cut = new MenuItem() { Header = "Cut" };
            MenuItem copy = new MenuItem() { Header = "Copy" };
            MenuItem paste = new MenuItem() { Header = "Paste" };
            MenuItem selectAll = new MenuItem() { Header = "Select All" };

            MenuItem setBreakPoint = new MenuItem() { Header = "Set Breakpoint" };

            //Initialize menu item events
            cut.Click += delegate { this.Cut(); };
            copy.Click += delegate { this.Copy(); };
            paste.Click += delegate { this.Paste(); };
            selectAll.Click += delegate { this.SelectAll(); };
            setBreakPoint.Click += SetBreakPoint_Click;
            //Add menu items to context menu
            contextMenu.Items.Add(cut);
            contextMenu.Items.Add(copy);
            contextMenu.Items.Add(paste);
            contextMenu.Items.Add(selectAll);
            contextMenu.Items.Add(setBreakPoint);

            //Initialize context menu
            ContextMenu = contextMenu;
        }

        private void SetBreakPoint_Click(object sender, RoutedEventArgs e)
        {
            int lineOffset = (Document.GetLineByOffset(CaretOffset).Offset);
            //var line = Document.GetLineByOffset(Document.GetLineByOffset(CaretOffset).Offset);
            //Select(line.Offset, line.Length);

            TextArea.TextView.LineTransformers.Add(new BreakPointColourizer(lineOffset));
        }

        private void Editor_Drop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            PluginHolder.instance.ParentWindow.OpenDocuments(files);
        }

        private void Document_TextChanged(object sender, EventArgs e)
        {
            UpdateFoldings();
        }

        private void TextArea_TextCopied(object sender, TextEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            string wordContext = GetClosedWordToCursor();
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                if (!CompletionData.Any(x => x.Text.StartsWith(wordContext)) || completionWindow != null)
                {
                    var com = new MyCompletionData(wordContext);
                    CompletionData.Add(com);
                }
            }
        }
        private void TextEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if(brackets.ContainsKey(e.Text))
            {
                Document.Insert(CaretOffset, brackets[e.Text]);
                CaretOffset--;
            }

            string wordContext = GetClosedWordToCursor();

            try
            {
                if (CompletionData.Any(x => x.Text.StartsWith(wordContext)))
                {
                    if(completionWindow == null)
                    {
                        // Open code completion after the user has pressed dot:
                        completionWindow = new EditorCompletionWindow(TextArea);

                        completionWindow.Show();
                        completionWindow.Closed += delegate
                        {
                            completionWindow = null;
                        };
                    }
                    else
                    {
                        //Something
                    }
                    
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

            }
          
            
            
        }

        private void TextEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 1 && completionWindow != null)
            {
                string wordContext = GetClosedWordToCursor();

                
                if (!char.IsLetterOrDigit(e.Text[0]) && wordContext != "")
                {
                    if (!CompletionData.Any(x => x.Text.StartsWith(wordContext)) || completionWindow != null)
                    {
                        var com = new MyCompletionData(wordContext);
                        CompletionData.Add(com);
                    }
                    EditorCompletionWindow.CompletionList.RequestInsertion(e);
                }
            }
            //e.Handled = true;
        }

        public void OpenDocument(string filename)
        {
            if (filename == "Untitled") return;

            //Initialize document properties
            OpenedDocument = filename;
            fileStream = File.Open(filename, FileMode.Open);
            Load(fileStream);
            SelectHighlighting(filename);
            InitializeFolding();
        }

        private void SelectHighlighting(string filename)
        {
            //Setup syntax highlighting
            SyntaxHighlighting = highlightingManager.GetHighlightingFromExtension(new FileInfo(filename).Extension);
            //Set up folding strategy
            var highlighting = HighlightingManager.Instance.HighlightingDefinitions.Where(x => x.Name.Contains("ml"));
            foldingStrategy = highlighting.Contains(SyntaxHighlighting) ? new XmlFoldingStrategy() : (object)new BraceFoldingStrategy();
        }

        public void SaveDocument()
        {
            if (IsUnoccupied()) return;
            try
            {
                Save(fileStream);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void SaveAs(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            fileStream.Close();
            fileName = saveFileDialog.FileName;
            fileStream = File.Open(fileName, FileMode.Open);

            if (Properties.Settings.Default.RecentFiles == null)
            {
                Properties.Settings.Default.RecentFiles = new StringCollection();
            }
            if (!Properties.Settings.Default.RecentFiles.Contains(fileName))
            {
                Properties.Settings.Default.RecentFiles.Add(fileName);
                Properties.Settings.Default.Save();
            }
        }

        public void SaveAs()
        {
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                SaveAs(saveFileDialog.FileName);
        }

        private void InitializeFolding()
        {
            if (foldingManager == null)
                foldingManager = FoldingManager.Install(TextArea);
            UpdateFoldings();
        }

        private void UpdateFoldings()
        {
            if (foldingStrategy is BraceFoldingStrategy braceFoldingStrategy)
                braceFoldingStrategy.UpdateFoldings(foldingManager, Document);
            else if (foldingStrategy is XmlFoldingStrategy xmlFoldingStrategy)
                xmlFoldingStrategy.UpdateFoldings(foldingManager, Document);
        }
    }
}