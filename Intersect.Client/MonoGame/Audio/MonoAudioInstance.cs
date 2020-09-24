using System;

using Intersect.Client.Framework.Audio;

using JetBrains.Annotations;

namespace Intersect.Client.MonoGame.Audio
{

    public abstract class MonoAudioInstance<TAudioSource> : GameAudioInstance where TAudioSource : GameAudioSource
    {

        /// <inheritdoc />
        protected MonoAudioInstance([NotNull] GameAudioSource source) : base(source)
        {
        }

        [NotNull]
        public new TAudioSource AudioSource => base.AudioSource as TAudioSource ?? throw new InvalidOperationException();
    }

}
