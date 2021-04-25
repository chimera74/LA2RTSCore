using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleScripts
{
    public class SyntaxException : Exception
    {
        public SyntaxException() : base()
        {

        }

        public SyntaxException(string message) : base(message)
        {

        }
    }
}
