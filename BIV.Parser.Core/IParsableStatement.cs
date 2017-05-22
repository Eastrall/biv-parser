namespace BIV.Parser.Core
{
    public interface IParsableStatement
    {
        void Parse(string[] fileContent, ref int currentIndex);
    }
}
