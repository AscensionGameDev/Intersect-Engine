using System.Diagnostics;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Localization;
using Intersect.Client.MonoGame.Audio;
using Intersect.Compression;
using Intersect.Configuration;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Intersect.Client.MonoGame.File_Management;


public partial class MonoContentManager : GameContentManager
{

    public MonoContentManager()
    {
        var rootPath = Path.GetFullPath(ClientConfiguration.ResourcesDirectory);

        if (!Directory.Exists(rootPath))
        {
            ApplicationContext.Context.Value?.Logger.LogError(Strings.Errors.ResourcesNotFound);

            Environment.Exit(1);
        }

        ContentWatcher = new ContentWatcher(rootPath);
    }

    //Graphic Loading
    public override void LoadTilesets(string[] tilesetnames)
    {
        mTilesetDict.Clear();

        IEnumerable<string> tilesetFiles = Array.Empty<string>();

        var dir = Path.Combine(ClientConfiguration.ResourcesDirectory, "tilesets");
        if (!Directory.Exists(dir))
        {
            if (!Directory.Exists(Path.Combine(ClientConfiguration.ResourcesDirectory, "packs")))
            {
                Directory.CreateDirectory(dir);
            }
        }
        else
        {
            tilesetFiles = Directory.GetFiles(dir).Select(f => Path.GetFileName(f));
        }

        foreach (var t in tilesetnames)
        {
            var realFilename = tilesetFiles.FirstOrDefault(file => t.Equals(file, StringComparison.InvariantCultureIgnoreCase)) ?? t;
            if (!string.IsNullOrWhiteSpace(t) &&
                (!string.IsNullOrWhiteSpace(realFilename) ||
                 AtlasReference.TryGet(Path.Combine(ClientConfiguration.ResourcesDirectory, "tilesets", t.ToLower()), out _) != null) &&
                !mTilesetDict.ContainsKey(t.ToLower()))
            {
                mTilesetDict.Add(
                    t.ToLower(),
                    Core.Graphics.Renderer.LoadTexture(
                        Path.Combine(ClientConfiguration.ResourcesDirectory, "tilesets", t),
                        Path.Combine(ClientConfiguration.ResourcesDirectory, "tilesets", realFilename)
                    )
                );
            }
        }

        TilesetsLoaded = true;
    }

    public override void LoadTexturePacks()
    {
        mTexturePackDict.Clear();
        var dir = Path.Combine(ClientConfiguration.ResourcesDirectory, "packs");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var items = Directory.GetFiles(dir, "*.meta");
        for (var i = 0; i < items.Length; i++)
        {
            var json = GzipCompression.ReadDecompressedString(items[i]);
            var obj = JObject.Parse(json);
            var frames = (JArray) obj["frames"];
            var img = obj["meta"]["image"].ToString();
            if (File.Exists(Path.Combine(ClientConfiguration.ResourcesDirectory, "packs", img)))
            {
                var platformText = Core.Graphics.Renderer.LoadTexture(Path.Combine(ClientConfiguration.ResourcesDirectory, "packs", img), Path.Combine(ClientConfiguration.ResourcesDirectory, "packs", img));
                if (platformText != null)
                {
                    foreach (var frame in frames)
                    {
                        var assetName = frame["filename"].ToString();
                        var sourceRect = new Rectangle(
                            int.Parse(frame["frame"]["x"].ToString()), int.Parse(frame["frame"]["y"].ToString()),
                            int.Parse(frame["frame"]["w"].ToString()), int.Parse(frame["frame"]["h"].ToString())
                        );

                        var rotated = bool.Parse(frame["rotated"].ToString());
                        var sourceSize = new Rectangle(
                            int.Parse(frame["spriteSourceSize"]["x"].ToString()),
                            int.Parse(frame["spriteSourceSize"]["y"].ToString()),
                            int.Parse(frame["spriteSourceSize"]["w"].ToString()),
                            int.Parse(frame["spriteSourceSize"]["h"].ToString())
                        );

                        AtlasReference.Add(new AtlasReference(assetName, sourceRect, rotated, sourceSize, platformText));
                    }
                }
            }
        }
    }

    public void LoadTextureGroup(string directory, Dictionary<string, IAsset> dict)
    {
        dict.Clear();
        var dir = Path.Combine(ClientConfiguration.ResourcesDirectory, directory);
        if (!Directory.Exists(dir))
        {
            if (!Directory.Exists(Path.Combine(ClientConfiguration.ResourcesDirectory, "packs")))
            {
                Directory.CreateDirectory(dir);
            }
        }
        else
        {
            var items = Directory.GetFiles(dir, "*.png");
            for (var i = 0; i < items.Length; i++)
            {
                var filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                dict.Add(filename, Core.Graphics.Renderer.LoadTexture(Path.Combine(dir, filename), Path.Combine(dir, items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar))));
            }
        }

