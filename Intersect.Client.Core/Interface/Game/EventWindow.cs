using Intersect.Client.Core;
using Intersect.Client.Core.Controls;
using Intersect.Client.Entities.Events;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
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

public partial class EventWindow : ImagePanel
{
    private readonly ImagePanel _panelEventFace;
    private readonly ScrollControl _areaEventDialog;
    private readonly Label _labelEventDialog;
    private readonly RichLabel _richLabelEventDialog;
    private readonly ScrollControl _areaEventDialogNoFace;
    private readonly Label _labelEventDialogNoFace;
    private readonly RichLabel _richLabelEventDialogNoFace;
    private readonly Button _buttonEventResponse1;
    private readonly Button _buttonEventResponse2;
    private readonly Button _buttonEventResponse3;
    private readonly Button _buttonEventResponse4;

    private readonly Typewriter? _writer;
    private bool _isTypewriting = false;
    private readonly long _typewriterResponseDelay = ClientConfiguration.Instance.TypewriterResponseDelay;

    private static Dialog CurrentDialog => Globals.EventDialogs[0];
    private static bool HasOneOption => CurrentDialog.Opt1.Length > 0;
    private static bool HasTwoOptions => CurrentDialog.Opt2.Length > 0;
    private static bool HasThreeOptions => CurrentDialog.Opt3.Length > 0;
    private static bool HasFourOptions => CurrentDialog.Opt4.Length > 0;

    public EventWindow(Canvas gameCanvas) : base(gameCanvas, nameof(EventWindow))
    {
        _panelEventFace = new ImagePanel(this, "EventFacePanel");

        _areaEventDialog = new ScrollControl(this, "EventDialogArea");
        _labelEventDialog = new Label(_areaEventDialog, "EventDialogLabel");
        _richLabelEventDialog = new RichLabel(_areaEventDialog);

        _areaEventDialogNoFace = new ScrollControl(this, "EventDialogAreaNoFace");
        _labelEventDialogNoFace = new Label(_areaEventDialogNoFace, "EventDialogLabel");
        _richLabelEventDialogNoFace = new RichLabel(_areaEventDialogNoFace);

        _buttonEventResponse1 = new Button(this, "Response1Button");
        _buttonEventResponse1.Clicked += (s, e) => CloseEventResponse(EventResponseType.OneOption);

        _buttonEventResponse2 = new Button(this, "Response2Button");
        _buttonEventResponse2.Clicked += (s, e) => CloseEventResponse(EventResponseType.TwoOption);

        _buttonEventResponse3 = new Button(this, "Response3Button");
        _buttonEventResponse3.Clicked += (s, e) => CloseEventResponse(EventResponseType.ThreeOption);

        _buttonEventResponse4 = new Button(this, "Response4Button");
        _buttonEventResponse4.Clicked += (s, e) => CloseEventResponse(EventResponseType.FourOption);

        _writer = Globals.Database.TypewriterBehavior == TypewriterBehavior.Off ? default : new Typewriter();

        Clicked += (s, e) => SkipTypewriting();
        Hide();
    }

