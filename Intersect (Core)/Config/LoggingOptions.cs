using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Config
{
    public class LoggingOptions
    {
        /// <summary>
        /// Determines whether chat logs should be written into the logging database
        /// </summary>
        public bool Chat { get; set; } = true;

        /// <summary>
        /// Determines whether trades should be written into the logging database
        /// </summary>
        public bool Trade { get; set; } = true;

        /// <summary>
        /// Determines if general user activity (logins, logouts, character creations/deletions, etc) should be written into the logging database
        /// </summary>
        public bool UserActivity { get; set; } = true;
    }
}
