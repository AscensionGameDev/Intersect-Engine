using System;
using System.Windows.Forms;

using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Maps;

using Microsoft.Xna.Framework.Graphics;

using WeifenLuo.WinFormsUI.Docking;

namespace Intersect.Editor.Forms.DockingElements
{

    public partial class FrmMapGrid : DockContent
    {

        //MonoGame Swap Chain
        private SwapChainRenderTarget mChain;

        private bool mDragging;

        private int mDragX;

        private int mDragY;

        private int mPosX;

        private int mPosY;

        private ToolTip mToolTip = new ToolTip();

        private MapGridItem mToolTipItem;

        public FrmMapGrid()
        {
            InitializeComponent();
            pnlMapGrid.MouseWheel += PnlMapGrid_MouseWheel;

            this.Icon = Properties.Resources.Icon;
        }

        private void frmMapGrid_Load(object sender, EventArgs e)
        {
            CreateSwapChain();
            if (Globals.MapGrid == null)
            {
                Globals.MapGrid = new MapGrid(
                    linkMapToolStripMenuItem, unlinkMapToolStripMenuItem, recacheMapToolStripMenuItem, contextMenuStrip
                );
            }

            InitLocalization();
        }

        private void InitLocalization()
        {
            Text = Strings.MapGrid.title;
            btnScreenshotWorld.Text = Strings.MapGrid.screenshotworld;
            btnGridView.Text = Strings.MapGrid.gridlines;
            btnFetchPreview.Text = Strings.MapGrid.preview;
            downloadMissingPreviewsToolStripMenuItem.Text = Strings.MapGrid.downloadmissing;
            reDownloadAllPreviewsToolStripMenuItem.Text = Strings.MapGrid.downloadall;
            unlinkMapToolStripMenuItem.Text = Strings.MapGrid.unlink;
            linkMapToolStripMenuItem.Text = Strings.MapGrid.link;
            recacheMapToolStripMenuItem.Text = Strings.MapGrid.recache;
        }

        public void InitGridWindow()
        {
            CreateSwapChain();
        }

        private void CreateSwapChain()
        {
            if (!Globals.ClosingEditor)
            {
                if (mChain != null)
                {
                    mChain.Dispose();
                }

                if (Graphics.GetGraphicsDevice() != null)
                {
                    if (pnlMapGrid.Width > 0 && pnlMapGrid.Height > 0)
                    {
                        if (pnlMapGrid.Width > 0 && pnlMapGrid.Height > 0)
                        {
                            mChain = new SwapChainRenderTarget(
                                Graphics.GetGraphicsDevice(), pnlMapGrid.Handle, pnlMapGrid.Width, pnlMapGrid.Height,
                                false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents,
                                PresentInterval.Immediate
                            );

                            Graphics.SetMapGridChain(mChain);
                        }
                    }
                }
            }
        }

        private void frmMapGrid_DockStateChanged(object sender, EventArgs e)
        {
            CreateSwapChain();
        }

        private void pnlMapGrid_Resize(object sender, EventArgs e)
        {
            CreateSwapChain();
        }

        private void PnlMapGrid_MouseWheel(object sender, MouseEventArgs e)
        {
            Globals.MapGrid.ZoomIn(e.Delta, e.X, e.Y);
        }

        private void pnlMapGrid_MouseMove(object sender, MouseEventArgs e)
        {
            mPosX = e.X;
            mPosY = e.Y;
            if (mDragging)
            {
                Globals.MapGrid.Move(mDragX - e.X, mDragY - e.Y);
                mDragX = e.X;
                mDragY = e.Y;
            }

            if (mToolTip.Active && mToolTipItem != null)
            {
                if (Globals.MapGrid.GetItemAt(mPosX, mPosY) != mToolTipItem)
                {
                    mToolTip.Hide(pnlMapGrid);
                    mToolTipItem = null;
                }
            }
            else
            {
                mToolTipItem = Globals.MapGrid.GetItemAt(mPosX, mPosY);
                if (mToolTipItem != null)
                {
                    mToolTip.Show(mToolTipItem.Name, pnlMapGrid);
                }
            }
        }

        private void pnlMapGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)
            {
                mDragging = true;
                mDragX = e.X;
                mDragY = e.Y;
            }
            else if (e.Button == MouseButtons.Right)
            {
                Globals.MapGrid.RightClickGrid(e.X, e.Y, pnlMapGrid);
            }
        }

        private void pnlMapGrid_MouseUp(object sender, MouseEventArgs e)
        {
            mDragging = false;
        }

        private void pnlMapGrid_MouseLeave(object sender, EventArgs e)
        {
            if (mToolTip.Active)
            {
                mToolTip.Hide(pnlMapGrid);
                mToolTipItem = null;
            }
        }

        private void pnlMapGrid_MouseHover(object sender, EventArgs e)
        {
        }

        private void btnGridView_Click(object sender, EventArgs e)
        {
            Globals.MapGrid.ShowLines = !Globals.MapGrid.ShowLines;
        }

        private void pnlMapGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Globals.MapGrid.DoubleClick(e.X, e.Y);
        }

        private void downloadMissingPreviewsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.MapGrid.FetchMissingPreviews(false);
        }

        private void reDownloadAllPreviewsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.MapGrid.FetchMissingPreviews(true);
        }

        private void btnScreenshotWorld_Click(object sender, EventArgs e)
        {
            Globals.MapGrid.ScreenshotWorld();
        }

        private void frmMapGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add)
            {
                var args = new MouseEventArgs(MouseButtons.None, 0, mPosX, mPosY, 120);
                PnlMapGrid_MouseWheel(null, args);
            }
            else if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract)
            {
                var args = new MouseEventArgs(MouseButtons.None, 0, mPosX, mPosY, -120);
                PnlMapGrid_MouseWheel(null, args);
            }

            var xDiff = 0;
            var yDiff = 0;
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
            {
                yDiff -= 20;
            }

            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
            {
                yDiff += 20;
            }

            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
            {
                xDiff -= 20;
            }

            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
            {
                xDiff += 20;
            }

            if (xDiff != 0 || yDiff != 0)
            {
                Globals.MapGrid.Move(xDiff, yDiff);
            }
        }

    }

}
