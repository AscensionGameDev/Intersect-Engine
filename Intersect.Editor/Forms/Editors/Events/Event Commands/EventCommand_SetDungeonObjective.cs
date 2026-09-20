using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;


public partial class EventCommandSetDungeonObjective : UserControl
{

    private readonly FrmEvent mEventEditor;

    private SetDungeonObjectiveCommand mMyCommand;

    public EventCommandSetDungeonObjective(SetDungeonObjectiveCommand refCommand, FrmEvent editor)
    {
        InitializeComponent();
        mMyCommand = refCommand;
        mEventEditor = editor;
        InitLocalization();
        txtObjective.Text = mMyCommand.Objective;
    }

    private void InitLocalization()
    {
        grpObjective.Text = Strings.EventSetDungeonObjective.title;
        lblObjective.Text = Strings.EventSetDungeonObjective.label;
        btnSave.Text = Strings.EventSetDungeonObjective.okay;
        btnCancel.Text = Strings.EventSetDungeonObjective.cancel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        mMyCommand.Objective = txtObjective.Text;
        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }

}
