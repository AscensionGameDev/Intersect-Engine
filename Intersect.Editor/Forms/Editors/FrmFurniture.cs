using DarkUI.Controls;
using DarkUI.Forms;
using Intersect.Editor.Content;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Furniture;
using Intersect.Framework.Core.GameObjects.Mapping.Tilesets;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors;

/// <summary>
/// Simple catalog editor for placeable guild-hall furniture (name, sprite, passable, default
/// facing). Deliberately minimal (no folders/search/undo) compared to the older item-style
/// editors - the catalog is expected to stay small.
/// </summary>
public class FrmFurniture : EditorForm
{
    private FurnitureDescriptor? _editorItem;

    private ListBox _lstFurniture;
    private Panel _pnlContainer;
    private TextBox _txtName;
    private RadioButton _rdoSpriteEntity;
    private RadioButton _rdoSpriteTileset;
    private ComboBox _cmbSprite;
    private ComboBox _cmbTileset;
    private CheckBox _chkPassable;
    private ComboBox _cmbDirection;
    private PictureBox _picPreview;
    private Panel _pnlTilesetScroll;
    private PictureBox _picTileset;
    private Label _lblTilesetHint;
    private DarkButton _btnNew;
    private DarkButton _btnDelete;

    private TilesetDescriptor? _pickerTileset;
    private Image? _pickerTilesetImage;
    private bool _pickerLoadFailed;

