using Intersect.Client.Core.Controls;
using Intersect.Client.Entities.Events;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Typewriting;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Enums;
using Intersect.Utilities;

namespace Intersect.Client.Interface.Game;

public partial class EventWindow : Panel
{
    private readonly GameFont? _defaultFont;

    private readonly Panel _promptPanel;

    private readonly ImagePanel _faceImage;
    private readonly ScrollControl _promptScroller;
    private readonly Label _promptTemplateLabel;
    private readonly RichLabel _promptLabel;

    private readonly Panel _optionsPanel;
    private readonly Button[] _optionButtons = new Button[4];

    private readonly Typewriter? _writer;
    private bool _isTypewriting;
    private readonly long _typewriterResponseDelay = ClientConfiguration.Instance.TypewriterResponseDelay;

    private readonly Dialog _dialog;

    private EventWindow(Canvas gameCanvas, Dialog dialog) : base(gameCanvas, nameof(EventWindow))
    {
        _dialog = dialog;
        _defaultFont = GameContentManager.Current.GetFont(name: "sourcesansproblack", 12);
        _writer = Globals.Database.TypewriterBehavior == TypewriterBehavior.Off ? default : new Typewriter();

        Alignment = [Alignments.Center];
        MinimumSize = new Point(520, 180);
        MaximumSize = new Point(720, 520);
        Padding = new Padding(16);

        _promptPanel = new Panel(this, nameof(_promptPanel))
        {
            Dock = Pos.Fill,
            DockChildSpacing = new Padding(8),
        };

        _optionsPanel = new Panel(this, nameof(_optionsPanel))
        {
            BackgroundColor = Color.Transparent,
            Dock = Pos.Bottom,
            DockChildSpacing = new Padding(8),
            Margin = new Margin(0, 8, 0, 0),
        };

        for (var optionIndex = 0; optionIndex < 4; ++optionIndex)
        {
            var optionButton = new Button(_optionsPanel, $"{nameof(_optionButtons)}[{optionIndex}]")
            {
                Dock = Pos.Top,
                Font = _defaultFont,
                UserData = (EventResponseType)(optionIndex + 1),
            };
            optionButton.Clicked += (sender, _) =>
            {
                if (sender.UserData is not EventResponseType eventResponseType)
                {
                    return;
                }

                CloseEventResponse(eventResponseType);
            };
            _optionButtons[optionIndex] = optionButton;
        }

        _optionsPanel.SizeToChildren(recursive: true);

        _faceImage = new ImagePanel(_promptPanel, nameof(_faceImage))
        {
            Dock = Pos.Left,
            MaintainAspectRatio = true,
            Margin = new Margin(8, 8, 0, 8),
            MaximumSize = new Point(128, 128),
        };

        _promptScroller = new ScrollControl(_promptPanel, nameof(_promptScroller))
        {
            Dock = Pos.Fill,
        };

        _promptTemplateLabel = new Label(_promptScroller, nameof(_promptTemplateLabel))
        {
            Font = _defaultFont,
            IsVisible = false,
        };

        _promptLabel = new RichLabel(_promptScroller, nameof(_promptLabel))
        {
            Dock = Pos.Fill,
            Font = _defaultFont,
            Padding = new Padding(8),
        };

        _promptPanel.SizeToChildren(recursive: true);


        #region Configure and Display

        if (_dialog.Face is { } faceTextureName)
        {
            var faceTexture = Globals.ContentManager.GetTexture(TextureType.Face, faceTextureName);
            _faceImage.Texture = faceTexture;
            if (faceTexture is not null)
            {
                _faceImage.IsVisible = true;
                _faceImage.SizeToContents();
            }
            else
            {
                _faceImage.IsVisible = false;
            }
        }
        else
        {
            _faceImage.Texture = null;
            _faceImage.IsVisible = false;
        }

        var visibleOptions = _dialog.Options.Where(option => !string.IsNullOrEmpty(option)).ToArray();
        if (visibleOptions.Length < 1)
        {
            visibleOptions = [Strings.EventWindow.Continue];
        }

        for (var optionIndex = 0; optionIndex < _optionButtons.Length; ++optionIndex)
        {
            var optionButton = _optionButtons[optionIndex];
            if (optionIndex < visibleOptions.Length)
            {
                optionButton.Text = visibleOptions[optionIndex];
                optionButton.IsVisible = true;
            }
            else
            {
                optionButton.IsVisible = false;
            }
        }

        // Name = $"{nameof(EventWindow)}_{Math.Max(1, visibleOptions.Length)}";
        // LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer?.GetResolutionString());

        SkipRender();
        ShowDialog();

        MakeModal(dim: true);
        BringToFront();
        Interface.InputBlockingComponents.Add(this);

        #endregion Configure and Display
    }

