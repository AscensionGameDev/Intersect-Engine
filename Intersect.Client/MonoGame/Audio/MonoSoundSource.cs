using Intersect.Client.Framework;
using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Content;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Logging;

using Microsoft.Xna.Framework.Audio;

using System;

namespace Intersect.Client.MonoGame.Audio
{
    public class MonoSoundSource : GameAudioSource
    {
        private int mInstanceCount;

        private SoundEffect mSound;

        public MonoSoundSource(IGameContext gameContext, AssetReference assetReference) : base(
            gameContext, assetReference, AudioType.SoundEffect
        )
        {
        }

        public SoundEffect Effect
        {
            get
            {
                if (mSound == null)
                {
                    LoadSound();
                }

                return mSound;
            }
        }

        public override IAudioInstance CreateInstance()
        {
            mInstanceCount++;

            return new MonoSoundInstance(GameContext, this);
        }

        public void ReleaseEffect()
        {
            if (--mInstanceCount > 0)
            {
                return;
            }

            mSound?.Dispose();
            mSound = null;
        }

        private void LoadSound()
        {
            using (var assetStream = GameContext.ContentManager.OpenRead(Reference))
            {
                try
                {
                    mSound = SoundEffect.FromStream(assetStream);
                }
                catch (Exception exception)
                {
                    Log.Error($"Error loading '{Reference}'.", exception);
                    ChatboxMsg.AddMessage(
                        new ChatboxMsg(
                            $"{Strings.Errors.LoadFile.ToString(Strings.Words.lcase_sound)} [{Reference}]",
                            new Color(0xBF, 0x0, 0x0)
                        )
                    );
                }
            }
        }
    }
}
