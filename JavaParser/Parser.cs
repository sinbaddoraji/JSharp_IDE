using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaParser
{
    public class Parser
    {
       
        public ICharStream inputStream;
        public ITokenSource lexer;
        public ITokenStream tokens;

        //Create class object (object that holds methods and that)

        List<string> allTokensFound = new List<string>();

        public enum StatementType { NormalStatement, Parameters, Method, Class, Variables, NestedStatement, ImportStatement, Brackets }

        List<Statement> statements = new List<Statement>();
        List<Statement> importStatements = new List<Statement>();
        List<Statement> classStatements = new List<Statement>();
        List<Statement> nestedStatements = new List<Statement>();
        List<Statement> variables = new List<Statement>();
        List<Statement> methodStatements = new List<Statement>();

        public Parser(ICharStream inputStream)
        {
            this.inputStream = inputStream;
            lexer = new JavaLexer(inputStream);

            while(true)
            {
                string currentToken = lexer.NextToken().Text.Trim();
                if (currentToken == "<EOF>") break;

                //Ignore white space and comments
                if(currentToken != "" && !currentToken.StartsWith(@"//") && !currentToken.StartsWith(@"/*"))
                    allTokensFound.Add(currentToken);
            }


            int i = 0;
            while (i < allTokensFound.Count)
            {
                Statement currentStatement = GetNextStatement(ref i);
                i++;
            }
           
        }
        void LogStatement(Statement statement, StatementType statementType)
        {
            Console.WriteLine(statement.ToString());

            if(statements.Contains(statement))
            {
                return;
            }
            if (statement.RawTokens != null && statement.RawTokens.Count == 0)
            {
                statement.RawTokens.Add(statement.startingKeyword);
            }
            else if(statement.startingKeyword == "")
            {
                statement.startingKeyword = statement.RawTokens[0];
            }
            statements.Add(statement);
                switch(statementType)
            {
                case StatementType.Class:
                    classStatements.Add(statement);
                    break;

                case StatementType.Method:
                    methodStatements.Add(statement);
                    break;

                case StatementType.Variables:
                    variables.Add(statement);
                    break;

                case StatementType.NestedStatement:
                    nestedStatements.Add(statement);
                    break;

                case StatementType.ImportStatement:
                    importStatements.Add(statement);
                    break;
            }
        }

        void FillInnerStatements(ref Statement statement, string currentToken,ref int startingIndex)
        {
            StatementType statementType = GetStatementType(currentToken);

            if (statementType == StatementType.NormalStatement || statementType == StatementType.ImportStatement)
            {
                if(statement.RawTokens == null)
                {
                    statement.RawTokens = new List<string>();
                }
                statement.RawTokens.Add(currentToken);
            }
            else if (statementType == StatementType.NestedStatement)
            {
                statement.RawTokens.Add(currentToken);

                Statement childStatement = GetNestedStatement(ref startingIndex, currentToken);
                if (childStatement.offset > startingIndex)
                {
                    startingIndex = childStatement.offset;
                }
                //statement.Children.Add(childStatement);

            }

        }

        Statement GetStatement(ref int startingIndex, string startKeyword, string endKeyword, bool recurse)
        {
            while (allTokensFound[startingIndex] != startKeyword) startingIndex++;
            var statement = GetStatement(ref startingIndex, endKeyword, recurse); ;
            return statement;
        }

        Statement GetStatement(ref int startingIndex, string endingKeyword, bool recurse)
        {
            Statement outputStatement = new Statement();

            int currentIndex = startingIndex;
            while(true)
            {
                string currentToken = allTokensFound[currentIndex];

              FillInnerStatements(ref outputStatement, currentToken, ref currentIndex);

                if (currentToken == endingKeyword) break;
                currentIndex++;
            }

            outputStatement.offset = currentIndex;
            if (outputStatement.offset > startingIndex)
            {
                startingIndex = outputStatement.offset;
            }
            LogStatement(outputStatement, GetStatementType(allTokensFound[startingIndex]));
            return outputStatement;
        }

        Statement GetNextStatement(ref int Startindex)
        {
            string currentToken = allTokensFound[Startindex];
            StatementType statementType = GetStatementType(currentToken);

            Statement outputStatement = null;
            if (statementType == StatementType.NormalStatement || statementType == StatementType.ImportStatement)
            {
                outputStatement = GetStatement(ref Startindex, ";", false);
                if (outputStatement.offset > Startindex)
                {
                    Startindex = outputStatement.offset;
                }
            }
            else if(statementType == StatementType.NestedStatement)
            {
                outputStatement = GetNestedStatement(ref Startindex, currentToken);
                if (outputStatement.offset > Startindex)
                {
                    Startindex = outputStatement.offset;
                }
            }
            else if (statementType == StatementType.Parameters)
            {
                outputStatement = GetStatement(ref Startindex, "(", ")", false);
                if (outputStatement.offset > Startindex)
                {
                    Startindex = outputStatement.offset;
                }
            }
            else if (statementType == StatementType.Class)
            {
                outputStatement = GetClass(ref Startindex);
                if (outputStatement.offset > Startindex)
                {
                    Startindex = outputStatement.offset;
                }
            }

            LogStatement(outputStatement, GetStatementType(allTokensFound[Startindex]));
            return outputStatement;
        }

        Statement GetNestedStatement(ref int StartIndex, string startingKeyword)
        {
            StartIndex++;

            Statement nestedStatement = new Statement();
            nestedStatement.startingKeyword = startingKeyword;

            nestedStatement.ParameterStatement = GetStatement(ref StartIndex, "(", ")", false);
                //GetStatement(ref StartIndex, ")", false);

            Statement brackets = GetStatement(ref StartIndex, "{", "}", true);

            if (nestedStatement.Children == null)
            {
                nestedStatement.Children = new List<Statement>();
            }
            nestedStatement.Children.Add(brackets);

            LogStatement(nestedStatement, StatementType.NestedStatement);
            return nestedStatement;
        }

        Statement GetClass(ref int StartIndex)
        {
            StartIndex++;

            Statement classStatement = new Statement();
            classStatement.startingKeyword = "class";
            classStatement.name = allTokensFound[StartIndex];

            Statement brackets = GetStatement(ref StartIndex, "{", "}", true);

            if (classStatement.Children == null)
            {
                classStatement.Children = new List<Statement>();
            }
            classStatement.Children.Add(brackets);

            LogStatement(classStatement, StatementType.NestedStatement);
            return classStatement;
        }




        StatementType GetStatementType(string token)
        {
            if(token == "import")
            {
                return StatementType.ImportStatement;
            }
            if (token == "class")
            {
                return StatementType.Class;
            }
            if (token == "if" || token == "while" || token == "for" || token == "catch")
            {
                return StatementType.NestedStatement;
            }
            if (token == "(")
            {
                return StatementType.Parameters;
            }
            if (token == "{")
            {
                return StatementType.Brackets;
            }
            return StatementType.NormalStatement;
        }
    }
}
