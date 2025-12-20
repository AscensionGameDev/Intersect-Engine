using Intersect.Client.Core;
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
using Intersect.Client.Utilities;
using Intersect.Configuration;
using Intersect.Core;
using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Framework.Threading;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Interface.Game;

public partial class EventWindow : Panel
{
    private readonly IFont? _defaultFont;
    private readonly Panel _promptPanel;
    private readonly ImagePanel _faceImage;
    private readonly ScrollControl _promptScroller;
    private readonly Label _promptTemplateLabel;
    private readonly RichLabel _promptLabel;
    private readonly ScrollControl _optionsScroller;
    private readonly List<Button> _optionButtons = new();
    private bool _typewriting;
    private readonly long _typewriterResponseDelay = ClientConfiguration.Instance.TypewriterResponseDelay;
    private Typewriter? _writer;
    private readonly Dialog _dialog;
    private static EventWindow? _instance;

    private EventWindow(Canvas gameCanvas, Dialog dialog) : base(gameCanvas, nameof(EventWindow))
    {
        ThreadQueue.Default.ThrowIfNotOnMainThread();

        _dialog = dialog;
        _defaultFont = GameContentManager.Current.GetFont(name: "sourcesansproblack");

        Size = new Point(618, 680);
        Dock = Pos.None;
        Padding = new Padding(16, 16, 16, 16);
        ShouldDrawBackground = false;
        MouseInputEnabled = false;
        Alignment = [Alignments.Center];
        MinimumSize = new Point(488, 150);
        MaximumSize = new Point(618, 680);

        _promptPanel = new Panel(this, nameof(_promptPanel))
        {
            Size = new Point(616, 200),
            Y = 180,
            Dock = Pos.None,
            Padding = Padding.Zero,
            Margin = Margin.Zero,
            ShouldDrawBackground = true,
            MinimumSize = new Point(616, 150),
            MaximumSize = new Point(616, 200),
            MouseInputEnabled = false,
        };

        _promptScroller = new ScrollControl(_promptPanel, nameof(_promptScroller))
        {
            Size = new Point(616, 196),
            Dock = Pos.Fill,
            Padding = new Padding(4, 4, 4, 4),
            Margin = Margin.Zero,
            ShouldDrawBackground = false,
            MinimumSize = new Point(616, 150),
            MaximumSize = new Point(616, 4096),
            RestrictToParent = true,
            MouseInputEnabled = true,
            OverflowX = OverflowBehavior.Hidden,
            OverflowY = OverflowBehavior.Auto,
        };

        _promptTemplateLabel = new Label(_promptScroller, nameof(_promptTemplateLabel))
        {
            Size = new Point(0, 20),
            Dock = Pos.None,
            ShouldDrawBackground = false,
            AutoSizeToContents = true,
            FontName = "sourcesansproblack",
            FontSize = 12,
            IsVisibleInTree = false,
        };

        _promptLabel = new RichLabel(_promptScroller, nameof(_promptLabel))
        {
            Size = new Point(16, 16),
            Dock = Pos.Fill,
            Padding = new Padding(8, 8, 8, 8),
            ShouldDrawBackground = false,
            Font = _defaultFont,
            FontSize = 12,
        };

        _optionsScroller = new ScrollControl(this, nameof(_optionsScroller))
        {
            Size = new Point(616, 120),
            Y = 386,
            Dock = Pos.None,
            Padding = Padding.Zero,
            Margin = Margin.Zero,
            ShouldDrawBackground = false,
            MinimumSize = new Point(616, 120),
            MaximumSize = new Point(4096, 4096),
            MouseInputEnabled = true,
            OverflowX = OverflowBehavior.Hidden,
            OverflowY = OverflowBehavior.Auto,
        };

        _faceImage = new ImagePanel(this, nameof(_faceImage))
        {
            Size = new Point(128, 128),
            X = 245, Y = 60,
            Dock = Pos.None,
            Padding = Padding.Zero,
            Margin = Margin.Zero,
            ShouldDrawBackground = true,
            MinimumSize = new Point(128, 128),
            MaximumSize = new Point(128, 128),
            RestrictToParent = true,
            MouseInputEnabled = false,
            MaintainAspectRatio = true,
        };

        CreateOptionButtons();

        try
        {
            Name = nameof(EventWindow);
            LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }
        catch (Exception ex)
        {
            ApplicationContext.CurrentContext.Logger?.LogWarning(
                ex,
                "Failed to load EventWindow JSON UI, using code fallback layout"
            );
        }

        ApplyFace();
        ApplyPromptAndTypewriter();
        RunOnMainThread(static @this => @this._promptScroller.ScrollToTop(), this);

        var dimmedBackground = ClientConfiguration.Instance.DimmedEventWindowBackground;
        MakeModal(dim: dimmedBackground);
        BringToFront();
        Interface.InputBlockingComponents.Add(this);
        ApplicationContext.CurrentContext.Logger?.LogTrace("Event window opened");
    }

