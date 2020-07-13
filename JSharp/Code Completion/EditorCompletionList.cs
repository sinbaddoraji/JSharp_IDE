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
        List<string> AutoCompleteStrings = new List<string>();

        public EditorCompletionList()
        {
            
        }

        public void Add(string data, bool isImportant = false)
        {
            if(!AutoCompleteStrings.Contains(data))
            {
                AutoCompleteStrings.Add(data);
                AutoCompleteStrings.Sort();

                int binraySearchIndex = AutoCompleteStrings.BinarySearch(data);
                if (binraySearchIndex < 0)
                {
                    AutoCompleteStrings.Insert(~binraySearchIndex, data);
                }

                int insertionIndex = AutoCompleteStrings.IndexOf(data);
                MyCompletionData newCompletionData = new MyCompletionData(data);

                if(!data.StartsWith("."))
                {
                    newCompletionData.Priority++;
                }
                if(isImportant)
                {
                    newCompletionData.Priority += 4;
                }
                if(insertionIndex >= AutoCompleteStrings.Count)
                {
                    this.CompletionData.Add(newCompletionData);
                }
                else
                {
                    CompletionData.Insert(insertionIndex, newCompletionData);
                }
                
                

                //var newCompletionData = new ObservableCollection<ICompletionData>(CompletionData.OrderBy(x => x.Text));
            }
        }

        public bool Contains(string data)
        {
            return AutoCompleteStrings.Contains(data);
        }
    }
}
