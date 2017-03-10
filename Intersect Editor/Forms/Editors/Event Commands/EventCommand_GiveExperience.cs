
using System;
using System.Windows.Forms;
using DarkUI.Controls;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_GiveExperience : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_GiveExperience(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            nudExperience.Value = _myCommand.Ints[0];
        }

        private void InitLocalization()
        {
            grpGiveExperience.Text = Strings.Get("eventgiveexperience", "title");
            lblExperience.Text = Strings.Get("eventgiveexperience", "label");
            btnSave.Text = Strings.Get("eventgiveexperience", "okay");
            btnCancel.Text = Strings.Get("eventgiveexperience", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Ints[0] = (int)nudExperience.Value;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}
