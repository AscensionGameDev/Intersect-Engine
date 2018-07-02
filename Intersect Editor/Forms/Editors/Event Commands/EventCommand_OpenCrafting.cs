using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandOpenCraftingTable : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandOpenCraftingTable(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbTable.Items.Clear();
            cmbTable.Items.AddRange(CraftingTableBase.Names);
            cmbTable.SelectedIndex = CraftingTableBase.ListIndex(mMyCommand.Guids[0]);
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
                mMyCommand.Guids[0] = CraftingTableBase.IdFromList(cmbTable.SelectedIndex);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}