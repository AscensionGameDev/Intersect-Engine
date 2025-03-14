using System.Reflection;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Maps;
using Intersect.Editor.Networking;
using Intersect.Framework.Core.GameObjects.Maps.MapList;
using Graphics = System.Drawing.Graphics;

namespace Intersect.Editor.Forms;


public partial class FrmWarpSelection : Form
{

    private Guid mCurrentMapId = Guid.Empty;

    private int mCurrentX;

    private int mCurrentY;

    private Guid mDrawnMap = Guid.Empty;

    private Image mMapImage;

    private List<Guid>? mRestrictMaps;

    private bool mResult;

    private bool mTileSelection = true;

    public FrmWarpSelection()
    {
        InitializeComponent();
        Icon = Program.Icon;

        InitLocalization();
        mapTreeList1.UpdateMapList(mCurrentMapId);
        pnlMap.Width = Options.Instance.Map.TileWidth * Options.Instance.Map.MapWidth;
        pnlMap.Height = Options.Instance.Map.TileHeight * Options.Instance.Map.MapHeight;
        pnlMap.BackColor = System.Drawing.Color.Black;
        mapTreeList1.SetSelect(NodeDoubleClick);

        typeof(Panel).InvokeMember(
            "DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null,
            pnlMap, new object[] {true}
        );
    }

    public void InitForm(bool tileSelection = true, List<Guid>? restrictMaps = null)
    {
        mapTreeList1.UpdateMapList(mCurrentMapId, restrictMaps);
        mRestrictMaps = restrictMaps;
        if (!tileSelection)
        {
            mTileSelection = false;
            Text = Strings.WarpSelection.mapselectiontitle;
        }
    }

    private void InitLocalization()
    {
        Text = Strings.WarpSelection.title;
        chkAlphabetical.Text = Strings.WarpSelection.alphabetical;
        btnOk.Text = Strings.WarpSelection.okay;
        btnCancel.Text = Strings.WarpSelection.cancel;
        grpMapList.Text = Strings.WarpSelection.maplist;
        grpMapPreview.Text = Strings.WarpSelection.mappreview;
    }

    private void NodeDoubleClick(object sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag is MapListMap mapListMap)
        {
            SelectTile(mapListMap.MapId, mCurrentX, mCurrentY);
        }
    }

    public void SelectTile(Guid mapId, int x, int y)
    {
        if (mCurrentMapId != mapId || x != mCurrentX || y != mCurrentY)
        {
            mCurrentMapId = mapId;
            mCurrentX = x;
            mCurrentY = y;
            mapTreeList1.UpdateMapList(mapId, mRestrictMaps);
            UpdatePreview();
        }

        btnRefreshPreview.Enabled = mCurrentMapId != Guid.Empty;
    }

    private void UpdatePreview()
    {
        if (mCurrentMapId != Guid.Empty)
        {
            if (mCurrentMapId != mDrawnMap)
            {
                var img = Database.LoadMapCacheLegacy(mCurrentMapId, -1);
                if (img != null)
                {
                    mMapImage = img;
                }
                else
                {
                    if (MapInstance.Get(mCurrentMapId) != null)
                    {
                        MapInstance.Get(mCurrentMapId).Delete();
                    }

                    Globals.MapsToFetch = new List<Guid>() {mCurrentMapId};
                    if (!Globals.MapsToScreenshot.Contains(mCurrentMapId))
                    {
                        Globals.MapsToScreenshot.Add(mCurrentMapId);
                    }

                    PacketSender.SendNeedMap(mCurrentMapId);
                    pnlMap.BackgroundImage = null;

                    //Use a timer to check when we have the map.
                    tmrMapCheck.Enabled = true;

                    return;
                }
            }

            var newBitmap = new Bitmap(pnlMap.Width, pnlMap.Height);
            var g = Graphics.FromImage(newBitmap);
            g.DrawImage(
                mMapImage, new Rectangle(0, 0, pnlMap.Width, pnlMap.Height),
                new Rectangle(0, 0, pnlMap.Width, pnlMap.Height), GraphicsUnit.Pixel
            );

            if (mTileSelection)
            {
                g.DrawRectangle(
                    new Pen(System.Drawing.Color.White, 2f),
                    new Rectangle(
                        mCurrentX * Options.Instance.Map.TileWidth, mCurrentY * Options.Instance.Map.TileHeight, Options.Instance.Map.TileWidth,
                        Options.Instance.Map.TileHeight
                    )
                );
            }

            g.Dispose();
            pnlMap.BackgroundImage = newBitmap;
            tmrMapCheck.Enabled = false;
            mDrawnMap = mCurrentMapId;

            return;
        }
        else
        {
            pnlMap.BackgroundImage = null;
        }
    }

    private void chkChronological_CheckedChanged(object sender, EventArgs e)
    {
        mapTreeList1.Chronological = chkAlphabetical.Checked;
        mapTreeList1.UpdateMapList(mCurrentMapId, mRestrictMaps);
    }

    private void frmWarpSelection_Load(object sender, EventArgs e)
    {
        mapTreeList1.BeginInvoke(mapTreeList1.MapListDelegate, mCurrentMapId, mRestrictMaps);
        UpdatePreview();
    }

    private void tmrMapCheck_Tick(object sender, EventArgs e)
    {
        if (mCurrentMapId != Guid.Empty)
        {
            var img = Database.LoadMapCacheLegacy(mCurrentMapId, -1);
            if (img != null)
            {
                UpdatePreview();
                tmrMapCheck.Enabled = false;
                img.Dispose();
            }
        }
        else
        {
            tmrMapCheck.Enabled = false;
        }
    }

    private void pnlMap_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.X >= pnlMap.Width || e.Y >= pnlMap.Height)
        {
            return;
        }

        if (e.X < 0 || e.Y < 0)
        {
            return;
        }

        mCurrentX = (int) Math.Floor((double) e.X / Options.Instance.Map.TileWidth);
        mCurrentY = (int) Math.Floor((double) e.Y / Options.Instance.Map.TileHeight);
        UpdatePreview();
    }

    private void pnlMap_DoubleClick(object sender, EventArgs e)
    {
        btnOk_Click(null, null);
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        if (mCurrentMapId != Guid.Empty)
        {
            mResult = true;
        }

        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    public bool GetResult()
    {
        return mResult;
    }

    public Guid GetMap()
    {
        return mCurrentMapId;
    }

    public int GetX()
    {
        return mCurrentX;
    }

    public int GetY()
    {
        return mCurrentY;
    }

    private void btnRefreshPreview_Click(object sender, EventArgs e)
    {
        if (mCurrentMapId != Guid.Empty)
        {
            mDrawnMap = Guid.Empty;
            Database.ClearMapCache(mCurrentMapId);
            UpdatePreview();
        }
    }

}
