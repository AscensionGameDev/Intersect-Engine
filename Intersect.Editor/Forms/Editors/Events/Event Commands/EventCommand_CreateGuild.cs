using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Events.Commands;
using Intersect.Framework.Core.GameObjects.Variables;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;


public partial class EventCommandCreateGuild : UserControl
{

    private readonly FrmEvent mEventEditor;

    private EventPage mCurrentPage;

    private CreateGuildCommand mMyCommand;

    public EventCommandCreateGuild(CreateGuildCommand refCommand, EventPage page, FrmEvent editor)
    {
        InitializeComponent();
        mMyCommand = refCommand;
        mCurrentPage = page;
        mEventEditor = editor;

        InitLocalization();
        cmbVariable.Items.Clear();
        cmbVariable.Items.AddRange(PlayerVariableDescriptor.Names);

        if (mMyCommand.VariableId != null && mMyCommand.VariableId != Guid.Empty )
        {
            cmbVariable.SelectedIndex = PlayerVariableDescriptor.ListIndex(mMyCommand.VariableId);
        }
        else if (cmbVariable.Items.Count > 0)
        {
            cmbVariable.SelectedIndex = 0;
        }
    }

    private void InitLocalization()
    {
        grpCreateGuild.Text = Strings.EventCreateGuild.Title;
        lblVariable.Text = Strings.EventCreateGuild.SelectVariable;
        btnSave.Text = Strings.EventCreateGuild.Okay;
        btnCancel.Text = Strings.EventCreateGuild.Cancel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        mMyCommand.VariableId = PlayerVariableDescriptor.IdFromList(cmbVariable.SelectedIndex);
        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }

}
