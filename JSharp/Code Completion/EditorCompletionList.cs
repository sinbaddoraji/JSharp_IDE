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
        static char[] alphabets;

        static EditorCompletionList()
        {
            alphabets = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        }

        public void Add(string data, bool isImportant = false)
        {
            if(!AutoCompleteStrings.Contains(data))
            {
                AutoCompleteStrings.Add(data);

                int binraySearchIndex = AutoCompleteStrings.BinarySearch(data);
                if (binraySearchIndex < 0)
                {
                    binraySearchIndex = ~binraySearchIndex;
                    AutoCompleteStrings.Insert(binraySearchIndex, data);
                }

                int insertionIndex = binraySearchIndex;
                MyCompletionData newCompletionData = new MyCompletionData(data);

                if(!data.StartsWith("."))
                {
                    newCompletionData.Priority++;
                }
                if(isImportant)
                {
                    newCompletionData.Priority += 4;
                }
                if(insertionIndex >= AutoCompleteStrings.Count || insertionIndex >= CompletionData.Count)
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
            try
            {
                return AutoCompleteStrings.IndexOf(data) != -1;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

    }
}
