using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.Framework.Gwen.Input;
using Intersect.Client.General;
using Intersect.GameObjects;
using System;

namespace Intersect.Client.Interface.Game.HDV
{
	public class HDVSellItem
	{
		private HDVWindow mWindow;
		private ItemDescWindow mDescWindow;
		public ImagePanel Container;
	
		public ImagePanel Pnl;
		private int mMySlot;

		public HDVSellItem(HDVWindow window, int index, ImagePanel container)
		{
			mWindow = window;
			mMySlot = index;
			Container = container;
			Pnl = new ImagePanel(Container, "HDVSellItemIcon");
			Pnl.HoverEnter += pnl_HoverEnter;
			Pnl.HoverLeave += pnl_HoverLeave;
			Pnl.RightClicked += Pnl_Remove;
			Pnl.DoubleClicked += Pnl_Remove;
		}

		private void pnl_HoverEnter(Base sender, EventArgs arguments)
		{
			if (InputHandler.MouseFocus != null)
			{
				return;
			}
			if (mDescWindow != null)
			{
				mDescWindow.Dispose();
				mDescWindow = null;
			}
			if (mMySlot >= 0)
			{
				if (Globals.Me.Inventory[mMySlot]?.Base != null)
				{

					mDescWindow = new ItemDescWindow(
						Globals.Me.Inventory[mMySlot].Base,
						mWindow.GetSellQuantity(),
						mWindow.X, mWindow.Y,
						Globals.Me.Inventory[mMySlot].StatBuffs,
						Globals.Me.Inventory[mMySlot].Tags,
						Globals.Me.Inventory[mMySlot].StringTags
					);
				}
			}
		}

		private void pnl_HoverLeave(Base sender, EventArgs arguments)
		{
			if (mDescWindow != null)
			{
				mDescWindow.Dispose();
				mDescWindow = null;
			}
		}

		private void Pnl_Remove(Base sender, ClickedEventArgs arguments)
		{
			SetSlot(-1);
			if (mDescWindow != null)
			{
				mDescWindow.Dispose();
				mDescWindow = null;
			}
		}

		public int GetSlot()
		{
			return mMySlot;
		}

		public void SetSlot(int index)
		{
			mMySlot = index;

			if (mMySlot >= 0)
			{
				var item = ItemBase.Get(Globals.Me.Inventory[mMySlot].ItemId);
				if (item != null)
				{
					var itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item, item.Icon);
					if (itemTex != null)
					{
						Pnl.Texture = itemTex;
					}
					else
					{
						if (Pnl.Texture != null)
						{
							Pnl.Texture = null;
						}
					}
				}
				else
				{
					if (Pnl.Texture != null)
					{
						Pnl.Texture = null;
					}
				}
			}
			else
			{
				if (Pnl.Texture != null)
				{
					Pnl.Texture = null;
				}
			}
		}
	}
}

