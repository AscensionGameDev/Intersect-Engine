using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Enums
{
    /// <summary>
    /// Used to determine warping logic across the different types of map instance a developer may request the creation of
    /// </summary>
    public enum MapInstanceType
    {
        /// <summary>
        /// The overworld instance - the instance that is commonly shared amongst players, and where players are placed in default situations
        /// </summary>
        Overworld = 0,

        /// <summary>
        /// The type used whenever you want a player to be alone on their own map instance
        /// </summary>
        Personal,

        /// <summary>
        /// An instance that is shared amongst members of a guild
        /// </summary>
        Guild,

        /// <summary>
        /// The instance that is shared amongst members of a party
        /// </summary>
        Shared,
    }
}
