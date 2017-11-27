using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

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
            grpChatboxText.Text = Strings.eventchatboxtext.title;
            lblText.Text = Strings.eventchatboxtext.text;
            lblColor.Text = Strings.eventchatboxtext.color;
            lblChannel.Text = Strings.eventchatboxtext.channel;
            lblCommands.Text = Strings.eventchatboxtext.commands;
            cmbChannel.Items.Clear();
            for (int i = 0; i < Strings.eventchatboxtext.channels.Length; i++)
            {
                cmbChannel.Items.Add(Strings.eventchatboxtext.channels[i]);
            }
            btnSave.Text = Strings.eventchatboxtext.okay;
            btnCancel.Text = Strings.eventchatboxtext.cancel;
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