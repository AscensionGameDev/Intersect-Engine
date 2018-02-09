using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Intersect.Editor.Classes.Core;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandOptions : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventPage mCurrentPage;
        private EventCommand mMyCommand;

        public EventCommandOptions(EventCommand refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            mCurrentPage = refPage;
            InitLocalization();
            txtShowOptions.Text = mMyCommand.Strs[0];
            txtShowOptionsOpt1.Text = mMyCommand.Strs[1];
            txtShowOptionsOpt2.Text = mMyCommand.Strs[2];
            txtShowOptionsOpt3.Text = mMyCommand.Strs[3];
            txtShowOptionsOpt4.Text = mMyCommand.Strs[4];
            cmbFace.Items.Clear();
            cmbFace.Items.Add(Strings.Get("general", "none"));
            cmbFace.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Face));
            if (cmbFace.Items.IndexOf(TextUtils.NullToNone(mMyCommand.Strs[5])) > -1)
            {
                cmbFace.SelectedIndex = cmbFace.Items.IndexOf(TextUtils.NullToNone(mMyCommand.Strs[5]));
            }
            else
            {
                cmbFace.SelectedIndex = 0;
            }
            UpdateFacePreview();
        }

        private void InitLocalization()
        {
            grpOptions.Text = Strings.Get("eventshowoptions", "title");
            lblText.Text = Strings.Get("eventshowoptions", "text");
            lblFace.Text = Strings.Get("eventshowoptions", "face");
            lblCommands.Text = Strings.Get("eventshowoptions", "commands");
            lblOpt1.Text = Strings.Get("eventshowoptions", "option1");
            lblOpt2.Text = Strings.Get("eventshowoptions", "option2");
            lblOpt3.Text = Strings.Get("eventshowoptions", "option3");
            lblOpt4.Text = Strings.Get("eventshowoptions", "option4");
            btnSave.Text = Strings.Get("eventshowoptions", "okay");
            btnCancel.Text = Strings.Get("eventshowoptions", "cancel");
        }

        private void UpdateFacePreview()
        {
            if (pnlFace == null) return;
            pnlFace.BackgroundImage = File.Exists($"resources/faces/{cmbFace?.Text}")
                    ? new Bitmap($"resources/faces/{cmbFace?.Text}")
                    : null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Strs[0] = txtShowOptions.Text;
            mMyCommand.Strs[1] = txtShowOptionsOpt1.Text;
            mMyCommand.Strs[2] = txtShowOptionsOpt2.Text;
            mMyCommand.Strs[3] = txtShowOptionsOpt3.Text;
            mMyCommand.Strs[4] = txtShowOptionsOpt4.Text;
            mMyCommand.Strs[5] = TextUtils.SanitizeNone(cmbFace.Text);
            if (mMyCommand.Ints[0] == 0)
            {
                for (var i = 0; i < 4; i++)
                {
                    mCurrentPage.CommandLists.Add(new CommandList());
                    mMyCommand.Ints[i] = mCurrentPage.CommandLists.Count - 1;
                }
            }
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
                "http://www.ascensiongamedev.com/community/topic/749-event-text-variables/");
        }
    }
}