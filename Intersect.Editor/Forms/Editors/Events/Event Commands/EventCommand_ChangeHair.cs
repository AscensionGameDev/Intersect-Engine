using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandChangeHair : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private ChangeHairCommand mMyCommand;

        public EventCommandChangeHair(ChangeHairCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            cmbHair.Items.Clear();
            cmbHair.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Hairs));
            if (cmbHair.Items.IndexOf(mMyCommand.Hair) > -1)
            {
                cmbHair.SelectedIndex = cmbHair.Items.IndexOf(mMyCommand.Hair);
            }
            else
            {
                cmbHair.SelectedIndex = 0;
            }

            UpdatePreview();
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpChangeHair.Text = Strings.EventChangeHair.title;
            lblHair.Text = Strings.EventChangeHair.label;
            btnSave.Text = Strings.EventChangeHair.okay;
            btnCancel.Text = Strings.EventChangeHair.cancel;
        }

        private void UpdatePreview()
        {
            var destBitmap = new Bitmap(pnlPreview.Width, pnlPreview.Height);
            var g = Graphics.FromImage(destBitmap);
            g.Clear(System.Drawing.Color.Black);
            if (File.Exists("resources/hairs/" + cmbHair.Text))
            {
                var sourceBitmap = new Bitmap("resources/hairs/" + cmbHair.Text);
                g.DrawImage(
                    sourceBitmap,
                    new Rectangle(
                        pnlPreview.Width / 2 - sourceBitmap.Width / 8, pnlPreview.Height / 2 - sourceBitmap.Height / 8,
                        sourceBitmap.Width / 4, sourceBitmap.Height / 4
                    ), new Rectangle(0, 0, sourceBitmap.Width / 4, sourceBitmap.Height / 4), GraphicsUnit.Pixel
                );
            }

            g.Dispose();
            pnlPreview.BackgroundImage = destBitmap;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mMyCommand.Hair = cmbHair.Text;
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
