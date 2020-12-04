using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Localization;
using Intersect.Client.MonoGame.Audio;
using Intersect.Logging;

using System;
using System.Collections.Generic;
using System.IO;

using Intersect.Client.MonoGame.Graphics;

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

        public override IFont FindFont(string fontName, int fontSize)
        {
            var font = base.FindFont(fontName, fontSize);

            if (font != null)
            {
                return font;
            }

            var assetReference = MonoFontGenerator.GenerateSpriteFont(
                new SpriteFontDescriptor
                {
                    FontName = fontName,
                    Size = fontSize,
                    CharacterRegions = new List<CharacterRegion>
                    {
                        new CharacterRegion
                        {
                            Start = 0x20,
                            End = 0x7F
                        }
                    }
                }
            );

            if (assetReference == default)
            {
                return default;
            }

            font = CreateFont(assetReference);

            if (font != default)
            {
                AssetLookup.Add(ContentType.Font, font);
            }

            return font;
        }
    }
}
