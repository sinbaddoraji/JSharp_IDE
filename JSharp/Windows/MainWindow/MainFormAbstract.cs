using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml;
using AvalonDock.Layout;
using AvalonDock.Themes;
using ControlzEx.Theming;
using JSharp.Inbuilt_Panes;
using JSharp.PluginCore;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace JSharp.Windows.MainWindow
{
    public partial class Main
    {
        #region Fields/Properties

        /// <summary>
        /// OpenFileDialog used for opening files as documents in JSharp
        /// </summary>
        private readonly OpenFileDialog openFileDialog;

        /// <summary>
        /// Path to folder being used as folder directory
        /// </summary>
        public string ProjectFolder;

        /// <summary>
        /// File Explorer for JSharp
        /// </summary>
        private FileExplorer fileExplorer;

        /// <summary>
        /// Find and Replace Pane for JSharp
        /// </summary>
        private FindReplace findReplacePane;

        /// <summary>
        /// List of files open in JSharp
        /// </summary>
        private IEnumerable<string> OpenedFiles => DocumentPane.Children.Select(document => ((TextEditor.TextEditor)document.Content).OpenedDocument);

        /// <summary>
        /// List of plug-ins that have successfully been loaded in JSharp
        /// </summary>
        public List<IPlugin> RegisteredPlugins => PluginHolder.Instance.RegisteredPlugins;

        /// <summary>
        /// List document panes initialized in JSharp
        /// </summary>
        public LayoutDocumentPane Documents => DocumentPane;

        /// <summary>
        /// Selected Document pane
        /// </summary>
        private LayoutContent SelectedDocumentPane => DocumentPane.SelectedContent;

        /// <summary>
        /// JSharp pane window location
        /// </summary>
        private enum PaneLocation { Left, Right, Bottom}

        /// <summary>
        /// JSharp pane window location
        /// </summary>
        private Dictionary<string, Pane> dictionaryOfPanes = new Dictionary<string, Pane>();

        /// <summary>
        /// Get the name of the selected document
        /// </summary>
        public string GetSelectedFile(bool shortName)
        {
            if (DocumentPane.SelectedContent == null) return "JSharp";

            var o = DocumentPane.SelectedContent.Content;
            if (o.GetType() != typeof(TextEditor.TextEditor)) return "JSharp";
            return shortName ? ((TextEditor.TextEditor)o).OpenedDocumentShortName : ((TextEditor.TextEditor)o).OpenedDocument;
        }

        /// <summary>
        /// Get the selected document editor
        /// </summary>
        public TextEditor.TextEditor GetSelectedTextEditor()
        {
            if (DocumentPane.SelectedContent == null || DocumentPane.SelectedContent.Content == null) return null;

            var o = DocumentPane.SelectedContent.Content;
            if (o.GetType() != typeof(TextEditor.TextEditor)) return null;
            return (TextEditor.TextEditor)o;
        }

        #endregion

        #region WindowSettings

        /// <summary>
        /// Get the selected document editor
        /// </summary>
        private void UseDarkTheme(bool useDarkTheme)
        {
            if (useDarkTheme)
            {
                //Visual studio dark theme
                DockManager.Theme = new Vs2013DarkTheme();
                ThemeManager.Current.ChangeTheme(this, "Dark.Blue");
                ThemeManager.Current.SyncTheme();
                Background = DockManager.Background;
                Foreground = new SolidColorBrush(Colors.White);
                TextEditor.TextEditor.TextBackground = Background;
                TextEditor.TextEditor.TextForeground = Foreground;
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
                var fontColour = (Color)ColorConverter.ConvertFromString("#FFDCDCDC");
                TitleForeground = new SolidColorBrush(fontColour);
                
                ThemeManager.Current.ChangeTheme(this, "Light.Cyan");
                ThemeManager.Current.SyncTheme();
            }
        }

        /// <summary>
        /// Perform closing commands (Make sure all settings are saved)
        /// </summary>
        private void PerformClosingCommands()
        {
            //Unload all registered plugins incase none managable objects were used in plugins
            PluginHolder.Instance.UnloadAllRegisteredPlugins();

            SaveAllDocuments();

            Properties.Settings.Default.OpenedFiles.Clear();
            foreach (var document in DocumentPane.Children)
            {
                var child = ((TextEditor.TextEditor)document.Content);

                if (child.OpenedDocument != null)
                    Properties.Settings.Default.OpenedFiles.Add(child.OpenedDocument);
            }

            UpdatePanes();
            //Write layout
            XmlWriter xmlWriter = XmlWriter.Create("layout.xml");
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("panes");

            foreach (var pane in dictionaryOfPanes)
            {
                xmlWriter.WriteStartElement("pane");
                xmlWriter.WriteAttributeString("title", pane.Key);
                xmlWriter.WriteAttributeString("paneLocation", pane.Value.paneLocation.ToString());
                xmlWriter.WriteAttributeString("width", pane.Value.Width.ToString());
                xmlWriter.WriteAttributeString("height", pane.Value.Height.ToString());
                xmlWriter.WriteAttributeString("isCollapsed", pane.Value.isCollapsed.ToString());
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            Properties.Settings.Default.Save();

            DebugCore.CloseProcess();
        }
        #endregion WindowSettings

        #region Pane Related Functions

        /// <summary>
        /// Add built-in panes to JSharp window
        /// </summary>
        private Task<bool> AddInbuiltPanes()
        {
            //LowerPaneItems.Add(new Pane(new CommandPrompt(), "Command prompt"));
            fileExplorer = new FileExplorer();
            fileExplorer.SetDirectory(Environment.CurrentDirectory);
            AddPane(new Pane(fileExplorer, "File Explorer", (int)(AnchorableShowStrategy.Left), false));

            findReplacePane = new FindReplace();
            AddPane(new Pane(findReplacePane, "Find and Replace", (int)(AnchorableShowStrategy.Left), false));

            var OutputWindow = new UserControl
            {
                Content = DebugCore.OutputTextbox
            };
            AddPane(new Pane(OutputWindow, "Output Window", (int)(AnchorableShowStrategy.Bottom), false));

            //_terminal = new JSharpTerminal();
            //AddPane(new Pane(_terminal, "Terminal"), 4);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Initialize all currently known panes to JSharp
        /// </summary>
        private Task<bool> InitalizePanes()
        {
            if (dictionaryOfPanes == null)
                dictionaryOfPanes = new Dictionary<string, Pane>();

            if(!File.Exists("layout.xml")) return Task.FromResult(true);
            try
            {
                XmlReader reader = XmlReader.Create("layout.xml");

                reader.MoveToContent();
                // Parse the file and display each of the nodes.

                while (reader.Read())
                {
                    if (reader.Name == "pane")
                    {
                        Pane currentPane = new Pane
                        {
                            title = reader.GetAttribute("title"),
                            paneLocation = int.Parse(reader.GetAttribute("paneLocation")),
                            isCollapsed = bool.Parse(reader.GetAttribute("isCollapsed")),
                            Width = double.Parse(reader.GetAttribute("width")),
                            Height = double.Parse(reader.GetAttribute("height"))
                        };

                        if (!dictionaryOfPanes.ContainsKey(currentPane.title))
                            dictionaryOfPanes.Add(currentPane.title, currentPane);
                    }
                    reader.MoveToNextAttribute();
                }
            }
            catch (Exception)
            {
                
            }
           

            return Task.FromResult(true);
        }

        /// <summary>
        /// Add pane to JSharp window
        /// </summary>
        private void AddPane(Pane pane)
        {
            if (pane == null) return;

            var lA = new LayoutAnchorable
            {
                Title = pane.title,
                Content = pane.content
            };
            pane.lA = lA;

            if (!dictionaryOfPanes.ContainsKey(pane.title))
                dictionaryOfPanes.Add(pane.title, pane);
            else

            {
                dictionaryOfPanes[pane.title].lA = pane.lA;
                pane.paneLocation = dictionaryOfPanes[pane.title].paneLocation;
                pane.isCollapsed = dictionaryOfPanes[pane.title].isCollapsed;
                pane.Width = dictionaryOfPanes[pane.title].Width;
                pane.Height = dictionaryOfPanes[pane.title].Height;
            }

            ((UserControl)pane.content).HorizontalAlignment = HorizontalAlignment.Stretch;
            ((UserControl)pane.content).VerticalAlignment = VerticalAlignment.Stretch;

            //panes[pane.paneLocation].Children.Add(lA);
            lA.CanDockAsTabbedDocument = false;
            lA.AddToLayout(DockManager, (AnchorableShowStrategy)pane.paneLocation);


            if (lA.Parent is LayoutAnchorablePane p)
            {
                p.DockWidth = new GridLength(pane.Width);
                p.DockHeight = new GridLength(pane.Height);
            }
            else if (lA.Parent is LayoutAnchorablePaneGroup pG)
            {

                pG.DockWidth = new GridLength(pane.Width);
                pG.DockHeight = new GridLength(pane.Height);
            }

            lA.AutoHideHeight = pane.Height;
            lA.AutoHideWidth = pane.Width;
            
            if (pane.isCollapsed)
            {
                lA.ToggleAutoHide();
            }
        }

        private void UpdatePanes()
        {
            foreach (var pane in dictionaryOfPanes)
            {
                switch (pane.Value.lA.Parent.GetSide())
                {
                    case AnchorSide.Left:
                        pane.Value.paneLocation = (int)AnchorableShowStrategy.Left;
                        break;
                    case AnchorSide.Top:
                        pane.Value.paneLocation = (int)AnchorableShowStrategy.Top;
                        break;
                    case AnchorSide.Right:
                        pane.Value.paneLocation = (int)AnchorableShowStrategy.Right;
                        break;
                    case AnchorSide.Bottom:
                        pane.Value.paneLocation = (int)AnchorableShowStrategy.Bottom;
                        break;
                }

                if(pane.Value.lA.IsAutoHidden)
                {
                    pane.Value.Width = pane.Value.lA.AutoHideWidth;
                    pane.Value.Height = pane.Value.lA.AutoHideHeight;
                }
                else
                {
                    if (pane.Value.lA.Parent is LayoutAnchorablePane p)
                    {
                        pane.Value.Width = p.DockWidth.Value;
                        pane.Value.Height = p.DockHeight.Value;
                    }
                    else if (pane.Value.lA.Parent is LayoutAnchorablePaneGroup pG)
                    {
                        pane.Value.Width = pG.DockWidth.Value;
                        pane.Value.Height = pG.DockHeight.Value;
                    }
                }
               

                pane.Value.isCollapsed = pane.Value.lA.IsAutoHidden;
            }
        }

        #endregion Pane Related Functions

        #region Document Related Functions

        /// <summary>
        /// Add document page to JSharp (Empty Text-Editor without any open editor or a specified TextEditor
        /// </summary>
        private void AddDocumentPage(string fileName, TextEditor.TextEditor e)
        {
            LayoutDocument newDocument = new LayoutDocument
            {
                Title = fileName ?? "Untitled Document",
                Content = e ?? new TextEditor.TextEditor()
            };

            ((TextEditor.TextEditor)newDocument.Content).HorizontalAlignment = HorizontalAlignment.Stretch;
            ((TextEditor.TextEditor)newDocument.Content).VerticalAlignment = VerticalAlignment.Stretch;

            ((TextEditor.TextEditor)newDocument.Content).DocumentChanged += delegate
            {
                newDocument.Title = ((TextEditor.TextEditor)newDocument.Content).OpenedDocumentShortName;
            };

            newDocument.IsSelectedChanged += NewDocument_IsSelectedChanged;

            DocumentPane.Children.Add(newDocument);
            SelectDocumentTab(DocumentPane.IndexOfChild(newDocument));
        }

        /// <summary>
        /// Open an array of files as JSharp documents
        /// </summary>
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

        /// <summary>
        /// Open file as JSharp  document
        /// </summary>
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

            TextEditor.TextEditor ex;
            string name = Path.GetFileName(filename);

            if (GetSelectedTextEditor()?.IsUnoccupied() == true)
            {
                ex = GetSelectedTextEditor();
                SelectedDocumentPane.Title = name;
            }
            else
            {
                ex = new TextEditor.TextEditor();
                AddDocumentPage(name, ex);
            }
            ex.OpenDocument(filename);
        }

        /// <summary>
        /// Save all currently open JSharp documents to their files
        /// </summary>
        private void SaveAllDocuments()
        {
            foreach (var document in DocumentPane.Children)
            {
                ((TextEditor.TextEditor)document.Content).SaveDocument();
            }
        }

        /// <summary>
        /// Select document tab
        /// </summary>
        private void SelectDocumentTab(int index)
        {
            DocumentPane.SelectedContentIndex = index;
        }

        #endregion Document Related Functions

        #region Plug-in Related Functions

        /// <summary>
        /// Load JSharp Plugin
        /// </summary>
        private void LoadPlugin(IPlugin plugin)
        {
            if (plugin.IsBackgroundPlugin)
                return;

            var paneControls = plugin.GetPaneControls();
            if (paneControls?.Length > 0)
            {
                foreach (var pane in paneControls)
                {
                    AddPane(pane);
                }
            }

            if (plugin.IsAddedToToolBar)
            {
                var toolBarItems = plugin.GetToolbarItems();
                //Add toolbar items
                foreach (var t in toolBarItems)
                    ToolBar.Items.Add(t);
            }

            var menuItems = plugin.GetMenuItems();
            if (menuItems == null) return;

            foreach (var menuItem in menuItems)
            {
                //Add menu items either to context menu or plug-ins menu
                if (!plugin.AddToContextMenu && !plugin.AddToMenu) break;

                if (plugin.AddToContextMenu)
                    TextEditor.TextEditor.GlobalEditorContntextMenu.Items.Add(menuItem);

                if (plugin.AddToMenu)
                    PluginMenu.Items.Add(menuItem);
            }
        }

        /// <summary>
        /// Load all Plugins currently known to JSharp
        /// </summary>
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