using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandChangeTag : UserControl
    {

        private readonly FrmEvent mEventEditor;

        private ChangeTagCommand mMyCommand;

        public EventCommandChangeTag(ChangeTagCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            mMyCommand = refCommand;
            mEventEditor = editor;
            cmbTag.Items.Clear();
            cmbTag.Items.Add(Strings.General.none);
            cmbTag.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Tag));
            if (cmbTag.Items.IndexOf(mMyCommand.Tag) > -1)
            {
                cmbTag.SelectedIndex = cmbTag.Items.IndexOf(mMyCommand.Tag);
            }
            else
            {
                cmbTag.SelectedIndex = 0;
            }

            UpdatePreview();
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpChangeTag.Text = Strings.EventChangeTag.title;
            lblTag.Text = Strings.EventChangeTag.label;
            btnSave.Text = Strings.EventChangeTag.okay;
            btnCancel.Text = Strings.EventChangeTag.cancel;
        }

        private void UpdatePreview()
        {
            var destBitmap = new Bitmap(pnlPreview.Width, pnlPreview.Height);
            var g = Graphics.FromImage(destBitmap);
            g.Clear(System.Drawing.Color.Black);
            if (File.Exists("resources/tags/" + cmbTag.Text))
            {
                var sourceBitmap = new Bitmap("resources/tags/" + cmbTag.Text);
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
            mMyCommand.Tag = cmbTag.Text;
            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

        private void cmbTag_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

    }

}
