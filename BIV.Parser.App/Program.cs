using BIV.Parser.Core;
using System;
using System.Diagnostics;

namespace BIV.Parser.App
{
    class Program
    {
        private static int nbBlocks = 0;
        private static int nbInstructions = 0;
        private static int nbVariables = 0;

        static void Main(string[] args)
        {
            Stopwatch s = new Stopwatch();
            
            using (var file = new BivFile(@"../example.biv"))
            {
                s.Start();
                file.Parse();
                s.Stop();
                
                foreach (var statement in file.Statements)
                    Display(statement);
            }
            
            Console.WriteLine("Read and Parse time: {0} ms", s.Elapsed.TotalMilliseconds);
            Console.WriteLine("Number of blocks: {0}", nbBlocks);
            Console.WriteLine("Nubmer of instructions: {0}", nbInstructions);
            Console.WriteLine("Number of variables: {0}", nbVariables);

            Console.ReadLine();
        }


        static void Display(IStatement statement, int index = 0)
        {
            
            DisplayTreeBranches(index);

            switch (statement.Type)
            {
                case StatementType.Block:
                    nbBlocks++;
                    var block = statement as Block;

                    Console.WriteLine("[Block]: {0}", block.Name);
                    foreach (var blockStatement in block.Statements)
                        Display(blockStatement, index + 1);
                    break;
                case StatementType.Instruction:
                    nbInstructions++;
                    var instruction = statement as Instruction;
                    Console.WriteLine("[Instruction]: {0} => {1}", statement.Name, string.Join(", ", instruction.Parameters));
                    break;
                case StatementType.Variable:
                    nbVariables++;
                    var variable = statement as Variable;

                    Console.WriteLine("[Variable]: {0} = {1}", variable.Name, variable.Value);
                    break;
                default:
                    break;
            }
        }

        static void DisplayTreeBranches(int branch)
        {
            if (branch > 0)
            {
                Console.Write("|");
                for (int i = 0; i < branch; i++)
                    Console.Write("-");
                Console.Write(" ");
            }
        }
    }
}