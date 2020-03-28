using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;

namespace Intersect.Client.Interface.Game
{

    class PictureWindow
    {

        //Controls
        private Canvas mGameCanvas;

        private ImagePanel mPicture;

        public PictureWindow(Canvas gameCanvas)
        {
            mGameCanvas = gameCanvas;
            mPicture = new ImagePanel(gameCanvas);
            mPicture.Clicked += MPicture_Clicked;
        }

        public string Picture { get; private set; }

        public int Size { get; private set; }

        public bool Clickable { get; private set; }

        public void Setup(string picture, int size, bool clickable)
        {
            Picture = picture;
            Size = size;
            Clickable = clickable;

            mPicture.Texture = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Image, picture);
            if (mPicture.Texture != null)
            {
                mPicture.SetSize(mPicture.Texture.GetWidth(), mPicture.Texture.GetHeight());
                Align.Center(mPicture);

                if (size != (int) PictureSize.Original) // Don't scale if you want to keep the original size.
                {
                    if (size == (int) PictureSize.StretchToFit)
                    {
                        mPicture.SetSize(mGameCanvas.Width, mGameCanvas.Height);
                        Align.Center(mPicture);
                    }
                    else
                    {
                        var n = 1;

                        //If you want half fullscreen size set n to 2.
                        if (size == (int) PictureSize.HalfScreen)
                        {
                            n = 2;
                        }

                        var ar = (float) mPicture.Width / (float) mPicture.Height;
                        var heightLimit = true;
                        if (mGameCanvas.Width < mGameCanvas.Height * ar)
                        {
                            heightLimit = false;
                        }

                        if (heightLimit)
                        {
                            var height = mGameCanvas.Height;
                            var width = mGameCanvas.Height * ar;
                            mPicture.SetSize((int) (width / n), (int) (height / n));
                            Align.Center(mPicture);
                        }
                        else
                        {
                            var width = mGameCanvas.Width;
                            var height = width / ar;
                            mPicture.SetSize((int) (width / n), (int) (height / n));
                            Align.Center(mPicture);
                        }
                    }
                }

                mPicture.BringToFront();
                mPicture.Show();
            }
            else
            {
                Close();
            }
        }

        private void MPicture_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Clickable)
            {
                Close();
            }
        }

        public void Close()
        {
            if (Picture != null)
            {
                Globals.Picture = null;
                Picture = null;
                mPicture.Hide();
            }
        }

        private enum PictureSize
        {

            Original = 0,

            FullScreen,

            HalfScreen,

            StretchToFit,

        }

    }

}
