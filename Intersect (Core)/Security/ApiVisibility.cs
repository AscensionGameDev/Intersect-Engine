using System;

namespace Intersect.Security
{

    [Flags]
    public enum ApiVisibility : uint
    {

        /// <summary>
        /// Anyone has access.
        /// </summary>
        Public = 0xFFFFFFFF,

        /// <summary>
        /// Privileged users have access.
        /// </summary>
        Restricted = 0x2,

        /// <summary>
        /// Data owners have access.
        /// </summary>
        Private = 0x1,

        /// <summary>
        /// No users have access.
        /// </summary>
        Hidden = 0

    }

}
