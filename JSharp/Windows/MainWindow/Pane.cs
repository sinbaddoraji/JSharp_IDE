using AvalonDock.Layout;
using JSharp.PluginCore;
using System.Collections.Generic;
namespace JSharp
{
    public class Pane
    {
        public object _content;
        public string _title;
        public int _paneLocation;
        public bool _isAutoHide;

        public LayoutAnchorable _parentPaneHolder;
        public double _width = 280;
        public double _height = 180;

        public Pane(object content, string title, int paneLocation, bool isCollapsed)
        {
            this._content = content;
            this._title = title;
            this._paneLocation = paneLocation;
            this._isAutoHide = isCollapsed;
        }

        public Pane()
        {

        }
    }
}