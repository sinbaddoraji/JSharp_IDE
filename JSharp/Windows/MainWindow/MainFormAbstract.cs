using AvalonDock.Layout;
using AvalonDock.Themes;
using ControlzEx.Theming;
using ICSharpCode.AvalonEdit.Highlighting;
using JSharp.Inbuilt_Panes;
using JSharp.PluginCore;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JSharp
{
    public partial class Main : MetroWindow
    {
        /*
         * This class contains all the abstract definitions for the main window
         * Note: public properties, objects and so on are public for plugin access and control
         */

        /*Property Declarations*/

        //Pane Related properties
        private readonly ObservableCollection<Pane> LeftPaneItemsUp = new ObservableCollection<Pane>();
        private readonly ObservableCollection<Pane> LeftPaneItemsDown = new ObservableCollection<Pane>();
        private readonly ObservableCollection<Pane> RightPaneItemsUp = new ObservableCollection<Pane>();
        private readonly ObservableCollection<Pane> RightPaneItemsDown = new ObservableCollection<Pane>();
        private readonly ObservableCollection<Pane> BottomPaneItemsLeft = new ObservableCollection<Pane>();
        private readonly ObservableCollection<Pane> BottomPaneItemsRight = new ObservableCollection<Pane>();
        //Dialogs
        private readonly System.Windows.Forms.OpenFileDialog openFileDialog;

        //Document Related Properties
        private IEnumerable<string> OpenedFiles
        {
            get
            {
                foreach (var document in documents.Children)
                {
                    yield return ((Editor)document.Content).OpenedDocument;
                }
            }
        }

        public StringCollection GetOpenedFiles(bool fromSettings)
        {
            if(fromSettings) return Properties.Settings.Default.OpenedFiles;

            StringCollection stringCollection = new StringCollection();
            foreach (var str in OpenedFiles)
            {
                stringCollection.Add(str);
            }
            Properties.Settings.Default.OpenedFiles = stringCollection;
            Properties.Settings.Default.Save();
            return Properties.Settings.Default.OpenedFiles;
        }

        public FileExplorer fileExplorer;

        public string GetSelectedFile
        {
            get
            {
                if (documents.SelectedContent == null) return "JSharp";

                var o = documents.SelectedContent.Content;
                if (o.GetType() != typeof(Editor)) return "JSharp";
                else return ((Editor)o).OpenedDocument;
            }
        }

        public Editor GetSelectedDocument()
        {
            if (documents.SelectedContent == null) return null;

            var o = documents.SelectedContent.Content;
            if (o.GetType() != typeof(Editor)) return null;
            else return (Editor)o;
        }

        public void SetSelectedDocument(Editor value)
        {
            var o = documents.SelectedContent.Content;
            if (o.GetType() == typeof(Editor))
                documents.SelectedContent.Content = value;
        }

        public LayoutDocumentPane Documents => documents;

        private LayoutContent SelectedDocumentFrame => documents.SelectedContent;

        private IOrderedEnumerable<IHighlightingDefinition> HighlightCollection { get; set; }

        #region WindowSettings

        public void SetWindowTheme(bool darkMode)
        {
            if (darkMode)
            {
                //Visual studio dark theme
                dockManager.Theme = new Vs2013DarkTheme();
                ThemeManager.Current.ChangeTheme(this, "Dark.Blue");
                ThemeManager.Current.SyncTheme();
                Background = dockManager.Background;
                Foreground = new SolidColorBrush(Colors.White);
                //Setup menu colors
               // menu.Background = Background;
                //menu.Foreground = Foreground;
                //Setup editor colors
                
                Editor.TextBackground = Background;
                Editor.TextForeground = Foreground;
                WindowTitleBrush = Background;
                TitleForeground = Foreground;
                //Set pane colours
            }
            else
            {
                //Visual studio dark theme
                dockManager.Theme = new Vs2013LightTheme();
                Foreground = new SolidColorBrush(Colors.Black);
                
                
                menu.Background = Background;
                
                //WindowTitleBrush = new SolidColorBrush(Colors.White);
                TitleForeground = new SolidColorBrush(Colors.Black);
                ThemeManager.Current.ChangeTheme(this, "Light.Cyan");
                statusBar.Background = WindowTitleBrush;
                ThemeManager.Current.SyncTheme();
            }
        }

        #endregion WindowSettings

        #region Pane Related Functions

        FindReplace findReplacePane;
        private void AddInbuiltPanes()
        {
            //LowerPaneItems.Add(new Pane(new CommandPrompt(), "Command prompt"));
            fileExplorer = new FileExplorer();
            fileExplorer.SetDirectory(Environment.CurrentDirectory);
            AddPane(new Pane(fileExplorer, "File Explorer"), 0);

            findReplacePane = new FindReplace();
            AddPane(new Pane(findReplacePane, "Find and Replace"), 1);
        }

        private void InitalizePanes()
        {
            foreach (var pane in LeftPaneItemsUp)
            {
                AddPane(pane, 0);
            }
            foreach (var pane in LeftPaneItemsDown)
            {
                AddPane(pane, 1);
            }
            foreach (var pane in RightPaneItemsUp)
            {
                AddPane(pane, 2);
            }
            foreach (var pane in RightPaneItemsDown)
            {
                AddPane(pane, 3);
            }
            foreach (var pane in BottomPaneItemsLeft)
            {
                AddPane(pane, 4);
            }
            foreach (var pane in BottomPaneItemsRight)
            {
                AddPane(pane, 5);
            }
            AddDocumentPage("untitled");//Add Default Page
        }

        public void AddPane(Pane pane, int paneLocation)
        {
            if (pane == null) return;
            LayoutAnchorable lA = new LayoutAnchorable { Title = pane.title };
            
            lA.Content = pane.content;
            ((UserControl)pane.content).HorizontalAlignment = HorizontalAlignment.Stretch;
            ((UserControl)pane.content).VerticalAlignment = VerticalAlignment.Stretch;

            new[] { leftPane, leftPane2, rightPane, rightPane2, LowerPane, LowerPane2 }[paneLocation].Children.Add(lA);
        }

        #endregion Pane Related Functions

        #region Document Related Functions

        private void AddDocumentPage(string fName = null, Editor e = null)
        {
            LayoutDocument newDocument = new LayoutDocument
            {
                Title = fName ?? "Untitled Document",
                Content = e ?? new Editor()
            };

            ((Editor)newDocument.Content).HorizontalAlignment = HorizontalAlignment.Stretch;
            ((Editor)newDocument.Content).VerticalAlignment = VerticalAlignment.Stretch;
            
           

            documents.Children.Add(newDocument);
            SelectTab(documents.IndexOfChild(newDocument));
        }

        public void OpenDocuments(string[] filenames)
        {
            foreach (var file in filenames)
            {
                try
                {
                    OpenDocument(file);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void OpenDocuments(StringCollection previouslyOpenedDocuments)
        {
            System.Collections.IList list = previouslyOpenedDocuments;
            if (list == null || list.Count < 1) return;

            for (int i = 0; i < list.Count; i++)
            {
                string file = (string)list[i];
                try
                {
                    OpenDocument(file);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public void OpenDocument(string filename)
        {
            if (Properties.Settings.Default.RecentFiles == null)
            {
                Properties.Settings.Default.RecentFiles = new StringCollection();
            }
            if(!Properties.Settings.Default.RecentFiles.Contains(filename))
            {
                Properties.Settings.Default.RecentFiles.Add(filename);
                Properties.Settings.Default.Save();
            }
            if (OpenedFiles.Contains(filename)) return;
            if (documents.Children.Count == 0)
            {
                AddDocumentPage("untitled");//Add Default Page
            }

            Editor ex;
            string name = Path.GetFileName(filename);
            if (GetSelectedDocument().IsUnoccupied())
            {
                ex = GetSelectedDocument();
                SelectedDocumentFrame.Title = name;
            }
            else
            {
                ex = new Editor();
                AddDocumentPage(name, ex);
            }
            ex.OpenDocument(filename);
        }

        public void SelectTab(int index)
        {
            documents.SelectedContentIndex = index;
        }

        public void SelectTab(string filename)
        {
            //SelectTab(documents.Children.IndexOf(tab));
        }

        #endregion Document Related Functions

        #region Plug-in Related Functions

        private void LoadPlugin(Plugin plugin)
        {
            if (plugin.IsBackgroundPlugin)
                return;

            var paneControls = plugin.GetPaneControls();
            if (paneControls != null && paneControls.Length > 0)
            {
                for (int i = 0; i < paneControls.Length; i++)
                {
                    AddPane(new Pane(paneControls[i], paneControls[i].Name), 0);
                }
            }

            if (plugin.IsAddedToToolBar)
            {
                var ToolBarItems = plugin.GetToolbarItems();
                //Add toolbar items
                for (int i = 0; i < ToolBarItems.Length; i++)
                    toolBar.Items.Add(ToolBarItems[i]);
            }

            var MenuItems = plugin.GetMenuItems();
            if (MenuItems == null) return;

            for (int i = 0; i < MenuItems.Length; i++)
            {
                //Add menu items either to context menu or plugins menu
                if (!plugin.AddToContextMenu && !plugin.AddToMenu) break;

                if (plugin.AddToContextMenu)
                    Editor.contextMenu.Items.Add(MenuItems[i]);

                if (plugin.AddToMenu)
                    PluginMenu.Items.Add(MenuItems[i]);
            }
        }

        private void LoadPlugins()
        {
            PluginHolder.instance = new PluginHolder { ParentWindow = this };
            PluginHolder.instance.Load();

            foreach (Plugin plugin in PluginHolder.instance.RegisteredPlugins)
            {
                LoadPlugin(plugin);
            }
        }

        #endregion Plugin Related Functions
    }
}