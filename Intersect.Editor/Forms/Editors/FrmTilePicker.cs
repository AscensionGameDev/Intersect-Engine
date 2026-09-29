using DarkUI.Controls;
using Intersect.Framework.Core.GameObjects.Mapping.Tilesets;

namespace Intersect.Editor.Forms.Editors;

/// <summary>
/// Small modal dialog that shows a tileset image so the user can click the exact tile they want, instead of
/// guessing raw X/Y tile coordinates blind. Used by <see cref="FrmDungeon"/> to pick the floor/wall tile for
/// procedurally generated dungeons.
/// </summary>
public class FrmTilePicker : Form
{
    private readonly int _tileWidth = Options.Instance.Map.TileWidth;
    private readonly int _tileHeight = Options.Instance.Map.TileHeight;

    private PictureBox _picTileset;
    private Panel _pnlScroll;
    private Label _lblSelection;
    private DarkButton _btnOk;
    private DarkButton _btnCancel;

    private Image _tilesetImage;
    private bool _loadFailed;

    public int SelectedX { get; private set; }

    public int SelectedY { get; private set; }

    public bool Result { get; private set; }

    public FrmTilePicker(Guid tilesetId, int initialX, int initialY)
    {
        SelectedX = Math.Max(0, initialX);
        SelectedY = Math.Max(0, initialY);

        Text = "Pick a Tile";
        BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new System.Drawing.Size(760, 600);

        _pnlScroll = new Panel
        {
            AutoScroll = true,
            BackColor = System.Drawing.Color.Black,
            Location = new System.Drawing.Point(8, 8),
            Size = new System.Drawing.Size(744, 540),
        };

        _picTileset = new PictureBox
        {
            Location = System.Drawing.Point.Empty,
            SizeMode = PictureBoxSizeMode.AutoSize,
        };
        _picTileset.Paint += PicTileset_Paint;
        _picTileset.MouseClick += PicTileset_MouseClick;
        _pnlScroll.Controls.Add(_picTileset);

        _lblSelection = new Label
        {
            AutoSize = true,
            ForeColor = System.Drawing.Color.Gainsboro,
            Location = new System.Drawing.Point(8, 554),
        };

        _btnOk = new DarkButton
        {
            Location = new System.Drawing.Point(580, 552),
            Size = new System.Drawing.Size(80, 26),
            Text = "OK",
        };
        _btnOk.Click += (_, _) =>
        {
            Result = true;
            Close();
        };

        _btnCancel = new DarkButton
        {
            Location = new System.Drawing.Point(668, 552),
            Size = new System.Drawing.Size(84, 26),
            Text = "Cancel",
        };
        _btnCancel.Click += (_, _) =>
        {
            Result = false;
            Close();
        };

        Controls.Add(_pnlScroll);
        Controls.Add(_lblSelection);
        Controls.Add(_btnOk);
        Controls.Add(_btnCancel);

        LoadTilesetImage(tilesetId);
        UpdateSelectionLabel();
    }

    private void LoadTilesetImage(Guid tilesetId)
    {
        var tileset = TilesetDescriptor.Get(tilesetId);
        if (tileset == null)
        {
            _loadFailed = true;
            return;
        }

        var path = Path.Combine(AppContext.BaseDirectory, "resources", "tilesets", tileset.Name);
        if (!File.Exists(path))
        {
            _loadFailed = true;
            return;
        }

        try
        {
            // Load into an independent copy so the file isn't left locked for the rest of the editor session.
            using var fileImage = Image.FromFile(path);
            _tilesetImage = new Bitmap(fileImage);
        }
        catch
        {
            _loadFailed = true;
            return;
        }

        _picTileset.Image = _tilesetImage;
        _picTileset.Size = _tilesetImage.Size;
    }

    private void PicTileset_Paint(object sender, PaintEventArgs e)
    {
        if (_loadFailed || _tilesetImage == null)
        {
            e.Graphics.Clear(System.Drawing.Color.Black);
            using var errorBrush = new SolidBrush(System.Drawing.Color.OrangeRed);
            e.Graphics.DrawString(
                "Could not load this tileset's image.", Font, errorBrush, 8, 8
            );

            return;
        }

        using var gridPen = new Pen(System.Drawing.Color.FromArgb(90, 255, 255, 255));
        for (var x = 0; x <= _tilesetImage.Width; x += _tileWidth)
        {
            e.Graphics.DrawLine(gridPen, x, 0, x, _tilesetImage.Height);
        }

        for (var y = 0; y <= _tilesetImage.Height; y += _tileHeight)
        {
            e.Graphics.DrawLine(gridPen, 0, y, _tilesetImage.Width, y);
        }

        using var selectionPen = new Pen(System.Drawing.Color.Yellow, 2);
        e.Graphics.DrawRectangle(
            selectionPen, SelectedX * _tileWidth, SelectedY * _tileHeight, _tileWidth - 1, _tileHeight - 1
        );
    }

    private void PicTileset_MouseClick(object sender, MouseEventArgs e)
    {
        if (_loadFailed || _tilesetImage == null)
        {
            return;
        }

        SelectedX = Math.Max(0, e.X / _tileWidth);
        SelectedY = Math.Max(0, e.Y / _tileHeight);
        UpdateSelectionLabel();
        _picTileset.Invalidate();
    }

    private void UpdateSelectionLabel()
    {
        _lblSelection.Text = _loadFailed
            ? "This tileset's image file could not be found."
            : $"Selected tile: ({SelectedX}, {SelectedY}) - click a tile above, then OK.";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _tilesetImage?.Dispose();
        }

        base.Dispose(disposing);
    }
}
