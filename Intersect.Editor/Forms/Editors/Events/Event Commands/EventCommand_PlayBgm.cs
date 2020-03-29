using System;
using System.Windows.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandPlayBgm : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private PlayBgmCommand mMyCommand;

        public EventCommandPlayBgm(PlayBgmCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbBgm.Items.Clear();
            cmbBgm.Items.Add(Strings.General.none);
            cmbBgm.Items.AddRange(GameContentManager.SmartSortedMusicNames);
            if (cmbBgm.Items.IndexOf(TextUtils.NullToNone(mMyCommand.File)) > -1)
            {
                cmbBgm.SelectedIndex = cmbBgm.Items.IndexOf(TextUtils.NullToNone(mMyCommand.File));
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
            mMyCommand.File = TextUtils.SanitizeNone(cmbBgm?.Text);
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
