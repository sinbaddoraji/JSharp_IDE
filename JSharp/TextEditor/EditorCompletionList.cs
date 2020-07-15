using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.ObjectModel;

namespace JSharp.Code_Completion
{
    public class EditorCompletionList: CompletionList
    {
        private readonly List<string> AutoCompleteStrings = new List<string>();

        public void Add(string data, bool isImportant = false)
        {
            if(!AutoCompleteStrings.Contains(data))
            {
                AutoCompleteStrings.Add(data);

                //Get predicted index of input in the list using binary search
                int binraySearchIndex = AutoCompleteStrings.BinarySearch(data);
                if (binraySearchIndex < 0) binraySearchIndex = ~binraySearchIndex;

                //Insert text at predicted index
                AutoCompleteStrings.Insert(binraySearchIndex, data);

                int insertionIndex = binraySearchIndex;
                MyCompletionData newCompletionData = new MyCompletionData(data);

                if(!data.StartsWith(".")) newCompletionData.Priority++;
                if (isImportant) newCompletionData.Priority += 10;

                //Add to completion data
                if (insertionIndex >= AutoCompleteStrings.Count || insertionIndex >= CompletionData.Count)
                    CompletionData.Add(newCompletionData);
                else
                    CompletionData.Insert(insertionIndex, newCompletionData);
            }
        }

        public bool Contains(string data)
        {
            return AutoCompleteStrings.IndexOf(data) != -1;
        }
    }
}
