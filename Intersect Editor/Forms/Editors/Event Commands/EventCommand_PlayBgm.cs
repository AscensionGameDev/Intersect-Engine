using System;
using System.Windows.Forms;
using Intersect.Editor.Classes.Core;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_PlayBgm : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private EventCommand _myCommand;

        public EventCommand_PlayBgm(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbBgm.Items.Clear();
            cmbBgm.Items.Add(Strings.Get("general", "none"));
            cmbBgm.Items.AddRange(GameContentManager.GetMusicNames());
            if (cmbBgm.Items.IndexOf(_myCommand.Strs[0]) > -1)
            {
                cmbBgm.SelectedIndex = cmbBgm.Items.IndexOf(_myCommand.Strs[0]);
            }
            else
            {
                cmbBgm.SelectedIndex = 0;
            }
        }

        private void InitLocalization()
        {
            grpBGM.Text = Strings.Get("eventplaybgm", "title");
            lblBGM.Text = Strings.Get("eventplaybgm", "label");
            btnSave.Text = Strings.Get("eventplaybgm", "okay");
            btnCancel.Text = Strings.Get("eventplaybgm", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Strs[0] = cmbBgm.Text;
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