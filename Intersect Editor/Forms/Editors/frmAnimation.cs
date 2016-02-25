/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using Intersect_Editor.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SFML.Graphics;
using SFML.System;

namespace Intersect_Editor.Forms
{
    public partial class frmAnimation : Form
    {

        private ByteBuffer[] _animationsBackup;
        private bool[] _changed;
        private int _editorIndex;

        //SFML Rendering Variables
        private RenderWindow lowerWindow;
        private RenderWindow upperWindow;
        private RenderTexture lowerDarkness;
        private RenderTexture upperDarkness;

        private int _lowerFrame;
        private int _upperFrame;

        private bool _playLower;
        private bool _playUpper;

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
            for (int i = 0; i < Intersect_Editor.Classes.EditorGraphics.AnimationFileNames.Count; i++)
            {
                cmbLowerGraphic.Items.Add(Intersect_Editor.Classes.EditorGraphics.AnimationFileNames[i]);
            }

            //Upper Animation Graphic
            cmbUpperGraphic.Items.Clear();
            cmbUpperGraphic.Items.Add("None");
            for (int i = 0; i < Intersect_Editor.Classes.EditorGraphics.AnimationFileNames.Count; i++)
            {
                cmbUpperGraphic.Items.Add(Intersect_Editor.Classes.EditorGraphics.AnimationFileNames[i]);
            }

            lowerWindow = new RenderWindow(picLowerAnimation.Handle);
            upperWindow = new RenderWindow(picUpperAnimation.Handle);
            lowerDarkness = new RenderTexture((uint)picLowerAnimation.Width,(uint)picLowerAnimation.Height);
            upperDarkness = new RenderTexture((uint)picUpperAnimation.Width, (uint)picUpperAnimation.Height);

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

            scrlLowerHorizontalFrames.Value = Globals.GameAnimations[_editorIndex].LowerAnimXFrames;
            lblLowerHorizontalFrames.Text = "Graphic Horizontal Frames: " + scrlLowerHorizontalFrames.Value;

            scrlLowerVerticalFrames.Value = Globals.GameAnimations[_editorIndex].LowerAnimYFrames;
            lblLowerVerticalFrames.Text = "Graphic Vertical Frames: " + scrlLowerVerticalFrames.Value;

            scrlLowerFrameCount.Value = Globals.GameAnimations[_editorIndex].LowerAnimFrameCount;
            lblLowerFrameCount.Text = "Graphic Frame Count: " + scrlLowerFrameCount.Value;
            UpdateLowerFrames();

            scrlLowerFrameDuration.Value = Globals.GameAnimations[_editorIndex].LowerAnimFrameSpeed;
            lblLowerFrameDuration.Text = "Frame Duration (ms): " + scrlLowerFrameDuration.Value;
            tmrLowerAnimation.Interval = scrlLowerFrameDuration.Value;

            scrlLowerLoopCount.Value = Globals.GameAnimations[_editorIndex].LowerAnimLoopCount;
            lblLowerLoopCount.Text = "Loop Count: " + scrlLowerLoopCount.Value;

            cmbUpperGraphic.SelectedIndex = cmbUpperGraphic.FindString(Globals.GameAnimations[_editorIndex].UpperAnimSprite);

            scrlUpperHorizontalFrames.Value = Globals.GameAnimations[_editorIndex].UpperAnimXFrames;
            lblUpperHorizontalFrames.Text = "Graphic Horizontal Frames: " + scrlUpperHorizontalFrames.Value;

            scrlUpperVerticalFrames.Value = Globals.GameAnimations[_editorIndex].UpperAnimYFrames;
            lblUpperVerticalFrames.Text = "Graphic Vertical Frames: " + scrlUpperVerticalFrames.Value;

            scrlUpperFrameCount.Value = Globals.GameAnimations[_editorIndex].UpperAnimFrameCount;
            lblUpperFrameCount.Text = "Graphic Frame Count: " + scrlUpperFrameCount.Value;
            UpdateUpperFrames();

