using SimpleScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSimpleScripts
{
    class Program
    {
        static void Main(string[] args)
        {
            HashSet<Command> commands = new HashSet<Command>();
            commands.Add(new Command("noparam", () => {
                Console.Out.WriteLine("noparam executed");
            }));
            commands.Add(new Command<int>("intparam", (p) => {
                Console.Out.WriteLine("intparam executed with param " + p.ToString());
            }));
            commands.Add(new Command<bool>("boolparam", (p) => {
                Console.Out.WriteLine("boolparam executed with param " + p.ToString());
            }));
            commands.Add(new Command<string>("stringparam", (p) => {
                Console.Out.WriteLine("stringparam executed with param " + p.ToString());
            }));
            Interpreter interpreter = new Interpreter(commands);

            try
            {
                interpreter.LoadScript(@"noparam()
                intparam(10)
                boolparam(true)
                stringparam(paramstr)");

                interpreter.ExecuteScript();
            } catch (SyntaxException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }

            Console.In.Read();
        }
    }
}
