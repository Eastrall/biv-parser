using System;
using System.Collections.Generic;
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

        private string _filePath;
        private IEnumerable<string> _tokens;
        private ICollection<IStatement> _statements;

        public IReadOnlyCollection<IStatement> Statements
        {
            get { return this._statements as IReadOnlyCollection<IStatement>; }
        }

        public BivFile(string filePath)
        {
            this._filePath = filePath;
            this._statements = new List<IStatement>();
        }

        public void Parse()
        {
            this.ReadFile();
        }

        private void ReadFile()
        {
            using (var fileStream = new FileStream(this._filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream))
            {
                string fileContent = reader.ReadToEnd();
                this._tokens = this.IgnoreComments(fileContent);
            }
            
            string[] parts = Regex.Split(string.Join("", this._tokens), @"([(){}=,;\n\r ])");

            this._tokens = from x in parts
                          let y = x.Trim()
                          where !string.IsNullOrEmpty(y)
                          select y;

            foreach (var token in this._tokens)
                Console.WriteLine(token.Trim());
        }

        private IEnumerable<string> IgnoreComments(string fileContent)
        {
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

            return from x in splitFileContent
                   where !string.IsNullOrEmpty(x)
                   select x;
        }

        public void Dispose()
        {
        }
    }
}
