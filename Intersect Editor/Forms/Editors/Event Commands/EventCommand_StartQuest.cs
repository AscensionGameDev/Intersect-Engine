using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandStartQuest : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventPage mCurrentPage;
        private EventCommand mMyCommand;

        public EventCommandStartQuest(EventCommand refCommand, EventPage page, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mCurrentPage = page;
            mEventEditor = editor;
            InitLocalization();
            cmbQuests.Items.Clear();
            cmbQuests.Items.AddRange(Database.GetGameObjectList(GameObjectType.Quest));
            cmbQuests.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Quest, refCommand.Ints[0]);
            chkShowOfferWindow.Checked = Convert.ToBoolean(refCommand.Ints[1]);
        }

        private void InitLocalization()
        {
            grpStartQuest.Text = Strings.EventStartQuest.title;
            lblQuest.Text = Strings.EventStartQuest.label;
            chkShowOfferWindow.Text = Strings.EventStartQuest.showwindow;
            btnSave.Text = Strings.EventStartQuest.okay;
            btnCancel.Text = Strings.EventStartQuest.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Ints[0] = Database.GameObjectIdFromList(GameObjectType.Quest, cmbQuests.SelectedIndex);
            mMyCommand.Ints[1] = Convert.ToInt32(chkShowOfferWindow.Checked);
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