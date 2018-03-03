using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Intersect.Editor.ContentManagement;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_ShowPicture : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommand_ShowPicture(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            cmbPicture.Items.Clear();
            cmbPicture.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Image));
            if (cmbPicture.Items.IndexOf(mMyCommand.Strs[0]) > -1)
            {
                cmbPicture.SelectedIndex = cmbPicture.Items.IndexOf(mMyCommand.Strs[0]);
            }
            else
            {
                cmbPicture.SelectedIndex = 0;
            }

            cmbSize.Items.Clear();
            cmbSize.Items.Add(Strings.EventShowPicture.original);
            cmbSize.Items.Add(Strings.EventShowPicture.fullscreen);
            cmbSize.Items.Add(Strings.EventShowPicture.halfscreen);
            cmbSize.Items.Add(Strings.EventShowPicture.stretchtofit);
            if (mMyCommand.Ints[0] > -1)
            {
                cmbSize.SelectedIndex = mMyCommand.Ints[0];
            }
            else
            {
                cmbSize.SelectedIndex = 0;
            }

            chkClick.Checked = Convert.ToBoolean(mMyCommand.Ints[1]);

            InitLocalization();
        }

        private void InitLocalization()
        {
            grpShowPicture.Text = Strings.EventShowPicture.title;
            lblPicture.Text = Strings.EventShowPicture.label;
            btnSave.Text = Strings.EventShowPicture.okay;
            btnCancel.Text = Strings.EventShowPicture.cancel;
            chkClick.Text = Strings.EventShowPicture.checkbox;
            lblSize.Text = Strings.EventShowPicture.size;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Strs[0] = cmbPicture.Text;
            mMyCommand.Ints[0] = cmbSize.SelectedIndex;
            mMyCommand.Ints[1] = Convert.ToInt32(chkClick.Checked);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }
    }
}
