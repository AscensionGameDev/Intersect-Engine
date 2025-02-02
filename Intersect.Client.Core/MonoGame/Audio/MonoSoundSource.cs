using Intersect.Client.Framework.Audio;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Localization;
using Intersect.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Audio;

namespace Intersect.Client.MonoGame.Audio;


public partial class MonoSoundSource : GameAudioSource
{
    private readonly string mPath;
    private readonly string mRealPath;
    private Func<Stream>? _createStream;

    private int mInstanceCount;

    private SoundEffect? _sound;

    public MonoSoundSource(string path, string realPath, string? name = default)
    {

        mPath = path;
        mRealPath = realPath;
        Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
    }

    public MonoSoundSource(Func<Stream> createStream, string? name = default)
    {
        _createStream = createStream;
        Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
    }

    public SoundEffect Effect
    {
        get
        {
            if (_sound == null)
            {
                LoadSound();
            }

            return _sound;
        }
    }

    public override bool IsLoaded => _sound != null;

    public override GameAudioInstance CreateInstance()
    {
        mInstanceCount++;

        return new MonoSoundInstance(this);
    }

    public void ReleaseEffect()
    {
        if (--mInstanceCount > 0)
        {
            return;
        }

        _sound?.Dispose();
        _sound = null;
    }

    private void LoadSound()
    {
        try
        {
            if (_createStream != null)
            {
                using (var stream = _createStream())
                {
                    _sound = SoundEffect.FromStream(stream);
                }
            }
            else if (Globals.ContentManager.SoundPacks != null && Globals.ContentManager.SoundPacks.Contains(mRealPath))
            {
                using (var stream = Globals.ContentManager.SoundPacks.GetAsset(mRealPath))
                {
                    _sound = SoundEffect.FromStream(stream);
                }
            }
            else
            {
                using (var fileStream = new FileStream(mRealPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    _sound = SoundEffect.FromStream(fileStream);
                }

            }
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(exception, $"Error loading '{mPath}'.");
            ChatboxMsg.AddMessage(
                new ChatboxMsg(
                    $"{Strings.Errors.LoadFile.ToString(Strings.Words.LcaseSound)} [{mPath}]",
                    new Color(0xBF, 0x0, 0x0),  Enums.ChatMessageType.Error
                )
            );
        }

    }

}
