using System;

namespace Intersect.Core.ExperimentalFeatures
{

    public interface IFlagProvider
    {

        bool IsEnabled(Guid flagId);

        bool IsEnabled(string flagName);

        bool TryGet(Guid flagId, out IExperimentalFlag flag);

        bool TryGet(string flagName, out IExperimentalFlag flag);

    }

}
