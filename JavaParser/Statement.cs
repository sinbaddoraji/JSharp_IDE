using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaParser
{
    public class Statement
    {
        public string startingKeyword;
        public string name;
        public Statement Parent;
        public List<string> Modifiers;
        public List<Statement> Children;
        public Statement ParameterStatement;
        public List<string> RawTokens;

        public int offset;

        public Parser.StatementType statementType;

        public override string ToString()
        {
            if(statementType == Parser.StatementType.Class)
            {
                return $"Class {name}";
            }
            if(RawTokens == null)
            {
                return startingKeyword;
            }
            string output = "";
            foreach (var item in RawTokens)
            {
                output += item + " ";
            }
            return output.Trim();
        }
    }
}