        var packItems = AtlasReference.GetAllFor(directory);
        foreach (var itm in packItems)
        {
            var filename = Path.GetFileName(itm.Name.ToLower().Replace("\\", "/"));
            if (!dict.ContainsKey(filename))
            {
                dict.Add(filename, Core.Graphics.Renderer.LoadTexture(Path.Combine(dir, filename), Path.Combine(dir, Path.Combine(dir, filename))));
            }
        }
    }

    public override void LoadItems()
    {
        LoadTextureGroup("items", mItemDict);
    }

    public override void LoadEntities()
    {
        LoadTextureGroup("entities", mEntityDict);
    }

    public override void LoadSpells()
    {
        LoadTextureGroup("spells", mSpellDict);
    }

    public override void LoadAnimations()
    {
        LoadTextureGroup("animations", mAnimationDict);
    }

    public override void LoadFaces()
    {
        LoadTextureGroup("faces", mFaceDict);
    }

    public override void LoadImages()
    {
        LoadTextureGroup("images", mImageDict);
    }

    public override void LoadFogs()
    {
        LoadTextureGroup("fogs", mFogDict);
    }

    public override void LoadResources()
    {
        LoadTextureGroup(ClientConfiguration.ResourcesDirectory, mResourceDict);
    }

    public override void LoadPaperdolls()
    {
        LoadTextureGroup("paperdolls", mPaperdollDict);
    }

    public override void LoadGui()
    {
        LoadTextureGroup("gui", mGuiDict);
    }

    public override void LoadMisc()
    {
        LoadTextureGroup("misc", mMiscDict);
    }

    public override void LoadFonts()
    {
        mFontDict.Clear();

        var assetDirectory = Path.Combine(ClientConfiguration.ResourcesDirectory, "fonts");
        if (!Directory.Exists(assetDirectory))
        {
            Directory.CreateDirectory(assetDirectory);
        }

        var fileInfos = Directory.GetFiles(assetDirectory, "*.xnb")
            .Select(filePath => new FileInfo(filePath))
            .ToArray();
        Dictionary<string, Dictionary<int, FileInfo>> discoveredFontFiles = [];
        foreach (var fileInfo in fileInfos)
        {
            var rawName = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var sizeSeparatorIndex = rawName.LastIndexOf(',');
            if (sizeSeparatorIndex < 1)
            {
                sizeSeparatorIndex = rawName.LastIndexOf('_');
            }

            if (sizeSeparatorIndex < 1)
            {
                ApplicationContext.CurrentContext.Logger.LogWarning(
                    "Ignoring invalid font file name: '{FileName}' located at {FilePath}",
                    fileInfo.Name,
                    fileInfo.FullName
                );
                continue;
            }

            var rawSize = rawName[(sizeSeparatorIndex + 1)..];
            if (!int.TryParse(rawSize, out var size))
            {
                ApplicationContext.CurrentContext.Logger.LogWarning(
                    "Font size '{Size}' in font file name '{FileName}' is not a valid integer",
                    rawSize,
                    rawName
                );
                continue;
            }

            if (size < 1)
            {
                ApplicationContext.CurrentContext.Logger.LogWarning(
                    "Font size '{Size}' in font file name '{FileName}' must be greater than or equal to 1",
                    size,
                    rawName
                );
                continue;
            }

            var fontName = rawName[..sizeSeparatorIndex].Trim().ToLowerInvariant();
            if (!discoveredFontFiles.TryGetValue(fontName, out var fontFamilyCollection))
            {
                fontFamilyCollection = [];
                discoveredFontFiles[fontName] = fontFamilyCollection;
            }

            if (fontFamilyCollection.TryGetValue(size, out var originalFileInfo))
            {
                ApplicationContext.CurrentContext.Logger.LogWarning(
                    "A font file for size {Size} already exists for the font family {Family}, the first one is located at {OriginalPath} and this one is located at {CurrentPath}",
                    size,
                    fontName,
                    originalFileInfo.FullName,
                    fileInfo.FullName
                );
                continue;
            }

            fontFamilyCollection[size] = fileInfo;
        }

        foreach (var (fontName, fontSourcesBySize) in discoveredFontFiles)
        {
            var font = Core.Graphics.Renderer.LoadFont(fontName, fontSourcesBySize);
            if (mFontDict.TryAdd(fontName, font))
            {
                continue;
            }

            throw new UnreachableException();
        }
    }

    public override void LoadShaders()
    {
        mShaderDict.Clear();

        const string shaderPrefix = "Intersect.Client.Resources.Shaders.";
        var availableShaders = typeof(MonoContentManager).Assembly
            .GetManifestResourceNames()
            .Where(resourceName =>
                resourceName.StartsWith(shaderPrefix)
                && resourceName.EndsWith(".xnb")
            ).ToArray();

        for (var i = 0; i < availableShaders.Length; i++)
        {
            var resourceFullName = availableShaders[i];
            var shaderNameWithoutExtension = resourceFullName.Substring(0, resourceFullName.Length - 4);
            var shaderName = shaderNameWithoutExtension.Substring(shaderPrefix.Length);
            mShaderDict.Add(shaderName, Core.Graphics.Renderer.LoadShader(resourceFullName));
        }
    }

    public override void LoadSounds()
    {
        mSoundDict.Clear();
        var dir = Path.Combine(ClientConfiguration.ResourcesDirectory, "sounds");
        if (!Directory.Exists(dir))
        {
            if (!Directory.Exists(Path.Combine(ClientConfiguration.ResourcesDirectory, "packs")))
            {
                Directory.CreateDirectory(dir);
            }
        }
        else
        {
            var items = Directory.GetFiles(dir, "*.wav");
            for (var i = 0; i < items.Length; i++)
            {
                var filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                mSoundDict.Add(
                    RemoveExtension(filename),
                    new MonoSoundSource(
                        Path.Combine(dir, filename), Path.Combine(dir, items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar))
                    )
                );
            }
        }

        // If we have a sound index file, load from it!
        if (File.Exists(Path.Combine(ClientConfiguration.ResourcesDirectory, "packs", "sound.index")))
        {
            SoundPacks = new AssetPacker(Path.Combine(ClientConfiguration.ResourcesDirectory, "packs", "sound.index"), Path.Combine(ClientConfiguration.ResourcesDirectory, "packs"));
            foreach(var item in SoundPacks.FileList)
            {
                if (!mSoundDict.ContainsKey(RemoveExtension(item).ToLower()))
                {
                    mSoundDict.Add(
                        RemoveExtension(item).ToLower(),
                        new MonoSoundSource(item, item)
                    );
                }
            }
        }

    }

    public override void LoadMusic()
    {
        mMusicDict.Clear();
        var dir = Path.Combine(ClientConfiguration.ResourcesDirectory, "music");
        if (!Directory.Exists(dir))
        {
            if (!Directory.Exists(Path.Combine(ClientConfiguration.ResourcesDirectory, "packs")))
            {
                Directory.CreateDirectory(dir);
            }
        }
        else
        {
            var items = Directory.GetFiles(dir, "*.ogg");
            for (var i = 0; i < items.Length; i++)
            {
                var filename = items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar).ToLower();
                mMusicDict.Add(RemoveExtension(filename), new MonoMusicSource(Path.Combine(dir, filename), Path.Combine(dir, items[i].Replace(dir, "").TrimStart(Path.DirectorySeparatorChar))));
            }
        }

        // If we have a music index file, load from it!
        if (File.Exists(Path.Combine(ClientConfiguration.ResourcesDirectory, "packs", "music.index")))
        {
            MusicPacks = new AssetPacker(Path.Combine(ClientConfiguration.ResourcesDirectory, "packs", "music.index"), Path.Combine(ClientConfiguration.ResourcesDirectory, "packs"));
            foreach (var item in MusicPacks.FileList)
            {
                if (!mMusicDict.ContainsKey(RemoveExtension(item).ToLower()))
                {
                    mMusicDict.Add(
                        RemoveExtension(item).ToLower(),
                        new MonoMusicSource(item, item)
                    );
                }
            }
        }
    }

    /// <inheritdoc />
    protected override TAsset Load<TAsset>(
        Dictionary<string, IAsset> lookup,
        ContentType contentType,
        string name,
        Func<Stream> createStream
    )
    {
        switch (contentType)
        {
            case ContentType.Animation:
            case ContentType.Entity:
            case ContentType.Face:
            case ContentType.Fog:
            case ContentType.Image:
            case ContentType.Interface:
            case ContentType.Item:
            case ContentType.Miscellaneous:
            case ContentType.Paperdoll:
            case ContentType.Resource:
            case ContentType.Spell:
            case ContentType.TextureAtlas:
            case ContentType.Tileset:
                var asset = Core.Graphics.Renderer.CreateTextureFromStreamFactory(name, createStream) as TAsset;
                lookup?.Add(name, asset);
                return asset;

            case ContentType.Font:
                throw new NotImplementedException();

            case ContentType.Shader:
                throw new NotImplementedException();

            case ContentType.Music:
                var music = new MonoMusicSource(createStream) as TAsset;
                lookup?.Add(RemoveExtension(name), music);
                return music;

            case ContentType.Sound:
                var sound = new MonoSoundSource(createStream, name) as TAsset;
                lookup?.Add(RemoveExtension(name), sound);
                return sound;

            default:
                throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null);
        }
    }

}
