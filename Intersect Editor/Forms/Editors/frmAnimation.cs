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
using System.Drawing;
using System.Windows.Forms;
using DarkUI.Controls;
using Intersect_Editor.Classes.Core;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using Color = System.Drawing.Color;


namespace Intersect_Editor.Forms
{
    public partial class frmAnimation : Form
    {
        private List<AnimationBase> _changed = new List<AnimationBase>();
        private AnimationBase _editorItem = null;
        private byte[] _copiedItem = null;

        //Mono Rendering Variables
        private SwapChainRenderTarget lowerWindow;
        private SwapChainRenderTarget upperWindow;
        private RenderTarget2D lowerDarkness;
        private RenderTarget2D upperDarkness;

        private int _lowerFrame;
        private int _upperFrame;

        private bool _playLower;
        private bool _playUpper;

        public frmAnimation()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
            lstAnimations.LostFocus += itemList_FocusChanged;
            lstAnimations.GotFocus += itemList_FocusChanged;
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.Animation)
            {
                InitEditor();
                if (_editorItem != null && !AnimationBase.GetObjects().ContainsValue(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (var item in _changed)
            {
                item.RestoreBackup();
                item.DeleteBackup();
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Send Changed items
            foreach (var item in _changed)
            {
                PacketSender.SendSaveObject(item);
                item.DeleteBackup();
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void lstAnimations_Click(object sender, EventArgs e)
        {
            _editorItem = AnimationBase.GetAnim(Database.GameObjectIdFromList(GameObject.Animation, lstAnimations.SelectedIndex));
            UpdateEditor();
        }

        private void frmAnimation_Load(object sender, EventArgs e)
        {
            //Animation Sound
            cmbSound.Items.Clear();
            cmbSound.Items.Add("None");
            cmbSound.Items.AddRange(GameContentManager.GetSoundNames());

            //Lower Animation Graphic
            cmbLowerGraphic.Items.Clear();
            cmbLowerGraphic.Items.Add("None");
            cmbLowerGraphic.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Animation));

            //Upper Animation Graphic
            cmbUpperGraphic.Items.Clear();
            cmbUpperGraphic.Items.Add("None");
            cmbUpperGraphic.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Animation));

            lowerWindow = new SwapChainRenderTarget(EditorGraphics.GetGraphicsDevice(),picLowerAnimation.Handle, picLowerAnimation.Width, picLowerAnimation.Height);
            upperWindow = new SwapChainRenderTarget(EditorGraphics.GetGraphicsDevice(),picUpperAnimation.Handle, picUpperAnimation.Width, picUpperAnimation.Height);
            lowerDarkness = EditorGraphics.CreateRenderTexture(picLowerAnimation.Width,picLowerAnimation.Height);
            upperDarkness = EditorGraphics.CreateRenderTexture(picUpperAnimation.Width, picUpperAnimation.Height);

            UpdateEditor();
        }

        public void InitEditor()
        {
            lstAnimations.Items.Clear();
            lstAnimations.Items.AddRange(Database.GetGameObjectList(GameObject.Animation));
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                cmbSound.SelectedIndex = cmbSound.FindString(_editorItem.Sound);

                cmbLowerGraphic.SelectedIndex =
                    cmbLowerGraphic.FindString(_editorItem.LowerAnimSprite);

                scrlLowerHorizontalFrames.Value = _editorItem.LowerAnimXFrames;
                lblLowerHorizontalFrames.Text = "Graphic Horizontal Frames: " + scrlLowerHorizontalFrames.Value;

                scrlLowerVerticalFrames.Value = _editorItem.LowerAnimYFrames;
                lblLowerVerticalFrames.Text = "Graphic Vertical Frames: " + scrlLowerVerticalFrames.Value;

                scrlLowerFrameCount.Value = _editorItem.LowerAnimFrameCount;
                lblLowerFrameCount.Text = "Graphic Frame Count: " + scrlLowerFrameCount.Value;
                UpdateLowerFrames();

                scrlLowerFrameDuration.Value = _editorItem.LowerAnimFrameSpeed;
                lblLowerFrameDuration.Text = "Frame Duration (ms): " + scrlLowerFrameDuration.Value;
                tmrLowerAnimation.Interval = scrlLowerFrameDuration.Value;

                scrlLowerLoopCount.Value = _editorItem.LowerAnimLoopCount;
                lblLowerLoopCount.Text = "Loop Count: " + scrlLowerLoopCount.Value;

                cmbUpperGraphic.SelectedIndex =
                    cmbUpperGraphic.FindString(_editorItem.UpperAnimSprite);

                scrlUpperHorizontalFrames.Value = _editorItem.UpperAnimXFrames;
                lblUpperHorizontalFrames.Text = "Graphic Horizontal Frames: " + scrlUpperHorizontalFrames.Value;

                scrlUpperVerticalFrames.Value = _editorItem.UpperAnimYFrames;
                lblUpperVerticalFrames.Text = "Graphic Vertical Frames: " + scrlUpperVerticalFrames.Value;

                scrlUpperFrameCount.Value = _editorItem.UpperAnimFrameCount;
                lblUpperFrameCount.Text = "Graphic Frame Count: " + scrlUpperFrameCount.Value;
                UpdateUpperFrames();

                scrlUpperFrameDuration.Value = _editorItem.UpperAnimFrameSpeed;
                lblUpperFrameDuration.Text = "Frame Duration (ms): " + scrlUpperFrameDuration.Value;
                tmrUpperAnimation.Interval = scrlUpperFrameDuration.Value;

                scrlUpperLoopCount.Value = _editorItem.UpperAnimLoopCount;
                lblUpperLoopCount.Text = "Loop Count: " + scrlUpperLoopCount.Value;

                LoadLowerLight();
                DrawLowerFrame();
                LoadUpperLight();
                DrawUpperFrame();

                if (_changed.IndexOf(_editorItem) == -1)
                {
                    _changed.Add(_editorItem);
                    _editorItem.MakeBackup();
                }
            }
            else
            {
                pnlContainer.Hide();
            }
            UpdateToolStripItems();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _editorItem.Name = txtName.Text;
            lstAnimations.Items[Database.GameObjectListIndex(GameObject.Animation,_editorItem.GetId())] = txtName.Text;
        }

