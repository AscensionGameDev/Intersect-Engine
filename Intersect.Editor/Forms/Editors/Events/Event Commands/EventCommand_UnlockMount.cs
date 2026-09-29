using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Events.Commands;
using Intersect.Framework.Core.GameObjects.Mounts;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;


public partial class EventCommandUnlockMount : UserControl
{

    private readonly FrmEvent mEventEditor;

    private UnlockMountCommand mMyCommand;

    public EventCommandUnlockMount(UnlockMountCommand refCommand, FrmEvent editor)
    {
        InitializeComponent();
        mMyCommand = refCommand;
        mEventEditor = editor;
        InitLocalization();

        // The mounts defined in the Mount Editor; no "none" entry since unlocking is additive.
        cmbMount.Items.Clear();
        cmbMount.Items.AddRange(MountDescriptor.Names);
        cmbMount.SelectedIndex = Math.Max(0, MountDescriptor.ListIndex(mMyCommand.MountId));
    }

    private void InitLocalization()
    {
        grpMount.Text = Strings.EventUnlockMount.title;
        lblMount.Text = Strings.EventUnlockMount.label;
        btnSave.Text = Strings.EventUnlockMount.okay;
        btnCancel.Text = Strings.EventUnlockMount.cancel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        mMyCommand.MountId = cmbMount.SelectedIndex >= 0 ? MountDescriptor.IdFromList(cmbMount.SelectedIndex) : Guid.Empty;

        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }

}
