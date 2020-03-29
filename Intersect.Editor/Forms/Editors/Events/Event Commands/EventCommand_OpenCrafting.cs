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
        }

        private void InitLocalization()
        {
            grpTable.Text = Strings.EventOpenCrafting.title;
            lblTable.Text = Strings.EventOpenCrafting.label;
            btnSave.Text = Strings.EventOpenCrafting.okay;
            btnCancel.Text = Strings.EventOpenCrafting.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbTable.SelectedIndex > -1)
            {
                mMyCommand.CraftingTableId = CraftingTableBase.IdFromList(cmbTable.SelectedIndex);
            }

            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
