using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Intersect.Editor.ContentManagement;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommandChangeSprite : UserControl
    {
        private readonly FrmEvent mEventEditor;
        private EventCommand mMyCommand;

        public EventCommandChangeSprite(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            InitLocalization();
            cmbSprite.Items.Clear();
            cmbSprite.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity));
            if (cmbSprite.Items.IndexOf(mMyCommand.Strs[0]) > -1)
            {
                cmbSprite.SelectedIndex = cmbSprite.Items.IndexOf(mMyCommand.Strs[0]);
            }
            else
            {
                cmbSprite.SelectedIndex = 0;
            }
            UpdatePreview();
        }

        private void InitLocalization()
        {
            grpChangeSprite.Text = Strings.EventChangeSprite.title;
            lblSprite.Text = Strings.EventChangeSprite.label;
            btnSave.Text = Strings.EventChangeSprite.okay;
            btnCancel.Text = Strings.EventChangeSprite.cancel;
        }

        private void UpdatePreview()
        {
            Bitmap destBitmap = new Bitmap(pnlPreview.Width, pnlPreview.Height);
            Graphics g = Graphics.FromImage(destBitmap);
            g.Clear(System.Drawing.Color.Black);
            if (File.Exists("resources/entities/" + cmbSprite.Text))
            {
                Bitmap sourceBitmap = new Bitmap("resources/entities/" + cmbSprite.Text);
                g.DrawImage(sourceBitmap,
                    new Rectangle(pnlPreview.Width / 2 - sourceBitmap.Width / 8,
                        pnlPreview.Height / 2 - sourceBitmap.Height / 8, sourceBitmap.Width / 4,
                        sourceBitmap.Height / 4),
                    new Rectangle(0, 0, sourceBitmap.Width / 4, sourceBitmap.Height / 4), GraphicsUnit.Pixel);
            }
            g.Dispose();
            pnlPreview.BackgroundImage = destBitmap;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Strs[0] = cmbSprite.Text;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }
    }
}