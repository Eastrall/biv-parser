using System;

namespace BIV.Parser.Core
{
    public class Variable : IStatement, IDisposable
    {
        public string Name { get; private set; }

        public object Value { get; private set; }

        public StatementType Type
        {
            get { return StatementType.Variable; }
        }

        public Variable()
            : this(string.Empty, null)
        {
        }

        public Variable(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public void Dispose()
        {
            this.Name = string.Empty;
            this.Value = null;
        }
    }
}
