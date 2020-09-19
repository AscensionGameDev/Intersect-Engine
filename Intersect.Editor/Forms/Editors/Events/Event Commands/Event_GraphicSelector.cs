using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventGraphicSelector : UserControl
    {

        private EventGraphic mEditingGraphic;

        private FrmEvent mEventEditor;

        private bool mLoading;

        private bool mMouseDown;

        private bool mNewRouteAction;

        private EventMoveRouteDesigner mRouteDesigner;

        private int mSpriteHeight;

        private int mSpriteWidth;

        private EventGraphic mTmpGraphic = new EventGraphic();

        public EventGraphicSelector(
            EventGraphic editingGraphic,
            FrmEvent eventEditor,
            EventMoveRouteDesigner moveRouteDesigner = null,
            bool newMoveRouteAction = false
        )
        {
            InitializeComponent();
            InitLocalization();
            mEditingGraphic = editingGraphic;
            mEventEditor = eventEditor;
            mLoading = true;
            cmbGraphicType.SelectedIndex = (int)mEditingGraphic.Type;
            UpdateGraphicList();
            if (cmbGraphic.Items.Contains(mEditingGraphic.Filename))
            {
                cmbGraphic.SelectedIndex = cmbGraphic.Items.IndexOf(mEditingGraphic.Filename);
            }

            mRouteDesigner = moveRouteDesigner;
            mNewRouteAction = newMoveRouteAction;
            mLoading = false;
            mTmpGraphic.CopyFrom(mEditingGraphic);
            UpdatePreview();
        }

        private void InitLocalization()
        {
            grpSelector.Text = Strings.EventGraphic.title;
            lblType.Text = Strings.EventGraphic.type;
            cmbGraphicType.Items.Clear();
            cmbGraphicType.Items.Add(Strings.EventGraphic.graphictype0);
            cmbGraphicType.Items.Add(Strings.EventGraphic.graphictype1);
            cmbGraphicType.Items.Add(Strings.EventGraphic.graphictype2);
            lblGraphic.Text = Strings.EventGraphic.graphic;
            grpPreview.Text = Strings.EventGraphic.preview;
            btnOk.Text = Strings.EventGraphic.okay;
            btnCancel.Text = Strings.EventGraphic.cancel;
        }

        private void GraphicTypeUpdated()
        {
            mTmpGraphic.Filename = "";
            mTmpGraphic.Type = EventGraphicType.None;
            mTmpGraphic.X = 0;
            mTmpGraphic.Y = 0;
            mTmpGraphic.Width = -1;
            mTmpGraphic.Height = -1;
            if (cmbGraphicType.SelectedIndex == 0) //No Graphic
            {
                cmbGraphic.Hide();
                lblGraphic.Hide();
                UpdatePreview();
            }
            else if (cmbGraphicType.SelectedIndex == 1) //Sprite
            {
                mTmpGraphic.Type = EventGraphicType.Sprite;
                cmbGraphic.Show();
                lblGraphic.Show();
                cmbGraphic.Items.Clear();
                cmbGraphic.Items.AddRange(
                    GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity)
                );

                if (cmbGraphic.Items.Count > 0)
                {
                    cmbGraphic.SelectedIndex = 0;
                }
            }
            else if (cmbGraphicType.SelectedIndex == 2) //Tileset
            {
                mTmpGraphic.Type = EventGraphicType.Tileset;
                mTmpGraphic.Width = 0;
                mTmpGraphic.Height = 0;
                lblGraphic.Show();
                cmbGraphic.Show();
                cmbGraphic.Items.Clear();
                foreach (var filename in TilesetBase.Names)
                {
                    if (File.Exists("resources/tilesets/" + filename))
                    {
                        cmbGraphic.Items.Add(filename);
                    }
                    else
                    {
                    }
                }

                if (cmbGraphic.Items.Count > 0)
                {
                    cmbGraphic.SelectedIndex = 0;
                }
            }
        }

        private void UpdateGraphicList()
        {
            cmbGraphic.Items.Clear();
            if (cmbGraphicType.SelectedIndex == 0) //No Graphic
            {
                cmbGraphic.Hide();
                lblGraphic.Hide();
            }
            else if (cmbGraphicType.SelectedIndex == 1) //Sprite
            {
                cmbGraphic.Show();
                lblGraphic.Show();
                cmbGraphic.Items.AddRange(
                    GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity)
                );

                if (cmbGraphic.Items.Count > 0)
                {
                    cmbGraphic.SelectedIndex = 0;
                }
            }
            else if (cmbGraphicType.SelectedIndex == 2) //Tileset
            {
                lblGraphic.Show();
                cmbGraphic.Show();
                foreach (var filename in TilesetBase.Names)
                {
                    if (File.Exists("resources/tilesets/" + filename))
                    {
                        cmbGraphic.Items.Add(filename);
                    }
                    else
                    {
                    }
                }

                if (cmbGraphic.Items.Count > 0)
                {
                    cmbGraphic.SelectedIndex = 0;
                }
            }
        }

        private void UpdatePreview()
        {
            Graphics graphics;
            Bitmap sourceBitmap = null;
            Bitmap destBitmap = null;
            if (cmbGraphicType.SelectedIndex == 1) //Sprite
            {
                sourceBitmap = new Bitmap("resources/entities/" + cmbGraphic.Text);
                mSpriteWidth = sourceBitmap.Width / Options.Instance.Sprites.NormalFrames;
                mSpriteHeight = sourceBitmap.Height / Options.Instance.Sprites.Directions;
            }
            else if (cmbGraphicType.SelectedIndex == 2) //Tileset
            {
                sourceBitmap = new Bitmap("resources/tilesets/" + cmbGraphic.Text);
            }

            if (sourceBitmap != null)
            {
                pnlGraphic.Show();
                destBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
                pnlGraphic.Width = sourceBitmap.Width;
                pnlGraphic.Height = sourceBitmap.Height;
                graphics = Graphics.FromImage(destBitmap);
                graphics.Clear(System.Drawing.Color.FromArgb(60, 63, 65));
                graphics.DrawImage(sourceBitmap, new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height));
                if (cmbGraphicType.SelectedIndex == 1)
                {
                    graphics.DrawRectangle(
                        new Pen(System.Drawing.Color.White, 2f),
                        new Rectangle(
                            mTmpGraphic.X * sourceBitmap.Width / Options.Instance.Sprites.NormalFrames, mTmpGraphic.Y * sourceBitmap.Height / Options.Instance.Sprites.Directions,
                            sourceBitmap.Width / Options.Instance.Sprites.NormalFrames, sourceBitmap.Height / Options.Instance.Sprites.Directions
                        )
                    );
                }
                else if (cmbGraphicType.SelectedIndex == 2)
                {
                    var selX = mTmpGraphic.X;
                    var selY = mTmpGraphic.Y;
                    var selW = mTmpGraphic.Width;
                    var selH = mTmpGraphic.Height;
                    if (selW < 0)
                    {
                        selX -= Math.Abs(selW);
                        selW = Math.Abs(selW);
                    }

                    if (selH < 0)
                    {
                        selY -= Math.Abs(selH);
                        selH = Math.Abs(selH);
                    }

                    graphics.DrawRectangle(
                        new Pen(System.Drawing.Color.White, 2f),
                        new Rectangle(
                            selX * Options.TileWidth, selY * Options.TileHeight,
                            Options.TileWidth + selW * Options.TileWidth, Options.TileHeight + selH * Options.TileHeight
                        )
                    );
                }

                sourceBitmap.Dispose();
                graphics.Dispose();
                pnlGraphic.BackgroundImage = destBitmap;
            }
            else
            {
                pnlGraphic.Hide();
            }
        }

        private void cmbGraphicType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!mLoading)
            {
                GraphicTypeUpdated();
            }
        }

        private void cmbGraphic_SelectedIndexChanged(object sender, EventArgs e)
        {
            mTmpGraphic.Filename = cmbGraphic.Text;
            UpdatePreview();
        }

        private void pnlGraphic_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > pnlGraphic.Width || e.Y > pnlGraphic.Height)
            {
                return;
            }

            if (cmbGraphicType.SelectedIndex == 1)
            {
                mTmpGraphic.X = (int) Math.Floor((double) e.X / mSpriteWidth);
                mTmpGraphic.Y = (int) Math.Floor((double) e.Y / mSpriteHeight);
            }
            else
            {
                mMouseDown = true;
                mTmpGraphic.X = (int) Math.Floor((double) e.X / Options.TileWidth);
                mTmpGraphic.Y = (int) Math.Floor((double) e.Y / Options.TileHeight);
            }

            mTmpGraphic.Width = 0;
            mTmpGraphic.Height = 0;
            if (mTmpGraphic.X < 0)
            {
                mTmpGraphic.X = 0;
            }

            if (mTmpGraphic.Y < 0)
            {
                mTmpGraphic.Y = 0;
            }

            UpdatePreview();
        }

        private void pnlGraphic_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.X > pnlGraphic.Width || e.Y > pnlGraphic.Height)
            {
                return;
            }

            if (cmbGraphicType.SelectedIndex != 2)
            {
                return;
            }

            var selX = mTmpGraphic.X;
            var selY = mTmpGraphic.Y;
            var selW = mTmpGraphic.Width;
            var selH = mTmpGraphic.Height;
            if (selW < 0)
            {
                selX -= Math.Abs(selW);
                selW = Math.Abs(selW);
            }

            if (selH < 0)
            {
                selY -= Math.Abs(selH);
                selH = Math.Abs(selH);
            }

            mTmpGraphic.X = selX;
            mTmpGraphic.Y = selY;
            mTmpGraphic.Width = selW;
            mTmpGraphic.Height = selH;
            mMouseDown = false;
            UpdatePreview();
        }

        private void pnlGraphic_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > pnlGraphic.Width || e.Y > pnlGraphic.Height)
            {
                return;
            }

            if (cmbGraphicType.SelectedIndex != 2)
            {
                return;
            }

            if (mMouseDown)
            {
                var tmpX = (int) Math.Floor((double) e.X / Options.TileWidth);
                var tmpY = (int) Math.Floor((double) e.Y / Options.TileHeight);
                mTmpGraphic.Width = tmpX - mTmpGraphic.X;
                mTmpGraphic.Height = tmpY - mTmpGraphic.Y;
            }

            UpdatePreview();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            mEditingGraphic.CopyFrom(mTmpGraphic);
            mEventEditor.CloseGraphicSelector(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mRouteDesigner != null && mNewRouteAction)
            {
                mRouteDesigner.RemoveLastAction();
            }

            mEventEditor.CloseGraphicSelector(this);
        }

    }

}
