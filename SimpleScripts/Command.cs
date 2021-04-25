using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleScripts
{
    public class Command
    {
        public string Name;
        public string Parameter;

        internal Action _action;

        public Command()
        {
        }

        public Command(string name, Action action)
        {
            Name = name.Trim();
            _action = action;
        }

        public virtual bool TryParseCommand(string line)
        {
            var str = line.Trim().ToLower();
            if (!(str.IndexOf(Name + "(") == 0))
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

        public virtual void Execute()
        {
            _action.Invoke();
        }

        public virtual Command Copy()
        {
            var r = new Command(Name, _action);
            return r;
        }
    }


    public class Command<T> : Command
    {
        public T ParsedParameter;
        internal Action<T> _paramAction;

        public Command(string name, Action<T> action)
        {
            if (!IsEligableType())
                throw new Exception("Type for Command<T> can't be " +
                    ParsedParameter.GetType().ToString() +
                    ". Only int, bool and string are supported");

            Name = name;
            _paramAction = action;
        }

        public override bool ParseParameter()
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    ParsedParameter = (T)converter.ConvertFromString(Parameter);
                    return true;
                }
            }
            catch (NotSupportedException)
            {
                return false;
            }

            return false;
        }

        private bool IsEligableType()
        {
            return typeof(T) == typeof(int) ||
                typeof(T) == typeof(bool) ||
                typeof(T) == typeof(string);
        }

        public override Command Copy()
        {
            var r = new Command<T>(Name, _paramAction);
            r.ParsedParameter = ParsedParameter;
            return r;
        }

        public override void Execute()
        {
            _paramAction.Invoke(ParsedParameter);
        }



    }
}
