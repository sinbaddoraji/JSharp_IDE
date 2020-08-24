using System.Windows.Controls;

namespace JSharp.PluginCore
{
    public interface IPlugin
    {
        /// <summary>
        /// plug-in name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// plug-in description
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// plug-in author
        /// </summary>
        string Author { get; set; }

        /// <summary>
        /// Version of the plug-in
        /// </summary>
        string Version { get; set; }

        /// <summary>
        /// 0, 1, 2 => left, right, down
        /// </summary>
        int PaneLocation { get; set; }

        /// <summary>
        /// Runs in background
        /// </summary>
        bool IsBackgroundPlugin { get; set; }

        /// <summary>
        /// Is added to "Plug-ins" menu
        /// </summary>
        bool AddToMenu { get; set; }

        /// <summary>
        /// Is added to JSharp Tool-bar
        /// </summary>
        bool IsAddedToToolBar { get; set; }

        /// <summary>
        /// Add to right click menu
        /// </summary>
        bool AddToContextMenu { get; set; }

        /// <summary>
        /// Exported tool-bars to the JSharp window
        /// </summary>
        object[] GetToolbarItems();

        /// <summary>
        /// Exported menu items to the JSharp window
        /// </summary>
        MenuItem[] GetMenuItems();

        /// <summary>
        /// Plug-in Parent window
        /// </summary>
        Windows.MainWindow.MainWindow ParentWindow { get; set; }

        /// <summary>
        /// Initialize plug-ins
        /// </summary>
        Pane[] GetPaneControls(); // User control within pane

        /// <summary>
        /// Initialize plug-ins
        /// </summary>
        void Init();

        /// <summary>
        /// Unload plug-ins
        /// </summary>
        void Unload();
    }
}