    private void ApplyFace()
    {
        if (_dialog.Face is { } faceTextureName)
        {
            var faceTexture = Globals.ContentManager?.GetTexture(TextureType.Face, faceTextureName);
            _faceImage.Texture = faceTexture;

            var show = faceTexture is not null;
            _faceImage.IsHidden = !show;
            _faceImage.IsVisibleInParent = show;

            if (show)
            {
                _faceImage.SizeToContents();
            }
        }
        else
        {
            _faceImage.Texture = null;
            _faceImage.IsHidden = true;
            _faceImage.IsVisibleInParent = false;
        }
    }

    private void ApplyPromptAndTypewriter()
    {
        SkipRender();

        _promptLabel.ClearText();
        var parsedText = TextColorParser.Parse(_dialog.Prompt ?? string.Empty, Color.White);

        foreach (var segment in parsedText)
        {
            _promptLabel.AddText(segment.Text, segment.Color, Alignments.Left, _promptTemplateLabel.Font);
        }

        _promptLabel.ForceImmediateRebuild();
        _ = _promptLabel.SizeToChildren();

        _typewriting = ClientConfiguration.Instance.TypewriterEnabled &&
                       Globals.Database?.TypewriterBehavior != TypewriterBehavior.Off;

        if (_typewriting)
        {
            _promptLabel.ClearText();
            _writer = new Typewriter(
                parsedText.ToArray(),
                (text, color) =>
                {
                    _promptLabel.AppendText(text, color, Alignments.Left, _promptTemplateLabel.Font);
                }
            );
        }
        else
        {
            _writer = null;
        }
    }

    private void CreateOptionButtons()
    {
        foreach (var button in _optionButtons)
        {
            button.Dispose();
        }

        _optionButtons.Clear();
        _optionsScroller.ClearChildren();

        var visibleOptions = _dialog.Options.Where(option => !string.IsNullOrEmpty(option)).ToArray();
        if (visibleOptions.Length < 1)
        {
            visibleOptions = [Strings.EventWindow.Continue];
        }

        for (var optionIndex = 0; optionIndex < visibleOptions.Length; optionIndex++)
        {
            var optionButton = new Button(_optionsScroller, $"OptionButton_{optionIndex}")
            {
                Dock = Pos.Top,
                Font = _defaultFont,
                FontSize = 12,
                Text = visibleOptions[optionIndex],
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

            _optionButtons.Add(optionButton);
        }
    }

    private void Update()
    {
        if (!IsVisibleInTree || !_typewriting || _writer is null)
        {
            return;
        }

        var writerCompleted = _writer.IsDone;

        foreach (var optionButton in _optionButtons)
        {
            optionButton.IsVisibleInParent = writerCompleted && !string.IsNullOrEmpty(optionButton.Text);
        }

        if (writerCompleted)
        {
            var disableResponse = Timing.Global.MillisecondsUtc - _writer.DoneAtMilliseconds < _typewriterResponseDelay;
            foreach (var optionButton in _optionButtons)
            {
                optionButton.IsDisabled = disableResponse;
            }
        }
        else if (Controls.IsControlJustPressed(Control.AttackInteract))
        {
            SkipTypewriting();
            PostLayout.Enqueue(_promptScroller.ScrollToBottom);
        }
        else
        {
            var soundIndex = Randomization.Next(0, ClientConfiguration.Instance.TypewriterSounds.Count);
            _writer.Write(ClientConfiguration.Instance.TypewriterSounds.ElementAtOrDefault(soundIndex));
            _promptScroller.ScrollToBottom();
        }
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

    private void SkipTypewriting()
    {
        if (_writer?.IsDone ?? true)
        {
            return;
        }

        _writer.End();
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

    protected override void Dispose(bool disposing)
    {
        EnsureControlRestored();
        base.Dispose(disposing);
    }

    protected override void OnMouseClicked(MouseButton mouseButton, Point mousePosition, bool userAction = true)
    {
        base.OnMouseClicked(mouseButton, mousePosition, userAction);
        SkipTypewriting();
    }
}