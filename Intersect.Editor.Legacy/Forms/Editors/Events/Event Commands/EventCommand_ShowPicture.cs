using System;
using System.Windows.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommand_ShowPicture : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private ShowPictureCommand mMyCommand;

        public EventCommand_ShowPicture(ShowPictureCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbPicture.Items.Clear();

            var sortedTextures = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Image);

            if (sortedTextures.Length > 0)
            {
                cmbPicture.Items.AddRange(sortedTextures);
            }
            else
            {
                cmbPicture.Items.Add(Strings.General.None);
            }

            cmbSize.Items.Clear();
            cmbSize.Items.Add(Strings.EventShowPicture.original);
            cmbSize.Items.Add(Strings.EventShowPicture.fullscreen);
            cmbSize.Items.Add(Strings.EventShowPicture.halfscreen);
            cmbSize.Items.Add(Strings.EventShowPicture.stretchtofit);

            if (mMyCommand != null)
            {
                cmbPicture.SelectedIndex = Math.Max(0, cmbPicture.Items.IndexOf(mMyCommand.File));
                cmbSize.SelectedIndex = Math.Max(0, mMyCommand.Size);
                chkClick.Checked = mMyCommand.Clickable;
                nudHideTime.Value = mMyCommand.HideTime;
                chkWaitUntilClosed.Checked = mMyCommand.WaitUntilClosed;
            }
        }

        private void InitLocalization()
        {
            grpShowPicture.Text = Strings.EventShowPicture.title;
            lblPicture.Text = Strings.EventShowPicture.label;
            btnSave.Text = Strings.EventShowPicture.okay;
            btnCancel.Text = Strings.EventShowPicture.cancel;
            chkClick.Text = Strings.EventShowPicture.checkbox;
            lblSize.Text = Strings.EventShowPicture.size;
            lblHide.Text = Strings.EventShowPicture.hide;
            chkWaitUntilClosed.Text = Strings.EventShowPicture.wait;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.File = cmbPicture.Text;
            mMyCommand.Size = cmbSize.SelectedIndex;
            mMyCommand.Clickable = chkClick.Checked;
            mMyCommand.HideTime = (int)nudHideTime.Value;
            mMyCommand.WaitUntilClosed = chkWaitUntilClosed.Checked;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
