using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSciptsPreprod
{
    public class TestNoParamCommand : Command
    {
        public const string COMMAND_NAME = "TestNoParam";

        public TestNoParamCommand()
        {
            Name = COMMAND_NAME;
        }

        public override void Execute()
        {
            Console.Out.Write("TestNoParam executed.\n");
        }

        public override Command Copy()
        {
            var r = new TestNoParamCommand();
            r.Parameter = Parameter;
            return r;
        }
    }
}
