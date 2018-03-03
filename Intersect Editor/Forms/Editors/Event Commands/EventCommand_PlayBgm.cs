using System;
using System.Windows.Forms;
using Intersect.Editor.ContentManagement;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandPlayBgm : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandPlayBgm(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbBgm.Items.Clear();
            cmbBgm.Items.Add(Strings.General.none);
            cmbBgm.Items.AddRange(GameContentManager.SmartSortedMusicNames);
            if (cmbBgm.Items.IndexOf(TextUtils.NullToNone(mMyCommand.Strs[0])) > -1)
            {
                cmbBgm.SelectedIndex = cmbBgm.Items.IndexOf(TextUtils.NullToNone(mMyCommand.Strs[0]));
            }
            else
            {
                cmbBgm.SelectedIndex = 0;
            }
        }

        private void InitLocalization()
        {
            grpBGM.Text = Strings.EventPlayBgm.title;
            lblBGM.Text = Strings.EventPlayBgm.label;
            btnSave.Text = Strings.EventPlayBgm.okay;
            btnCancel.Text = Strings.EventPlayBgm.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Strs[0] = TextUtils.SanitizeNone(cmbBgm?.Text);
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}