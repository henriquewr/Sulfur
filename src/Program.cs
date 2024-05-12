using Sulfur.Main;
using Sulfur.Runtime;
using System;
using System.IO;
using Environment = Sulfur.Runtime.Environment;

namespace Sulfur
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var environment = Environment.CreateGlobalEnv();

            var examplesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Examples");

            var hangManGame = File.ReadAllText(Path.Combine(examplesPath, "Hangman.sf"));
            var fibonacci = File.ReadAllText(Path.Combine(examplesPath, "Fibonacci.sf"));
            var sourceText = File.ReadAllText("source.sf");

            var source = hangManGame; //change here

            var program = Parser.CreateAST(source);
            var result = Interpreter.Evaluate(program, environment);
        }
    }
}
