using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandOptions : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private EventPage mCurrentPage;

        private ShowOptionsCommand mMyCommand;

        public EventCommandOptions(ShowOptionsCommand refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mCurrentPage = refPage;
            InitLocalization();
            txtShowOptions.Text = mMyCommand.Text;
            txtShowOptionsOpt1.Text = mMyCommand.Options[0];
            txtShowOptionsOpt2.Text = mMyCommand.Options[1];
            txtShowOptionsOpt3.Text = mMyCommand.Options[2];
            txtShowOptionsOpt4.Text = mMyCommand.Options[3];
            cmbFace.Items.Clear();
            cmbFace.Items.Add(Strings.General.none);
            cmbFace.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Face));
            if (cmbFace.Items.IndexOf(TextUtils.NullToNone(mMyCommand.Face)) > -1)
            {
                cmbFace.SelectedIndex = cmbFace.Items.IndexOf(TextUtils.NullToNone(mMyCommand.Face));
            }
            else
            {
                cmbFace.SelectedIndex = 0;
            }

            UpdateFacePreview();
        }

        private void InitLocalization()
        {
            grpOptions.Text = Strings.EventShowOptions.title;
            lblText.Text = Strings.EventShowOptions.text;
            lblFace.Text = Strings.EventShowOptions.face;
            lblCommands.Text = Strings.EventShowOptions.commands;
            lblOpt1.Text = Strings.EventShowOptions.option1;
            lblOpt2.Text = Strings.EventShowOptions.option2;
            lblOpt3.Text = Strings.EventShowOptions.option3;
            lblOpt4.Text = Strings.EventShowOptions.option4;
            btnSave.Text = Strings.EventShowOptions.okay;
            btnCancel.Text = Strings.EventShowOptions.cancel;
        }

        private void UpdateFacePreview()
        {
            if (pnlFace == null)
            {
                return;
            }

            pnlFace.BackgroundImage = File.Exists($"resources/faces/{cmbFace?.Text}")
                ? new Bitmap($"resources/faces/{cmbFace?.Text}")
                : null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Text = txtShowOptions.Text;
            mMyCommand.Options[0] = txtShowOptionsOpt1.Text;
            mMyCommand.Options[1] = txtShowOptionsOpt2.Text;
            mMyCommand.Options[2] = txtShowOptionsOpt3.Text;
            mMyCommand.Options[3] = txtShowOptionsOpt4.Text;
            mMyCommand.Face = TextUtils.SanitizeNone(cmbFace.Text);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void cmbFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFacePreview();
        }

        private void lblCommands_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(
                "http://www.ascensiongamedev.com/community/topic/749-event-text-variables/"
            );
        }

    }

}
