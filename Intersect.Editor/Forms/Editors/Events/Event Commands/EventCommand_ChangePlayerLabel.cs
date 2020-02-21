using System;
using System.Windows.Forms;
using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    public partial class EventCommandChangePlayerLabel : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private ChangePlayerLabelCommand mMyCommand;

        public EventCommandChangePlayerLabel(ChangePlayerLabelCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            InitLocalization();
            mMyCommand = refCommand;
            mEventEditor = editor;

            Color color = refCommand.Color;
            if (color == null) color = Color.White;
            pnlLightColor.BackColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
            chkPlayerNameColor.Checked = refCommand.MatchNameColor;
            cmbPosition.SelectedIndex = refCommand.Position;
            txtLabel.Text = refCommand.Value;
        }

        private void InitLocalization()
        {
            grpChangeLabel.Text = Strings.EventChangePlayerLabel.title;
            btnSave.Text = Strings.EventChangePlayerLabel.okay;
            btnCancel.Text = Strings.EventChangePlayerLabel.cancel;
            btnSelectLightColor.Text = Strings.EventChangePlayerLabel.select;
            chkPlayerNameColor.Text = Strings.EventChangePlayerLabel.copyplayernamecolor;

            lblValue.Text = Strings.EventChangePlayerLabel.value;
            lblStringTextVariables.Text = Strings.EventChangePlayerLabel.hint;

            lblPosition.Text = Strings.EventChangePlayerLabel.position;
            cmbPosition.Items.Clear();
            foreach (var position in Strings.EventChangePlayerLabel.positions)
            {
              cmbPosition.Items.Add(position.Value);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Color = Color.FromArgb(pnlLightColor.BackColor.A, pnlLightColor.BackColor.R, pnlLightColor.BackColor.G, pnlLightColor.BackColor.B);
            mMyCommand.MatchNameColor = chkPlayerNameColor.Checked;
            mMyCommand.Position = cmbPosition.SelectedIndex;
            mMyCommand.Value = txtLabel.Text;

            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void btnSelectLightColor_Click(object sender, EventArgs e)
        {
            colorDialog.Color = System.Drawing.Color.White;
            colorDialog.ShowDialog();
            pnlLightColor.BackColor = colorDialog.Color;
            EditorGraphics.TilePreviewUpdated = true;
        }

        private void lblStringTextVariables_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(
                "http://www.ascensiongamedev.com/community/topic/749-event-text-variables/");
        }
    }
}