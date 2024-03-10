using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

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
            cmbTable.Items.AddRange(CraftingTableBase.Names);
            cmbTable.SelectedIndex = CraftingTableBase.ListIndex(mMyCommand.CraftingTableId);
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
                mMyCommand.CraftingTableId = CraftingTableBase.IdFromList(cmbTable.SelectedIndex);
            }

            mMyCommand.JournalMode = chkJournalMode.Checked;

            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
