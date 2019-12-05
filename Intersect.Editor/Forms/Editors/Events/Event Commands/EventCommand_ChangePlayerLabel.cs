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

            if (mMyCommand.VariableType == Enums.VariableTypes.PlayerVariable)
            {
              rdoPlayerVariables.Checked = true;
            }
            else
            {
              rdoGlobalVariables.Checked = true;
            }

            LoadVariableList();
        }

        private void InitLocalization()
        {
            grpChangeLabel.Text = Strings.EventChangePlayerLabel.title;
            btnSave.Text = Strings.EventChangePlayerLabel.okay;
            btnCancel.Text = Strings.EventChangePlayerLabel.cancel;
            btnSelectLightColor.Text = Strings.EventChangePlayerLabel.select;
            chkPlayerNameColor.Text = Strings.EventChangePlayerLabel.copyplayernamecolor;

            rdoPlayerVariables.Text = Strings.EventChangePlayerLabel.player;
            rdoGlobalVariables.Text = Strings.EventChangePlayerLabel.global;

            lblPosition.Text = Strings.EventChangePlayerLabel.position;
            cmbPosition.Items.Clear();
            foreach (var position in Strings.EventChangePlayerLabel.positions)
            {
              cmbPosition.Items.Add(position.Value);
            }
        }

        private void LoadVariableList()
        {
          cmbVariable.Items.Clear();
          if (rdoPlayerVariables.Checked)
          {
            cmbVariable.Items.AddRange(PlayerVariableBase.Names);
            cmbVariable.SelectedIndex = PlayerVariableBase.ListIndex(mMyCommand.VariableId);
          }
          else
          {
            cmbVariable.Items.AddRange(ServerVariableBase.Names);
            cmbVariable.SelectedIndex = ServerVariableBase.ListIndex(mMyCommand.VariableId);
          }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Color = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
            mMyCommand.MatchNameColor = chkPlayerNameColor.Checked;
            mMyCommand.Position = cmbPosition.SelectedIndex;

            if (rdoPlayerVariables.Checked)
            {
              mMyCommand.VariableType = Enums.VariableTypes.PlayerVariable;
              mMyCommand.VariableId = PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }
            else
            {
              mMyCommand.VariableType = Enums.VariableTypes.ServerVariable;
              mMyCommand.VariableId = ServerVariableBase.IdFromList(cmbVariable.SelectedIndex);
            }

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
    }
}