using System;
using System.Windows.Forms;

using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandChatboxText : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private AddChatboxTextCommand mMyCommand;

        public EventCommandChatboxText(AddChatboxTextCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            txtAddText.Text = mMyCommand.Text;
            cmbColor.Items.Clear();
            foreach (Color.ChatColor color in Enum.GetValues(typeof(Color.ChatColor)))
            {
                cmbColor.Items.Add(Globals.GetColorName(color));
            }

            cmbColor.SelectedIndex = cmbColor.Items.IndexOf(mMyCommand.Color);
            if (cmbColor.SelectedIndex == -1)
            {
                cmbColor.SelectedIndex = 0;
            }

            cmbChannel.SelectedIndex = (int) mMyCommand.Channel;
        }

        private void InitLocalization()
        {
            grpChatboxText.Text = Strings.EventChatboxText.title;
            lblText.Text = Strings.EventChatboxText.text;
            lblColor.Text = Strings.EventChatboxText.color;
            lblChannel.Text = Strings.EventChatboxText.channel;
            lblCommands.Text = Strings.EventChatboxText.commands;
            cmbChannel.Items.Clear();
            for (var i = 0; i < Strings.EventChatboxText.channels.Count; i++)
            {
                cmbChannel.Items.Add(Strings.EventChatboxText.channels[i]);
            }

            btnSave.Text = Strings.EventChatboxText.okay;
            btnCancel.Text = Strings.EventChatboxText.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Text = txtAddText.Text;
            mMyCommand.Color = cmbColor.Text;
            mMyCommand.Channel = (ChatboxChannel) cmbChannel.SelectedIndex;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void lblCommands_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(
                "http://www.ascensiongamedev.com/community/topic/749-event-text-variables/"
            );
        }

    }

}
