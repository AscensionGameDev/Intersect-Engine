using Intersect.Client.Framework;
using Intersect.Client.Framework.Audio;

using System;

namespace Intersect.Client.MonoGame.Audio
{
    public abstract class MonoAudioInstance<TAudioSource> : GameAudioInstance where TAudioSource : GameAudioSource
    {
        /// <inheritdoc />
        protected MonoAudioInstance(IGameContext gameContext, GameAudioSource source) : base(gameContext, source)
        {
        }

        public new TAudioSource AudioSource =>
            base.AudioSource as TAudioSource ?? throw new InvalidOperationException();
    }
}