    public FrmFurniture()
    {
        ApplyHooks();
        Text = "Furniture Editor";
        Icon = Program.Icon;
        ClientSize = new Size(700, 620);
        BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        StartPosition = FormStartPosition.CenterParent;

        _lstFurniture = new ListBox
        {
            Location = new System.Drawing.Point(8, 8),
            Size = new Size(220, 540),
            BackColor = System.Drawing.Color.FromArgb(30, 30, 33),
            ForeColor = System.Drawing.Color.Gainsboro,
            BorderStyle = BorderStyle.FixedSingle,
        };
        _lstFurniture.SelectedIndexChanged += (_, _) => AssignSelected();

        _btnNew = new DarkButton
        {
            Location = new System.Drawing.Point(8, 552),
            Size = new Size(105, 26),
            Text = "Nouveau",
        };
        _btnNew.Click += (_, _) => PacketSender.SendCreateObject(GameObjectType.Furniture);

        _btnDelete = new DarkButton
        {
            Location = new System.Drawing.Point(123, 552),
            Size = new Size(105, 26),
            Text = "Supprimer",
        };
        _btnDelete.Click += (_, _) => DeleteSelected();

        _pnlContainer = new Panel
        {
            Location = new System.Drawing.Point(240, 8),
            Size = new Size(452, 570),
            Visible = false,
        };

        var lblName = new Label { Text = "Nom :", ForeColor = System.Drawing.Color.Gainsboro, Location = new System.Drawing.Point(0, 4), AutoSize = true };
        _txtName = new TextBox { Location = new System.Drawing.Point(90, 0), Size = new Size(250, 22) };
        _txtName.TextChanged += (_, _) =>
        {
            if (_editorItem == null) return;
            _editorItem.Name = _txtName.Text;
            RefreshList();
        };

        var lblSource = new Label { Text = "Source du sprite :", ForeColor = System.Drawing.Color.Gainsboro, Location = new System.Drawing.Point(0, 34), AutoSize = true };
        _rdoSpriteEntity = new RadioButton
        {
            Text = "Sprite d'entité",
            ForeColor = System.Drawing.Color.Gainsboro,
            Location = new System.Drawing.Point(130, 32),
            AutoSize = true,
            Checked = true,
        };
        _rdoSpriteEntity.CheckedChanged += (_, _) =>
        {
            if (!_rdoSpriteEntity.Checked || _editorItem == null) return;
            _editorItem.UseTilesetSprite = false;
            UpdateSpriteSourceVisibility();
            DrawPreview();
        };

        _rdoSpriteTileset = new RadioButton
        {
            Text = "Tuile de tileset",
            ForeColor = System.Drawing.Color.Gainsboro,
            Location = new System.Drawing.Point(260, 32),
            AutoSize = true,
        };
        _rdoSpriteTileset.CheckedChanged += (_, _) =>
        {
            if (!_rdoSpriteTileset.Checked || _editorItem == null) return;
            _editorItem.UseTilesetSprite = true;
            UpdateSpriteSourceVisibility();
        };

        var lblSprite = new Label { Text = "Sprite :", ForeColor = System.Drawing.Color.Gainsboro, Location = new System.Drawing.Point(0, 64), AutoSize = true };
        _cmbSprite = new ComboBox { Location = new System.Drawing.Point(90, 60), Size = new Size(250, 22), DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbSprite.SelectedIndexChanged += (_, _) =>
        {
            if (_editorItem == null) return;
            _editorItem.Sprite = TextUtils.SanitizeNone(_cmbSprite.Text) ?? string.Empty;
            DrawPreview();
        };

        var lblTileset = new Label { Text = "Tileset :", ForeColor = System.Drawing.Color.Gainsboro, Location = new System.Drawing.Point(0, 64), AutoSize = true };
        _cmbTileset = new ComboBox { Location = new System.Drawing.Point(90, 60), Size = new Size(250, 22), DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbTileset.SelectedIndexChanged += (_, _) =>
        {
            if (_editorItem == null || _cmbTileset.SelectedItem is not TilesetListEntry entry) return;
            _editorItem.TilesetId = entry.Tileset.Id;
            _editorItem.TileX = 0;
            _editorItem.TileY = 0;
            LoadPickerTileset(entry.Tileset);
        };

        _chkPassable = new CheckBox
        {
            Text = "Traversable (les joueurs peuvent marcher dessus)",
            ForeColor = System.Drawing.Color.Gainsboro,
            Location = new System.Drawing.Point(0, 92),
            AutoSize = true,
        };
        _chkPassable.CheckedChanged += (_, _) =>
        {
            if (_editorItem == null) return;
            _editorItem.Passable = _chkPassable.Checked;
        };

        var lblDirection = new Label { Text = "Orientation par défaut :", ForeColor = System.Drawing.Color.Gainsboro, Location = new System.Drawing.Point(0, 122), AutoSize = true };
        _cmbDirection = new ComboBox { Location = new System.Drawing.Point(150, 118), Size = new Size(120, 22), DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbDirection.Items.AddRange(new object[] { Direction.Down, Direction.Up, Direction.Left, Direction.Right });
        _cmbDirection.SelectedIndexChanged += (_, _) =>
        {
            if (_editorItem == null || _cmbDirection.SelectedItem is not Direction dir) return;
            _editorItem.DefaultDirection = dir;
        };

        _picPreview = new PictureBox
        {
            Location = new System.Drawing.Point(0, 160),
            Size = new Size(96, 96),
            BackColor = System.Drawing.Color.Black,
            BorderStyle = BorderStyle.FixedSingle,
        };

        _pnlTilesetScroll = new Panel
        {
            Location = new System.Drawing.Point(0, 160),
            Size = new Size(444, 340),
            BackColor = System.Drawing.Color.Black,
            BorderStyle = BorderStyle.FixedSingle,
            AutoScroll = true,
            Visible = false,
        };
        _picTileset = new PictureBox
        {
            Location = System.Drawing.Point.Empty,
            SizeMode = PictureBoxSizeMode.Normal,
        };
        _picTileset.Paint += PicTileset_Paint;
        _picTileset.MouseClick += PicTileset_MouseClick;
        _pnlTilesetScroll.Controls.Add(_picTileset);

        _lblTilesetHint = new Label
        {
            Text = "Clique une tuile pour la choisir.",
            ForeColor = System.Drawing.Color.Gainsboro,
            Location = new System.Drawing.Point(0, 504),
            AutoSize = true,
            Visible = false,
        };

        _pnlContainer.Controls.Add(lblName);
        _pnlContainer.Controls.Add(_txtName);
        _pnlContainer.Controls.Add(lblSource);
        _pnlContainer.Controls.Add(_rdoSpriteEntity);
        _pnlContainer.Controls.Add(_rdoSpriteTileset);
        _pnlContainer.Controls.Add(lblSprite);
        _pnlContainer.Controls.Add(_cmbSprite);
        _pnlContainer.Controls.Add(lblTileset);
        _pnlContainer.Controls.Add(_cmbTileset);
        _pnlContainer.Controls.Add(_chkPassable);
        _pnlContainer.Controls.Add(lblDirection);
        _pnlContainer.Controls.Add(_cmbDirection);
        _pnlContainer.Controls.Add(_picPreview);
        _pnlContainer.Controls.Add(_pnlTilesetScroll);
        _pnlContainer.Controls.Add(_lblTilesetHint);

        _btnSave = new DarkButton { Location = new System.Drawing.Point(490, 588), Size = new Size(90, 26), Text = "Enregistrer" };
        _btnSave.Click += (_, _) => Save();

        _btnCancel = new DarkButton { Location = new System.Drawing.Point(586, 588), Size = new Size(90, 26), Text = "Fermer" };
        _btnCancel.Click += (_, _) => Close();

        Controls.Add(_lstFurniture);
        Controls.Add(_btnNew);
        Controls.Add(_btnDelete);
        Controls.Add(_pnlContainer);
        Controls.Add(_btnSave);
        Controls.Add(_btnCancel);

        Load += (_, _) =>
        {
            _cmbSprite.Items.Clear();
            _cmbSprite.Items.Add("Aucun");
            _cmbSprite.Items.AddRange(GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity));

            _cmbTileset.Items.Clear();
            foreach (var pair in TilesetDescriptor.Lookup.OrderBy(p => ((TilesetDescriptor)p.Value).Name))
            {
                _cmbTileset.Items.Add(new TilesetListEntry((TilesetDescriptor)pair.Value));
            }

            InitEditor();
        };
        FormClosed += (_, _) =>
        {
            Globals.CurrentEditor = -1;
        };
    }

    protected override void GameObjectUpdatedDelegate(GameObjectType type)
    {
        if (type == GameObjectType.Furniture)
        {
            InitEditor();
        }
    }

    public void InitEditor()
    {
        var selectedId = _editorItem?.Id;
        _lstFurniture.Items.Clear();
        foreach (var pair in FurnitureDescriptor.Lookup.OrderBy(p => ((FurnitureDescriptor)p.Value).Name))
        {
            _lstFurniture.Items.Add(new FurnitureListEntry((FurnitureDescriptor)pair.Value));
        }

        if (selectedId.HasValue)
        {
            for (var i = 0; i < _lstFurniture.Items.Count; i++)
            {
                if (((FurnitureListEntry)_lstFurniture.Items[i]).Descriptor.Id == selectedId.Value)
                {
                    _lstFurniture.SelectedIndex = i;
                    break;
                }
            }
        }
    }

    private void RefreshList()
    {
        var selectedId = _editorItem?.Id;
        InitEditor();
        if (selectedId.HasValue)
        {
            AssignSelected();
        }
    }

    private void AssignSelected()
    {
        if (_lstFurniture.SelectedItem is FurnitureListEntry entry)
        {
            _editorItem = entry.Descriptor;
        }
        else
        {
            _editorItem = null;
        }

        UpdateEditor();
    }

    private void UpdateEditor()
    {
        if (_editorItem == null)
        {
            _pnlContainer.Visible = false;
            return;
        }

        _pnlContainer.Visible = true;
        _txtName.Text = _editorItem.Name;
        _cmbSprite.SelectedIndex = _cmbSprite.FindStringExact(string.IsNullOrEmpty(_editorItem.Sprite) ? "Aucun" : _editorItem.Sprite);
        _chkPassable.Checked = _editorItem.Passable;
        _cmbDirection.SelectedItem = _editorItem.DefaultDirection;

        _rdoSpriteEntity.Checked = !_editorItem.UseTilesetSprite;
        _rdoSpriteTileset.Checked = _editorItem.UseTilesetSprite;

        if (_editorItem.TilesetId != Guid.Empty && TilesetDescriptor.TryGet(_editorItem.TilesetId, out var tileset))
        {
            for (var i = 0; i < _cmbTileset.Items.Count; i++)
            {
                if (((TilesetListEntry)_cmbTileset.Items[i]).Tileset.Id == tileset.Id)
                {
                    _cmbTileset.SelectedIndex = i;
                    break;
                }
            }

            LoadPickerTileset(tileset);
        }
        else
        {
            _cmbTileset.SelectedIndex = -1;
            _pickerTileset = null;
            _pickerTilesetImage = null;
        }

        UpdateSpriteSourceVisibility();
        DrawPreview();
    }

    private void UpdateSpriteSourceVisibility()
    {
        var useTileset = _editorItem?.UseTilesetSprite ?? false;
        _cmbSprite.Visible = !useTileset;
        _cmbTileset.Visible = useTileset;
        _picPreview.Visible = !useTileset;
        _pnlTilesetScroll.Visible = useTileset;
        _lblTilesetHint.Visible = useTileset;
    }

    private void DrawPreview()
    {
        var bmp = new Bitmap(_picPreview.Width, _picPreview.Height);
        using var gfx = System.Drawing.Graphics.FromImage(bmp);
        gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, _picPreview.Width, _picPreview.Height));

        var spritePath = "resources/entities/" + (_editorItem?.Sprite ?? string.Empty);
        if (!string.IsNullOrEmpty(_editorItem?.Sprite) && File.Exists(spritePath))
        {
            using var img = Image.FromFile(spritePath);
            var frameWidth = img.Width / Options.Instance.Sprites.NormalFrames;
            var frameHeight = img.Height / Options.Instance.Sprites.Directions;
            gfx.DrawImage(
                img, new Rectangle(0, 0, frameWidth, frameHeight),
                new Rectangle(0, 0, frameWidth, frameHeight), GraphicsUnit.Pixel
            );
        }

        _picPreview.Image = bmp;
    }

