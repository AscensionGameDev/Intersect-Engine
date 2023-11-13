using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandStartCommonEvent : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private StartCommmonEventCommand mMyCommand;

        public EventCommandStartCommonEvent(StartCommmonEventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();

            cmbEvent.Items.Clear();
            cmbEvent.Items.AddRange(EventBase.Names);

            cmbEvent.SelectedIndex = EventBase.ListIndex(refCommand.EventId);
            chkAllInInstance.Checked = refCommand.AllInInstance;
            chkOverworldOverride.Checked = refCommand.AllowInOverworld;
            chkOverworldOverride.Enabled = chkAllInInstance.Checked;
        }

        private void InitLocalization()
        {
            grpCommonEvent.Text = Strings.EventStartCommonEvent.title;
            lblCommonEvent.Text = Strings.EventStartCommonEvent.label;
            chkAllInInstance.Text = Strings.EventStartCommonEvent.AllInInstance;
            chkOverworldOverride.Text = Strings.EventStartCommonEvent.AllowInOverworld;

            ToolTip overworldWarningTooltip = new ToolTip()
            {
                InitialDelay = 1000,
                ReshowDelay = 500,
            };

            overworldWarningTooltip.SetToolTip(chkOverworldOverride, Strings.EventStartCommonEvent.OverworldOverrideTooltip.ToString());

            btnSave.Text = Strings.EventStartCommonEvent.okay;
            btnCancel.Text = Strings.EventStartCommonEvent.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.EventId = EventBase.IdFromList(cmbEvent.SelectedIndex);
            mMyCommand.AllInInstance = chkAllInInstance.Checked;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void chkAllInInstance_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkAllInInstance.Checked)
            {
                chkOverworldOverride.Checked = false;
            }
            chkOverworldOverride.Enabled = chkAllInInstance.Checked;
        }
    }

}
