using Antlr4.Runtime;
using System;
using System.Collections.Generic;

namespace JavaParser
{
    public class Parser
    {
        public ITokenSource lexer;
        private readonly List<string> Tokens = new List<string>();
        private readonly List<Statement> _statements = new List<Statement>();
        private readonly List<Statement> _importStatements = new List<Statement>();
        private readonly List<Statement> _classStatements = new List<Statement>();
        private readonly List<Statement> _nestedStatements = new List<Statement>();
        private readonly List<Statement> _variables = new List<Statement>();
        private readonly List<Statement> _methodStatements = new List<Statement>();

        public enum StatementType
        {
            NormalStatement,
            Parameters,
            Method,
            Class,
            Variables,
            NestedStatement,
            ImportStatement,
            Brackets,
            Error
        }

        public Parser(ICharStream inputStream)
        {
            lexer = new JavaLexer(inputStream);

            while (true)
            {
                var currentToken = lexer.NextToken().Text.Trim();
                if (currentToken == "<EOF>") break;

                //Ignore white space and comments
                if (currentToken != "" && !currentToken.StartsWith("//") && !currentToken.StartsWith("/*"))
                    Tokens.Add(currentToken);
            }

            for (var i = 0; i < Tokens.Count; i++)
            {
                Statement statement = GetNextStatement(ref i);
                Console.WriteLine(statement);
            }
            Console.ReadKey();
        }

        private void LogStatement(Statement statement, StatementType statementType)
        {
            if (statement == null || _statements.Contains(statement))
            {
                return;
            }
            if (statement.RawTokens?.Count == 0)
            {
                statement.RawTokens.Add(statement.startingKeyword);
            }
            else if (statement.startingKeyword?.Length == 0)
            {
                statement.startingKeyword = statement.RawTokens[0];
            }

            if(!_statements.Contains(statement))
                _statements.Add(statement);

            switch (statementType)
            {
                case StatementType.Class:
                    if(!_classStatements.Contains(statement))
                        _classStatements.Add(statement);
                    break;

                case StatementType.Method:
                    if(!_methodStatements.Contains(statement))
                        _methodStatements.Add(statement);
                    break;

                case StatementType.Variables:
                    if(!_variables.Contains(statement))
                        _variables.Add(statement);
                    break;

                case StatementType.NestedStatement:
                    _nestedStatements.Add(statement);
                    break;

                case StatementType.ImportStatement:
                    _importStatements.Add(statement);
                    break;

                case StatementType.NormalStatement:
                    //Do Nothing
                    break;

                case StatementType.Parameters:
                    //Do nothing
                    break;

                case StatementType.Brackets:
                    //Do nothing
                    break;

                default: break;
                    //throw new ArgumentOutOfRangeException(nameof(statementType), statementType, null);
            }
        }

        private Statement GetStatement(ref int startingIndex, string startKeyword, string endKeyword, Statement parentStatement = null, bool recurse = true)
        {
            if (startingIndex >= Tokens.Count) return null;

            for (; (startingIndex < Tokens.Count) &&
                (Tokens[startingIndex] != startKeyword);
                startingIndex++) { }

            return startingIndex >= Tokens.Count
                ? null : GetStatement(ref startingIndex, endKeyword, parentStatement, recurse);
        }

