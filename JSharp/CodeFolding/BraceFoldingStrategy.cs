using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace JSharp.CodeFolding
{
    internal class BraceFoldingStrategy
    {
        /*
		 * This class is a slightly modified brace folding strategy.
		 * The original code can be found in the Avalon edit repository
		 */

        public static void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            var newFoldings = CreateNewFoldings(document, out var firstErrorOffset);
            manager.UpdateFoldings(newFoldings, firstErrorOffset);
        }

        private static IEnumerable<NewFolding> CreateNewFoldings(ITextSource document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        private static IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            var newFoldings = new List<NewFolding>();
            var startOffsets = new Stack<int>();
            for (int i = 0, lastNewLineOffset = 0; i < document.TextLength; i++)
            {
                var c = document.GetCharAt(i);
                switch (c)
                {
                    case '{':
                        startOffsets.Push(i);
                        break;
                    case '\n':
                    case '\r':
                        lastNewLineOffset = i + 1;
                        break;
                    case '}' when startOffsets.Count > 0:
                        var startOffset = startOffsets.Pop();

                        //Create folding if completing } is not on the same line
                        if (startOffset < lastNewLineOffset)
                            newFoldings.Add(new NewFolding(startOffset, i + 1));
                        break;
                }
            }
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }
    }
}