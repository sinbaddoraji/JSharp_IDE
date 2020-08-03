using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace JSharp.TextEditor
{
    /// <summary>
    /// Completion data for a completion window in a JSharp TextEditor
    /// </summary>
    public class EditorCompletionData : ICompletionData
    {
        public EditorCompletionData(string text) => Text = text;

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