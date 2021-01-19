using System;

using Intersect.Client.Framework.Audio;

namespace Intersect.Client.MonoGame.Audio
{

    public abstract class MonoAudioInstance<TSource> : GameAudioInstance where TSource : GameAudioSource
    {

        /// <inheritdoc />
        protected MonoAudioInstance(GameAudioSource source) : base(source)
        {
        }

        public new TSource Source => base.Source as TSource ?? throw new InvalidOperationException();

    }

}
