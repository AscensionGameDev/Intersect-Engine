using System;
using System.Windows.Forms;
using Intersect;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_ChatboxText : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private EventCommand _myCommand;

        public EventCommand_ChatboxText(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            txtAddText.Text = _myCommand.Strs[0];
            cmbColor.Items.Clear();
            foreach (Color.ChatColor color in Enum.GetValues(typeof(Color.ChatColor)))
            {
                cmbColor.Items.Add(Globals.GetColorName(color));
            }
            cmbColor.SelectedIndex = cmbColor.Items.IndexOf(_myCommand.Strs[1]);
            if (cmbColor.SelectedIndex == -1) cmbColor.SelectedIndex = 0;
            cmbChannel.SelectedIndex = _myCommand.Ints[0];
        }

        private void InitLocalization()
        {
            grpChatboxText.Text = Strings.Get("eventchatboxtext", "title");
            lblText.Text = Strings.Get("eventchatboxtext", "text");
            lblColor.Text = Strings.Get("eventchatboxtext", "color");
            lblChannel.Text = Strings.Get("eventchatboxtext", "channel");
            lblCommands.Text = Strings.Get("eventchatboxtext", "commands");
            cmbChannel.Items.Clear();
            for (int i = 0; i < 3; i++)
            {
                cmbChannel.Items.Add(Strings.Get("eventchatboxtext", "channel" + i));
            }
            btnSave.Text = Strings.Get("eventchatboxtext", "okay");
            btnCancel.Text = Strings.Get("eventchatboxtext", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Strs[0] = txtAddText.Text;
            _myCommand.Strs[1] = cmbColor.Text;
            _myCommand.Ints[0] = cmbChannel.SelectedIndex;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void lblCommands_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ascensiongamedev.com/community/topic/749-event-text-variables/");
        }
    }
}