using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System.Collections.Generic;

namespace JSharp
{
    internal class BraceFoldingStrategy
    {
        /*
		 * This class is a slightly modified brace folding strategy.
		 * The original code can be found in the Avalon edit repository
		 */

        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document, out int firstErrorOffset);
            manager.UpdateFoldings(newFoldings, firstErrorOffset);
        }

        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            List<NewFolding> newFoldings = new List<NewFolding>();
            Stack<int> startOffsets = new Stack<int>();
            for (int i = 0, lastNewLineOffset = 0; i < document.TextLength; i++)
            {
                char c = document.GetCharAt(i);
                if (c == '{')
                {
                    startOffsets.Push(i);
                }
                else if (c == '\n' || c == '\r')
                {
                    lastNewLineOffset = i + 1;
                }
                else if (c == '}' && startOffsets.Count > 0)
                {
                    int startOffset = startOffsets.Pop();

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