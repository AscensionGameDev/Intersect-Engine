
using Intersect_Editor.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Forms;
using Intersect_Editor.Classes.Core;
using Intersect;
using Intersect.GameObjects;
using Intersect.Localization;
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
            cmbSound.Items.Add(Strings.Get("general","none"));
            cmbSound.Items.AddRange(GameContentManager.GetSoundNames());

            //Lower Animation Graphic
            cmbLowerGraphic.Items.Clear();
            cmbLowerGraphic.Items.Add(Strings.Get("general","none"));
            cmbLowerGraphic.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Animation));

            //Upper Animation Graphic
            cmbUpperGraphic.Items.Clear();
            cmbUpperGraphic.Items.Add(Strings.Get("general","none"));
            cmbUpperGraphic.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Animation));

            lowerWindow = new SwapChainRenderTarget(EditorGraphics.GetGraphicsDevice(),picLowerAnimation.Handle, picLowerAnimation.Width, picLowerAnimation.Height);
            upperWindow = new SwapChainRenderTarget(EditorGraphics.GetGraphicsDevice(),picUpperAnimation.Handle, picUpperAnimation.Width, picUpperAnimation.Height);
            lowerDarkness = EditorGraphics.CreateRenderTexture(picLowerAnimation.Width,picLowerAnimation.Height);
            upperDarkness = EditorGraphics.CreateRenderTexture(picUpperAnimation.Width, picUpperAnimation.Height);

            InitLocalization();
            UpdateEditor();
        }

        private void InitLocalization()
        {
            Text = Strings.Get("animationeditor", "title");
            toolStripItemNew.Text = Strings.Get("animationeditor", "new");
            toolStripItemDelete.Text = Strings.Get("animationeditor", "delete");
            toolStripItemCopy.Text = Strings.Get("animationeditor", "copy");
            toolStripItemPaste.Text = Strings.Get("animationeditor", "paste");
            toolStripItemUndo.Text = Strings.Get("animationeditor", "undo");

            grpAnimations.Text = Strings.Get("animationeditor", "animations");

            grpGeneral.Text = Strings.Get("animationeditor", "general");
            lblName.Text = Strings.Get("animationeditor", "name");
            lblSound.Text = Strings.Get("animationeditor", "sound");
            labelDarkness.Text = Strings.Get("animationeditor", "simulatedarkness", scrlDarkness.Value);
            btnSwap.Text = Strings.Get("animationeditor", "swap");

            grpLower.Text = Strings.Get("animationeditor", "lowergroup");
            lblLowerGraphic.Text = Strings.Get("animationeditor", "lowergraphic");
            lblLowerHorizontalFrames.Text = Strings.Get("animationeditor", "lowerhorizontalframes");
            lblLowerVerticalFrames.Text = Strings.Get("animationeditor", "lowerverticalframes");
            lblLowerFrameCount.Text = Strings.Get("animationeditor", "lowerframecount");
            lblLowerFrameDuration.Text = Strings.Get("animationeditor", "lowerframeduration");
            lblLowerLoopCount.Text = Strings.Get("animationeditor", "lowerloopcount");
            grpLowerPlayback.Text = Strings.Get("animationeditor", "lowerplayback");
            lblLowerFrame.Text = Strings.Get("animationeditor", "lowerframe", scrlLowerFrame.Value);
            btnPlayLower.Text = Strings.Get("animationeditor", "lowerplay");
            grpLowerFrameOpts.Text = Strings.Get("animationeditor", "lowerframeoptions");
            btnLowerClone.Text = Strings.Get("animationeditor", "lowerclone");

            grpUpper.Text = Strings.Get("animationeditor", "uppergroup");
            lblUpperGraphic.Text = Strings.Get("animationeditor", "uppergraphic");
            lblUpperHorizontalFrames.Text = Strings.Get("animationeditor", "upperhorizontalframes");
            lblUpperVerticalFrames.Text = Strings.Get("animationeditor", "upperverticalframes");
            lblUpperFrameCount.Text = Strings.Get("animationeditor", "upperframecount");
            lblUpperFrameDuration.Text = Strings.Get("animationeditor", "upperframeduration");
            lblUpperLoopCount.Text = Strings.Get("animationeditor", "upperloopcount");
            grpUpperPlayback.Text = Strings.Get("animationeditor", "upperplayback");
            lblUpperFrame.Text = Strings.Get("animationeditor", "upperframe", scrlUpperFrame.Value);
            btnPlayUpper.Text = Strings.Get("animationeditor", "upperplay");
            grpUpperFrameOpts.Text = Strings.Get("animationeditor", "upperframeoptions");
            btnUpperClone.Text = Strings.Get("animationeditor", "upperclone");

            btnSave.Text = Strings.Get("animationeditor", "save");
            btnCancel.Text = Strings.Get("animationeditor", "cancel");
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

                nudLowerHorizontalFrames.Value = _editorItem.LowerAnimXFrames;
                nudLowerVerticalFrames.Value = _editorItem.LowerAnimYFrames;
                nudLowerFrameCount.Value = _editorItem.LowerAnimFrameCount;
                UpdateLowerFrames();

                nudLowerFrameDuration.Value = _editorItem.LowerAnimFrameSpeed * 10;
                tmrLowerAnimation.Interval = (int)nudLowerFrameDuration.Value;
                nudLowerLoopCount.Value = _editorItem.LowerAnimLoopCount;

                cmbUpperGraphic.SelectedIndex =
                    cmbUpperGraphic.FindString(_editorItem.UpperAnimSprite);

                nudUpperHorizontalFrames.Value = _editorItem.UpperAnimXFrames;
                nudUpperVerticalFrames.Value = _editorItem.UpperAnimYFrames;
                nudUpperFrameCount.Value = _editorItem.UpperAnimFrameCount;
                UpdateUpperFrames();

                nudUpperFrameDuration.Value = _editorItem.UpperAnimFrameSpeed * 10;
                tmrUpperAnimation.Interval = (int)nudUpperFrameDuration.Value;
                nudUpperLoopCount.Value = _editorItem.UpperAnimLoopCount;

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
            lstAnimations.Items[Database.GameObjectListIndex(GameObject.Animation,_editorItem.Id)] = txtName.Text;
        }

        private void cmbSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Sound = cmbSound.Text;
        }

        private void cmbLowerGraphic_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.LowerAnimSprite = cmbLowerGraphic.Text;
        }

        private void cmbUpperGraphic_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.UpperAnimSprite = cmbUpperGraphic.Text;
        }

        private void tmrLowerAnimation_Tick(object sender, EventArgs e)
        {
            DrawLowerFrame();
            if (_playLower)
            {
                _lowerFrame++;
                if (_lowerFrame >= (int)nudLowerFrameCount.Value)
                {
                    _lowerFrame = 0;
                }
            }
        }

        void UpdateLowerFrames()
        {
            LightBase[] newArray;
            scrlLowerFrame.Maximum = (int)nudLowerFrameCount.Value;
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
            scrlUpperFrame.Maximum = (int)nudUpperFrameCount.Value;
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
                EditorGraphics.DrawLight(
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
                long w = animTexture.Width/(int)nudLowerHorizontalFrames.Value;
                long h = animTexture.Height/(int)nudLowerVerticalFrames.Value;
                long x = 0;
                if (_lowerFrame > 0)
                {
                    x = (_lowerFrame%(int)nudLowerHorizontalFrames.Value)*w;
                }
                long y = (int) Math.Floor(_lowerFrame/nudLowerHorizontalFrames.Value)*h;
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
                EditorGraphics.DrawLight(
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
                long w = animTexture.Width / (int)nudUpperHorizontalFrames.Value;
                long h = animTexture.Height / (int)nudUpperVerticalFrames.Value;
                long x = 0;
                if (_upperFrame > 0)
                {
                    x = (_upperFrame % (int)nudUpperHorizontalFrames.Value) * w;
                }
                long y = (int)Math.Floor(_upperFrame / nudUpperHorizontalFrames.Value) * h;
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
                if (_upperFrame >= (int)nudUpperFrameCount.Value)
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
            lblLowerFrame.Text = Strings.Get("animationeditor","lowerframe", scrlLowerFrame.Value);
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
            lblUpperFrame.Text = Strings.Get("animationeditor","upperframe",scrlUpperFrame.Value);
            LoadUpperLight();
            DrawUpperFrame();
        }

        private void btnPlayLower_Click(object sender, EventArgs e)
        {
            _playLower = !_playLower;
            if (_playLower)
            {
                btnPlayLower.Text = Strings.Get("animationeditor", "lowerstop");
            }
            else
            {
                btnPlayLower.Text = Strings.Get("animationeditor", "lowerplay");
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
                btnPlayUpper.Text = Strings.Get("animationeditor", "upperstop");
            }
            else
            {
                btnPlayUpper.Text = Strings.Get("animationeditor", "upperplay");
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
            labelDarkness.Text = Strings.Get("animationeditor", "simulatedarkness",scrlDarkness.Value);
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
                if (DarkMessageBox.ShowWarning(Strings.Get("animationeditor", "deleteprompt"),
                        Strings.Get("animationeditor", "deletetitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) == DialogResult.Yes)
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
                if (DarkMessageBox.ShowWarning(Strings.Get("animationeditor","undoprompt"),
                        Strings.Get("animationeditor", "undotitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) == DialogResult.Yes)
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

        private void nudLowerHorizontalFrames_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.LowerAnimXFrames = (int)nudLowerHorizontalFrames.Value;
        }

        private void nudLowerVerticalFrames_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.LowerAnimYFrames = (int)nudLowerVerticalFrames.Value;
        }

        private void nudLowerFrameCount_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.LowerAnimFrameCount = (int)nudLowerFrameCount.Value;
            UpdateLowerFrames();
        }

        private void nudLowerFrameDuration_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.LowerAnimFrameSpeed = (int)nudLowerFrameDuration.Value / 10;
            tmrLowerAnimation.Interval = (int)nudLowerFrameDuration.Value / 10;
        }

        private void nudLowerLoopCount_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.LowerAnimLoopCount = (int)nudLowerLoopCount.Value;
        }

        private void nudUpperHorizontalFrames_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.UpperAnimXFrames = (int)nudUpperHorizontalFrames.Value;
        }

        private void nudUpperVerticalFrames_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.UpperAnimYFrames = (int)nudUpperVerticalFrames.Value;
        }

        private void nudUpperFrameCount_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.UpperAnimFrameCount = (int)nudUpperFrameCount.Value;
            UpdateUpperFrames();
        }

        private void nudUpperFrameDuration_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.UpperAnimFrameSpeed = (int)nudUpperFrameDuration.Value / 10;
            tmrUpperAnimation.Interval = (int)nudUpperFrameDuration.Value / 10;
        }

        private void nudUpperLoopCount_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.UpperAnimLoopCount = (int)nudUpperLoopCount.Value;
        }
    }
}