        private Statement GetStatement(ref int startingIndex, string endingKeyword, Statement parentStatement = null, bool recurse = true)
        {
            if (startingIndex >= Tokens.Count) return null;

            var statement = new Statement();
            var currentIndex = startingIndex;

            for (; currentIndex < Tokens.Count; currentIndex++)
            {
                var currentToken = Tokens[currentIndex];

                if (currentIndex != startingIndex)
                {
                    switch (GetStatementType(startingIndex))
                    {
                        case StatementType.NormalStatement:
                            (statement.RawTokens ?? (statement.RawTokens = new List<string>())).Add(currentToken);
                            break;

                        case StatementType.ImportStatement:
                            (statement.RawTokens ?? (statement.RawTokens = new List<string>())).Add(currentToken);

                            break;

                        case StatementType.NestedStatement:
                            (statement.RawTokens ?? (statement.RawTokens = new List<string>())).Add(currentToken);

                            Statement childStatement = GetNestedStatement(ref startingIndex, currentToken);
                            if (childStatement.offset > startingIndex)
                            {
                                startingIndex = childStatement.offset;
                            }

                       (statement.Children ?? (statement.Children = new List<Statement>())).Add(childStatement);
                            break;

                        case StatementType.Brackets:

                            (statement.RawTokens ?? (statement.RawTokens = new List<string>())).Add(currentToken);

                            childStatement = GetStatement(ref startingIndex, "{", "}");
                            if (childStatement.offset > startingIndex)
                            {
                                startingIndex = childStatement.offset;
                            }

                       (statement.Children ?? (statement.Children = new List<Statement>())).Add(childStatement);

                            break;

                        case StatementType.Class:
                            statement.statementType = StatementType.Class;
                            break;

                        case StatementType.Parameters:
                            //Do nothing
                            break;

                        case StatementType.Method:
                            //Implement later
                            break;

                        case StatementType.Variables:
                            (statement.RawTokens ?? (statement.RawTokens = new List<string>())).Add(currentToken);

                            if (recurse)
                            {
                                while (GetStatementType(startingIndex) == StatementType.Variables) startingIndex--;
                            }
                            var s = GetStatement(ref startingIndex, ";", statement, false);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (currentIndex >= Tokens.Count) break;

                if (statement.statementType == StatementType.Class)
                {
                    if (Tokens[currentIndex] == "{")
                    {
                        GetStatement(ref currentIndex, "{", "}", statement);
                    }
                    endingKeyword = "{";
                    statement.startingKeyword = "class";
                    statement.name = Tokens[currentIndex];
                }
                else if (currentToken == endingKeyword)
                {
                    break;
                }
            }

            statement.offset = currentIndex;
            if (statement.offset > startingIndex)
            {
                startingIndex = statement.offset;
            }

            LogStatement(statement, GetStatementType(startingIndex));

            if(parentStatement != null)
            {
                (parentStatement.Children ?? (parentStatement.Children = new List<Statement>())).Add(statement);
            }
            return statement;
        }

        private Statement GetNextStatement(ref int Startindex)
        {
            string currentToken = Tokens[Startindex];
            StatementType statementType = GetStatementType(Startindex);

            Statement outputStatement = null;
            if(Tokens[Startindex] == ";")
            {
                return GetStatement(ref Startindex, ";");
            }
            switch (statementType)
            {
                case StatementType.NormalStatement:
                    outputStatement = GetStatement(ref Startindex, ";");
                    break;

                case StatementType.ImportStatement:
                    outputStatement = GetStatement(ref Startindex,"import", ";", null, false);
                    break;

                case StatementType.NestedStatement:
                    outputStatement = GetNestedStatement(ref Startindex, currentToken);
                    break;

                case StatementType.Parameters:
                    outputStatement = GetStatement(ref Startindex, "(", ")");
                    break;

                case StatementType.Class:
                    outputStatement = GetClass(ref Startindex);
                    break;

                case StatementType.Method:
                    //Implement later
                    break;

                case StatementType.Variables:
                    //Implement later
                    break;

                case StatementType.Brackets:
                    //Implement later
                    break;

                 default:
                    throw new ArgumentOutOfRangeException();
            }

            if (outputStatement != null && outputStatement.offset > Startindex)
                Startindex = outputStatement.offset;

            LogStatement(outputStatement, GetStatementType(Startindex));
            return outputStatement;
        }

        private Statement GetNestedStatement(ref int StartIndex, string startingKeyword)
        {
            StartIndex++;

            Statement nestedStatement = new Statement
            {
                startingKeyword = startingKeyword,
            };

            if(startingKeyword != "try" && startingKeyword != "catch" && startingKeyword != "else")
            {
                nestedStatement.ParameterStatement = GetStatement(ref StartIndex, "(", ")");
            }

            Statement brackets = GetStatement(ref StartIndex, "{", "}");
            (nestedStatement.Children ?? (nestedStatement.Children = new List<Statement>())).Add(brackets);

            LogStatement(nestedStatement, StatementType.NestedStatement);
            return nestedStatement;
        }

        private Statement GetClass(ref int StartIndex)
        {
            StartIndex++;

            Statement classStatement = new Statement
            {
                startingKeyword = "class",
                name = Tokens[StartIndex]
            };

            Statement brackets = GetStatement(ref StartIndex, "{", "}");
            (classStatement.Children ?? (classStatement.Children = new List<Statement>())).Add(brackets);

            LogStatement(classStatement, StatementType.NestedStatement);
            return classStatement;
        }

        private StatementType GetStatementType(int tokenIndex)
        {
            if (tokenIndex >= Tokens.Count) return StatementType.Error;

            string previousToken = tokenIndex != 0 ? Tokens[tokenIndex - 1] : null;
            //string nextToken = tokenIndex < Tokens.Count - 1 ? Tokens[tokenIndex + 1] : null;

            switch (Tokens[tokenIndex])
            {
                case "import":
                    return StatementType.ImportStatement;

                case "if":
                case "else":
                case "while":
                case "for":
                case "try":
                case "do":
                case "catch":
                    return StatementType.NestedStatement;

                case "public":
                case "private":
                case "protected":
                case "static":
                case "virtual":
                    //
                    return StatementType.Method;

                case "(":
                    return StatementType.Parameters;

                case "{":
                    return StatementType.Brackets;

                case "=":
                case "int":
                case "String":
                case "string":
                case "char":
                case "Integer":
                case "float":
                case "double":
                case "byte":
                case "short":
                case "bool":
                case "Boolean":
                    return StatementType.Variables;

                case ".":
                    return StatementType.NormalStatement;

                default:
                    switch (previousToken)
                    {
                        case "class":
                            return StatementType.Class;

                        default:
                            return StatementType.NormalStatement;
                    }
            }

           
        }
    }
}