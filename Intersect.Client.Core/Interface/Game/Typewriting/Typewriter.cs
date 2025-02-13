using Intersect.Client.Core;
using Intersect.Configuration;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Typewriting;

internal sealed class Typewriter
{
    public delegate void TextWrittenHandler(string text);

    private static HashSet<string> FullStops => ClientConfiguration.Instance.TypewriterFullStops;
    private static long FullStopSpeed => ClientConfiguration.Instance.TypewriterFullStopDelay;
    private static HashSet<string> PartialStops => ClientConfiguration.Instance.TypewriterPauses;
    private static long PartialStopSpeed => ClientConfiguration.Instance.TypewriterPauseDelay;
    private static long TypingSpeed => ClientConfiguration.Instance.TypewriterPartDelay;

    private int _offset;
    private string? _lastText;
    private long _nextUpdateTime;

    private readonly TextWrittenHandler _textWrittenHandler;
    private readonly string _text;

    public bool IsDone { get; private set; }
    public long DoneAtMilliseconds { get; private set; }

    public Typewriter(string text, TextWrittenHandler textWrittenHandler)
    {
        _text = text.ReplaceLineEndings("\n");
        _textWrittenHandler = textWrittenHandler;
        _nextUpdateTime = Timing.Global.MillisecondsUtc;

        _offset = 0;
        _lastText = null;

        IsDone = false;
    }

    public void Write(string? soundName)
    {
        if (IsDone)
        {
            return;
        }

        if (_offset >= _text.Length)
        {
            End();
            return;
        }

        if (Timing.Global.MillisecondsUtc < _nextUpdateTime)
        {
            return;
        }

        var emitSound = false;
        while (_nextUpdateTime <= Timing.Global.MillisecondsUtc)
        {
            if (_offset >= _text.Length)
            {
                End();
                break;
            }

            emitSound |= _offset % ClientConfiguration.Instance.TypewriterSoundFrequency == 0;

            string nextText;
            if (char.IsSurrogatePair(_text, _offset))
            {
                nextText = _text[_offset..(_offset + 2)];
                _offset += 2;
            }
            else
            {
                nextText = _text[_offset..(_offset + 1)];
                ++_offset;
            }

            _nextUpdateTime += GetTypingDelayFor(nextText, _lastText);
            _textWrittenHandler(nextText);
            _lastText = nextText;
        }

        if (!emitSound || string.IsNullOrEmpty(soundName))
        {
            return;
        }

        Audio.StopAllGameSoundsOf(ClientConfiguration.Instance.TypewriterSounds.ToArray());
        Audio.AddGameSound(soundName, false);
    }

    private static long GetTypingDelayFor(string next, string? last)
    {
        if (last == null)
        {
            return TypingSpeed;
        }

        if (next == "\n")
        {
            return FullStopSpeed;
        }

        if (next == last)
        {
            return TypingSpeed;
        }

        if (FullStops.Contains(last))
        {
            return FullStopSpeed;
        }

        return PartialStops.Contains(last) ? PartialStopSpeed : TypingSpeed;
    }

    public void End()
    {
        if (IsDone || _text.Length < 1)
        {
            return;
        }

        if (_offset < _text.Length)
        {
            _textWrittenHandler(_text[_offset..]);
        }

        IsDone = true;
        DoneAtMilliseconds = Timing.Global.MillisecondsUtc;
    }
}
