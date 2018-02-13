using System;
using Intersect;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI;
using Color = Intersect.Color;
using Point = IntersectClientExtras.GenericClasses.Point;

namespace Intersect_Client.Classes.Entities
{
    public class ChatBubble
    {
        private GameTexture mBubbleTex;
        private Entity mOwner;
        private Color mRenderColor;
        private long mRenderTimer;
        private string mSourceText;
        private Point[,] mTexSections;
        private string[] mText;
        private Rectangle mTextBounds;
        private Rectangle mTextureBounds;

        public ChatBubble(Entity owner, string text)
        {
            mOwner = owner;
            mSourceText = text;
            mRenderTimer = Globals.System.GetTimeMs() + 5000;
            mBubbleTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Misc, "chatbubble.png");
        }

        public bool Update()
        {
            if (mRenderTimer < Globals.System.GetTimeMs())
            {
                return false;
            }
            return true;
        }

        public float Draw(float yoffset = 0f)
        {
            if (mText == null && mSourceText.Trim().Length > 0)
            {
                mText = Gui.WrapText(mSourceText, 200, GameGraphics.GameFont);
            }
            var y = (int) Math.Ceiling(mOwner.GetTopPos());
            var x = (int) Math.Ceiling(mOwner.GetCenterPos().X);
            if (mTextureBounds.Width == 0)
            {
                //Gotta Calculate Bounds
                for (int i = mText.Length - 1; i > -1; i--)
                {
                    var textSize = GameGraphics.Renderer.MeasureText(mText[i], GameGraphics.GameFont, 1);
                    if (textSize.X > mTextureBounds.Width) mTextureBounds.Width = (int) textSize.X + 16;
                    mTextureBounds.Height += (int) textSize.Y + 2;
                    if (textSize.X > mTextBounds.Width) mTextBounds.Width = (int) textSize.X;
                    mTextBounds.Height += (int) textSize.Y + 2;
                }
                mTextureBounds.Height += 16;
                if (mTextureBounds.Width < 48) mTextureBounds.Width = 48;
                if (mTextureBounds.Height < 32) mTextureBounds.Height = 32;
                mTextureBounds.Width = (int) (Math.Round(mTextureBounds.Width / 8.0) * 8.0);
                mTextureBounds.Height = (int) (Math.Round(mTextureBounds.Height / 8.0) * 8.0);
                if ((mTextureBounds.Width / 8) % 2 != 0) mTextureBounds.Width += 8;
                mTexSections = new Point[mTextureBounds.Width / 8, mTextureBounds.Height / 8];
                for (int x1 = 0; x1 < mTextureBounds.Width / 8; x1++)
                {
                    for (int y1 = 0; y1 < mTextureBounds.Height / 8; y1++)
                    {
                        if (x1 == 0) mTexSections[x1, y1].X = 0;
                        else if (x1 == 1) mTexSections[x1, y1].X = 1;
                        else if (x1 == mTextureBounds.Width / 16 - 1) mTexSections[x1, y1].X = 3;
                        else if (x1 == mTextureBounds.Width / 16) mTexSections[x1, y1].X = 4;
                        else if (x1 == mTextureBounds.Width / 8 - 1) mTexSections[x1, y1].X = 7;
                        else if (x1 == mTextureBounds.Width / 8 - 2) mTexSections[x1, y1].X = 6;
                        else mTexSections[x1, y1].X = 2;

                        if (y1 == 0) mTexSections[x1, y1].Y = 0;
                        else if (y1 == 1) mTexSections[x1, y1].Y = 1;
                        else if (y1 == mTextureBounds.Height / 8 - 1) mTexSections[x1, y1].Y = 3;
                        else if (y1 == mTextureBounds.Height / 8 - 2) mTexSections[x1, y1].Y = 2;
                        else mTexSections[x1, y1].Y = 1;
                    }
                }
            }

            if (mBubbleTex != null)
            {
                //Draw Background if available
                //Draw Top Left
                for (int x1 = 0; x1 < mTextureBounds.Width / 8; x1++)
                {
                    for (int y1 = 0; y1 < mTextureBounds.Height / 8; y1++)
                    {
                        GameGraphics.Renderer.DrawTexture(mBubbleTex,
                            new FloatRect(mTexSections[x1, y1].X * 8, mTexSections[x1, y1].Y * 8, 8, 8),
                            new FloatRect(x - mTextureBounds.Width / 2 + (x1 * 8),
                                y - mTextureBounds.Height - yoffset + (y1 * 8), 8, 8),
                            IntersectClientExtras.GenericClasses.Color.White);
                    }
                }
                for (int i = mText.Length - 1; i > -1; i--)
                {
                    var textSize = GameGraphics.Renderer.MeasureText(mText[i], GameGraphics.GameFont, 1);
                    GameGraphics.Renderer.DrawString(mText[i], GameGraphics.GameFont,
                        (int) (x - mTextureBounds.Width / 2 + (mTextureBounds.Width - textSize.X) / 2f),
                        (int) ((y) - mTextureBounds.Height - yoffset + 8 + (i * 16)), 1,
                        IntersectClientExtras.GenericClasses.Color.FromArgb(CustomColors.ChatBubbleTextColor.ToArgb()), true, null);
                }
            }
            yoffset += mTextureBounds.Height;
            return yoffset;
        }
    }
}