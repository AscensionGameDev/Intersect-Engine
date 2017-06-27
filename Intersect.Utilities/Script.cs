using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Intersect.Utilities
{
    public abstract class Script
    {
        public struct Result
        {
            public int Code;
            public string Message;
            public Exception Exception;
        }

        public static Regex COMMAND_REGEX = new Regex(@"^([^ \t'""]+)(?:[ \t]+(""[^""]*""|'[^']*'|[^ \t'""]+))*$");

        public string Name { get; }

        protected Script()
        {
            Name = GetType().Name.ToLower();
        }

        public abstract Result Run(string[] environmentArgs, string[] commandArgs);
    }
}
