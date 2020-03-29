using System;

namespace Intersect.Utilities.Scripts
{

    public class Exit : Script
    {

        public override Result Run(string[] environmentArgs, string[] commandArgs)
        {
            Environment.Exit(0);

            return new Result();
        }

    }

}