            scrlUpperFrameDuration.Value = Globals.GameAnimations[_editorIndex].UpperAnimFrameSpeed;
            lblUpperFrameDuration.Text = "Frame Duration (ms): " + scrlUpperFrameDuration.Value;
            tmrUpperAnimation.Interval = scrlUpperFrameDuration.Value;

            scrlUpperLoopCount.Value = Globals.GameAnimations[_editorIndex].UpperAnimLoopCount;
            lblUpperLoopCount.Text = "Loop Count: " + scrlUpperLoopCount.Value;

            LoadLowerLight();
            DrawLowerFrame();
            LoadUpperLight();
            DrawUpperFrame();

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
            UpdateLowerFrames();
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
            UpdateUpperFrames();
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
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var temp = new AnimationStruct();
            Globals.GameAnimations[_editorIndex].Load(temp.AnimData(), _editorIndex);
            UpdateEditor();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxAnimations; i++)
            {
                Globals.GameAnimations[i].Load(_animationsBackup[i].ToArray(),i);
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void tmrLowerAnimation_Tick(object sender, EventArgs e)
        {
            DrawLowerFrame();
            if (_playLower)
            {
                _lowerFrame++;
                if (_lowerFrame >= scrlLowerFrameCount.Value)
                {
                    _lowerFrame = 0;
                }
            }
        }

        void UpdateLowerFrames()
        {
            Light[] newArray;
            scrlLowerFrame.Maximum = scrlLowerFrameCount.Value;
            if (Globals.GameAnimations[_editorIndex].LowerAnimFrameCount !=
                Globals.GameAnimations[_editorIndex].LowerLights.Length)
            {
                newArray = new Light[Globals.GameAnimations[_editorIndex].LowerAnimFrameCount];
                for (int i = 0; i < newArray.Length; i++)
                {
                    if (i < Globals.GameAnimations[_editorIndex].LowerLights.Length)
                    {
                        newArray[i] = Globals.GameAnimations[_editorIndex].LowerLights[i];
                    }
                    else
                    {
                        newArray[i] = new Light(-1,-1);
                    }
                }
                Globals.GameAnimations[_editorIndex].LowerLights = newArray;
            }
        }

        void UpdateUpperFrames()
        {
            Light[] newArray;
            scrlUpperFrame.Maximum = scrlUpperFrameCount.Value;
            if (Globals.GameAnimations[_editorIndex].UpperAnimFrameCount !=
                Globals.GameAnimations[_editorIndex].UpperLights.Length)
            {
                newArray = new Light[Globals.GameAnimations[_editorIndex].UpperAnimFrameCount];
                for (int i = 0; i < newArray.Length; i++)
                {
                    if (i < Globals.GameAnimations[_editorIndex].UpperLights.Length)
                    {
                        newArray[i] = Globals.GameAnimations[_editorIndex].UpperLights[i];
                    }
                    else
                    {
                        newArray[i] = new Light(-1, -1);
                    }
                }
                Globals.GameAnimations[_editorIndex].UpperLights = newArray;
            }
        }

        void DrawLowerFrame()
        {
            if (!_playLower) _lowerFrame = scrlLowerFrame.Value - 1;
            lowerWindow.Clear(SFML.Graphics.Color.White);
            lowerDarkness.Clear(SFML.Graphics.Color.Black);
            if (Classes.EditorGraphics.AnimationFileNames.IndexOf(cmbLowerGraphic.Text) > -1)
            {
                Texture animTexture =
                    Classes.EditorGraphics.AnimationTextures[Classes.EditorGraphics.AnimationFileNames.IndexOf(cmbLowerGraphic.Text)
                        ];
                long w = animTexture.Size.X / scrlLowerHorizontalFrames.Value;
                long h = animTexture.Size.Y / scrlLowerVerticalFrames.Value;
                long x = 0;
                if (_lowerFrame > 0) { x = (_lowerFrame % scrlLowerHorizontalFrames.Value) * w; }
                long y = (int)Math.Floor(_lowerFrame / (double)scrlLowerHorizontalFrames.Value) * h;
                Sprite animSprite = new Sprite(animTexture,new IntRect((int)picLowerAnimation.Width / 2 - (int)w / 2, (int)picLowerAnimation.Height / 2 - (int)h / 2, (int)w, (int)h));
                animSprite.TextureRect = new IntRect((int)x, (int)y, (int)w, (int)h);
                lowerWindow.Draw(animSprite);
            }
            if (_lowerFrame < Globals.GameAnimations[_editorIndex].LowerLights.Length)
            {
                Classes.EditorGraphics.DrawLight(
                    picLowerAnimation.Width/2 - Globals.GameAnimations[_editorIndex].LowerLights[_lowerFrame].Size/2 +
                    Globals.GameAnimations[_editorIndex].LowerLights[_lowerFrame].OffsetX,
                    picLowerAnimation.Height/2 - Globals.GameAnimations[_editorIndex].LowerLights[_lowerFrame].Size/2 +
                    Globals.GameAnimations[_editorIndex].LowerLights[_lowerFrame].OffsetY,
                    Globals.GameAnimations[_editorIndex].LowerLights[_lowerFrame].Size,
                    Globals.GameAnimations[_editorIndex].LowerLights[_lowerFrame].Intensity,
                    Globals.GameAnimations[_editorIndex].LowerLights[_lowerFrame].Expand,
                    Globals.GameAnimations[_editorIndex].LowerLights[_lowerFrame].Color, lowerDarkness);
            }
            RectangleShape rs = new RectangleShape(new Vector2f(lowerDarkness.Size.X, lowerDarkness.Size.Y));
            rs.FillColor =
                new SFML.Graphics.Color(new SFML.Graphics.Color(255, 255, 255,
                    (byte) (((float) (100-scrlDarkness.Value)/100f)*255)));
            lowerDarkness.Draw(rs);
            lowerDarkness.Display();
            Sprite darknessSprite = new Sprite(lowerDarkness.Texture);
            lowerWindow.Draw(darknessSprite,new RenderStates(BlendMode.Multiply));
            lowerWindow.DispatchEvents();
            lowerWindow.Display();
        }

        void DrawUpperFrame()
        {
            if (!_playUpper) _upperFrame = scrlUpperFrame.Value - 1;
            upperWindow.Clear(SFML.Graphics.Color.White);
            upperDarkness.Clear(SFML.Graphics.Color.Black);
            if (Classes.EditorGraphics.AnimationFileNames.IndexOf(cmbUpperGraphic.Text) > -1)
            {
                Texture animTexture =
                    Classes.EditorGraphics.AnimationTextures[Classes.EditorGraphics.AnimationFileNames.IndexOf(cmbUpperGraphic.Text)
                        ];
                long w = animTexture.Size.X / scrlUpperHorizontalFrames.Value;
                long h = animTexture.Size.Y / scrlUpperVerticalFrames.Value;
                long x = 0;
                if (_upperFrame > 0) { x = (_upperFrame % scrlUpperHorizontalFrames.Value) * w; }
                long y = (int)Math.Floor(_upperFrame / (double)scrlUpperHorizontalFrames.Value) * h;
                Sprite animSprite = new Sprite(animTexture, new IntRect((int)picUpperAnimation.Width / 2 - (int)w / 2, (int)picUpperAnimation.Height / 2 - (int)h / 2, (int)w, (int)h));
                animSprite.TextureRect = new IntRect((int)x, (int)y, (int)w, (int)h);
                upperWindow.Draw(animSprite);
            }
            if (_upperFrame < Globals.GameAnimations[_editorIndex].UpperLights.Length)
            {
                Classes.EditorGraphics.DrawLight(
                    picUpperAnimation.Width / 2 - Globals.GameAnimations[_editorIndex].UpperLights[_upperFrame].Size / 2 +
                    Globals.GameAnimations[_editorIndex].UpperLights[_upperFrame].OffsetX,
                    picUpperAnimation.Height / 2 - Globals.GameAnimations[_editorIndex].UpperLights[_upperFrame].Size / 2 +
                    Globals.GameAnimations[_editorIndex].UpperLights[_upperFrame].OffsetY,
                    Globals.GameAnimations[_editorIndex].UpperLights[_upperFrame].Size,
                    Globals.GameAnimations[_editorIndex].UpperLights[_upperFrame].Intensity,
                    Globals.GameAnimations[_editorIndex].UpperLights[_upperFrame].Expand,
                    Globals.GameAnimations[_editorIndex].UpperLights[_upperFrame].Color, upperDarkness);
            }
            RectangleShape rs = new RectangleShape(new Vector2f(upperDarkness.Size.X, upperDarkness.Size.Y));
            rs.FillColor =
                new SFML.Graphics.Color(new SFML.Graphics.Color(255, 255, 255,
                    (byte)(((float)(100 - scrlDarkness.Value) / 100f) * 255)));
            upperDarkness.Draw(rs);
            upperDarkness.Display();
            Sprite darknessSprite = new Sprite(upperDarkness.Texture);
            upperWindow.Draw(darknessSprite, new RenderStates(BlendMode.Multiply));
            upperWindow.DispatchEvents();
            upperWindow.Display();
        }

        private void tmrUpperAnimation_Tick(object sender, EventArgs e)
        {
            DrawUpperFrame();
            if (_playUpper)
            {
                _upperFrame++;
                if (_upperFrame >= scrlUpperFrameCount.Value)
                {
                    _upperFrame = 0;
                }
            }
        }

        private void frmAnimation_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void scrlLowerFrame_Scroll(object sender, ScrollEventArgs e)
        {
            lblLowerFrame.Text = @"Frame: " + scrlLowerFrame.Value;
            LoadLowerLight();
            DrawLowerFrame();
        }

        private void LoadLowerLight()
        {
            lightEditorLower.CanClose = false;
            lightEditorLower.LoadEditor(Globals.GameAnimations[_editorIndex].LowerLights[scrlLowerFrame.Value - 1]);
        }

        private void LoadUpperLight()
        {
            lightEditorUpper.CanClose = false;
            lightEditorUpper.LoadEditor(Globals.GameAnimations[_editorIndex].UpperLights[scrlUpperFrame.Value - 1]);
        }

        private void scrlUpperFrame_Scroll(object sender, ScrollEventArgs e)
        {
            lblUpperFrame.Text = @"Frame: " + scrlUpperFrame.Value;
            LoadUpperLight();
            DrawUpperFrame();
        }

        private void btnPlayLower_Click(object sender, EventArgs e)
        {
            _playLower = !_playLower;
            if (_playLower)
            {
                btnPlayLower.Text = "Stop Lower Animation";
            }
            else
            {
                btnPlayLower.Text = "Play Lower Animation";
            }
        }

        private void btnLowerClone_Click(object sender, EventArgs e)
        {
            if (scrlLowerFrame.Value > 1)
            {
                Globals.GameAnimations[_editorIndex].LowerLights[scrlLowerFrame.Value-1] = new Light(Globals.GameAnimations[_editorIndex].LowerLights[scrlLowerFrame.Value - 2]);
                LoadLowerLight();
                DrawLowerFrame();
            }
        }

        private void btnPlayUpper_Click(object sender, EventArgs e)
        {
            _playUpper = !_playUpper;
            if (_playUpper)
            {
                btnPlayUpper.Text = "Stop Upper Animation";
            }
            else
            {
                btnPlayUpper.Text = "Play Upper Animation";
            }
        }

        private void btnUpperClone_Click(object sender, EventArgs e)
        {
            if (scrlUpperFrame.Value > 1)
            {
                Globals.GameAnimations[_editorIndex].UpperLights[scrlUpperFrame.Value - 1] = new Light(Globals.GameAnimations[_editorIndex].UpperLights[scrlUpperFrame.Value - 2]);
                LoadUpperLight();
                DrawUpperFrame();
            }
        }

        private void scrlDarkness_Scroll(object sender, ScrollEventArgs e)
        {
            labelDarkness.Text = "Simulate Darkness: " + scrlDarkness.Value;
        }
    }
}
