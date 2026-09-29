using DarkUI.Controls;
using Intersect.Editor.Networking;
using Intersect.Framework.Core.GameObjects.Mapping.Tilesets;

namespace Intersect.Editor.Forms.Editors;

/// <summary>
/// Lets the mapper mark QUADRANTS of a tileset's tiles as always-blocked, once, directly on the
/// tileset image - like RPG Maker's tileset passability grid, but with 4 clickable corners per
/// tile instead of only "fully walkable or fully blocked" - instead of having to paint the
/// Blocked attribute by hand on every map that uses that tile. Click a quadrant to toggle it; red
/// = blocked. A map can still override an individual tile back to walkable with the normal
/// "Walkable" attribute if needed (e.g. a door in an otherwise-blocked wall tileset).
/// </summary>
public class FrmTilesetPassability : Form
{
    private readonly int _tileWidth = Options.Instance.Map.TileWidth;
    private readonly int _tileHeight = Options.Instance.Map.TileHeight;

    private readonly TilesetDescriptor _tileset;

    private PictureBox _picTileset;
    private Panel _pnlScroll;
    private Label _lblInfo;
    private DarkButton _btnSave;
    private DarkButton _btnClose;
    private DarkButton _btnZoomIn;
    private DarkButton _btnZoomOut;
    private DarkButton _btnClearAll;
    private Label _lblZoom;

    private Image _tilesetImage;
    private bool _loadFailed;
    private bool _hasUnsavedChanges;
    private int _zoom = 2;

    private const int MinZoom = 1;
    private const int MaxZoom = 6;

    public FrmTilesetPassability(TilesetDescriptor tileset)
    {
        _tileset = tileset;

        Text = $"Tileset Passability - {tileset.Name}";
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
            Size = new System.Drawing.Size(744, 528),
        };

        _picTileset = new PictureBox
        {
            Location = System.Drawing.Point.Empty,
            SizeMode = PictureBoxSizeMode.Normal,
        };
        _picTileset.Paint += PicTileset_Paint;
        _picTileset.MouseClick += PicTileset_MouseClick;
        _pnlScroll.Controls.Add(_picTileset);

        _lblInfo = new Label
        {
            AutoSize = false,
            ForeColor = System.Drawing.Color.Gainsboro,
            Location = new System.Drawing.Point(8, 542),
            Size = new System.Drawing.Size(430, 40),
            Text = "Clique un coin de case pour le bloquer/débloquer (clic droit = bloquer toute la case).",
        };

        _btnZoomOut = new DarkButton
        {
            Location = new System.Drawing.Point(444, 552),
            Size = new System.Drawing.Size(26, 26),
            Text = "-",
        };
        _btnZoomOut.Click += (_, _) => SetZoom(_zoom - 1);

        _lblZoom = new Label
        {
            AutoSize = false,
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
            ForeColor = System.Drawing.Color.Gainsboro,
            Location = new System.Drawing.Point(472, 554),
            Size = new System.Drawing.Size(36, 22),
            Text = $"x{_zoom}",
        };

        _btnZoomIn = new DarkButton
        {
            Location = new System.Drawing.Point(510, 552),
            Size = new System.Drawing.Size(26, 26),
            Text = "+",
        };
        _btnZoomIn.Click += (_, _) => SetZoom(_zoom + 1);

        _btnClearAll = new DarkButton
        {
            Location = new System.Drawing.Point(544, 552),
            Size = new System.Drawing.Size(100, 26),
            Text = "Tout effacer",
        };
        _btnClearAll.Click += (_, _) => ClearAll();

        _btnSave = new DarkButton
        {
            Location = new System.Drawing.Point(652, 552),
            Size = new System.Drawing.Size(50, 26),
            Text = "Save",
        };
        _btnSave.Click += (_, _) => SaveChanges();

        _btnClose = new DarkButton
        {
            Location = new System.Drawing.Point(708, 552),
            Size = new System.Drawing.Size(44, 26),
            Text = "X",
        };
        _btnClose.Click += (_, _) => Close();

        Controls.Add(_pnlScroll);
        Controls.Add(_lblInfo);
        Controls.Add(_btnZoomOut);
        Controls.Add(_lblZoom);
        Controls.Add(_btnZoomIn);
        Controls.Add(_btnClearAll);
        Controls.Add(_btnSave);
        Controls.Add(_btnClose);

