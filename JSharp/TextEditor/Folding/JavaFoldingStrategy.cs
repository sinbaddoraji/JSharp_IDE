using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace JSharp.TextEditor.Folding
{
    internal static class JavaFoldingStrategy
    {
        /// <summary>
        /// Update code foldings
        /// </summary>
        public static void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            var newFoldings = CreateNewFoldings(document, out var firstErrorOffset);
            manager.UpdateFoldings(newFoldings, firstErrorOffset);
        }

        /// <summary>
        /// Create new code foldings
        /// </summary>
        private static IEnumerable<NewFolding> CreateNewFoldings(ITextSource document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        /// <summary>
        /// Create new code foldings
        /// </summary>
        private static IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            var newFoldings = new List<NewFolding>();
            var startOffsets = new Stack<int>();
            for (int i = 0, lastNewLineOffset = 0; i < document.TextLength; i++)
            {
                if (document.GetCharAt(i) == '{')
                {
                    startOffsets.Push(i);
                }
                else if (document.GetCharAt(i) == '\n' || document.GetCharAt(i) == '\r')
                {
                    lastNewLineOffset = i + 1;
                }
                else if (document.GetCharAt(i) == '}' && startOffsets.Count > 0)
                {
                    var startOffset = startOffsets.Pop();

                    //Create folding if completing } is not on the same line
                    if (startOffset < lastNewLineOffset)
                        newFoldings.Add(new NewFolding(startOffset, i + 1));
                }
            }
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }
    }
}