using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Events.Commands;
using Intersect.Framework.Core.GameObjects.Variables;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;


public partial class EventCommandChangeName : UserControl
{

    private readonly FrmEvent mEventEditor;

    private ChangeNameCommand mMyCommand;

    private EventPage mCurrentPage;

    public EventCommandChangeName(ChangeNameCommand refCommand, EventPage refPage, FrmEvent editor)
    {
        InitializeComponent();
        mMyCommand = refCommand;
        mCurrentPage = refPage;
        mEventEditor = editor;

        cmbVariable.Items.Clear();
        cmbVariable.Items.AddRange(PlayerVariableDescriptor.Names);
        
        InitLocalization();
    }

    private void InitLocalization()
    {
        grpChangeLevel.Text = Strings.EventChangeName.title;
        btnSave.Text = Strings.EventChangeName.okay;
        btnCancel.Text = Strings.EventChangeName.cancel;

        lblVariable.Text = Strings.EventChangeName.variable;
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
