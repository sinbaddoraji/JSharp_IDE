using JSharp.PluginCore;
using JSharp.Windows.MainWindow;
using System.Windows.Controls;

namespace PluginList
{
    public class Entry : IPlugin
    {
        private readonly MenuItem pluginsMenuItem = new MenuItem();
        private readonly MenuItem[] plugins = new MenuItem[1];

        public string Name { get; set; } = "Plugins";
        public string Description { get; set; } = "Built-In plugin for viewing JSharp plugins";
        public string Author { get; set; } = "Osinachi Nwagboso";
        public string Version { get; set; } = "1.0.0.0";
        public int PaneLocation { get; set; } = -1;
        public bool IsBackgroundPlugin { get; set; } = false;
        public bool AddToMenu { get; set; } = true;
        public bool IsAddedToToolBar { get; set; } = false;
        public bool AddToContextMenu { get; set; } = false;
        public Main ParentWindow { get; set; }
        public static Main Parent;

        public MenuItem[] GetMenuItems() => plugins;

        public UserControl[] GetPaneControls() => null;

        public object[] GetToolbarItems() => null;

        public void Init()
        {
            plugins[0] = pluginsMenuItem;
            pluginsMenuItem.Header = "Show Plugins";
            pluginsMenuItem.Click += delegate { new ViewPluginsWindow().ShowDialog(); };

            Parent = ParentWindow;
        }

        public void Unload() { }
    }
}


