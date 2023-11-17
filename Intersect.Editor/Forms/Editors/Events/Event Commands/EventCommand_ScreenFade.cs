using Intersect.Configuration;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using static Intersect.Editor.Localization.Strings;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;
public partial class EventCommand_ScreenFade : UserControl
{
    private readonly FrmEvent mEventEditor;

    private ScreenFadeCommand mMyCommand;

    public EventCommand_ScreenFade(ScreenFadeCommand refCommand, FrmEvent editor)
    {
        InitializeComponent();
        mMyCommand = refCommand;
        mEventEditor = editor;
        InitLocalization();
        PopulateForm();
    }

    private void InitLocalization()
    {
        grpFade.Text = EventScreenFade.Title;
        lblAction.Text = EventScreenFade.Action;
        lblSpeed.Text = EventScreenFade.Speed;
        cmbFadeTypes.Items.AddRange(EventScreenFade.FadeTypes.Values.ToArray());
        chkWaitForCompletion.Text = EventScreenFade.WaitForComplete;
        btnCancel.Text = EventScreenFade.Cancel;
        btnSave.Text = EventScreenFade.Okay;
    }

    private void PopulateForm()
    {
        var prevType = (int)mMyCommand.FadeType;
        
        cmbFadeTypes.SelectedIndex = cmbFadeTypes.Items.Count > prevType ? prevType : -1;
        nudFadeSpeed.Value = mMyCommand.SpeedMs == default ? (decimal)ClientConfiguration.Instance.FadeSpeedMs : mMyCommand.SpeedMs;
        chkWaitForCompletion.Checked = mMyCommand.WaitForCompletion;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        mMyCommand.FadeType = (FadeType)cmbFadeTypes.SelectedIndex;
        mMyCommand.WaitForCompletion = chkWaitForCompletion.Checked;
        mMyCommand.SpeedMs = (int)nudFadeSpeed.Value;

        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }
}
