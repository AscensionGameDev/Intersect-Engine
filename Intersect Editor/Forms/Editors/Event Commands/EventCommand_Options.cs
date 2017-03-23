using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect_Editor.Classes.Core;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_Options : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private EventPage _currentPage;
        private EventCommand _myCommand;

        public EventCommand_Options(EventCommand refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            _currentPage = refPage;
            InitLocalization();
            txtShowOptions.Text = _myCommand.Strs[0];
            txtShowOptionsOpt1.Text = _myCommand.Strs[1];
            txtShowOptionsOpt2.Text = _myCommand.Strs[2];
            txtShowOptionsOpt3.Text = _myCommand.Strs[3];
            txtShowOptionsOpt4.Text = _myCommand.Strs[4];
            cmbFace.Items.Clear();
            cmbFace.Items.Add(Strings.Get("general", "none"));
            cmbFace.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Face));
            if (cmbFace.Items.IndexOf(_myCommand.Strs[5]) > -1)
            {
                cmbFace.SelectedIndex = cmbFace.Items.IndexOf(_myCommand.Strs[5]);
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
            if (File.Exists("resources/faces/" + cmbFace.Text))
            {
                pnlFace.BackgroundImage = new Bitmap("resources/faces/" + cmbFace.Text);
            }
            else
            {
                pnlFace.BackgroundImage = null;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Strs[0] = txtShowOptions.Text;
            _myCommand.Strs[1] = txtShowOptionsOpt1.Text;
            _myCommand.Strs[2] = txtShowOptionsOpt2.Text;
            _myCommand.Strs[3] = txtShowOptionsOpt3.Text;
            _myCommand.Strs[4] = txtShowOptionsOpt4.Text;
            _myCommand.Strs[5] = cmbFace.Text;
            if (_myCommand.Ints[0] == 0)
            {
                for (var i = 0; i < 4; i++)
                {
                    _currentPage.CommandLists.Add(new CommandList());
                    _myCommand.Ints[i] = _currentPage.CommandLists.Count - 1;
                }
            }
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void cmbFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFacePreview();
        }

        private void lblCommands_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ascensiongamedev.com/community/topic/749-event-text-variables/");
        }
    }
}