using System;
using System.Collections.Generic;
using System.Text;

namespace BIV.Parser.Core
{
    public interface IStatement
    {
        string Name { get; }

        StatementType Type { get; }
    }
}
