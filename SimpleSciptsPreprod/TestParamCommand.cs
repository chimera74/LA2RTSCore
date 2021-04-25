using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSciptsPreprod
{
    public class TestParamCommand : Command
    {   
        public int IntParameter;

        public const string COMMAND_NAME = "TestParam";

        public TestParamCommand()
        {
            Name = COMMAND_NAME;
        }

        public override bool ParseParameter()
        {
            try
            {
                IntParameter = Int32.Parse(Parameter);
            } catch (Exception)
            {
                return false;
            }

            return true;
        }

        public override void Execute()
        {
            Console.Out.Write("TestParam executed with parameter " + IntParameter.ToString() + ".\n");
        }

        public override Command Copy()
        {
            var r = new TestParamCommand();
            r.IntParameter = IntParameter;
            r.Parameter = Parameter;
            return r;
        }
    }
}
