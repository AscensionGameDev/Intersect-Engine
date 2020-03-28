using System;
using System.Text.RegularExpressions;

namespace Intersect.Utilities
{

    public abstract class Script
    {

        public static Regex CommandRegex = new Regex(@"^([^ \t'""]+)(?:[ \t]+(""[^""]*""|'[^']*'|[^ \t'""]+))*$");

        protected Script()
        {
            Name = GetType().Name.ToLower();
        }

        public string Name { get; }

        public abstract Result Run(string[] environmentArgs, string[] commandArgs);

        public struct Result
        {

            public int Code;

            public string Message;

            public Exception Exception;

        }

    }

}