    protected override void OnMouseClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseClicked(mouseButton, mousePosition, userAction);

        SkipTypewriting();
    }

    public override void Dispose()
    {
        EnsureControlRestored();
        base.Dispose();
    }

    private static EventWindow? _instance;

    private void Update()
    {
        // Handle typewriting
        if (_isTypewriting && IsVisible)
        {
            var voiceIdx = Randomization.Next(0, ClientConfiguration.Instance.TypewriterSounds.Count);

            // Always show option 1 ("continue" if options empty)
            var writerCompleted = _writer?.IsDone ?? true;
            for (var optionIndex = 0; optionIndex < _optionButtons.Length; ++optionIndex)
            {
                var optionButton = _optionButtons[optionIndex];
                var optionText = _dialog?.Options[optionIndex];
                optionButton.IsVisible = writerCompleted && !string.IsNullOrEmpty(optionText);
            }

            _writer?.Write(ClientConfiguration.Instance.TypewriterSounds.ElementAtOrDefault(voiceIdx));
            if (writerCompleted)
            {
                var disableResponse = _writer != default &&
                                      Timing.Global.MillisecondsUtc - _writer.DoneAtMilliseconds <
                                      _typewriterResponseDelay;
                foreach (var optionButton in _optionButtons)
                {
                    optionButton.IsDisabled = disableResponse;
                }
            }
            else if (Controls.IsControlPressed(Control.AttackInteract))
            {
                SkipTypewriting();
            }

            return;
        }

        _isTypewriting = ClientConfiguration.Instance.TypewriterEnabled &&
                         Globals.Database.TypewriterBehavior == TypewriterBehavior.Word;
    }

    public static void ShowOrUpdateDialog(Canvas canvas)
    {
        if (_instance is { } instance)
        {
            instance.Update();
            return;
        }

        var availableDialog = Globals.EventDialogs.FirstOrDefault();
        if (availableDialog == null)
        {
            return;
        }

        _instance = new EventWindow(canvas, availableDialog);
    }

    private void ShowDialog()
    {
        _promptLabel.ClearText();
        _promptLabel.AddText(_dialog?.Prompt ?? string.Empty, _promptTemplateLabel);

        _ = _promptLabel.SizeToChildren();

        // Do this _after_ sizing so we have lines broken up
        if (_isTypewriting)
        {
            _writer?.Initialize(_promptLabel.FormattedLabels);
            foreach (var button in _optionButtons)
            {
                button.Hide();
            }
        }

        Defer(
            () =>
            {
                SizeToChildren(recursive: true);

                _promptScroller.ScrollToTop();
            }
        );
    }

    public void CloseEventResponse(EventResponseType response)
    {
        if (!(_writer?.IsDone ?? true))
        {
            SkipTypewriting();
            return;
        }

        if (_dialog is not { ResponseSent: false } dialog)
        {
            return;
        }

        PacketSender.SendEventResponse((byte)response, dialog);
        dialog.ResponseSent = true;

        EnsureDestroyed();
    }

    private void EnsureControlRestored()
    {
        if (_instance == this)
        {
            _instance = null;
        }
        _ = Interface.InputBlockingComponents.Remove(this);
        RemoveModal();
    }

    private void EnsureDestroyed()
    {
        EnsureControlRestored();

        if (Parent is { } parent)
        {
            parent.RemoveChild(this, dispose: true);
        }
        else
        {
            DelayedDelete();
        }
    }

    private void SkipTypewriting()
    {
        if (_writer?.IsDone ?? true)
        {
            return;
        }

        _writer.End();
    }
}
