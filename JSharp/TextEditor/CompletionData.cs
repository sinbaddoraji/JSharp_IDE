using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace JSharp.Code_Completion
{
    public class MyCompletionData : ICompletionData
    {
        public MyCompletionData(string text) => Text = text;

        public ImageSource Image => null;

        public string Text { get; }

        public object Content => Text;

        public object Description => Text;

        public double Priority { get; set; }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }
    }
}