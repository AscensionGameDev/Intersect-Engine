using System.Diagnostics.CodeAnalysis;
using Intersect.Async;
using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Compression;
using Intersect.Configuration;
using Intersect.Core;
using Intersect.Plugins;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Framework.File_Management;


public abstract partial class GameContentManager : IContentManager
{
    public enum UI
    {
        Menu,

        InGame,

        Shared,

        Debug,
    }

    public static GameContentManager Current
    {
        get => _current ?? throw new InvalidOperationException("Content manager not initialized");
        private set => _current = value;
    }

    protected readonly Dictionary<string, IAsset> mAnimationDict = [];

    protected readonly Dictionary<string, IAsset> mEntityDict = [];

    protected readonly Dictionary<string, IAsset> mFaceDict = [];

    protected readonly Dictionary<string, IAsset> mFogDict = [];

    protected readonly Dictionary<string, IFont> mFontDict = [];

    protected readonly Dictionary<string, IAsset> mGuiDict = [];

    protected readonly Dictionary<string, IAsset> mImageDict = [];

    protected readonly Dictionary<string, IAsset> mItemDict = [];

    protected readonly Dictionary<string, IAsset> mMiscDict = [];

    protected readonly Dictionary<string, IAsset> mMusicDict = [];

    protected readonly Dictionary<string, IAsset> mPaperdollDict = [];

    protected readonly Dictionary<string, IAsset> mResourceDict = [];

    protected readonly Dictionary<string, GameShader> mShaderDict = [];

    protected readonly Dictionary<string, IAsset> mSoundDict = [];

    protected readonly Dictionary<KeyValuePair<UI, string>, string> mUiDict = [];

    protected readonly Dictionary<string, IAsset> mSpellDict = [];

    protected readonly  Dictionary<string, IAsset> mTexturePackDict = [];

