namespace BIV.Parser.Core
{
    public class Variable : IStatement
    {
        public string Name { get; private set; }

        public object Value { get; private set; }

        public StatementType Type
        {
            get { return StatementType.Variable; }
        }

        public Variable()
        {
                
        }
    }
}
