using System;
using System.Windows.Forms;
using Intersect.Editor.Classes.Core;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_PlayBgs : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private EventCommand _myCommand;

        public EventCommand_PlayBgs(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbSound.Items.Clear();
            cmbSound.Items.Add(Strings.Get("general", "none"));
            cmbSound.Items.AddRange(GameContentManager.GetSoundNames());
            if (cmbSound.Items.IndexOf(TextUtils.NullToNone(_myCommand.Strs[0])) > -1)
            {
                cmbSound.SelectedIndex = cmbSound.Items.IndexOf(TextUtils.NullToNone(_myCommand.Strs[0]));
            }
            else
            {
                cmbSound.SelectedIndex = 0;
            }
        }

        private void InitLocalization()
        {
            grpPlayBGS.Text = Strings.Get("eventplaybgs", "title");
            lblSound.Text = Strings.Get("eventplaybgs", "label");
            btnSave.Text = Strings.Get("eventplaybgs", "okay");
            btnCancel.Text = Strings.Get("eventplaybgs", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Strs[0] = TextUtils.SanitizeNone(cmbSound?.Text);
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}