using System;
using System.Drawing;
using Intersect.Editor.ContentManagement;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.GameObjects;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.Entities
{
    public class AnimationInstance
    {
        private int mRenderDir;
        private float mRenderX;
        private float mRenderY;
        private bool mInfiniteLoop;
        private int mLowerFrame;
        private int mLowerLoop;
        private long mLowerTimer;
        public AnimationBase MyBase;
        private bool mShowLower = true;
        private bool mShowUpper = true;
        private int mUpperFrame;
        private int mUpperLoop;
        private long mUpperTimer;

        public AnimationInstance(AnimationBase animBase, bool loopForever)
        {
            MyBase = animBase;
            mLowerLoop = animBase.Lower.LoopCount;
            mUpperLoop = animBase.Upper.LoopCount;
            mLowerTimer = Globals.System.GetTimeMs() + animBase.Lower.FrameSpeed;
            mUpperTimer = Globals.System.GetTimeMs() + animBase.Upper.FrameSpeed;
            mInfiniteLoop = loopForever;
        }

        public void Draw(RenderTarget2D target, bool upper = false)
        {
            if (!upper)
            {
                //Draw Lower
                Texture2D tex = GameContentManager.GetTexture(GameContentManager.TextureType.Animation,
                    MyBase.Lower.Sprite);
                if (mShowLower)
                {
                    if (mLowerFrame >= MyBase.Lower.FrameCount) return;
                    if (tex != null)
                    {
                        if (MyBase.Lower.XFrames > 0 && MyBase.Lower.YFrames > 0)
                        {
                            int frameWidth = (int) tex.Width / MyBase.Lower.XFrames;
                            int frameHeight = (int) tex.Height / MyBase.Lower.YFrames;
                            EditorGraphics.DrawTexture(tex,
                                new RectangleF((mLowerFrame % MyBase.Lower.XFrames) * frameWidth,
                                    (float) Math.Floor((double) mLowerFrame / MyBase.Lower.XFrames) * frameHeight,
                                    frameWidth,
                                    frameHeight),
                                new RectangleF(mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth,
                                    frameHeight),
                                System.Drawing.Color.White, target, BlendState.NonPremultiplied);
                        }
                    }
                    EditorGraphics.AddLight(
                        Options.MapWidth * Options.TileWidth + (int) mRenderX + MyBase.Lower.Lights[mLowerFrame].OffsetX,
                        Options.MapHeight * Options.TileHeight + (int) mRenderY +
                        MyBase.Lower.Lights[mLowerFrame].OffsetY,
                        MyBase.Lower.Lights[mLowerFrame]);
                }
            }
            else
            {
                //Draw Upper
                Texture2D tex = GameContentManager.GetTexture(GameContentManager.TextureType.Animation,
                    MyBase.Upper.Sprite);
                if (mShowUpper)
                {
                    if (mUpperFrame >= MyBase.Upper.FrameCount) return;
                    if (tex != null)
                    {
                        if (MyBase.Upper.XFrames > 0 && MyBase.Upper.YFrames > 0)
                        {
                            int frameWidth = (int) tex.Width / MyBase.Upper.XFrames;
                            int frameHeight = (int) tex.Height / MyBase.Upper.YFrames;
                            EditorGraphics.DrawTexture(tex,
                                new RectangleF((mUpperFrame % MyBase.Upper.XFrames) * frameWidth,
                                    (float) Math.Floor((double) mUpperFrame / MyBase.Upper.XFrames) * frameHeight,
                                    frameWidth,
                                    frameHeight),
                                new RectangleF(mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth,
                                    frameHeight),
                                System.Drawing.Color.White, target, BlendState.NonPremultiplied);
                        }
                    }
                    EditorGraphics.AddLight(
                        Options.MapWidth * Options.TileWidth + (int) mRenderX + MyBase.Upper.Lights[mUpperFrame].OffsetX,
                        Options.MapHeight * Options.TileHeight + (int) mRenderY +
                        MyBase.Upper.Lights[mUpperFrame].OffsetY,
                        MyBase.Upper.Lights[mUpperFrame]);
                }
            }
        }

        public void SetPosition(float x, float y, int dir)
        {
            mRenderX = x;
            mRenderY = y;
            mRenderDir = dir;
        }

        public void Update()
        {
            if (mLowerTimer < Globals.System.GetTimeMs() && mShowLower)
            {
                mLowerFrame++;
                if (mLowerFrame >= MyBase.Lower.FrameCount)
                {
                    mLowerLoop--;
                    mLowerFrame = 0;
                    if (mLowerLoop < 0)
                    {
                        if (mInfiniteLoop)
                        {
                            mLowerLoop = MyBase.Lower.LoopCount;
                        }
                        else
                        {
                            mShowLower = false;
                        }
                    }
                }
                mLowerTimer = Globals.System.GetTimeMs() + MyBase.Lower.FrameSpeed;
            }
            if (mUpperTimer < Globals.System.GetTimeMs() && mShowUpper)
            {
                mUpperFrame++;
                if (mUpperFrame >= MyBase.Upper.FrameCount)
                {
                    mUpperLoop--;
                    mUpperFrame = 0;
                    if (mUpperLoop < 0)
                    {
                        if (mInfiniteLoop)
                        {
                            mUpperLoop = MyBase.Upper.LoopCount;
                        }
                        else
                        {
                            mShowUpper = false;
                        }
                    }
                }
                mUpperTimer = Globals.System.GetTimeMs() + MyBase.Upper.FrameSpeed;
            }
        }
    }
}