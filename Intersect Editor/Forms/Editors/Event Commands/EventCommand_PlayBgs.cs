using System;
using System.Windows.Forms;
using Intersect.Editor.Classes.Core;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandPlayBgs : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandPlayBgs(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbSound.Items.Clear();
            cmbSound.Items.Add(Strings.general.none);
            cmbSound.Items.AddRange(GameContentManager.SmartSortedSoundNames);
            if (cmbSound.Items.IndexOf(TextUtils.NullToNone(mMyCommand.Strs[0])) > -1)
            {
                cmbSound.SelectedIndex = cmbSound.Items.IndexOf(TextUtils.NullToNone(mMyCommand.Strs[0]));
            }
            else
            {
                cmbSound.SelectedIndex = 0;
            }
        }

        private void InitLocalization()
        {
            grpPlayBGS.Text = Strings.eventplaybgs.title;
            lblSound.Text = Strings.eventplaybgs.label;
            btnSave.Text = Strings.eventplaybgs.okay;
            btnCancel.Text = Strings.eventplaybgs.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Strs[0] = TextUtils.SanitizeNone(cmbSound?.Text);
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