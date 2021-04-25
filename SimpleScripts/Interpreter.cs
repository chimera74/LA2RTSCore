using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleScripts
{
    public class Interpreter
    {
        private HashSet<Command> _commandSet;
        private Queue<Command> _commandQ;

        public Interpreter(HashSet<Command> commandSet)
        {
            if (commandSet == null)
                    throw new ArgumentNullException("Command set is null");
            _commandSet = commandSet;
            _commandQ = new Queue<Command>();
        }

        public void LoadScript(string scriptText)
        {
            try
            {
                var reader = new StringReader(scriptText);
                string line = null;
                int lineCount = 0;
                do
                {
                    line = reader.ReadLine();
                    if (line == null)
                        break;
                    if (string.IsNullOrEmpty(line.Trim()))
                        continue;

                    bool found = false;
                    foreach (Command c in _commandSet)
                    {
                        if (c.TryParseCommand(line))
                        {
                            _commandQ.Enqueue(c.Copy());
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        throw new SyntaxException("Script syntax error at line " + lineCount + ". Unknown command \"" + line + "\".");
                    }

                    lineCount++;
                } while (line != null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ExecuteScript()
        {
            Command nextComm = null;
            while (_commandQ.Count > 0)
            {
                nextComm = _commandQ.Dequeue();
                nextComm.Execute();
            }
        }

    }
}
