using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BIV.Parser.Core
{
    public sealed class BivFile : IDisposable
    {
        private const string SingleLineComment = "//";
        private const string MultiLineCommentBegin = "/*";
        private const string MultiLineCommentEnd = "*/";
        private readonly char[] SplitCharacters = new char[] { '\n', '\r' };

        private int _currentTokenIndex;
        private string _filePath;
        private IEnumerable<string> _tokens;
        private ICollection<IStatement> _statements;

        public IReadOnlyCollection<IStatement> Statements
        {
            get { return this._statements as IReadOnlyCollection<IStatement>; }
        }

        public BivFile(string filePath)
        {
            this._currentTokenIndex = 0;
            this._filePath = filePath;
            this._statements = new List<IStatement>();
        }

        public void Parse()
        {
            this.ReadFile();

            string token = null;
            while ((token = this.GetToken()) != null)
            {
                if (token == "{")
                    this._statements.Add(this.ParseBlock());
                else if (token == "(")
                    this._statements.Add(this.ParseInstruction());
                else if (token == "=")
                    this._statements.Add(this.ParseVariable());
            }
        }

        public string GetToken()
        {
            if (this._currentTokenIndex + 1 > this._tokens.Count())
                return null;

            return this._tokens.ElementAt(this._currentTokenIndex++);
        }

        public string GetPreviousToken()
        {
            if (this._currentTokenIndex <= 0)
                return null;

            return this._tokens.ElementAt(this._currentTokenIndex - 1);
        }

        public bool NextTokenIs(string token)
        {
            if (this._currentTokenIndex > this._tokens.Count())
                return false;

            return string.Equals(this._tokens.ElementAt(this._currentTokenIndex + 1), token, StringComparison.OrdinalIgnoreCase);
        }

        private void ReadFile()
        {
            using (var fileStream = new FileStream(this._filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream))
            {
                string fileContent = reader.ReadToEnd();
                string[] splitFileContent = fileContent.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < splitFileContent.Length; ++i)
                {
                    string line = splitFileContent[i];

                    if (string.IsNullOrEmpty(line) || line.StartsWith(SingleLineComment))
                    {
                        splitFileContent[i] = string.Empty;
                        continue;
                    }

                    if (line.Contains(SingleLineComment))
                        splitFileContent[i] = line.Remove(line.IndexOf(SingleLineComment));

                    if (line.Contains(MultiLineCommentBegin))
                    {
                        splitFileContent[i++] = line.Remove(line.IndexOf(MultiLineCommentBegin)).Trim();
                        while (!splitFileContent[i].Contains(MultiLineCommentEnd))
                        {
                            splitFileContent[i++] = string.Empty;
                            continue;
                        }
                        int removeStartIndex = splitFileContent[i].IndexOf(MultiLineCommentEnd) + MultiLineCommentEnd.Length;
                        splitFileContent[i] = splitFileContent[i].Substring(removeStartIndex).Trim();
                    }
                }

                var tokens = from x in splitFileContent
                             where !string.IsNullOrEmpty(x)
                             select x;

                fileContent = string.Join("", tokens);

                string[] parts = Regex.Split(fileContent, @"([(){}=,;\n\r])");

                this._tokens = from x in parts
                               let y = x.Trim()
                               where !string.IsNullOrEmpty(y)
                               select y;
            }
        }

        private Block ParseBlock()
        {
            var block = new Block()
            {
                Name = this._tokens.ElementAt(this._currentTokenIndex - 2)
            };
            string token = null;

            while ((token = this.GetToken()) != "}")
            {
                if (token == "{")
                    block.AddStatement(this.ParseBlock());
                else if (token == "(")
                    block.AddStatement(this.ParseInstruction());
                else if (token == "=")
                    block.AddStatement(this.ParseVariable());
            }

            return block;
        }

        private Instruction ParseInstruction()
        {
            string parameter = null;
            var instruction = new Instruction()
            {
                Name = this._tokens.ElementAt(this._currentTokenIndex - 2)
            };
            
            while ((parameter = this.GetToken()) != ")")
                instruction.AddParameter(parameter);

            string endDelimiter = this.GetToken();

            if (endDelimiter != ";")
                throw new InvalidDataException("Invalid instruction format. Missing ';' for instruction " + instruction.Name);

            return instruction;
        }

        private Variable ParseVariable()
        {
            string variableName = this._tokens.ElementAt(this._currentTokenIndex - 2);
            string variableValue = this.GetToken();
            string endDelimiter = this.GetToken();

            if (endDelimiter != ";")
                throw new InvalidDataException("Invalid variable format. Missing ';' for variable " + variableName);

            return new Variable(variableName, variableValue);
        }

        public void Dispose()
        {
        }
    }
}
