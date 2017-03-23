using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect_Editor.Classes.Core;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_ChangeSprite : UserControl
    {
        private readonly FrmEvent _eventEditor;
        private EventCommand _myCommand;

        public EventCommand_ChangeSprite(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            InitLocalization();
            cmbSprite.Items.Clear();
            cmbSprite.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Entity));
            if (cmbSprite.Items.IndexOf(_myCommand.Strs[0]) > -1)
            {
                cmbSprite.SelectedIndex = cmbSprite.Items.IndexOf(_myCommand.Strs[0]);
            }
            else
            {
                cmbSprite.SelectedIndex = 0;
            }
            UpdatePreview();
        }

        private void InitLocalization()
        {
            grpChangeSprite.Text = Strings.Get("eventchangesprite", "title");
            lblSprite.Text = Strings.Get("eventchangesprite", "label");
            btnSave.Text = Strings.Get("eventchangesprite", "okay");
            btnCancel.Text = Strings.Get("eventchangesprite", "cancel");
        }

        private void UpdatePreview()
        {
            Bitmap destBitmap = new Bitmap(pnlPreview.Width, pnlPreview.Height);
            Graphics g = Graphics.FromImage(destBitmap);
            g.Clear(Color.Black);
            if (File.Exists("resources/entities/" + cmbSprite.Text))
            {
                Bitmap sourceBitmap = new Bitmap("resources/entities/" + cmbSprite.Text);
                g.DrawImage(sourceBitmap,
                    new Rectangle(pnlPreview.Width / 2 - sourceBitmap.Width / 8,
                        pnlPreview.Height / 2 - sourceBitmap.Height / 8, sourceBitmap.Width / 4, sourceBitmap.Height / 4),
                    new Rectangle(0, 0, sourceBitmap.Width / 4, sourceBitmap.Height / 4), GraphicsUnit.Pixel);
            }
            g.Dispose();
            pnlPreview.BackgroundImage = destBitmap;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Strs[0] = cmbSprite.Text;
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }
    }
}