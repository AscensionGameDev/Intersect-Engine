using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandChangeFace : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private ChangeFaceCommand mMyCommand;

        public EventCommandChangeFace(ChangeFaceCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            cmbFace.Items.Clear();
            cmbFace.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Face));
            if (cmbFace.Items.IndexOf(mMyCommand.Face) > -1)
            {
                cmbFace.SelectedIndex = cmbFace.Items.IndexOf(mMyCommand.Face);
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
            grpChangeFace.Text = Strings.EventChangeFace.title;
            lblFace.Text = Strings.EventChangeFace.label;
            btnSave.Text = Strings.EventChangeFace.okay;
            btnCancel.Text = Strings.EventChangeFace.cancel;
        }

        private void UpdatePreview()
        {
            var destBitmap = new Bitmap(pnlPreview.Width, pnlPreview.Height);
            var g = Graphics.FromImage(destBitmap);
            g.Clear(System.Drawing.Color.Black);
            if (File.Exists("resources/faces/" + cmbFace.Text))
            {
                var sourceBitmap = new Bitmap("resources/faces/" + cmbFace.Text);
                g.DrawImage(
                    sourceBitmap,
                    new Rectangle(
                        pnlPreview.Width / 2 - sourceBitmap.Width / 2, pnlPreview.Height / 2 - sourceBitmap.Height / 2,
                        sourceBitmap.Width, sourceBitmap.Height
                    ), new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), GraphicsUnit.Pixel
                );
            }

            g.Dispose();
            pnlPreview.BackgroundImage = destBitmap;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Face = cmbFace.Text;
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