    public void Update()
    {
        if (IsHidden)
        {
            _ = Interface.InputBlockingElements.Remove(this);
        }
        else
        {
            if (!Interface.InputBlockingElements.Contains(this))
            {
                Interface.InputBlockingElements.Add(this);
            }
        }

        if (Globals.EventDialogs.Count <= 0)
        {
            return;
        }

        // Handle typewriting
        if (_isTypewriting && !IsHidden)
        {
            var voiceIdx = Randomization.Next(0, ClientConfiguration.Instance.TypewriterSounds.Count);

            // Always show option 1 ("continue" if options empty)
            _buttonEventResponse1.IsHidden = !(_writer?.IsDone ?? true);
            _buttonEventResponse2.IsHidden = !(_writer?.IsDone ?? true) || string.IsNullOrEmpty(CurrentDialog.Opt2);
            _buttonEventResponse3.IsHidden = !(_writer?.IsDone ?? true) || string.IsNullOrEmpty(CurrentDialog.Opt3);
            _buttonEventResponse4.IsHidden = !(_writer?.IsDone ?? true) || string.IsNullOrEmpty(CurrentDialog.Opt4);

            _writer?.Write(ClientConfiguration.Instance.TypewriterSounds.ElementAtOrDefault(voiceIdx));
            if (_writer?.IsDone ?? true)
            {
                var disableResponse = _writer != default && Timing.Global.MillisecondsUtc - _writer.DoneAtMilliseconds < _typewriterResponseDelay;
                _buttonEventResponse1.IsDisabled = disableResponse;
                _buttonEventResponse2.IsDisabled = disableResponse;
                _buttonEventResponse3.IsDisabled = disableResponse;
                _buttonEventResponse4.IsDisabled = disableResponse;
            }
            else if (Controls.IsControlPressed(Control.AttackInteract))
            {
                SkipTypewriting();
            }

            return;
        }

        // We have dialog to show let's setup
        Show();
        MakeModal();
        BringToFront();
        _areaEventDialog.ScrollToTop();

        var responseCount = 0;
        var maxResponse = 1;

        if (HasOneOption)
        {
            responseCount++;
        }

        if (HasTwoOptions)
        {
            responseCount++;
            maxResponse++;
        }

        if (HasThreeOptions)
        {
            responseCount++;
            maxResponse++;
        }

        if (HasFourOptions)
        {
            responseCount++;
            maxResponse++;
        }

        _isTypewriting = ClientConfiguration.Instance.TypewriterEnabled && Globals.Database.TypewriterBehavior == TypewriterBehavior.Word;

        Name = $"EventDialogWindow_{maxResponse}Response{(maxResponse == 1 ? string.Empty : 's')}";
        LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer?.GetResolutionString());

        var faceTex = Globals.ContentManager.GetTexture(TextureType.Face, CurrentDialog.Face);
        _panelEventFace.Texture = faceTex;
        _panelEventFace.IsHidden = faceTex == null;
        _areaEventDialog.IsHidden = faceTex == null;
        _areaEventDialogNoFace.IsHidden = faceTex != null;

        if (responseCount == 0)
        {
            _buttonEventResponse1.Show();
            _buttonEventResponse1.SetText(Strings.EventWindow.Continue);
            _buttonEventResponse2.Hide();
            _buttonEventResponse3.Hide();
            _buttonEventResponse4.Hide();
        }
        else
        {
            _buttonEventResponse1.IsHidden = !HasOneOption;
            _buttonEventResponse1.SetText(CurrentDialog.Opt1);
            _buttonEventResponse2.IsHidden = !HasTwoOptions;
            _buttonEventResponse2.SetText(CurrentDialog.Opt2);
            _buttonEventResponse3.IsHidden = !HasThreeOptions;
            _buttonEventResponse3.SetText(CurrentDialog.Opt3);
            _buttonEventResponse4.IsHidden = !HasFourOptions;
            _buttonEventResponse4.SetText(CurrentDialog.Opt4);
        }

        if (faceTex != null)
        {
            ShowDialog(_richLabelEventDialog, _labelEventDialog, _areaEventDialog, CurrentDialog.Prompt);
        }
        else
        {
            ShowDialog(_richLabelEventDialogNoFace, _labelEventDialogNoFace, _areaEventDialogNoFace, CurrentDialog.Prompt);
        }
    }

    private void ShowDialog(RichLabel dialogLabel, Label dialogLabelTemplate, ScrollControl dialogArea, string prompt)
    {
        if (dialogLabel == default || dialogLabelTemplate == default || dialogArea == default)
        {
            return;
        }

        dialogLabel.ClearText();
        dialogLabel.Width = dialogArea.Width - dialogArea.VerticalScrollBar.Width;

        dialogLabel.AddText(prompt, dialogLabelTemplate);

        _ = dialogLabel.SizeToChildren(false, true);

        // Do this _after_ sizing so we have lines broken up
        if (_isTypewriting)
        {
            _writer?.Initialize(dialogLabel.FormattedLabels);
            _buttonEventResponse1.Hide();
            _buttonEventResponse2.Hide();
            _buttonEventResponse3.Hide();
            _buttonEventResponse4.Hide();
        }

        dialogArea.ScrollToTop();
    }

    public void CloseEventResponse(EventResponseType response)
    {
        if (!(_writer?.IsDone ?? true))
        {
            SkipTypewriting();
            return;
        };

        var eventDialog = CurrentDialog;
        if (eventDialog.ResponseSent != 0)
        {
            return;
        }

        PacketSender.SendEventResponse((byte)response, eventDialog);
        RemoveModal();
        Hide();
        eventDialog.ResponseSent = 1;
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
