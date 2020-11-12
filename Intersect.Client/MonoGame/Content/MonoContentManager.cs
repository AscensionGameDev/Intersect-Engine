using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Content;
using Intersect.Client.Localization;
using Intersect.Client.MonoGame.Audio;
using Intersect.Logging;

using System;
using System.IO;

namespace Intersect.Client.MonoGame.Content
{
    internal class MonoContentManager : GameContentManager
    {
        public MonoContentManager(MonoGameContext gameContext) : base(gameContext, new FileSystemAssetProvider())
        {
            Current = this;

            if (!Directory.Exists("resources"))
            {
                Log.Error(Strings.Errors.resourcesnotfound);

                Environment.Exit(1);
            }
        }

        /// <inheritdoc />
        protected override IAudioSource CreateMusic(AssetReference assetReference) => new MonoMusicSource(GameContext, assetReference);

        /// <inheritdoc />
        protected override IAudioSource CreateSound(AssetReference assetReference) => new MonoSoundSource(GameContext, assetReference);
    }
}
