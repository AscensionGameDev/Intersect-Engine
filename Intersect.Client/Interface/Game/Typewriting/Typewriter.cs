using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Client.Core;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Configuration;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game.Typewriting
{

    internal sealed class Typewriter
    {
        private static List<char> _fullstopChars => ClientConfiguration.Instance.TypewriterFullStopCharacters;
        private static long _fullStopSpeed => ClientConfiguration.Instance.TypewriterFullStopDelay;
        private static List<char> _partialstopChars => ClientConfiguration.Instance.TypewriterPauseCharacters;
        private static long _partialStopSpeed => ClientConfiguration.Instance.TypewriterPartialStopDelay;
        private static long _typingSpeed => ClientConfiguration.Instance.TypewriterPartDelay;

        private List<Label> _labels;
        private string[] _lines;
        private Label _currentLabel => _labels.ElementAtOrDefault(_lineIndex);
        private string _currentLine => _lines.ElementAtOrDefault(_lineIndex);
        private int _lineIndex;
        private int _charIndex;
        private char? _lastChar;
        private long _nextUpdateTime;

        public bool IsDone { get; private set; }

        public long DoneAtMilliseconds { get; private set; }

        public void NewLine()
        {
            _lineIndex++;
            if (_lineIndex >= _lines.Length)
            {
                End();
                return;
            }
            _charIndex = 0;
            _lastChar = null;
        }

        public void Initialize(List<Label> labels)
        {
            _nextUpdateTime = Timing.Global.Milliseconds;
            _labels = labels;
            _lines = _labels.Select(l => l.Text).ToArray();
            _labels.ForEach(l => l.SetText(string.Empty)); // Clear the text from the labels. The writer is the captain now
            _lineIndex = 0;
            _charIndex = 0;
            _lastChar = null;
            IsDone = false;
        }

        public void Write(string voice)
        {
            if (IsDone)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_currentLine) || _currentLabel == default)
            {
                End();
                return;
            }

            if (Timing.Global.Milliseconds < _nextUpdateTime)
            {
                return;
            }

            if (!string.IsNullOrEmpty(voice) && _charIndex % ClientConfiguration.Instance.TypewriterSoundFrequency == 0)
            {
                Audio.StopAllGameSoundsOf(ClientConfiguration.Instance.TypewriterSounds.ToArray());
                Audio.AddGameSound(voice ?? string.Empty, false);
            }

            _charIndex++;
            if (_charIndex >= _currentLine.Length)
            {
                _currentLabel?.SetText(_currentLine);
                NewLine();
                return;
            }

            _lastChar = _currentLine[_charIndex - 1];

            var written = _currentLine?.Substring(0, _charIndex) ?? string.Empty;
            _currentLabel?.SetText(written);

            _nextUpdateTime = Timing.Global.Milliseconds + GetTypingDelayFor(_currentLine.ElementAtOrDefault(_charIndex), _lastChar.Value);
        }

        private static long GetTypingDelayFor(char currentChar, char? lastChar)
        {
            if (lastChar == null)
            {
                return _typingSpeed;
            }

            var lastCharVal = lastChar.GetValueOrDefault();
            // Allows things like ellipses
            if (currentChar == lastCharVal)
            {
                return _typingSpeed;
            }
            if (_fullstopChars.Contains(lastCharVal))
            {
                return _fullStopSpeed;
            }
            if (_partialstopChars.Contains(lastCharVal))
            {
                return _partialStopSpeed;
            }

            return _typingSpeed;
        }

        public void End()
        {
            if (IsDone || (_lines?.Length ?? 0) == 0)
            {
                return;
            }

            for (var i = 0; i < _lines.Length; i++)
            {
                if (i >= _labels.Count)
                {
                    continue;
                }

                _labels[i].SetText(_lines[i]);
            }

            IsDone = true;
            DoneAtMilliseconds = Timing.Global.Milliseconds;
        }
    }
}
