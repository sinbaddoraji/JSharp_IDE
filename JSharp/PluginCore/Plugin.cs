using System.Windows.Controls;

namespace JSharp.PluginCore
{
    public abstract class Plugin
    {
        public string Name { get; set; } = "JSharp Plugin";

        public string Description { get; set; } = "Just a JSharp plugin";

        public string Author { get; set; } = "Osinachi Nwagboso";

        public string Version { get; set; } = "0.0.0.0";

        public int PaneLocation { get; set; } = 0; //0, 1, 2 => left, right, down

        public bool IsBackgroundPlugin { get; set; } = false; // Runs in background

        public bool AddToMenu { get; set; } = false; //Is added to "Plugins" menu

        public bool IsAddedToToolBar { get; set; } = false; //Is added to JSharp Toolbar

        public bool AddToContextMenu { get; set; } = false; //Add to right click menu

        public bool HasWindow { get; set; } = false; // Has a physical window

        public abstract object[] GetToolbarItems();

        public abstract MenuItem[] GetMenuItems();

        public Main ParentWindow { get; set; }

        public abstract UserControl[] GetPaneControls(); // User control within pane

        public abstract void Init();

        public abstract void Unload();
    }
}