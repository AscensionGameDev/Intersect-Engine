using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Intersect.Server.Models
{
    public class User
    {
        public Guid Guid { get; set; }

        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public string Email { get; set; }

        [NotNull]
        public string Password { get; set; }

        [NotNull]
        public string Salt { get; set; }
        
        public long Power { get; set; }
    }
}
