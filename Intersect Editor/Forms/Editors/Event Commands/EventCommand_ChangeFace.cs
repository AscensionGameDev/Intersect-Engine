
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Intersect_Editor.Classes.Core;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_ChangeFace : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_ChangeFace(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            cmbFace.Items.Clear();
            cmbFace.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Face));
            if (cmbFace.Items.IndexOf(_myCommand.Strs[0]) > -1)
            {
                cmbFace.SelectedIndex = cmbFace.Items.IndexOf(_myCommand.Strs[0]);
            }
            else
            {
                cmbFace.SelectedIndex = 0;
            }
            UpdatePreview();
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpChangeFace.Text = Strings.Get("eventchangeface", "title");
            lblFace.Text = Strings.Get("eventchangeface", "label");
            btnSave.Text = Strings.Get("eventchangeface", "okay");
            btnCancel.Text = Strings.Get("eventchangeface", "cancel");
        }

        private void UpdatePreview()
        {
            Bitmap destBitmap = new Bitmap(pnlPreview.Width, pnlPreview.Height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(destBitmap);
            g.Clear(Color.Black);
            if (File.Exists("resources/faces/" + cmbFace.Text))
            {
                Bitmap sourceBitmap = new Bitmap("resources/faces/" + cmbFace.Text);
                g.DrawImage(sourceBitmap,new Rectangle(pnlPreview.Width/2 - sourceBitmap.Width/2,pnlPreview.Height/2 - sourceBitmap.Height/2,sourceBitmap.Width,sourceBitmap.Height), new Rectangle(0,0,sourceBitmap.Width,sourceBitmap.Height),GraphicsUnit.Pixel);
            }
            g.Dispose();
            pnlPreview.BackgroundImage = destBitmap;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _myCommand.Strs[0] = cmbFace.Text;
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
