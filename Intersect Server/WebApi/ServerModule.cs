using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace Intersect.Server.WebApi
{
    public abstract class ServerModule : NancyModule
    {
        protected ServerModule()
        {
            
        }
    }
}
