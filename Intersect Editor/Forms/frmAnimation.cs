using Intersect_Editor.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Intersect_Editor.Forms
{
    public partial class frmAnimation : Form
    {

        private ByteBuffer[] _animationsBackup;
        private bool[] _changed;
        private int _editorIndex;
        private Bitmap _lowerAnimImage;
        private Bitmap _upperAnimImage;
        private int _lowerFrame = 0;
        private int _upperFrame = 0;

        public frmAnimation()
        {
            InitializeComponent();
        }

        private void frmAnimation_Load(object sender, EventArgs e)
        {
            lstAnimations.SelectedIndex = 0;

            //Animation Sound
            cmbSound.Items.Clear();
            cmbSound.Items.Add("None");
            for (int i = 0; i < Intersect_Editor.Classes.Audio.SoundFileNames.Length; i++)
            {
                cmbSound.Items.Add(Intersect_Editor.Classes.Audio.SoundFileNames[i]);
            }

            //Lower Animation Graphic
            cmbLowerGraphic.Items.Clear();
            cmbLowerGraphic.Items.Add("None");
            for (int i = 0; i < Intersect_Editor.Classes.Graphics.AnimationFileNames.Length; i++)
            {
                cmbLowerGraphic.Items.Add(Intersect_Editor.Classes.Graphics.AnimationFileNames[i]);
            }

            //Upper Animation Graphic
            cmbUpperGraphic.Items.Clear();
            cmbUpperGraphic.Items.Add("None");
            for (int i = 0; i < Intersect_Editor.Classes.Graphics.AnimationFileNames.Length; i++)
            {
                cmbUpperGraphic.Items.Add(Intersect_Editor.Classes.Graphics.AnimationFileNames[i]);
            }

            UpdateEditor();
        }

        public void InitEditor()
        {
            _animationsBackup = new ByteBuffer[Constants.MaxAnimations];
            _changed = new bool[Constants.MaxAnimations];
            for (var i = 0; i < Constants.MaxAnimations; i++)
            {
                _animationsBackup[i] = new ByteBuffer();
                _animationsBackup[i].WriteBytes(Globals.GameAnimations[i].AnimData());
                lstAnimations.Items.Add((i + 1) + ") " + Globals.GameAnimations[i].Name);
                _changed[i] = false;
            }
        }

        private void UpdateEditor()
        {
            _editorIndex = lstAnimations.SelectedIndex;

            txtName.Text = Globals.GameAnimations[_editorIndex].Name;
            cmbSound.SelectedIndex = cmbSound.FindString(Globals.GameAnimations[_editorIndex].Sound);

            cmbLowerGraphic.SelectedIndex = cmbLowerGraphic.FindString(Globals.GameAnimations[_editorIndex].LowerAnimSprite);
            if (cmbLowerGraphic.SelectedIndex > 0) { _lowerAnimImage = (Bitmap)Bitmap.FromFile("Resources/Animations/" + cmbLowerGraphic.Text); }
            else { _lowerAnimImage = null; }

            scrlLowerHorizontalFrames.Value = Globals.GameAnimations[_editorIndex].LowerAnimXFrames;
            lblLowerHorizontalFrames.Text = "Graphic Horizontal Frames: " + scrlLowerHorizontalFrames.Value;

            scrlLowerVerticalFrames.Value = Globals.GameAnimations[_editorIndex].LowerAnimYFrames;
            lblLowerVerticalFrames.Text = "Graphic Vertical Frames: " + scrlLowerVerticalFrames.Value;

            scrlLowerFrameCount.Value = Globals.GameAnimations[_editorIndex].LowerAnimFrameCount;
            lblLowerFrameCount.Text = "Graphic Frame Count: " + scrlLowerFrameCount.Value;

            scrlLowerFrameDuration.Value = Globals.GameAnimations[_editorIndex].LowerAnimFrameSpeed;
            lblLowerFrameDuration.Text = "Frame Duration (ms): " + scrlLowerFrameDuration.Value;
            tmrLowerAnimation.Interval = scrlLowerFrameDuration.Value;

            scrlLowerLoopCount.Value = Globals.GameAnimations[_editorIndex].LowerAnimLoopCount;
            lblLowerLoopCount.Text = "Loop Count: " + scrlLowerLoopCount.Value;

            cmbUpperGraphic.SelectedIndex = cmbUpperGraphic.FindString(Globals.GameAnimations[_editorIndex].UpperAnimSprite);
            if (cmbUpperGraphic.SelectedIndex > 0) { _upperAnimImage = (Bitmap)Bitmap.FromFile("Resources/Animations/" + cmbUpperGraphic.Text); }
            else { _upperAnimImage = null; }

            scrlUpperHorizontalFrames.Value = Globals.GameAnimations[_editorIndex].UpperAnimXFrames;
            lblUpperHorizontalFrames.Text = "Graphic Horizontal Frames: " + scrlUpperHorizontalFrames.Value;

            scrlUpperVerticalFrames.Value = Globals.GameAnimations[_editorIndex].UpperAnimYFrames;
            lblUpperVerticalFrames.Text = "Graphic Vertical Frames: " + scrlUpperVerticalFrames.Value;

            scrlUpperFrameCount.Value = Globals.GameAnimations[_editorIndex].UpperAnimFrameCount;
            lblUpperFrameCount.Text = "Graphic Frame Count: " + scrlUpperFrameCount.Value;

            scrlUpperFrameDuration.Value = Globals.GameAnimations[_editorIndex].UpperAnimFrameSpeed;
            lblUpperFrameDuration.Text = "Frame Duration (ms): " + scrlUpperFrameDuration.Value;
            tmrUpperAnimation.Interval = scrlUpperFrameDuration.Value;

            scrlUpperLoopCount.Value = Globals.GameAnimations[_editorIndex].UpperAnimLoopCount;
            lblUpperLoopCount.Text = "Loop Count: " + scrlUpperLoopCount.Value;

            _changed[_editorIndex] = true;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            Globals.GameAnimations[_editorIndex].Name = txtName.Text;
            lstAnimations.Items[_editorIndex] = (_editorIndex + 1) + ") " + txtName.Text;
        }

        private void cmbSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameAnimations[_editorIndex].Sound = cmbSound.Text;
        }

        private void cmbLowerGraphic_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameAnimations[_editorIndex].LowerAnimSprite = cmbLowerGraphic.Text;
            if (cmbLowerGraphic.SelectedIndex > 0) { _lowerAnimImage = (Bitmap)Bitmap.FromFile("Resources/Animations/" + cmbLowerGraphic.Text); }
            else { _lowerAnimImage = null; }
        }

        private void scrlLowerHorizontalFrames_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameAnimations[_editorIndex].LowerAnimXFrames = scrlLowerHorizontalFrames.Value;
            lblLowerHorizontalFrames.Text = "Graphic Horizontal Frames: " + scrlLowerHorizontalFrames.Value;
        }

        private void scrlLowerVerticalFrames_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameAnimations[_editorIndex].LowerAnimYFrames = scrlLowerVerticalFrames.Value;
            lblLowerVerticalFrames.Text = "Graphic Vertical Frames: " + scrlLowerVerticalFrames.Value;
        }

        private void scrlLowerFrameCount_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameAnimations[_editorIndex].LowerAnimFrameCount = scrlLowerFrameCount.Value;
            lblLowerFrameCount.Text = "Graphic Frame Count: " + scrlLowerFrameCount.Value;
        }

        private void scrlLowerFrameDuration_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameAnimations[_editorIndex].LowerAnimFrameSpeed = scrlLowerFrameDuration.Value;
            lblLowerFrameDuration.Text = "Frame Duration (ms): " + scrlLowerFrameDuration.Value;
            tmrLowerAnimation.Interval = scrlLowerFrameDuration.Value;
        }

        private void scrlLowerLoopCount_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameAnimations[_editorIndex].LowerAnimLoopCount = scrlLowerLoopCount.Value;
            lblLowerLoopCount.Text = "Loop Count: " + scrlLowerLoopCount.Value;
        }

        private void cmbUpperGraphic_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameAnimations[_editorIndex].UpperAnimSprite = cmbUpperGraphic.Text;
            if (cmbUpperGraphic.SelectedIndex > 0) { _upperAnimImage = (Bitmap)Bitmap.FromFile("Resources/Animations/" + cmbUpperGraphic.Text); }
            else { _upperAnimImage = null; }
        }

        private void scrlUpperHorizontalFrames_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameAnimations[_editorIndex].UpperAnimXFrames = scrlUpperHorizontalFrames.Value;
            lblUpperHorizontalFrames.Text = "Graphic Horizontal Frames: " + scrlUpperHorizontalFrames.Value;
        }

        private void scrlUpperVerticalFrames_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameAnimations[_editorIndex].UpperAnimYFrames = scrlUpperVerticalFrames.Value;
            lblUpperVerticalFrames.Text = "Graphic Vertical Frames: " + scrlUpperVerticalFrames.Value;
        }

        private void scrlUpperFrameCount_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameAnimations[_editorIndex].UpperAnimFrameCount = scrlUpperFrameCount.Value;
            lblUpperFrameCount.Text = "Graphic Frame Count: " + scrlUpperFrameCount.Value;
        }

        private void scrlUpperFrameDuration_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameAnimations[_editorIndex].UpperAnimFrameSpeed = scrlUpperFrameDuration.Value;
            lblUpperFrameDuration.Text = "Frame Duration (ms): " + scrlUpperFrameDuration.Value;
            tmrUpperAnimation.Interval = scrlUpperFrameDuration.Value;
        }

        private void scrlUpperLoopCount_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameAnimations[_editorIndex].UpperAnimLoopCount = scrlUpperLoopCount.Value;
            lblUpperLoopCount.Text = "Loop Count: " + scrlUpperLoopCount.Value;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxAnimations; i++)
            {
                if (_changed[i])
                {
                    PacketSender.SendAnimation(i, Globals.GameAnimations[i].AnimData());
                }
            }

            Hide();
            Dispose();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var temp = new AnimationStruct();
            Globals.GameAnimations[_editorIndex].Load(temp.AnimData());
            UpdateEditor();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxAnimations; i++)
            {
                Globals.GameAnimations[i].Load(_animationsBackup[i].ToArray());
            }

            Hide();
            Dispose();
        }

        private void tmrLowerAnimation_Tick(object sender, EventArgs e)
        {
            var gfx = picLowerAnimation.CreateGraphics();
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picLowerAnimation.Width, picLowerAnimation.Height));
            if (_lowerAnimImage != null)
            {
                int w = _lowerAnimImage.Width / scrlLowerHorizontalFrames.Value;
                int h = _lowerAnimImage.Height / scrlLowerVerticalFrames.Value;
                int x = 0;
                if (_lowerFrame > 0) { x = (_lowerFrame % scrlLowerHorizontalFrames.Value) * w; }
                int y = (int)Math.Floor(_lowerFrame / (double)scrlLowerHorizontalFrames.Value) * h;
                gfx.DrawImage(_lowerAnimImage, new Rectangle(picLowerAnimation.Width / 2 - w / 2, picLowerAnimation.Height / 2 - h / 2, w, h), new Rectangle(x, y, w, h), GraphicsUnit.Pixel);
            }
            _lowerFrame++;
            if (_lowerFrame >= scrlLowerFrameCount.Value) { _lowerFrame = 0; }
            gfx.Dispose();
        }

        private void tmrUpperAnimation_Tick(object sender, EventArgs e)
        {
            var gfx = picUpperAnimation.CreateGraphics();
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picUpperAnimation.Width, picUpperAnimation.Height));
            if (_upperAnimImage != null)
            {
                int w = _upperAnimImage.Width / scrlUpperHorizontalFrames.Value;
                int h = _upperAnimImage.Height / scrlUpperVerticalFrames.Value;
                int x = 0;
                if (_upperFrame > 0) { x = (_upperFrame % scrlUpperHorizontalFrames.Value) * w; }
                int y = (int)Math.Floor(_upperFrame / (double)scrlUpperHorizontalFrames.Value) * h;
                gfx.DrawImage(_upperAnimImage, new Rectangle(picUpperAnimation.Width /2 - w/2, picUpperAnimation.Height/2 - h/2, w, h), new Rectangle(x, y, w, h), GraphicsUnit.Pixel);
            }
            _upperFrame++;
            if (_upperFrame >= scrlUpperFrameCount.Value) { _upperFrame = 0; }
            gfx.Dispose();
        }
    }
}