        LoadTilesetImage();
    }

    private void SetZoom(int zoom)
    {
        _zoom = Math.Clamp(zoom, MinZoom, MaxZoom);
        _lblZoom.Text = $"x{_zoom}";

        if (_tilesetImage != null)
        {
            _picTileset.Size = new System.Drawing.Size(_tilesetImage.Width * _zoom, _tilesetImage.Height * _zoom);
        }

        _picTileset.Invalidate();
    }

    private void ClearAll()
    {
        if (_tileset.BlockedTileMasks.Count == 0)
        {
            return;
        }

        var confirm = MessageBox.Show(
            "Effacer TOUT le blocage de ce tileset ? Cette action n'est pas annulable (sauf en ne cliquant pas Enregistrer).",
            "Tout effacer",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        );

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        _tileset.BlockedTileMasks.Clear();
        _hasUnsavedChanges = true;
        _lblInfo.Text = "Tout effacé - clique \"Enregistrer\" pour appliquer.";
        _picTileset.Invalidate();
    }

    private void LoadTilesetImage()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "resources", "tilesets", _tileset.Name);
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

        // Note: we deliberately do NOT assign _picTileset.Image - the tileset is drawn manually
        // in PicTileset_Paint (scaled by the zoom level), to avoid the PictureBox's own
        // unscaled auto-rendering fighting with ours.
        SetZoom(_zoom);
    }

    private void PicTileset_Paint(object sender, PaintEventArgs e)
    {
        if (_loadFailed || _tilesetImage == null)
        {
            e.Graphics.Clear(System.Drawing.Color.Black);
            using var errorBrush = new SolidBrush(System.Drawing.Color.OrangeRed);
            e.Graphics.DrawString(
                "Impossible de charger l'image de ce tileset.", Font, errorBrush, 8, 8
            );

            return;
        }

        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
        e.Graphics.DrawImage(
            _tilesetImage,
            new System.Drawing.Rectangle(0, 0, _tilesetImage.Width * _zoom, _tilesetImage.Height * _zoom)
        );

        var tileWidth = _tileWidth * _zoom;
        var tileHeight = _tileHeight * _zoom;
        var imageWidth = _tilesetImage.Width * _zoom;
        var imageHeight = _tilesetImage.Height * _zoom;

        using var gridPen = new Pen(System.Drawing.Color.FromArgb(90, 255, 255, 255));
        for (var x = 0; x <= imageWidth; x += tileWidth)
        {
            e.Graphics.DrawLine(gridPen, x, 0, x, imageHeight);
        }

        for (var y = 0; y <= imageHeight; y += tileHeight)
        {
            e.Graphics.DrawLine(gridPen, 0, y, imageWidth, y);
        }

        // Faint quadrant dividers so it's clear each tile has 4 independently clickable corners.
        using var quadrantPen = new Pen(System.Drawing.Color.FromArgb(45, 255, 255, 255));
        for (var x = tileWidth / 2; x <= imageWidth; x += tileWidth)
        {
            e.Graphics.DrawLine(quadrantPen, x, 0, x, imageHeight);
        }

        for (var y = tileHeight / 2; y <= imageHeight; y += tileHeight)
        {
            e.Graphics.DrawLine(quadrantPen, 0, y, imageWidth, y);
        }

        using var blockedBrush = new SolidBrush(System.Drawing.Color.FromArgb(150, 220, 30, 30));
        using var blockedPen = new Pen(System.Drawing.Color.FromArgb(220, 255, 60, 60), 1);
        var halfWidth = tileWidth / 2;
        var halfHeight = tileHeight / 2;
        foreach (var (point, mask) in _tileset.BlockedTileMasks)
        {
            var baseX = point.X * tileWidth;
            var baseY = point.Y * tileHeight;

            foreach (var quadrant in QuadrantsOf((TileQuadrant)mask))
            {
                var rect = GetQuadrantRect(baseX, baseY, halfWidth, halfHeight, quadrant);
                e.Graphics.FillRectangle(blockedBrush, rect);
                e.Graphics.DrawRectangle(blockedPen, rect);
            }
        }
    }

    private static IEnumerable<TileQuadrant> QuadrantsOf(TileQuadrant mask)
    {
        if (mask.HasFlag(TileQuadrant.TopLeft))
        {
            yield return TileQuadrant.TopLeft;
        }

        if (mask.HasFlag(TileQuadrant.TopRight))
        {
            yield return TileQuadrant.TopRight;
        }

        if (mask.HasFlag(TileQuadrant.BottomLeft))
        {
            yield return TileQuadrant.BottomLeft;
        }

        if (mask.HasFlag(TileQuadrant.BottomRight))
        {
            yield return TileQuadrant.BottomRight;
        }
    }

    private static System.Drawing.Rectangle GetQuadrantRect(int baseX, int baseY, int halfWidth, int halfHeight, TileQuadrant quadrant) =>
        quadrant switch
        {
            TileQuadrant.TopLeft => new System.Drawing.Rectangle(baseX, baseY, halfWidth, halfHeight),
            TileQuadrant.TopRight => new System.Drawing.Rectangle(baseX + halfWidth, baseY, halfWidth, halfHeight),
            TileQuadrant.BottomLeft => new System.Drawing.Rectangle(baseX, baseY + halfHeight, halfWidth, halfHeight),
            TileQuadrant.BottomRight => new System.Drawing.Rectangle(baseX + halfWidth, baseY + halfHeight, halfWidth, halfHeight),
            _ => System.Drawing.Rectangle.Empty,
        };

    private void PicTileset_MouseClick(object sender, MouseEventArgs e)
    {
        if (_loadFailed || _tilesetImage == null)
        {
            return;
        }

        // Convert from on-screen (zoomed) pixels back to real tileset pixels before figuring
        // out which tile/quadrant was clicked.
        var imageX = e.X / _zoom;
        var imageY = e.Y / _zoom;
        var tileX = imageX / _tileWidth;
        var tileY = imageY / _tileHeight;
        if (tileX < 0 || tileY < 0)
        {
            return;
        }

        if (e.Button == MouseButtons.Right)
        {
            // Right-click: toggle the WHOLE tile at once (all 4 quadrants together).
            var currentlyFullyBlocked = _tileset.GetTileBlockMask(tileX, tileY) == (byte)TileQuadrant.All;
            foreach (TileQuadrant quadrant in Enum.GetValues<TileQuadrant>())
            {
                if (quadrant is TileQuadrant.None or TileQuadrant.All)
                {
                    continue;
                }

                _tileset.SetTileQuadrantBlocked(tileX, tileY, quadrant, !currentlyFullyBlocked);
            }
        }
        else
        {
            // Left-click: toggle just the quadrant that was clicked.
            var withinTileX = imageX - tileX * _tileWidth;
            var withinTileY = imageY - tileY * _tileHeight;
            var quadrant = (withinTileX < _tileWidth / 2, withinTileY < _tileHeight / 2) switch
            {
                (true, true) => TileQuadrant.TopLeft,
                (false, true) => TileQuadrant.TopRight,
                (true, false) => TileQuadrant.BottomLeft,
                (false, false) => TileQuadrant.BottomRight,
            };

            var currentlyBlocked = ((TileQuadrant)_tileset.GetTileBlockMask(tileX, tileY)).HasFlag(quadrant);
            _tileset.SetTileQuadrantBlocked(tileX, tileY, quadrant, !currentlyBlocked);
        }

        _hasUnsavedChanges = true;
        _lblInfo.Text = "Modifications non enregistrées - clique \"Enregistrer\" pour les appliquer à toutes les cartes.";
        _picTileset.Invalidate();
    }

    private void SaveChanges()
    {
        PacketSender.SendSaveObject(_tileset);
        _hasUnsavedChanges = false;
        _lblInfo.Text = "Enregistré. Les cartes utilisant ce tileset appliqueront le blocage au prochain chargement.";
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_hasUnsavedChanges)
        {
            var result = MessageBox.Show(
                "Tu as des modifications non enregistrées. Les enregistrer avant de fermer ?",
                "Modifications non enregistrées",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );

            switch (result)
            {
                case DialogResult.Yes:
                    SaveChanges();
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

        base.OnFormClosing(e);
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
