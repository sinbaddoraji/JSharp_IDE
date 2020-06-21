using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using JSharp.Code_Completion;
using System;
using System.Collections.Generic;
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
        private CompletionWindow completionWindow;

        public static string FilterOptions { get; set; }

        public string OpenedDocument { get; private set; }

        private object foldingStrategy;
        private FoldingManager foldingManager;

        private Stream fileStream;

        public static ContextMenu contextMenu = new ContextMenu();

        public bool IsUnoccupied() => OpenedDocument == null;

        public static Brush TextForeground = null;
        public static Brush TextBackground = null;

        public Editor()
        {
            //Initialize objects
            saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = FilterOptions;
            foldingManager = FoldingManager.Install(TextArea);
            foldingStrategy = new BraceFoldingStrategy();

            //Initialize default properties
            ShowLineNumbers = true;
            FontSize = 12;
            AllowDrop = true;

            //Initialize event handlers
            TextArea.TextEntering += TextEditor_TextArea_TextEntering;
            TextArea.TextEntered += TextEditor_TextArea_TextEntered;
            TextArea.TextCopied += TextArea_TextCopied;
            TextArea.Document.TextChanged += Document_TextChanged;
            Drop += Editor_Drop;

            //Setup other elements
            InitalizeContextMenu();

            if (TextForeground != null) Foreground = TextForeground;
            if (TextBackground != null) Background = TextBackground;
        }

        private void InitalizeContextMenu()
        {
            //Initialize menu item objects
            MenuItem cut = new MenuItem() { Header = "Cut" };
            MenuItem copy = new MenuItem() { Header = "Copy" };
            MenuItem paste = new MenuItem() { Header = "Paste" };
            MenuItem selectAll = new MenuItem() { Header = "Select All" };

            //Initialize menu item events
            cut.Click += delegate { this.Cut(); };
            copy.Click += delegate { this.Copy(); };
            paste.Click += delegate { this.Paste(); };
            selectAll.Click += delegate { this.SelectAll(); };

            //Add menu items to context menu
            contextMenu.Items.Add(cut);
            contextMenu.Items.Add(copy);
            contextMenu.Items.Add(paste);
            contextMenu.Items.Add(selectAll);

            //Initialize context menu
            ContextMenu = contextMenu;
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

        private void TextEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            
            
            if (e.Text == ".")
            {
                // Open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Add(new MyCompletionData("class"));
                data.Add(new MyCompletionData("void"));
                data.Add(new MyCompletionData("int"));
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }
        }

        private void TextEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        public void OpenDocument(string filename)
        {
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
            SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(new FileInfo(filename).Extension);
            //Set up folding strategy
            var highlighting = HighlightingManager.Instance.HighlightingDefinitions.Where(x => x.Name.Contains("ml"));
            foldingStrategy = highlighting.Contains(SyntaxHighlighting) ? new XmlFoldingStrategy() : (object)new BraceFoldingStrategy();
        }

        public void SaveDocument()
        {
            if (IsUnoccupied()) return;
            Save(fileStream);
        }

        public void SaveAs(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            fileStream.Close();
            fileName = saveFileDialog.FileName;
            fileStream = File.Open(fileName, FileMode.Open);
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