using System;
using System.Windows.Forms;
using Intersect.Editor.Classes.Core;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;
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
            cmbBgm.Items.Add(Strings.general.none);
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
            grpBGM.Text = Strings.eventplaybgm.title;
            lblBGM.Text = Strings.eventplaybgm.label;
            btnSave.Text = Strings.eventplaybgm.okay;
            btnCancel.Text = Strings.eventplaybgm.cancel;
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