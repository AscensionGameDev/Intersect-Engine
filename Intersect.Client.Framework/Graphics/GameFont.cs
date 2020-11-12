using System;
using System.Text.RegularExpressions;

using Intersect.Client.Framework.Content;

namespace Intersect.Client.Framework.Graphics
{
    public abstract class GameFont : IFont
    {
        public static readonly Regex FontAssetNameFormat = new Regex("^(.+)(?:_(\\d+))$");

        public static string FormatAssetName(string fontName, int fontSize) => $"{fontName}_{fontSize}";

        protected GameFont(IGameContext gameContext, AssetReference assetReference)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            Reference = assetReference;

            var match = FontAssetNameFormat.Match(Name);

            if (!match.Success || match.Groups.Count < 3)
            {
                FontName = Name;
                Size = -1;
            }
            else
            {
                FontName = match.Groups[0].Value;

                var sizeGroup = match.Groups[1];

                if (!sizeGroup.Success || !int.TryParse(sizeGroup.Value, out int size))
                {
                    size = 0;
                }

                Size = size;
            }
        }

        /// <summary>
        /// The game context the font belongs to.
        /// </summary>
        protected IGameContext GameContext { get; }

        /// <inheritdoc/>
        public string Name => Reference.Name;

        /// <inheritdoc/>
        public AssetReference Reference { get; }

        /// <inheritdoc/>
        public string FontName { get; }

        /// <inheritdoc/>
        public int Size { get; }

        public abstract TFont AsPlatformFont<TFont>();

        public override string ToString() => $"{FontName},{Size}";
    }
}
