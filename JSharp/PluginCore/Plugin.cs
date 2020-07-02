using System.Windows.Controls;

namespace JSharp.PluginCore
{
    public abstract class Plugin
    {
        /// <summary>
        /// plug-in name
        /// </summary>
        public string Name { get; set; } = "JSharp Plug-in";

        /// <summary>
        /// plug-in description
        /// </summary>
        public string Description { get; set; } = "Just a JSharp plug-in";

        /// <summary>
        /// plug-in author
        /// </summary>
        public string Author { get; set; } = "Osinachi Nwagboso";

        /// <summary>
        /// Version of the plug-in
        /// </summary>
        public string Version { get; set; } = "0.0.0.0";

        /// <summary>
        /// 0, 1, 2 => left, right, down
        /// </summary>
        public int PaneLocation { get; set; } = 0;

        /// <summary>
        /// Runs in background
        /// </summary>
        public bool IsBackgroundPlugin { get; set; }

        /// <summary>
        /// Is added to "Plug-ins" menu
        /// </summary>
        public bool AddToMenu { get; set; }

        /// <summary>
        /// Is added to JSharp Tool-bar
        /// </summary>
        public bool IsAddedToToolBar { get; set; }

        /// <summary>
        /// Add to right click menu
        /// </summary>
        public bool AddToContextMenu { get; set; }

        /// <summary>
        /// Has a physical window
        /// </summary>
        public bool HasWindow { get; set; }

        /// <summary>
        /// Exported tool-bars to the JSharp window
        /// </summary>
        public abstract object[] GetToolbarItems();

        /// <summary>
        /// Exported menu items to the JSharp window
        /// </summary>
        public abstract MenuItem[] GetMenuItems();

        /// <summary>
        /// Plug-in Parent window
        /// </summary>
        public Main ParentWindow { get; set; }

        /// <summary>
        /// Initialize plug-ins
        /// </summary>
        public abstract UserControl[] GetPaneControls(); // User control within pane

        /// <summary>
        /// Initialize plug-ins
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Unload plug-ins
        /// </summary>
        public abstract void Unload();
    }
}