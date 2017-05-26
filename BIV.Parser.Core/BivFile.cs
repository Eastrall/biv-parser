using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BIV.Parser.Core
{
    public sealed class BivFile : IDisposable
    {
        private string _filePath;
        private TokenScanner _scanner;
        private ICollection<IStatement> _statements;

        public IReadOnlyCollection<IStatement> Statements
        {
            get { return this._statements as IReadOnlyCollection<IStatement>; }
        }

        public BivFile(string filePath)
        {
            this._filePath = filePath;
            this._statements = new List<IStatement>();
            this._scanner = new TokenScanner(filePath, @"([(){}=,;\n\r])");
        }

        public void Parse()
        {
            this._scanner.Read();

            string token = null;
            while ((token = this._scanner.GetToken()) != null)
            {
                if (token == "{")
                    this._statements.Add(this.ParseBlock());
                else if (token == "(")
                    this._statements.Add(this.ParseInstruction());
                else if (token == "=")
                    this._statements.Add(this.ParseVariable());
            }
        }

        private Block ParseBlock()
        {
            string token = null;
            var block = new Block()
            {
                Name = this._scanner.GetPreviousToken()
            };

            while ((token = this._scanner.GetToken()) != "}")
            {
                if (token == null)
                    break;
                if (token == "{")
                    block.AddStatement(this.ParseBlock());
                else if (token == "(")
                    block.AddStatement(this.ParseInstruction());
                else if (token == "=")
                    block.AddStatement(this.ParseVariable());
            }

            return block;
        }

        private Instruction ParseInstruction()
        {
            string parameter = null;
            var instruction = new Instruction()
            {
                Name = this._scanner.GetPreviousToken()
            };

            if (instruction.Name == "SetOutput")
            {
            }
            
            while ((parameter = this._scanner.GetToken()) != ")")
                instruction.AddParameter(parameter);

            string endDelimiter = this._scanner.GetToken();

            //if (endDelimiter != ";")
            //    throw new InvalidDataException("Invalid instruction format. Missing ';' for instruction " + instruction.Name);

            return instruction;
        }

        private Variable ParseVariable()
        {
            string variableName = this._scanner.GetPreviousToken();
            string variableValue = this._scanner.GetToken();
            string endDelimiter = this._scanner.GetToken();

            if (endDelimiter != ";")
                throw new InvalidDataException("Invalid variable format. Missing ';' for variable " + variableName);

            return new Variable(variableName, variableValue);
        }

        public void Dispose()
        {
            foreach (var statement in this._statements)
            {
                if (statement is IDisposable)
                    (statement as IDisposable).Dispose();
            }

            this._statements.Clear();
            this._scanner.Dispose();
        }
    }
}
