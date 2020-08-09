using JSharp.PluginCore;
namespace JSharp
{
    public class Pane
    {
        public readonly object content;
        public readonly string title;
        public readonly int paneLocation;

        public Pane(object content, string title, int paneLocation)
        {
            this.content = content;
            this.title = title;
            this.paneLocation = paneLocation;
        }
    }
}