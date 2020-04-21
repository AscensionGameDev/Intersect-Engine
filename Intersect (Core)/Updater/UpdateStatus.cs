using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Updater
{
    public enum UpdateStatus
    {
        Checking = 0,
        Updating = 1,
        Restart,
        Done,
        Error,
        None
    }
}
