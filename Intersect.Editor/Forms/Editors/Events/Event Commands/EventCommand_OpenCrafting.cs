using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Crafting;
using Intersect.Framework.Core.GameObjects.Events.Commands;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;


public partial class EventCommandOpenCraftingTable : UserControl
{

    private readonly FrmEvent mEventEditor;

    private OpenCraftingTableCommand mMyCommand;

    public EventCommandOpenCraftingTable(OpenCraftingTableCommand refCommand, FrmEvent editor)
    {
        InitializeComponent();
        mMyCommand = refCommand;
        mEventEditor = editor;
        InitLocalization();
        cmbTable.Items.Clear();
        cmbTable.Items.AddRange(CraftingTableDescriptor.Names);
        cmbTable.SelectedIndex = CraftingTableDescriptor.ListIndex(mMyCommand.CraftingTableId);
        chkJournalMode.Checked = mMyCommand.JournalMode;
    }

    private void InitLocalization()
    {
        grpTable.Text = Strings.EventOpenCrafting.title;
        lblTable.Text = Strings.EventOpenCrafting.label;
        btnSave.Text = Strings.EventOpenCrafting.okay;
        btnCancel.Text = Strings.EventOpenCrafting.cancel;
        chkJournalMode.Text = Strings.EventOpenCrafting.JournalMode;

        ToolTip toolTip1 = new ToolTip();

        toolTip1.AutoPopDelay = 5000;
        toolTip1.InitialDelay = 1000;
        toolTip1.ReshowDelay = 500;
        toolTip1.ShowAlways = true;

        toolTip1.SetToolTip(chkJournalMode, Strings.EventOpenCrafting.JournalModeTooltip);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        if (cmbTable.SelectedIndex > -1)
        {
            mMyCommand.CraftingTableId = CraftingTableDescriptor.IdFromList(cmbTable.SelectedIndex);
        }

        mMyCommand.JournalMode = chkJournalMode.Checked;

        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }

}
