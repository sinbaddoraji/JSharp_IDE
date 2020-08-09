using AvalonDock.Layout;
using JSharp.PluginCore;
using System.Collections.Generic;
namespace JSharp
{
    public class Pane
    {
        public object content;
        public string title;
        public int paneLocation;
        public bool isCollapsed;

        public LayoutAnchorable lA;
        public double Width = 280;
        public double Height = 180;

        public Pane(object content, string title, int paneLocation, bool isCollapsed)
        {
            this.content = content;
            this.title = title;
            this.paneLocation = paneLocation;
            this.isCollapsed = isCollapsed;
        }

        public Pane()
        {

        }
    }
}