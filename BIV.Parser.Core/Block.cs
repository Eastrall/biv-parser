using System;
using System.Collections.Generic;
using System.Linq;

namespace BIV.Parser.Core
{
    public class Block : IStatement, IDisposable
    {
        private ICollection<IStatement> _statements;

        public string Name { get; set; }

        public StatementType Type
        {
            get { return StatementType.Block; }
        }

        public IReadOnlyCollection<IStatement> Statements
        {
            get { return this._statements as IReadOnlyCollection<IStatement>; }
        }

        public Block this[string blockName]
        {
            get { return this.GetBlockByName(blockName); }
        }

        public Block()
            : this(string.Empty)
        {
        }

        public Block(string name)
        {
            this.Name = name;
            this._statements = new List<IStatement>();
        }

        public Block GetBlockByName(string name)
        {
            return (from x in this.Statements
                    where x.Name.ToLower() == name.ToLower()
                    where x.Type == StatementType.Block
                    select x).FirstOrDefault() as Block;
        }

        internal void AddStatement(IStatement statement)
        {
            this._statements.Add(statement);
        }

        public void Dispose()
        {
            if (this._statements.Any())
                this._statements.Clear();
        }
    }
}
