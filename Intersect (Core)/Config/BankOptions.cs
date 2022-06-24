using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Config
{
    public partial class BankOptions
    {
        /// <summary>
        /// The intersect default maximum bank slot value a guild OR player could ever have
        /// </summary>
        public const int DefaultMaxSlots = 500;

        /// <summary>
        /// The max amount of bank slots possible, player, guild, or otherwise.
        /// </summary>
        public int MaxSlots { get; set; } = DefaultMaxSlots;
    }
}