    private void LoadPickerTileset(TilesetDescriptor tileset)
    {
        _pickerTileset = tileset;
        _pickerLoadFailed = false;
        _pickerTilesetImage?.Dispose();
        _pickerTilesetImage = null;

        var path = Path.Combine(AppContext.BaseDirectory, "resources", "tilesets", tileset.Name);
        if (!File.Exists(path))
        {
            _pickerLoadFailed = true;
            _picTileset.Invalidate();
            return;
        }

        try
        {
            using var fileImage = Image.FromFile(path);
            _pickerTilesetImage = new Bitmap(fileImage);
        }
        catch
        {
            _pickerLoadFailed = true;
            return;
        }

        _picTileset.Size = new Size(_pickerTilesetImage.Width, _pickerTilesetImage.Height);
        _picTileset.Invalidate();
    }

    private void PicTileset_Paint(object sender, PaintEventArgs e)
    {
        if (_pickerLoadFailed || _pickerTilesetImage == null)
        {
            e.Graphics.Clear(System.Drawing.Color.Black);
            using var errorBrush = new SolidBrush(System.Drawing.Color.OrangeRed);
            e.Graphics.DrawString("Impossible de charger l'image de ce tileset.", Font, errorBrush, 8, 8);

            return;
        }

        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        e.Graphics.DrawImage(_pickerTilesetImage, new Rectangle(0, 0, _pickerTilesetImage.Width, _pickerTilesetImage.Height));

        var tileWidth = Options.Instance.Map.TileWidth;
        var tileHeight = Options.Instance.Map.TileHeight;

        using var gridPen = new Pen(System.Drawing.Color.FromArgb(90, 255, 255, 255));
        for (var x = 0; x <= _pickerTilesetImage.Width; x += tileWidth)
        {
            e.Graphics.DrawLine(gridPen, x, 0, x, _pickerTilesetImage.Height);
        }

        for (var y = 0; y <= _pickerTilesetImage.Height; y += tileHeight)
        {
            e.Graphics.DrawLine(gridPen, 0, y, _pickerTilesetImage.Width, y);
        }

        if (_editorItem == null || _editorItem.TilesetId != _pickerTileset?.Id)
        {
            return;
        }

        using var selectedBrush = new SolidBrush(System.Drawing.Color.FromArgb(120, 60, 180, 255));
        using var selectedPen = new Pen(System.Drawing.Color.FromArgb(220, 100, 200, 255), 2);
        var rect = new Rectangle(_editorItem.TileX * tileWidth, _editorItem.TileY * tileHeight, tileWidth, tileHeight);
        e.Graphics.FillRectangle(selectedBrush, rect);
        e.Graphics.DrawRectangle(selectedPen, rect);
    }

