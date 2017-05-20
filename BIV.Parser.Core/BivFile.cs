using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BIV.Parser.Core
{
    public sealed class BivFile : IDisposable
    {
        private const string SingleLineComment = "//";
        private const string MultiLineCommentBegin = "/*";
        private const string MultiLineCommentEnd = "*/";
        private readonly char[] SplitCharacters = new char[] { '\n', '\r' };

        private string filePath;
        private string[] fileContent;

        public ICollection<IStatement> Statements { get; private set; }

        public BivFile(string filePath)
        {
            this.filePath = filePath;

            this.Statements = new List<IStatement>();
        }

        public void Parse()
        {
            this.ReadFile();

            if (this.fileContent.Length <= 0)
                return;

            for (int i = 0; i < this.fileContent.Length; i++)
            {
                string line = this.fileContent[i];

                if (string.IsNullOrEmpty(line) || line.StartsWith(SingleLineComment))
                    continue;

                if (line.Contains(SingleLineComment))
                    line = line.Remove(line.IndexOf(SingleLineComment));

                if (line.Contains(MultiLineCommentBegin))
                {
                    int tmp = i;

                    this.fileContent[tmp++] = line.Remove(line.IndexOf(MultiLineCommentBegin)).Trim();
                    while (!this.fileContent[tmp].Contains(MultiLineCommentEnd))
                    {
                        this.fileContent[tmp++] = string.Empty;
                        continue;
                    }
                    int removeStartIndex = this.fileContent[tmp].IndexOf(MultiLineCommentEnd) + MultiLineCommentEnd.Length;
                    this.fileContent[tmp] = this.fileContent[tmp].Substring(removeStartIndex).Trim();
                    line = this.fileContent[i];
                }

                Console.WriteLine(line);
            }
        }

        private void ReadFile()
        {
            using (var fileStream = new FileStream(this.filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream))
                this.fileContent = (from x in reader.ReadToEnd().Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries)
                                    select x.Trim()).ToArray();
        }

        public void Dispose()
        {
        }
    }
}
