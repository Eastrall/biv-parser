namespace BIV.Parser.Core
{
    public interface IStatement
    {
        string Name { get; }

        StatementType Type { get; }
    }
}