    private void PicTileset_MouseClick(object sender, MouseEventArgs e)
    {
        if (_pickerLoadFailed || _pickerTilesetImage == null || _editorItem == null)
        {
            return;
        }

        var tileWidth = Options.Instance.Map.TileWidth;
        var tileHeight = Options.Instance.Map.TileHeight;
        var tileX = e.X / tileWidth;
        var tileY = e.Y / tileHeight;
        if (tileX < 0 || tileY < 0)
        {
            return;
        }

        _editorItem.TileX = tileX;
        _editorItem.TileY = tileY;
        _picTileset.Invalidate();
    }

    private void Save()
    {
        foreach (var pair in FurnitureDescriptor.Lookup)
        {
            PacketSender.SendSaveObject((FurnitureDescriptor)pair.Value);
        }

        Close();
    }

    private void DeleteSelected()
    {
        if (_editorItem == null)
        {
            return;
        }

        if (DarkMessageBox.ShowWarning(
                "Supprimer ce meuble définitivement ?", "Supprimer",
                DarkDialogButton.YesNo, Icon
            ) == DialogResult.Yes)
        {
            PacketSender.SendDeleteObject(_editorItem);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _pickerTilesetImage?.Dispose();
        }

        base.Dispose(disposing);
    }

    private sealed class FurnitureListEntry(FurnitureDescriptor descriptor)
    {
        public FurnitureDescriptor Descriptor { get; } = descriptor;

        public override string ToString() => Descriptor.Name;
    }

    private sealed class TilesetListEntry(TilesetDescriptor tileset)
    {
        public TilesetDescriptor Tileset { get; } = tileset;

        public override string ToString() => Tileset.Name;
    }
}
