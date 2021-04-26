using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events.Commands;
using Intersect.Utilities;

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
            // EntityTag.FileName
            cmbTag.Items.Clear();
            cmbTag.Items.Add(Strings.General.none);
            cmbTag.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Tag));
            cmbTag.SelectedIndex = cmbTag.FindString(TextUtils.NullToNone(mMyCommand.EntityTag.TagName));
            // EntityTag.TagPos
            cmbTagPos.Items.Clear();
            cmbTagPos.Items.AddRange(Enum.GetNames(typeof(TagPosition)));
            cmbTagPos.SelectedIndex = cmbTagPos.FindString(mMyCommand.EntityTag.TagPos.ToString());
            UpdatePreview();
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpChangeTag.Text = Strings.EventChangeTag.title;
            lblTag.Text = Strings.EventChangeTag.lblTag;
            lblTagPos.Text = Strings.EventChangeTag.lblTagPos;
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
            var selectedTag = TextUtils.SanitizeNone(cmbTag.Text);
            var selectedPos = (TagPosition) Enum.Parse(typeof(TagPosition), cmbTagPos.SelectedItem.ToString());
            mMyCommand.EntityTag = new Tag(selectedTag, selectedPos);

            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e) { mEventEditor.CancelCommandEdit(); }

        private void cmbTag_SelectedIndexChanged(object sender, EventArgs e) { UpdatePreview(); }
    }
}
