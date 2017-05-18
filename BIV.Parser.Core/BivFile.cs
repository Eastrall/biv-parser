using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BIV.Parser.Core
{
    public sealed class BivFile : IDisposable
    {
        private const string SingleLineComment = "//";
        private const string MultiLineCommentBegin = "/*";
        private const string MultiLineCommentEnd = "*/";

        private string filePath;

        public ICollection<IStatement> Statements { get; private set; }

        public BivFile(string filePath)
        {
            this.filePath = filePath;

            this.Statements = new List<IStatement>();
        }

        public void Parse()
        {
            using (var fileStream = new FileStream(this.filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (line.StartsWith(SingleLineComment))
                        continue;
                    if (line.Contains(MultiLineCommentBegin))
                    {
                        line = null;
                        while (!reader.ReadLine().Trim().Contains(MultiLineCommentEnd))
                            continue;
                    }

                    Console.WriteLine(line);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
