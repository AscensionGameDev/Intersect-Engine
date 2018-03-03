using System;
using System.Windows.Forms;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandChatboxText : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandChatboxText(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            txtAddText.Text = mMyCommand.Strs[0];
            cmbColor.Items.Clear();
            foreach (Color.ChatColor color in Enum.GetValues(typeof(Color.ChatColor)))
            {
                cmbColor.Items.Add(Globals.GetColorName(color));
            }
            cmbColor.SelectedIndex = cmbColor.Items.IndexOf(mMyCommand.Strs[1]);
            if (cmbColor.SelectedIndex == -1) cmbColor.SelectedIndex = 0;
            cmbChannel.SelectedIndex = mMyCommand.Ints[0];
        }

        private void InitLocalization()
        {
            grpChatboxText.Text = Strings.EventChatboxText.title;
            lblText.Text = Strings.EventChatboxText.text;
            lblColor.Text = Strings.EventChatboxText.color;
            lblChannel.Text = Strings.EventChatboxText.channel;
            lblCommands.Text = Strings.EventChatboxText.commands;
            cmbChannel.Items.Clear();
            for (int i = 0; i < Strings.EventChatboxText.channels.Length; i++)
            {
                cmbChannel.Items.Add(Strings.EventChatboxText.channels[i]);
            }
            btnSave.Text = Strings.EventChatboxText.okay;
            btnCancel.Text = Strings.EventChatboxText.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Strs[0] = txtAddText.Text;
            mMyCommand.Strs[1] = cmbColor.Text;
            mMyCommand.Ints[0] = cmbChannel.SelectedIndex;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void lblCommands_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(
                "http://www.ascensiongamedev.com/community/topic/749-event-text-variables/");
        }
    }
}