using System;
using System.Windows.Forms;
using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandChangeItems : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventPage mCurrentPage;
        private EventCommand mMyCommand;

        public EventCommandChangeItems(EventCommand refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mCurrentPage = refPage;
            InitLocalization();
            cmbItem.Items.Clear();
            cmbItem.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            cmbAction.SelectedIndex = mMyCommand.Ints[0];
            cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, mMyCommand.Guids[1]);
            if (mMyCommand.Ints[2] < 1)
            {
                nudGiveTakeAmount.Value = 1;
            }
            else
            {
                nudGiveTakeAmount.Value = mMyCommand.Ints[2];
            }
            lblAmount.Text = Strings.EventChangeItems.amount;
        }

        private void InitLocalization()
        {
            grpChangeItems.Text = Strings.EventChangeItems.title;
            lblAction.Text = Strings.EventChangeItems.action;
            cmbAction.Items.Clear();
            for (int i = 0; i < Strings.EventChangeItems.actions.Length; i++)
            {
                cmbAction.Items.Add(Strings.EventChangeItems.actions[i]);
            }
            btnSave.Text = Strings.EventChangeItems.okay;
            btnCancel.Text = Strings.EventChangeItems.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Ints[0] = cmbAction.SelectedIndex;
            mMyCommand.Guids[1] = Database.GameObjectIdFromList(GameObjectType.Item, cmbItem.SelectedIndex);
            mMyCommand.Ints[2] = (int) nudGiveTakeAmount.Value;
            if (mMyCommand.Ints[4] == 0)
                // command.Ints[4, and 5] are reserved for when the action succeeds or fails
            {
                for (var i = 0; i < 2; i++)
                {
                    mCurrentPage.CommandLists.Add(new CommandList());
                    mMyCommand.Ints[4 + i] = mCurrentPage.CommandLists.Count - 1;
                }
            }
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}