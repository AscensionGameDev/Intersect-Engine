using System;
using System.Windows.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandPlayBgs : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private PlaySoundCommand mMyCommand;

        public EventCommandPlayBgs(PlaySoundCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbSound.Items.Clear();
            cmbSound.Items.Add(Strings.General.none);
            cmbSound.Items.AddRange(GameContentManager.SmartSortedSoundNames);
            if (cmbSound.Items.IndexOf(TextUtils.NullToNone(mMyCommand.File)) > -1)
            {
                cmbSound.SelectedIndex = cmbSound.Items.IndexOf(TextUtils.NullToNone(mMyCommand.File));
            }
            else
            {
                cmbSound.SelectedIndex = 0;
            }
        }

        private void InitLocalization()
        {
            grpPlayBGS.Text = Strings.EventPlayBgs.title;
            lblSound.Text = Strings.EventPlayBgs.label;
            btnSave.Text = Strings.EventPlayBgs.okay;
            btnCancel.Text = Strings.EventPlayBgs.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.File = TextUtils.SanitizeNone(cmbSound?.Text);
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
