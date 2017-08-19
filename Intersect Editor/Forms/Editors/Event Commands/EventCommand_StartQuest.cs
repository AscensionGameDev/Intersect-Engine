using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_StartQuest : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private EventPage _currentPage;
        private EventCommand _myCommand;

        public EventCommand_StartQuest(EventCommand refCommand, EventPage page, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _currentPage = page;
            _eventEditor = editor;
            InitLocalization();
            cmbQuests.Items.Clear();
            cmbQuests.Items.AddRange(Database.GetGameObjectList(GameObjectType.Quest));
            cmbQuests.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Quest, refCommand.Ints[0]);
            chkShowOfferWindow.Checked = Convert.ToBoolean(refCommand.Ints[1]);
        }

        private void InitLocalization()
        {
            grpStartQuest.Text = Strings.Get("eventstartquest", "title");
            lblQuest.Text = Strings.Get("eventstartquest", "label");
            chkShowOfferWindow.Text = Strings.Get("eventstartquest", "showwindow");
            btnSave.Text = Strings.Get("eventstartquest", "okay");
            btnCancel.Text = Strings.Get("eventstartquest", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = Database.GameObjectIdFromList(GameObjectType.Quest, cmbQuests.SelectedIndex);
            _myCommand.Ints[1] = Convert.ToInt32(chkShowOfferWindow.Checked);
            if (_myCommand.Ints[4] == 0)
                // command.Ints[4, and 5] are reserved for when the action succeeds or fails
            {
                for (var i = 0; i < 2; i++)
                {
                    _currentPage.CommandLists.Add(new CommandList());
                    _myCommand.Ints[4 + i] = _currentPage.CommandLists.Count - 1;
                }
            }
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}