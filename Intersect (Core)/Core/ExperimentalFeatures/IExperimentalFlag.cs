using System;

namespace Intersect.Core.ExperimentalFeatures
{

    public interface IExperimentalFlag : IEquatable<IExperimentalFlag>
    {

        Guid Guid { get; }

        string Name { get; }

        bool Enabled { get; }

        /// <summary>
        /// Creates a clone of this <see cref="IExperimentalFlag"/> with the given enablement.
        /// </summary>
        /// <param name="enabled">the new enablement state</param>
        /// <returns>a clone of this flag with the new enablement state</returns>
        IExperimentalFlag With(bool enabled);

    }

}