        private void cmbSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Sound = cmbSound.Text;
        }

        private void cmbLowerGraphic_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.LowerAnimSprite = cmbLowerGraphic.Text;
        }

        private void scrlLowerHorizontalFrames_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.LowerAnimXFrames = scrlLowerHorizontalFrames.Value;
            lblLowerHorizontalFrames.Text = "Graphic Horizontal Frames: " + scrlLowerHorizontalFrames.Value;
        }

        private void scrlLowerVerticalFrames_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.LowerAnimYFrames = scrlLowerVerticalFrames.Value;
            lblLowerVerticalFrames.Text = "Graphic Vertical Frames: " + scrlLowerVerticalFrames.Value;
        }

        private void scrlLowerFrameCount_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.LowerAnimFrameCount = scrlLowerFrameCount.Value;
            lblLowerFrameCount.Text = "Graphic Frame Count: " + scrlLowerFrameCount.Value;
            UpdateLowerFrames();
        }

        private void scrlLowerFrameDuration_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.LowerAnimFrameSpeed = scrlLowerFrameDuration.Value;
            lblLowerFrameDuration.Text = "Frame Duration (ms): " + scrlLowerFrameDuration.Value;
            tmrLowerAnimation.Interval = scrlLowerFrameDuration.Value;
        }

        private void scrlLowerLoopCount_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.LowerAnimLoopCount = scrlLowerLoopCount.Value;
            lblLowerLoopCount.Text = "Loop Count: " + scrlLowerLoopCount.Value;
        }

        private void cmbUpperGraphic_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.UpperAnimSprite = cmbUpperGraphic.Text;
        }

        private void scrlUpperHorizontalFrames_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.UpperAnimXFrames = scrlUpperHorizontalFrames.Value;
            lblUpperHorizontalFrames.Text = "Graphic Horizontal Frames: " + scrlUpperHorizontalFrames.Value;
        }

        private void scrlUpperVerticalFrames_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.UpperAnimYFrames = scrlUpperVerticalFrames.Value;
            lblUpperVerticalFrames.Text = "Graphic Vertical Frames: " + scrlUpperVerticalFrames.Value;
        }

        private void scrlUpperFrameCount_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.UpperAnimFrameCount = scrlUpperFrameCount.Value;
            lblUpperFrameCount.Text = "Graphic Frame Count: " + scrlUpperFrameCount.Value;
            UpdateUpperFrames();
        }

        private void scrlUpperFrameDuration_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.UpperAnimFrameSpeed = scrlUpperFrameDuration.Value;
            lblUpperFrameDuration.Text = "Frame Duration (ms): " + scrlUpperFrameDuration.Value;
            tmrUpperAnimation.Interval = scrlUpperFrameDuration.Value;
        }

        private void scrlUpperLoopCount_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.UpperAnimLoopCount = scrlUpperLoopCount.Value;
            lblUpperLoopCount.Text = "Loop Count: " + scrlUpperLoopCount.Value;
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
            LightBase[] newArray;
            scrlLowerFrame.Maximum = scrlLowerFrameCount.Value;
            if (_editorItem.LowerAnimFrameCount !=
                _editorItem.LowerLights.Length)
            {
                newArray = new LightBase[_editorItem.LowerAnimFrameCount];
                for (int i = 0; i < newArray.Length; i++)
                {
                    if (i < _editorItem.LowerLights.Length)
                    {
                        newArray[i] = _editorItem.LowerLights[i];
                    }
                    else
                    {
                        newArray[i] = new LightBase(-1, -1);
                    }
                }
                _editorItem.LowerLights = newArray;
            }
        }

        void UpdateUpperFrames()
        {
            LightBase[] newArray;
            scrlUpperFrame.Maximum = scrlUpperFrameCount.Value;
            if (_editorItem.UpperAnimFrameCount !=
                _editorItem.UpperLights.Length)
            {
                newArray = new LightBase[_editorItem.UpperAnimFrameCount];
                for (int i = 0; i < newArray.Length; i++)
                {
                    if (i < _editorItem.UpperLights.Length)
                    {
                        newArray[i] = _editorItem.UpperLights[i];
                    }
                    else
                    {
                        newArray[i] = new LightBase(-1, -1);
                    }
                }
                _editorItem.UpperLights = newArray;
            }
        }

        void DrawLowerFrame()
        {
            if (lowerWindow == null || _editorItem == null) return;
            if (!_playLower) _lowerFrame = scrlLowerFrame.Value - 1;
            GraphicsDevice graphicsDevice = EditorGraphics.GetGraphicsDevice();

            graphicsDevice.SetRenderTarget(lowerDarkness);
            graphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            if (_lowerFrame < _editorItem.LowerLights.Length)
            {
                Classes.EditorGraphics.DrawLight(
                    picLowerAnimation.Width / 2 +
                    _editorItem.LowerLights[_lowerFrame].OffsetX,
                    picLowerAnimation.Height / 2 +
                    _editorItem.LowerLights[_lowerFrame].OffsetY,
                    _editorItem.LowerLights[_lowerFrame], lowerDarkness);
            }
            EditorGraphics.DrawTexture(EditorGraphics.GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                new RectangleF(0, 0, lowerDarkness.Width, lowerDarkness.Height),
                Color.FromArgb((byte) (((float) (100 - scrlDarkness.Value)/100f)*255), 255, 255, 255), lowerDarkness,
                BlendState.Additive);


            graphicsDevice.SetRenderTarget(lowerWindow);
            graphicsDevice.Clear(Microsoft.Xna.Framework.Color.White);
            Texture2D animTexture = GameContentManager.GetTexture(GameContentManager.TextureType.Animation,
                cmbLowerGraphic.Text);
            if (animTexture != null)
            {
                long w = animTexture.Width/scrlLowerHorizontalFrames.Value;
                long h = animTexture.Height/scrlLowerVerticalFrames.Value;
                long x = 0;
                if (_lowerFrame > 0)
                {
                    x = (_lowerFrame%scrlLowerHorizontalFrames.Value)*w;
                }
                long y = (int) Math.Floor(_lowerFrame/(double) scrlLowerHorizontalFrames.Value)*h;
                EditorGraphics.DrawTexture(animTexture, new RectangleF(x, y, w, h),
                    new RectangleF(picLowerAnimation.Width/2 - (int) w/2,
                        (int) picLowerAnimation.Height/2 - (int) h/2, w, h), lowerWindow);
            }
            EditorGraphics.DrawTexture(lowerDarkness, 0, 0, lowerWindow, EditorGraphics.MultiplyState);
            lowerWindow.Present();
        }

        void DrawUpperFrame()
        {
            if (upperWindow == null || _editorItem == null) return;
            if (!_playUpper) _upperFrame = scrlUpperFrame.Value - 1;
            GraphicsDevice graphicsDevice = EditorGraphics.GetGraphicsDevice();

            graphicsDevice.SetRenderTarget(upperDarkness);
            graphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            if (_upperFrame < _editorItem.UpperLights.Length)
            {
                Classes.EditorGraphics.DrawLight(
                    picUpperAnimation.Width / 2 +
                    _editorItem.UpperLights[_upperFrame].OffsetX,
                    picUpperAnimation.Height / 2 +
                    _editorItem.UpperLights[_upperFrame].OffsetY,
                    _editorItem.UpperLights[_upperFrame], upperDarkness);
            }
            EditorGraphics.DrawTexture(EditorGraphics.GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                new RectangleF(0, 0, upperDarkness.Width, upperDarkness.Height),
                Color.FromArgb((byte)(((float)(100 - scrlDarkness.Value) / 100f) * 255), 255, 255, 255), upperDarkness,
                BlendState.Additive);


            graphicsDevice.SetRenderTarget(upperWindow);
            graphicsDevice.Clear(Microsoft.Xna.Framework.Color.White);
            Texture2D animTexture = GameContentManager.GetTexture(GameContentManager.TextureType.Animation,
                cmbUpperGraphic.Text);
            if (animTexture != null)
            {
                long w = animTexture.Width / scrlUpperHorizontalFrames.Value;
                long h = animTexture.Height / scrlUpperVerticalFrames.Value;
                long x = 0;
                if (_upperFrame > 0)
                {
                    x = (_upperFrame % scrlUpperHorizontalFrames.Value) * w;
                }
                long y = (int)Math.Floor(_upperFrame / (double)scrlUpperHorizontalFrames.Value) * h;
                EditorGraphics.DrawTexture(animTexture, new RectangleF(x, y, w, h),
                    new RectangleF(picUpperAnimation.Width / 2 - (int)w / 2,
                        (int)picUpperAnimation.Height / 2 - (int)h / 2, w, h), upperWindow);
            }
            EditorGraphics.DrawTexture(upperDarkness, 0, 0, upperWindow, EditorGraphics.MultiplyState);
            upperWindow.Present();
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

        private void scrlLowerFrame_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblLowerFrame.Text = @"Frame: " + scrlLowerFrame.Value;
            LoadLowerLight();
            DrawLowerFrame();
        }

        private void LoadLowerLight()
        {
            lightEditorLower.CanClose = false;
            lightEditorLower.LoadEditor(_editorItem.LowerLights[scrlLowerFrame.Value - 1]);
        }

        private void LoadUpperLight()
        {
            lightEditorUpper.CanClose = false;
            lightEditorUpper.LoadEditor(_editorItem.UpperLights[scrlUpperFrame.Value - 1]);
        }

        private void scrlUpperFrame_Scroll(object sender, ScrollValueEventArgs e)
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
                _editorItem.LowerLights[scrlLowerFrame.Value - 1] = new LightBase(_editorItem.LowerLights[scrlLowerFrame.Value - 2]);
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
                _editorItem.UpperLights[scrlUpperFrame.Value - 1] = new LightBase(_editorItem.UpperLights[scrlUpperFrame.Value - 2]);
                LoadUpperLight();
                DrawUpperFrame();
            }
        }

        private void scrlDarkness_Scroll(object sender, ScrollValueEventArgs e)
        {
            labelDarkness.Text = "Simulate Darkness: " + scrlDarkness.Value;
        }

        private void btnSwap_Click(object sender, EventArgs e)
        {
            string LowerAnimSprite = _editorItem.LowerAnimSprite;
            int LowerAnimXFrames = _editorItem.LowerAnimXFrames;
            int LowerAnimYFrames = _editorItem.LowerAnimYFrames;
            int LowerAnimFrameCount = _editorItem.LowerAnimFrameCount;
            int LowerAnimFrameSpeed = _editorItem.LowerAnimFrameSpeed;
            int LowerAnimLoopCount = _editorItem.LowerAnimLoopCount;
            LightBase[] LowerLights = _editorItem.LowerLights;
            _editorItem.LowerAnimSprite = _editorItem.UpperAnimSprite;
            _editorItem.LowerAnimXFrames = _editorItem.UpperAnimXFrames;
            _editorItem.LowerAnimYFrames = _editorItem.UpperAnimYFrames;
            _editorItem.LowerAnimFrameCount = _editorItem.UpperAnimFrameCount;
            _editorItem.LowerAnimFrameSpeed = _editorItem.UpperAnimFrameSpeed;
            _editorItem.LowerAnimLoopCount = _editorItem.UpperAnimLoopCount;
            _editorItem.LowerLights = _editorItem.UpperLights;

            _editorItem.UpperAnimSprite = LowerAnimSprite;
            _editorItem.UpperAnimXFrames = LowerAnimXFrames;
            _editorItem.UpperAnimYFrames = LowerAnimYFrames;
            _editorItem.UpperAnimFrameCount = LowerAnimFrameCount;
            _editorItem.UpperAnimFrameSpeed = LowerAnimFrameSpeed;
            _editorItem.UpperAnimLoopCount = LowerAnimLoopCount;
            _editorItem.UpperLights = LowerLights;

            UpdateEditor();
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.Animation);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstAnimations.Focused)
            {
                if (
                    MessageBox.Show("Are you sure you want to delete this game object? This action cannot be reverted!",
                        "Delete Object", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstAnimations.Focused)
            {
                _copiedItem = _editorItem.GetData();
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && _copiedItem != null && lstAnimations.Focused)
            {
                _editorItem.Load(_copiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                if (MessageBox.Show("Are you sure you want to undo changes made to this game object? This action cannot be reverted!",
                        "Undo Changes", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _editorItem.RestoreBackup();
                    UpdateEditor();
                }
            }
        }

        private void itemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Z)
                {
                    toolStripItemUndo_Click(null, null);
                }
                else if (e.KeyCode == Keys.V)
                {
                    toolStripItemPaste_Click(null, null);
                }
                else if (e.KeyCode == Keys.C)
                {
                    toolStripItemCopy_Click(null, null);
                }
            }
            else
            {
                if (e.KeyCode == Keys.Delete)
                {
                    toolStripItemDelete_Click(null, null);
                }
            }
        }

        private void UpdateToolStripItems()
        {
            toolStripItemCopy.Enabled = _editorItem != null && lstAnimations.Focused;
            toolStripItemPaste.Enabled = _editorItem != null && _copiedItem != null && lstAnimations.Focused;
            toolStripItemDelete.Enabled = _editorItem != null && lstAnimations.Focused;
            toolStripItemUndo.Enabled = _editorItem != null && lstAnimations.Focused;
        }

        private void itemList_FocusChanged(object sender, EventArgs e)
        {
            UpdateToolStripItems();
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.N)
                {
                    toolStripItemNew_Click(null, null);
                }
            }
        }
    }
}
