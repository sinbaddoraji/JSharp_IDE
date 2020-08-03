﻿using System;
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
using System.Windows.Media.Media3D;
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
        #region Fields/Properties

        /// <summary>
        /// Avalon dock panes located at the top left corner of the dock window
        /// </summary>
        private readonly ObservableCollection<Pane> LeftPaneItemsUp = new ObservableCollection<Pane>();

        /// <summary>
        /// Avalon dock panes located at the bottom left corner of the dock window
        /// </summary>
        private readonly ObservableCollection<Pane> LeftPaneItemsDown = new ObservableCollection<Pane>();

        /// <summary>
        /// Avalon dock panes located at the top right corner of the dock window
        /// </summary>
        private readonly ObservableCollection<Pane> RightPaneItemsUp = new ObservableCollection<Pane>();

        /// <summary>
        /// Avalon dock panes located at the lower right corner of the dock window
        /// </summary>
        private readonly ObservableCollection<Pane> RightPaneItemsDown = new ObservableCollection<Pane>();

        /// <summary>
        /// Avalon dock panes located at the Bottom Pane located at the left
        /// </summary>
        private readonly ObservableCollection<Pane> BottomPaneItemsLeft = new ObservableCollection<Pane>();

        /// <summary>
        /// Avalon dock panes located at the Bottom Pane located at the right
        /// </summary>
        private readonly ObservableCollection<Pane> BottomPaneItemsRight = new ObservableCollection<Pane>();

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
        public FileExplorer fileExplorer;

        /// <summary>
        /// Find and Replace Pane for JSharp
        /// </summary>
        public FindReplace findReplacePane;

        /// <summary>
        /// Find and Replace Pane for JSharp
        /// </summary>
        public LayoutAnchorablePane[] panes;

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
        private enum PaneLocation { UpperLeft, LowerLeft, UpperRight, LowerRight, BottomLeft, BottomRight }

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
        public void UseDarkTheme(bool useDarkTheme)
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
                Color fontColour = (Color)ColorConverter.ConvertFromString("#FFDCDCDC");
                TitleForeground = new SolidColorBrush(fontColour);
                
                ThemeManager.Current.ChangeTheme(this, "Light.Cyan");
                ThemeManager.Current.SyncTheme();
            }
        }

        #endregion WindowSettings

        #region Pane Related Functions

        /// <summary>
        /// Add built-in panes to JSharp window
        /// </summary>
        private Task<bool> AddInbuiltPanes()
        {
            //LowerPaneItems.Add(new Pane(new CommandPrompt(), "Command prompt"));
            fileExplorer = new Inbuilt_Panes.FileExplorer();
            fileExplorer.SetDirectory(Environment.CurrentDirectory);
            AddPane(new Pane(fileExplorer, "File Explorer"), (int)PaneLocation.UpperLeft);

            findReplacePane = new FindReplace();
            AddPane(new Pane(findReplacePane, "Find and Replace"), (int)PaneLocation.LowerLeft);

            UserControl OutputWindow = new UserControl
            {
                Content = DebugCore.OutputTextbox
            };
            AddPane(new Pane(OutputWindow, "Output Window"), (int)PaneLocation.BottomLeft);

            //_terminal = new JSharpTerminal();
            //AddPane(new Pane(_terminal, "Terminal"), 4);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Initialize all currently known panes to JSharp
        /// </summary>
        private Task<bool> InitalizePanes()
        {
            for (int i = 0; i < LeftPaneItemsUp.Count; i++)
            {
                Pane pane = LeftPaneItemsUp[i];
                AddPane(pane, 0);
            }
            for (int i = 0; i < LeftPaneItemsDown.Count; i++)
            {
                Pane pane = LeftPaneItemsDown[i];
                AddPane(pane, 1);
            }
            for (int i = 0; i < RightPaneItemsUp.Count; i++)
            {
                Pane pane = RightPaneItemsUp[i];
                AddPane(pane, 2);
            }
            for (int i = 0; i < RightPaneItemsDown.Count; i++)
            {
                Pane pane = RightPaneItemsDown[i];
                AddPane(pane, 3);
            }
            for (int i = 0; i < BottomPaneItemsLeft.Count; i++)
            {
                Pane pane = BottomPaneItemsLeft[i];
                AddPane(pane, 4);
            }
            for (int i = 0; i < BottomPaneItemsRight.Count; i++)
            {
                Pane pane = BottomPaneItemsRight[i];
                AddPane(pane, 5);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Add pane to JSharp window
        /// </summary>
        public void AddPane(Pane pane, int paneLocation)
        {
            if (pane == null) return;
            LayoutAnchorable lA = new LayoutAnchorable { Title = pane.title };
            lA.Content = pane.content;
            
            ((UserControl)pane.content).HorizontalAlignment = HorizontalAlignment.Stretch;
            ((UserControl)pane.content).VerticalAlignment = VerticalAlignment.Stretch;

            if(panes == null)
            {
                panes = new[] { LeftPane, LeftPane2, RightPane, RightPane2, LowerPane, LowerPane2 };
            }
            panes[paneLocation].Children.Add(lA);
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
        public void SelectDocumentTab(int index)
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
                    TextEditor.TextEditor.GlobalEditorContntextMenu.Items.Add(MenuItems[i]);

                if (plugin.AddToMenu)
                    PluginMenu.Items.Add(MenuItems[i]);
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