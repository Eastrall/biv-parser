using System.Collections.Generic;
using System.Linq;

namespace BIV.Parser.Core
{
    public class Block : IStatement
    {
        public string Name { get; private set; }

        public StatementType Type
        {
            get { return StatementType.Block; }
        }

        public ICollection<IStatement> Statements { get; private set; }

        public Block this[string blockName]
        {
            get { return this.GetBlockByName(blockName); }
        }

        public Block()
        {
            this.Statements = new List<IStatement>();
        }

        public Block GetBlockByName(string name)
        {
            return (from x in this.Statements
                    where x.Name.ToLower() == name.ToLower()
                    where x.Type == StatementType.Block
                    select x).FirstOrDefault() as Block;
        }
    }
}
