using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Dungeons;
using Intersect.Framework.Core.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;


public partial class EventCommandEnterDungeon : UserControl
{

    private readonly FrmEvent mEventEditor;

    private EnterDungeonCommand mMyCommand;

    public EventCommandEnterDungeon(EnterDungeonCommand refCommand, FrmEvent editor)
    {
        InitializeComponent();
        mMyCommand = refCommand;
        mEventEditor = editor;
        InitLocalization();
        cmbDungeon.Items.Clear();
        cmbDungeon.Items.AddRange(DungeonDescriptor.Names);
        cmbDungeon.SelectedIndex = DungeonDescriptor.ListIndex(mMyCommand.DungeonId);
    }

    private void InitLocalization()
    {
        grpDungeon.Text = Strings.EventEnterDungeon.title;
        lblDungeon.Text = Strings.EventEnterDungeon.label;
        btnSave.Text = Strings.EventEnterDungeon.okay;
        btnCancel.Text = Strings.EventEnterDungeon.cancel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        if (cmbDungeon.SelectedIndex > -1)
        {
            mMyCommand.DungeonId = DungeonDescriptor.IdFromList(cmbDungeon.SelectedIndex);
        }

        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }

}
