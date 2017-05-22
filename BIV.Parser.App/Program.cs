﻿using BIV.Parser.Core;
using System;

namespace BIV.Parser.App
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var file = new BivFile(@"C:\Users\gomes\Documents\Project Hellion\Hellion\binary\data\res\dataSub1\character.inc"))
            {
                file.Parse();

                foreach (var statement in file.Statements)
                    Display(statement);
            }

            Console.ReadLine();
        }

        static void Display(IStatement statement, int index = 0)
        {
            DisplayTreeBranches(index);

            switch (statement.Type)
            {
                case StatementType.Block:
                    var block = statement as Block;

                    Console.WriteLine("[Block]: {0}", block.Name);
                    foreach (var blockStatement in block.Statements)
                        Display(blockStatement, index + 1);
                    break;
                case StatementType.Instruction:
                    Console.WriteLine("[Instruction]: {0} => ", statement.Name);
                    // Todo: display params
                    break;
                case StatementType.Variable:
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