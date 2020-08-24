using System.Collections.Generic;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace JSharp.TextEditor.Completion
{
    /// <inheritdoc />
    /// <summary>
    /// A type of list used to hold specificity Editor Completion data
    /// </summary>
    public class EditorCompletionList: CompletionList
    {
        private readonly List<string> _autoCompleteStrings = new List<string>();

        public EditorCompletionData EditorCompletionData
        {
            get => default;
            set
            {
            }
        }

        public bool Contains(string data) => _autoCompleteStrings.IndexOf(data) != -1;

        public void Add(string data, bool isImportant = false)
        {
            if (_autoCompleteStrings.Contains(data)) return;
            _autoCompleteStrings.Add(data);

            //Get predicted index of input in the list using binary search
            var binraySearchIndex = _autoCompleteStrings.BinarySearch(data);
            if (binraySearchIndex < 0) binraySearchIndex = ~binraySearchIndex;

            //Insert text at predicted index
            _autoCompleteStrings.Insert(binraySearchIndex, data);

            var insertionIndex = binraySearchIndex;
            var newCompletionData = new EditorCompletionData(data);

            if(!data.StartsWith(".")) newCompletionData.Priority++;
            if (isImportant) newCompletionData.Priority += 10;

            //Add to completion data
            if (insertionIndex >= _autoCompleteStrings.Count || insertionIndex >= CompletionData.Count)
                CompletionData.Add(newCompletionData);
            else
                CompletionData.Insert(insertionIndex, newCompletionData);
        }
    }
}
