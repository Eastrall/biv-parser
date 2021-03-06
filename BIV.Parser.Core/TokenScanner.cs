﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BIV.Parser.Core
{
    public class TokenScanner : IDisposable
    {
        private const string SingleLineComment = "//";
        private const string MultiLineCommentBegin = "/*";
        private const string MultiLineCommentEnd = "*/";
        private readonly char[] SplitCharacters = new char[] { '\n', '\r' };

        private int _currentTokenIndex;
        private string _filePath;
        private string _splitRegex;
        private string[] _tokens;

        public TokenScanner(string filePath, string splitRegex)
        {
            this._currentTokenIndex = 0;
            this._filePath = filePath;
            this._splitRegex = splitRegex;
        }

        public void Read()
        {
            this._currentTokenIndex = 0;
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

                string[] parts = Regex.Split(fileContent, this._splitRegex);

                this._tokens = (from x in parts
                                let y = x.Trim()
                                where !string.IsNullOrEmpty(y)
                                select y).ToArray();
            }
        }

        public string GetToken()
        {
            if (this._currentTokenIndex + 1 > this._tokens.Count())
                return null;

            return this._tokens[this._currentTokenIndex++];
        }

        public string GetPreviousToken()
        {
            if (this._currentTokenIndex <= 0)
                return null;

            return this._tokens[this._currentTokenIndex - 2];
        }

        public bool NextTokenIs(string token)
        {
            if (this._currentTokenIndex > this._tokens.Count())
                return false;

            return string.Equals(this._tokens[this._currentTokenIndex + 1], token, StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
        }
    }
}
