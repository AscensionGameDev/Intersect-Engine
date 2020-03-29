using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandChangeItems : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private EventPage mCurrentPage;

        private ChangeItemsCommand mMyCommand;

        public EventCommandChangeItems(ChangeItemsCommand refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mCurrentPage = refPage;
            InitLocalization();
            cmbItem.Items.Clear();
            cmbItem.Items.AddRange(ItemBase.Names);
            cmbAction.SelectedIndex = mMyCommand.Add ? 0 : 1;
            cmbItem.SelectedIndex = ItemBase.ListIndex(mMyCommand.ItemId);
            if (mMyCommand.Quantity < 1)
            {
                nudGiveTakeAmount.Value = 1;
            }
            else
            {
                nudGiveTakeAmount.Value = mMyCommand.Quantity;
            }

            lblAmount.Text = Strings.EventChangeItems.amount;
        }

        private void InitLocalization()
        {
            grpChangeItems.Text = Strings.EventChangeItems.title;
            lblAction.Text = Strings.EventChangeItems.action;
            cmbAction.Items.Clear();
            for (var i = 0; i < Strings.EventChangeItems.actions.Count; i++)
            {
                cmbAction.Items.Add(Strings.EventChangeItems.actions[i]);
            }

            btnSave.Text = Strings.EventChangeItems.okay;
            btnCancel.Text = Strings.EventChangeItems.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Add = !Convert.ToBoolean(cmbAction.SelectedIndex);
            mMyCommand.ItemId = ItemBase.IdFromList(cmbItem.SelectedIndex);
            mMyCommand.Quantity = (int) nudGiveTakeAmount.Value;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
