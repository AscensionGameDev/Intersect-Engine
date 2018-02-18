using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen;
using IntersectClientExtras.Gwen.Control;
using Intersect_Client.Classes.General;

namespace Intersect.Client.Classes.UI
{
	class PictureWindow
	{
		private enum PictureSize
		{
			Original = 0,
			FullScreen,
			HalfScreen,
			StretchToFit,
		}

		//Controls
		private ImagePanel mPicture;

		public PictureWindow(Canvas gameCanvas, string picture, int size, bool clickable)
		{
			mPicture = new ImagePanel(gameCanvas);
			mPicture.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Image, picture);
			if (mPicture.Texture != null)
			{
				mPicture.SetSize(mPicture.Texture.GetWidth(), mPicture.Texture.GetHeight());
				Align.Center(mPicture);

				//Assign it to be clickable if property set
				if (clickable)
				{
					mPicture.Clicked += MPicture_Clicked;
				}

				if (size != (int)PictureSize.Original) // Don't scale if you want to keep the original size.
				{
					if (size == (int)PictureSize.StretchToFit)
					{
						mPicture.SetSize(gameCanvas.Width, gameCanvas.Height);
						Align.Center(mPicture);
					}
					else
					{
						int n = 1;

						//If you want half fullscreen size set n to 2.
						if (size == (int)PictureSize.HalfScreen) n = 2;

						var ar = (float)mPicture.Width / (float)mPicture.Height;
						var heightLimit = true;
						if (gameCanvas.Width < gameCanvas.Height * ar)
							heightLimit = false;

						if (heightLimit)
						{
							var height = gameCanvas.Height;
							var width = gameCanvas.Height * ar;
							mPicture.SetSize((int)(width / n), (int)(height / n));
							Align.Center(mPicture);
						}
						else
						{
							var width = gameCanvas.Width;
							var height = width / ar;
							mPicture.SetSize((int)(width / n), (int)(height / n));
							Align.Center(mPicture);
						}
					}
				}
			}
			else
			{
				Close();
			}
		}

		private void MPicture_Clicked(Base sender, IntersectClientExtras.Gwen.Control.EventArguments.ClickedEventArgs arguments)
		{
			Close();
		}

		public void Close()
		{
			mPicture.Parent.RemoveChild(mPicture, false);
			mPicture.Dispose();
		}
	}
}
