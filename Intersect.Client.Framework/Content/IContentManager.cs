using JetBrains.Annotations;

using System;
using System.Collections.Generic;
using System.IO;

using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Graphics;
using Intersect.Plugins;

namespace Intersect.Client.Framework.Content
{

    public interface IContentManager
    {

        void LoadAll();

        TAsset Load<TAsset>(ContentType contentType, [NotNull] string assetName) where TAsset : class, IAsset;

        TAsset Load<TAsset>(ContentType contentType, [NotNull] string assetName, [NotNull] Func<string, Stream> createStream)
            where TAsset : class, IAsset;

        TAsset LoadEmbedded<TAsset>(
            [NotNull] IPluginContext context,
            ContentType contentType,
            [NotNull] string assetName
        ) where TAsset : class, IAsset;

        IFont LoadFont(string name, int size);

        IAudioSource LoadMusic(string musicName);

        IAudioSource LoadSound(string soundName);

        ITexture LoadTexture(TextureType textureType, string textureFileName);

        void LoadAudio();

        void LoadMusic();

        void LoadSounds();

        void LoadTilesets(params string[] tilesetNames);

        void LoadTilesets(IEnumerable<string> tilesetNames);

        string[] GetTextureNames(TextureType textureType);
    }

}
