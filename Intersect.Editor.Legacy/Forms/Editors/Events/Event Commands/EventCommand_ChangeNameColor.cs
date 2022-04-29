using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandChangeNameColor : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private ChangeNameColorCommand mMyCommand;

        public EventCommandChangeNameColor(ChangeNameColorCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;

            var color = refCommand.Color;
            if (color == null)
            {
                color = Color.White;
            }

            pnlColor.BackColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
            chkOverride.Checked = refCommand.Override;
            chkRemove.Checked = refCommand.Remove;
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpChangeLevel.Text = Strings.EventChangeNameColor.title;
            btnSave.Text = Strings.EventChangeNameColor.okay;
            btnCancel.Text = Strings.EventChangeNameColor.cancel;
            btnSelectLightColor.Text = Strings.EventChangeNameColor.select;
            chkOverride.Text = Strings.EventChangeNameColor.adminoverride;
            chkRemove.Text = Strings.EventChangeNameColor.remove;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Color = Color.FromArgb(
                pnlColor.BackColor.A, pnlColor.BackColor.R, pnlColor.BackColor.G, pnlColor.BackColor.B
            );

            mMyCommand.Override = chkOverride.Checked;
            mMyCommand.Remove = chkRemove.Checked;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void btnSelectLightColor_Click(object sender, EventArgs e)
        {
            colorDialog.Color = pnlColor.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                pnlColor.BackColor = colorDialog.Color;
            }
        }

    }

}
