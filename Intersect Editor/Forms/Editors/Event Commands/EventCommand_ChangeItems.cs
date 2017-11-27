using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

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
            cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, mMyCommand.Ints[1]);
            if (mMyCommand.Ints[2] < 1)
            {
                nudGiveTakeAmount.Value = 1;
            }
            else
            {
                nudGiveTakeAmount.Value = mMyCommand.Ints[2];
            }
            lblAmount.Text = Strings.eventchangeitems.amount;
        }

        private void InitLocalization()
        {
            grpChangeItems.Text = Strings.eventchangeitems.title;
            lblAction.Text = Strings.eventchangeitems.action;
            cmbAction.Items.Clear();
            for (int i = 0; i < Strings.eventchangeitems.actions.Length; i++)
            {
                cmbAction.Items.Add(Strings.eventchangeitems.actions[i]);
            }
            btnSave.Text = Strings.eventchangeitems.okay;
            btnCancel.Text = Strings.eventchangeitems.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Ints[0] = cmbAction.SelectedIndex;
            mMyCommand.Ints[1] = Database.GameObjectIdFromList(GameObjectType.Item, cmbItem.SelectedIndex);
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