using System;

using JetBrains.Annotations;

namespace Intersect.Core.ExperimentalFeatures
{

    public struct ExperimentalFlagAlias : IExperimentalFlag
    {

        [NotNull] private readonly IFlagProvider mFlagProvider;

        [NotNull] private readonly string mTargetName;

        private Guid mCachedGuid;

        [NotNull]
        public string Name { get; }

        public ExperimentalFlagAlias(
            [NotNull] IFlagProvider flagProvider,
            [NotNull] string targetName,
            [NotNull] string aliasName
        )
        {
            if (string.IsNullOrWhiteSpace(targetName))
            {
                throw new ArgumentNullException(nameof(targetName));
            }

            if (string.IsNullOrWhiteSpace(aliasName))
            {
                throw new ArgumentNullException(nameof(aliasName));
            }

            mFlagProvider = flagProvider;
            mTargetName = targetName;
            mCachedGuid = default(Guid);

            Name = aliasName;
        }

        /// <inheritdoc />
        public Guid Guid
        {
            get
            {
                if (mCachedGuid != default(Guid))
                {
                    return mCachedGuid;
                }

                if (!mFlagProvider.TryGet(mTargetName, out var flag))
                {
                    throw new ArgumentException(nameof(mTargetName));
                }

                return mCachedGuid = flag.Guid;
            }
        }

        /// <inheritdoc />
        public bool Enabled => mFlagProvider.IsEnabled(Guid);

        /// <inheritdoc cref="IEquatable{T}" />
        public bool Equals(IExperimentalFlag other)
        {
            return Guid == other?.Guid && Enabled == other.Enabled;
        }

        /// <inheritdoc />
        public IExperimentalFlag With(bool enabled)
        {
            if (!mFlagProvider.TryGet(Guid, out var flag))
            {
                throw new InvalidOperationException();
            }

            return flag.With(enabled);
        }

    }

}
