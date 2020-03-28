using System;

using JetBrains.Annotations;

namespace Intersect.Core.ExperimentalFeatures
{

    public interface IFlagProvider
    {

        bool IsEnabled(Guid flagId);

        bool IsEnabled([NotNull] string flagName);

        bool TryGet(Guid flagId, out IExperimentalFlag flag);

        bool TryGet([NotNull] string flagName, out IExperimentalFlag flag);

    }

}
