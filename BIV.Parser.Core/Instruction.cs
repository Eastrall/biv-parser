using System;
using System.Collections.Generic;

namespace BIV.Parser.Core
{
    public class Instruction : IStatement
    {
        private ICollection<object> _parameters;

        public string Name { get; private set; }

        public IReadOnlyCollection<object> Parameters
        {
            get { return this._parameters as IReadOnlyCollection<object>; }
        }

        public StatementType Type
        {
            get { return StatementType.Instruction; }
        }

        public Instruction()
        {
            this._parameters = new List<object>();
        }

        void Parse(string[] fileContent, ref int currentIndex)
        {
        }
    }
}
