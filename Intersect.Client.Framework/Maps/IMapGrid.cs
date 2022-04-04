using System;

namespace Intersect.Client.Framework.Maps
{
    public interface IMapGrid
    {

        /// <summary>
        /// The contents of the current map grid known to the client.
        /// </summary>
        Guid[,] Content { get; }

        /// <summary>
        /// The Width of the current map grid.
        /// </summary>
        long Width { get; }

        /// <summary>
        /// The Height of the current map grid.
        /// </summary>
        long Height { get; }
    }
}