    protected readonly Dictionary<string, IAsset> mTilesetDict = [];
    private static GameContentManager? _current;
    private static readonly char[] DirectorySeparators = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];

    public Dictionary<ContentType, ICollection<IAsset>> Content => Textures;

    public Dictionary<ContentType, ICollection<IAsset>> Textures => new()
    {
        { ContentType.Animation, mAnimationDict.Values },
        { ContentType.Entity, mEntityDict.Values },
        { ContentType.Face, mFaceDict.Values },
        { ContentType.Fog, mFogDict.Values },
        { ContentType.Image, mImageDict.Values },
        { ContentType.Interface, mGuiDict.Values },
        { ContentType.Item, mItemDict.Values },
        { ContentType.Miscellaneous, mMiscDict.Values },
        { ContentType.Paperdoll, mPaperdollDict.Values },
        { ContentType.Resource, mResourceDict.Values },
        { ContentType.Spell, mSpellDict.Values },
        { ContentType.Tileset, mTilesetDict.Values },
    };

    /// <summary>
    /// Contains all indexed files and their caches from sound pack files.
    /// </summary>
    public IAssetPacker SoundPacks { get; set; }

    /// <summary>
    /// Contains all indexed files and their caches from music pack files.
    /// </summary>
    public IAssetPacker MusicPacks { get; set; }

    public bool TilesetsLoaded { get; set; } = false;

    public ContentWatcher ContentWatcher { get; protected set; }

    protected GameContentManager()
    {
        Current = this;
    }

    //Content Loading
    public void LoadAll()
    {
        LoadTexturePacks();
        LoadEntities();
        LoadItems();
        LoadAnimations();
        LoadSpells();
        LoadFaces();
        LoadImages();
        LoadFogs();
        LoadResources();
        LoadPaperdolls();
        LoadMisc();
        LoadGui();
        LoadFonts();
        LoadShaders();
    }

    public abstract void LoadTexturePacks();

    public abstract void LoadTilesets(string[] tilesetnames);

    public abstract void LoadItems();

    public abstract void LoadEntities();

    public abstract void LoadSpells();

    public abstract void LoadAnimations();

    public abstract void LoadFaces();

    public abstract void LoadImages();

    public abstract void LoadFogs();

    public abstract void LoadResources();

    public abstract void LoadPaperdolls();

    public abstract void LoadGui();

    public abstract void LoadMisc();

    public abstract void LoadFonts();

    public abstract void LoadShaders();

    //Audio Loading
    public void LoadAudio()
    {
        LoadSounds();
        LoadMusic();
    }

    public abstract void LoadSounds();

    public abstract void LoadMusic();

    public static string RemoveExtension(string fileNameOrPath)
    {
        var directorySeparator = fileNameOrPath.LastIndexOfAny(DirectorySeparators);
        var extensionOffset = fileNameOrPath.LastIndexOf('.');

        if (directorySeparator > extensionOffset || extensionOffset < 0)
        {
            return fileNameOrPath;
        }

        return fileNameOrPath[..extensionOffset];
    }

    public string[] GetTextureNames(TextureType type)
    {
        switch (type)
        {
            case TextureType.Tileset:
                return mTilesetDict.Keys.ToArray();

            case TextureType.Item:
                return mItemDict.Keys.ToArray();

            case TextureType.Entity:
                return mEntityDict.Keys.ToArray();

            case TextureType.Spell:
                return mSpellDict.Keys.ToArray();

            case TextureType.Animation:
                return mAnimationDict.Keys.ToArray();

            case TextureType.Face:
                return mFaceDict.Keys.ToArray();

            case TextureType.Image:
                return mImageDict.Keys.ToArray();

            case TextureType.Fog:
                return mFogDict.Keys.ToArray();

            case TextureType.Resource:
                return mResourceDict.Keys.ToArray();

            case TextureType.Paperdoll:
                return mPaperdollDict.Keys.ToArray();

            case TextureType.Gui:
                return mGuiDict.Keys.ToArray();

            case TextureType.Misc:
                return mMiscDict.Keys.ToArray();
        }

        return null;
    }

    public bool TryGetTexture(TextureType textureType, string textureName, [NotNullWhen(true)] out IGameTexture? texture)
    {
        texture = GetTexture(textureType, textureName);
        return texture != default;
    }

    //Content Getters
    public virtual IGameTexture? GetTexture(TextureType type, string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        IDictionary<string, IAsset> textureDict;
        switch (type)
        {
            case TextureType.Tileset:
                textureDict = mTilesetDict;

                break;

            case TextureType.Item:
                textureDict = mItemDict;

                break;

            case TextureType.Entity:
                textureDict = mEntityDict;

                break;

            case TextureType.Spell:
                textureDict = mSpellDict;

                break;

            case TextureType.Animation:
                textureDict = mAnimationDict;

                break;

            case TextureType.Face:
                textureDict = mFaceDict;

                break;

            case TextureType.Image:
                textureDict = mImageDict;

                break;

            case TextureType.Fog:
                textureDict = mFogDict;

                break;

            case TextureType.Resource:
                textureDict = mResourceDict;

                break;

            case TextureType.Paperdoll:
                textureDict = mPaperdollDict;

                break;

            case TextureType.Gui:
                textureDict = mGuiDict;

                break;

            case TextureType.Misc:
                textureDict = mMiscDict;

                break;

            default:
                return null;
        }

        if (textureDict == null)
        {
            return null;
        }

        return textureDict.TryGetValue(name.ToLower(), out var asset) ? asset as IGameTexture : default;
    }

    public bool TryGetShader(string shaderName, [NotNullWhen(true)] out GameShader? shader)
    {
        shader = GetShader(shaderName);
        return shader != default;
    }

    public virtual GameShader? GetShader(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        return mShaderDict?.GetValueOrDefault(name.ToLower());
    }

    public virtual IFont? GetFont(string? name)
    {
        if (name == null)
        {
            return null;
        }

        var lookupName = name.Trim().ToLowerInvariant();
        if (mFontDict.TryGetValue(lookupName, out var font))
        {
            return font;
        }

        ApplicationContext.CurrentContext.Logger.LogWarning("Missing font '{FontName}'", name);
        return null;
    }

    public virtual GameAudioSource? GetMusic(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        if (mMusicDict == null)
        {
            return null;
        }

        return mMusicDict.TryGetValue(name.ToLower(), out var music) ? music as GameAudioSource : default;
    }

    public virtual GameAudioSource? GetSound(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        if (mSoundDict == null)
        {
            return null;
        }

        return mSoundDict.TryGetValue(name.ToLower(), out var sound) ? sound as GameAudioSource : default;
    }

    public virtual bool GetLayout(UI stage, string name, string resolution, bool skipCache, Action<string, bool> layoutHandler)
    {
        var isReload = false;

        var stageName = stage.ToString().ToLowerInvariant();
        if (stage == UI.InGame)
        {
            stageName = "game";
        }

        var resourcePartialDirectory = Path.Combine("gui", "layouts", stageName);

        void onLayoutReady()
        {
            var result = GetLayout(stage, name, resolution, skipCache || isReload, out var cacheHit);
            layoutHandler(result, cacheHit);
            isReload = true;
        }

        void addChangeListener(string targetPath)
        {
            ContentWatcher?.AddEventListener(ContentWatcher.Event.Change, targetPath, () => onLayoutReady());
            onLayoutReady();
        }

        void addCreateOrChangeListener(string targetPath, bool createIfMissing)
        {
            var resourcePath = Path.Combine(ClientConfiguration.ResourcesDirectory, targetPath);
            if (File.Exists(resourcePath))
            {
                addChangeListener(targetPath);
            }
            else
            {
                if (createIfMissing)
                {
                    layoutHandler?.Invoke(string.Empty, false);
                }

                ContentWatcher?.AddEventListener(ContentWatcher.Event.Create, targetPath, () => addChangeListener(targetPath));
            }
        }

        if (!string.IsNullOrWhiteSpace(resolution))
        {
            var resolutionPath = Path.Combine(resourcePartialDirectory, $"{name}.{resolution}.json");
            addCreateOrChangeListener(resolutionPath, false);
        }

        var genericPath = Path.Combine(resourcePartialDirectory, $"{name}.json");
        addCreateOrChangeListener(genericPath, true);

        return true;
    }

    protected virtual string GetLayout(UI stage, string name, string resolution, bool skipCache, out bool cacheHit)
    {
        cacheHit = false;
        var key = new KeyValuePair<UI, string>(stage, $"{name}.{resolution}.json");
        if (!skipCache && mUiDict.TryGetValue(key, out var rawLayout))
        {
            cacheHit = true;
            return rawLayout;
        }

        var stageName = stage.ToString().ToLowerInvariant();
        if (stage == UI.InGame)
        {
            stageName = "game";
        }

        var resourceDirectory = Path.Combine(ClientConfiguration.ResourcesDirectory, "gui", "layouts", stageName);

        if (!Directory.Exists(resourceDirectory))
        {
            Directory.CreateDirectory(resourceDirectory);
        }

        var resolutionPath = Path.Combine(resourceDirectory, $"{name}.{resolution}.json");
        if (!string.IsNullOrWhiteSpace(resolution) && File.Exists(resolutionPath)) {
            rawLayout = File.ReadAllText(resolutionPath);
            mUiDict[key] = rawLayout;
            return rawLayout;
        }

        var genericPath = Path.Combine(resourceDirectory, $"{name}.json");
        if (File.Exists(genericPath))
        {
            rawLayout = ExceptionRepeater.Run(() => File.ReadAllText(genericPath), 5);
            mUiDict[key] = rawLayout;
            return rawLayout;
        }

        return string.Empty;
    }

    public virtual string GetUIJson(UI stage, string name, string resolution, out bool cacheHit)
    {
        return GetLayout(stage, name, resolution, false, out cacheHit);
    }

    public virtual void SaveUIJson(UI stage, string name, string json, string? resolution)
    {
        var stageName = stage.ToString().ToLowerInvariant();
        if (stage == UI.InGame)
        {
            stageName = "game";
        }

        var stagePartialPath = Path.Combine("gui", "layouts", stageName);

        resolution = resolution?.Trim();

        string resourceName;
        if (!string.IsNullOrWhiteSpace(resolution))
        {
            resourceName = Path.Combine(stagePartialPath, $"{name}.{resolution?.Trim()}.json");
            var resourcePath = Path.Combine(ClientConfiguration.ResourcesDirectory, resourceName);
            if (File.Exists(resourcePath))
            {
                ContentWatcher?.Modify(resourceName, () =>
                {
                    try
                    {
                        File.WriteAllText(resourcePath, json);
                    }
                    catch (Exception exception)
                    {
                        ApplicationContext.Context.Value?.Logger.LogDebug(
                            exception,
                            "Error occurred saving {ResourcePath}",
                            resourcePath
                        );
                    }
                });
                return;
            }
        }

        resourceName = Path.Combine(stagePartialPath, $"{name}.json");
        ContentWatcher?.Modify(resourceName, () =>
        {
            var resourcePath = Path.Combine(ClientConfiguration.ResourcesDirectory, resourceName);
            try
            {
                File.WriteAllText(resourcePath, json);
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogDebug(
                    exception,
                    "Error occurred saving {ResourcePath}",
                    resourcePath
                );
            }
        });
    }

    protected Dictionary<string, IAsset> GetAssetLookup(ContentType contentType)
    {
        switch (contentType)
        {
            case ContentType.Animation:
                return mAnimationDict;

            case ContentType.Entity:
                return mEntityDict;

            case ContentType.Face:
                return mFaceDict;

            case ContentType.Fog:
                return mFogDict;

            case ContentType.Image:
                return mImageDict;

            case ContentType.Interface:
                return mGuiDict;

            case ContentType.Item:
                return mItemDict;

            case ContentType.Miscellaneous:
                return mMiscDict;

            case ContentType.Paperdoll:
                return mPaperdollDict;

            case ContentType.Resource:
                return mResourceDict;

            case ContentType.Spell:
                return mSpellDict;

            case ContentType.TextureAtlas:
                return mTexturePackDict;

            case ContentType.Tileset:
                return mTilesetDict;

            case ContentType.Font:
                throw new NotImplementedException();

            case ContentType.Shader:
                throw new NotImplementedException();

            case ContentType.Music:
                return mMusicDict;

            case ContentType.Sound:
                return mSoundDict;

            default:
                throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null);
        }
    }

    protected abstract TAsset Load<TAsset>(
        Dictionary<string, IAsset> lookup,
        ContentType contentType,
        string assetName,
        Func<Stream> createStream
    ) where TAsset : class, IAsset;

    /// <inheritdoc />
    public TAsset Find<TAsset>(ContentType contentType, string assetName) where TAsset : class, IAsset
    {
        var assetLookup = GetAssetLookup(contentType);

        if (assetLookup.TryGetValue(assetName, out var asset))
        {
            return asset as TAsset;
        }

        return null;
    }

    /// <inheritdoc />
    public TAsset Load<TAsset>(ContentType contentType, string assetPath, string assetAlias) where TAsset : class, IAsset
    {
        if (!File.Exists(assetPath))
        {
            throw new FileNotFoundException($@"Asset does not exist at '{assetPath}'.");
        }

        return Load<TAsset>(contentType, assetAlias.ToLower(), () => File.OpenRead(assetPath));
    }

    /// <inheritdoc />
    public TAsset Load<TAsset>(ContentType contentType, string assetName, Func<Stream> createStream)
        where TAsset : class, IAsset
    {
        var assetLookup = GetAssetLookup(contentType);

        if (assetLookup.TryGetValue(assetName, out var asset))
        {
            return asset as TAsset ?? throw new InvalidOperationException();
        }

        return Load<TAsset>(assetLookup, contentType, assetName, createStream);
    }

    /// <inheritdoc />
    public TAsset LoadEmbedded<TAsset>(IPluginContext context, ContentType contentType, string assetName)
        where TAsset : class, IAsset
    {
        var manifestResourceName = context.EmbeddedResources.Resolve(assetName);
        return Load<TAsset>(contentType, assetName, () => context.EmbeddedResources.Read(manifestResourceName));
    }

    /// <inheritdoc />
    public TAsset LoadEmbedded<TAsset>(IPluginContext context, ContentType contentType, string assetName, string assetAlias)
        where TAsset : class, IAsset
    {
        var manifestResourceName = context.EmbeddedResources.Resolve(assetName);
        return Load<TAsset>(contentType, assetName, assetAlias);
    }

}
