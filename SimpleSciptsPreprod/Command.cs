using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleSciptsPreprod
{
    public abstract class Command
    {
        public string Name;
        public string Parameter;

        public virtual bool TryParseName(string line)
        {
            var str = line.Trim().ToLower();
            if (!(str.IndexOf(Name.ToLower() + "(") == 0))
                return false;
                                         
            Regex searchTerm = new Regex(@"(?<=\().+?(?=\))");
            Parameter = (searchTerm.Match(str).Groups.Count > 0) ?
                    searchTerm.Match(str).Groups[0].Value : null;

            if (Parameter == null)
                throw new ArgumentException("Error parsing parameter on \"" + Name + "\" function. Param is null");

            if (!ParseParameter())
                throw new ArgumentException("Error parsing parameter on \"" + Name + "\" function. Parameter is \"" + Parameter + "\".");

            return true;
        }

        public virtual bool ParseParameter()
        {
            return true;
        }

        public abstract void Execute();
        public abstract Command Copy();

        public override bool Equals(object obj)
        {
            var otherClient = obj as Command;
            if (otherClient == null)
                return false;

            return Name.Equals(otherClient.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

    }
}
