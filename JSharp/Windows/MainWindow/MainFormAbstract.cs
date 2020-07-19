using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using AvalonDock.Layout;
using AvalonDock.Themes;
using ControlzEx.Theming;
using JSharp.Inbuilt_Panes;
using JSharp.PluginCore;
using JSharp.Properties;
using JSharp.TextEditor;
using MahApps.Metro.Controls;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace JSharp.Windows.MainWindow
{
    public partial class Main : MetroWindow
    {
        /*
         * This class contains all the abstract definitions for the main window
         * Note: public properties, objects and so on are public for plug-in access and control
         */

        /*Property Declarations*/

        //Pane Related properties
        private readonly ObservableCollection<Pane> LeftPaneItemsUp = new ObservableCollection<Pane>();
        private readonly ObservableCollection<Pane> LeftPaneItemsDown = new ObservableCollection<Pane>();
        private readonly ObservableCollection<Pane> RightPaneItemsUp = new ObservableCollection<Pane>();
        private readonly ObservableCollection<Pane> RightPaneItemsDown = new ObservableCollection<Pane>();
        private readonly ObservableCollection<Pane> BottomPaneItemsLeft = new ObservableCollection<Pane>();
        private readonly ObservableCollection<Pane> BottomPaneItemsRight = new ObservableCollection<Pane>();
        //Dialogues
        private readonly OpenFileDialog openFileDialog;

        public string ProjectFolder;

        //Document Related Properties
        private IEnumerable<string> OpenedFiles
        {
            get
            {
                foreach (var document in DocumentPane.Children)
                {
                    yield return ((Editor)document.Content).OpenedDocument;
                }
            }
        }

        public StringCollection GetOpenedFiles(bool fromSettings)
        {
            if(fromSettings) return Settings.Default.OpenedFiles;
            StringCollection stringCollection = new StringCollection();
            foreach (var str in OpenedFiles)
            {
                stringCollection.Add(str);
            }
            Settings.Default.OpenedFiles = stringCollection;
            Settings.Default.Save();
            return Settings.Default.OpenedFiles;
            
        }

        public FileExplorer fileExplorer;

        public string GetSelectedFile(bool shortName)
        {
            if (DocumentPane.SelectedContent == null) return "JSharp";

            var o = DocumentPane.SelectedContent.Content;
            if (o.GetType() != typeof(Editor)) return "JSharp";
            return shortName ? ((Editor)o).OpenedDocumentShortName : ((Editor)o).OpenedDocument;
        }

        public Editor GetSelectedDocument()
        {
            if (DocumentPane.SelectedContent == null) return null;

            var o = DocumentPane.SelectedContent.Content;
            if (o.GetType() != typeof(Editor)) return null;
            return (Editor)o;
        }

        public void SetSelectedDocument(Editor value)
        {
            var o = DocumentPane.SelectedContent.Content;
            if (o.GetType() == typeof(Editor))
                DocumentPane.SelectedContent.Content = value;
        }

        public LayoutDocumentPane Documents => DocumentPane;

        private LayoutContent SelectedDocumentFrame => DocumentPane.SelectedContent;

        #region WindowSettings

        public void SetWindowTheme(bool darkMode)
        {
            if (darkMode)
            {
                //Visual studio dark theme
                DockManager.Theme = new Vs2013DarkTheme();
                ThemeManager.Current.ChangeTheme(this, "Dark.Blue");
                ThemeManager.Current.SyncTheme();
                Background = DockManager.Background;
                Foreground = new SolidColorBrush(Colors.White);
                Editor.TextBackground = Background;
                Editor.TextForeground = Foreground;
                WindowTitleBrush = Background;
                TitleForeground = Foreground;
                //Set pane colours
            }
            else
            {
                //Visual studio dark theme
                DockManager.Theme = new Vs2013LightTheme();
                Foreground = new SolidColorBrush(Colors.Black);
                
                
                Background = Background;

                Menu.Background = Background;
                Color fontColour = (Color)ColorConverter.ConvertFromString("#FFDCDCDC");
                TitleForeground = new SolidColorBrush(fontColour);
                
                ThemeManager.Current.ChangeTheme(this, "Light.Cyan");
                ThemeManager.Current.SyncTheme();
            }
        }

        #endregion WindowSettings

        #region Pane Related Functions

        FindReplace findReplacePane;
        //JSharpTerminal _terminal;

        private Task<bool> AddInbuiltPanes()
        {
            //LowerPaneItems.Add(new Pane(new CommandPrompt(), "Command prompt"));
            fileExplorer = new Inbuilt_Panes.FileExplorer();
            fileExplorer.SetDirectory(Environment.CurrentDirectory);
            AddPane(new Pane(fileExplorer, "File Explorer"), 0);

            findReplacePane = new FindReplace();
            AddPane(new Pane(findReplacePane, "Find and Replace"), 1);

            //_terminal = new JSharpTerminal();
            //AddPane(new Pane(_terminal, "Terminal"), 4);

            return Task.FromResult(true);
        }

        private Task<bool> InitalizePanes()
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
            return Task.FromResult(true);
        }

        public void AddPane(Pane pane, int paneLocation)
        {
            if (pane == null) return;
            LayoutAnchorable lA = new LayoutAnchorable { Title = pane.title };
            lA.Content = pane.content;
            ((UserControl)pane.content).HorizontalAlignment = HorizontalAlignment.Stretch;
            ((UserControl)pane.content).VerticalAlignment = VerticalAlignment.Stretch;

            new[] { LeftPane, LeftPane2, RightPane, RightPane2, LowerPane, LowerPane2 }[paneLocation].Children.Add(lA);
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
            
           

            DocumentPane.Children.Add(newDocument);
            SelectTab((int) DocumentPane.IndexOfChild(newDocument));
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
            IList list = previouslyOpenedDocuments;
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
            if (Settings.Default.RecentFiles == null)
            {
                Settings.Default.RecentFiles = new StringCollection();
            }
            if(!Settings.Default.RecentFiles.Contains(filename))
            {
                Settings.Default.RecentFiles.Add(filename);
                Settings.Default.Save();
            }
            if (OpenedFiles.Contains(filename)) return;
            if (DocumentPane.Children.Count == 0)
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
            DocumentPane.SelectedContentIndex = index;
        }

        public void SelectTab(string filename)
        {
            //SelectTab(documents.Children.IndexOf(tab));
        }

        #endregion Document Related Functions

        #region Plug-in Related Functions

        private void LoadPlugin(IPlugin plugin)
        {
            if (plugin.IsBackgroundPlugin)
                return;

            var paneControls = plugin.GetPaneControls();
            if (paneControls?.Length > 0)
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
                    ToolBar.Items.Add(ToolBarItems[i]);
            }

            var MenuItems = plugin.GetMenuItems();
            if (MenuItems == null) return;

            for (int i = 0; i < MenuItems.Length; i++)
            {
                //Add menu items either to context menu or plug-ins menu
                if (!plugin.AddToContextMenu && !plugin.AddToMenu) break;

                if (plugin.AddToContextMenu)
                    Editor.EditorContntextMenu.Items.Add(MenuItems[i]);

                if (plugin.AddToMenu)
                    PluginMenu.Items.Add(MenuItems[i]);
            }
        }

        private Task<bool> LoadPlugins()
        {
            PluginHolder.Instance = new PluginHolder { ParentWindow = this };
            PluginHolder.Instance.Load();

            foreach (IPlugin plugin in PluginHolder.Instance.RegisteredPlugins)
            {
                LoadPlugin(plugin);
            }

            return Task.FromResult(true);
        }

        #endregion Plug-in Related Functions
    }
}