using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSciptsPreprod
{
    class Program
    {
        static void Main(string[] args)
        {

            string scriptSrc = @"testparam(35)


testnoparam()
";

            Queue<Command> commandQ = new Queue<Command>();
            HashSet<Command> commandSet = new HashSet<Command>();
            commandSet.Add(new TestNoParamCommand());
            commandSet.Add(new TestParamCommand());

            try
            {
                var reader = new StringReader(scriptSrc);
                string line = null;
                do
                {
                    line = reader.ReadLine();
                    if (line == null)
                        break;
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    foreach (Command c in commandSet)
                    {
                        if (c.TryParseName(line))
                        {
                            commandQ.Enqueue(c.Copy());
                        }
                    }

                } while (line != null);

                Command nextComm = null;
                while (commandQ.Count > 0)
                {
                    nextComm = commandQ.Dequeue();
                    nextComm.Execute();
                }
            } catch (ArgumentException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }

            Console.In.Read();
        }
    }
}
