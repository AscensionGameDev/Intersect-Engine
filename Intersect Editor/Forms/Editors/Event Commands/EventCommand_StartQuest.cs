
using System;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Editor.Classes.Core;
using Intersect_Library;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_StartQuest : UserControl
    {
        private EventCommand _myCommand;
        private EventPage _currentPage;
        private readonly FrmEvent _eventEditor;
        public EventCommand_StartQuest(EventCommand refCommand, EventPage page, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _currentPage = page;
            _eventEditor = editor;
            InitLocalization();
            cmbQuests.Items.Clear();
            cmbQuests.Items.AddRange(Database.GetGameObjectList(GameObject.Quest));
            cmbQuests.SelectedIndex = Database.GameObjectListIndex(GameObject.Quest, refCommand.Ints[0]);
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
            _myCommand.Ints[0] = Database.GameObjectIdFromList(GameObject.Quest, cmbQuests.SelectedIndex);
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
