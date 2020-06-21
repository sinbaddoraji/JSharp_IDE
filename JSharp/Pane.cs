namespace JSharp
{
    public class Pane
    {
        public readonly object content;
        public readonly string title;

        public Pane(object content, string title)
        {
            this.content = content;
            this.title = title;
        }
    }
}