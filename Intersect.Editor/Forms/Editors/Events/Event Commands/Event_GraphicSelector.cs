using Intersect.Editor.Content;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Events;
using Graphics = System.Drawing.Graphics;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;


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
        InitIdleMotionControls();
        UpdatePreview();
    }

    private DarkUI.Controls.DarkComboBox cmbIdleMotionType;

    private DarkUI.Controls.DarkNumericUpDown nudIdleMotionSpeed;

    private DarkUI.Controls.DarkNumericUpDown nudIdleMotionAmplitude;

    private DarkUI.Controls.DarkNumericUpDown nudIdleMotionPivot;

    // Procedural idle animation (sway/float/bounce) - purely code-driven, no hand-drawn frames needed.
    // Added as a second row inside grpSelector; grpSelector grows taller and grpPreview shrinks/shifts
    // down by the same amount so the overall dialog size stays unchanged.
    private void InitIdleMotionControls()
    {
        const int grow = 30;
        grpSelector.Height += grow;
        grpPreview.Location = new System.Drawing.Point(grpPreview.Location.X, grpPreview.Location.Y + grow);
        grpPreview.Height -= grow;

        var lblIdleMotionType = new DarkUI.Controls.DarkLabel
        {
            Text = "Idle Motion:",
            Location = new System.Drawing.Point(6, 51),
            Size = new System.Drawing.Size(77, 13),
            ForeColor = System.Drawing.Color.Gainsboro,
        };

        cmbIdleMotionType = new DarkUI.Controls.DarkComboBox
        {
            Location = new System.Drawing.Point(89, 48),
            Size = new System.Drawing.Size(121, 21),
            DropDownStyle = ComboBoxStyle.DropDownList,
        };
        cmbIdleMotionType.Items.AddRange(System.Enum.GetNames(typeof(IdleMotionType)));
        cmbIdleMotionType.SelectedIndex = (int)mTmpGraphic.IdleMotionType;
        cmbIdleMotionType.SelectedIndexChanged += (_, _) =>
            mTmpGraphic.IdleMotionType = (IdleMotionType)cmbIdleMotionType.SelectedIndex;

        var lblSpeed = new DarkUI.Controls.DarkLabel
        {
            Text = "Speed:",
            Location = new System.Drawing.Point(216, 51),
            Size = new System.Drawing.Size(40, 13),
            ForeColor = System.Drawing.Color.Gainsboro,
        };

        nudIdleMotionSpeed = new DarkUI.Controls.DarkNumericUpDown
        {
            Location = new System.Drawing.Point(258, 48),
            Size = new System.Drawing.Size(60, 21),
            DecimalPlaces = 1,
            Increment = 0.1M,
            Minimum = 0.1M,
            Maximum = 10M,
            Value = (decimal)mTmpGraphic.IdleMotionSpeed,
        };
        nudIdleMotionSpeed.ValueChanged += (_, _) =>
            mTmpGraphic.IdleMotionSpeed = (float)nudIdleMotionSpeed.Value;

        var lblAmplitude = new DarkUI.Controls.DarkLabel
        {
            Text = "Amplitude:",
            Location = new System.Drawing.Point(326, 51),
            Size = new System.Drawing.Size(60, 13),
            ForeColor = System.Drawing.Color.Gainsboro,
        };

        nudIdleMotionAmplitude = new DarkUI.Controls.DarkNumericUpDown
        {
            Location = new System.Drawing.Point(392, 48),
            Size = new System.Drawing.Size(60, 21),
            DecimalPlaces = 1,
            Increment = 0.5M,
            Minimum = 0M,
            Maximum = 90M,
            Value = (decimal)mTmpGraphic.IdleMotionAmplitude,
        };
        nudIdleMotionAmplitude.ValueChanged += (_, _) =>
            mTmpGraphic.IdleMotionAmplitude = (float)nudIdleMotionAmplitude.Value;

        var lblPivot = new DarkUI.Controls.DarkLabel
        {
            Text = "Pivot:",
            Location = new System.Drawing.Point(464, 51),
            Size = new System.Drawing.Size(36, 13),
            ForeColor = System.Drawing.Color.Gainsboro,
        };

        nudIdleMotionPivot = new DarkUI.Controls.DarkNumericUpDown
        {
            Location = new System.Drawing.Point(504, 48),
            Size = new System.Drawing.Size(60, 21),
            DecimalPlaces = 0,
            Increment = 5M,
            Minimum = 0M,
            Maximum = 100M,
            Value = (decimal)mTmpGraphic.IdleMotionPivotPercent,
        };
        nudIdleMotionPivot.ValueChanged += (_, _) =>
            mTmpGraphic.IdleMotionPivotPercent = (float)nudIdleMotionPivot.Value;
        new ToolTip().SetToolTip(
            nudIdleMotionPivot,
            "0 = pivote depuis le haut, 100 = pivote depuis le bas (comme un tronc d'arbre), 50 = tout le sprite bascule en bloc."
        );

        grpSelector.Controls.Add(lblIdleMotionType);
        grpSelector.Controls.Add(cmbIdleMotionType);
        grpSelector.Controls.Add(lblSpeed);
        grpSelector.Controls.Add(nudIdleMotionSpeed);
        grpSelector.Controls.Add(lblAmplitude);
        grpSelector.Controls.Add(nudIdleMotionAmplitude);
        grpSelector.Controls.Add(lblPivot);
        grpSelector.Controls.Add(nudIdleMotionPivot);
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
        mTmpGraphic.Filename = string.Empty;
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

            var sortedTextures =
                GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity);

            if (sortedTextures.Length > 0)
            {
                cmbGraphic.Items.AddRange(sortedTextures);
            }
            else
            {
                cmbGraphic.Items.Add(Strings.General.None);
            }

            cmbGraphic.SelectedIndex = 0;
        }
        else if (cmbGraphicType.SelectedIndex == 2) //Tileset
        {
            mTmpGraphic.Type = EventGraphicType.Tileset;
            mTmpGraphic.Width = 0;
            mTmpGraphic.Height = 0;
            lblGraphic.Show();
            cmbGraphic.Show();
            cmbGraphic.Items.Clear();

            var sortedTextures =
                GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Tileset);

            if (sortedTextures.Length > 0)
            {
                cmbGraphic.Items.AddRange(sortedTextures);
            }
            else
            {
                cmbGraphic.Items.Add(Strings.General.None);
            }

            cmbGraphic.SelectedIndex = 0;
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

            var sortedTextures =
                GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity);

            if (sortedTextures.Length > 0)
            {
                cmbGraphic.Items.AddRange(sortedTextures);
            }
            else
            {
                cmbGraphic.Items.Add(Strings.General.None);
            }

            cmbGraphic.SelectedIndex = 0;
        }
        else if (cmbGraphicType.SelectedIndex == 2) //Tileset
        {
            lblGraphic.Show();
            cmbGraphic.Show();

            var sortedTextures =
                GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Tileset);

            if (sortedTextures.Length > 0)
            {
                cmbGraphic.Items.AddRange(sortedTextures);
            }
            else
            {
                cmbGraphic.Items.Add(Strings.General.None);
            }

            cmbGraphic.SelectedIndex = 0;
        }
    }

    private void UpdatePreview()
    {
        Graphics graphics;
        Bitmap sourceBitmap = null;
        Bitmap destBitmap = null;
        if (cmbGraphicType.SelectedIndex == 1) //Sprite
        {
            var spriteFile = "resources/entities/" + cmbGraphic.Text;
            if (File.Exists(spriteFile))
            {
                sourceBitmap = new Bitmap(spriteFile);
                mSpriteWidth = sourceBitmap.Width / Options.Instance.Sprites.NormalFrames;
                mSpriteHeight = sourceBitmap.Height / Options.Instance.Sprites.Directions;
            }
        }
        else if (cmbGraphicType.SelectedIndex == 2) //Tileset
        {
            var tileSetFile = "resources/tilesets/" + cmbGraphic.Text;
            if (File.Exists(tileSetFile))
            {
                sourceBitmap = new Bitmap(tileSetFile);
            }
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
                        selX * Options.Instance.Map.TileWidth, selY * Options.Instance.Map.TileHeight,
                        Options.Instance.Map.TileWidth + selW * Options.Instance.Map.TileWidth, Options.Instance.Map.TileHeight + selH * Options.Instance.Map.TileHeight
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
            mTmpGraphic.X = (int) Math.Floor((double) e.X / Options.Instance.Map.TileWidth);
            mTmpGraphic.Y = (int) Math.Floor((double) e.Y / Options.Instance.Map.TileHeight);
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
            var tmpX = (int) Math.Floor((double) e.X / Options.Instance.Map.TileWidth);
            var tmpY = (int) Math.Floor((double) e.Y / Options.Instance.Map.TileHeight);
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
