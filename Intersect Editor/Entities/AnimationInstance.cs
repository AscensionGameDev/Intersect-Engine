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
            mLowerLoop = animBase.LowerAnimLoopCount;
            mUpperLoop = animBase.UpperAnimLoopCount;
            mLowerTimer = Globals.System.GetTimeMs() + animBase.LowerAnimFrameSpeed;
            mUpperTimer = Globals.System.GetTimeMs() + animBase.UpperAnimFrameSpeed;
            mInfiniteLoop = loopForever;
        }

        public void Draw(RenderTarget2D target, bool upper = false)
        {
            if (!upper)
            {
                //Draw Lower
                Texture2D tex = GameContentManager.GetTexture(GameContentManager.TextureType.Animation,
                    MyBase.LowerAnimSprite);
                if (mShowLower)
                {
                    if (mLowerFrame >= MyBase.LowerAnimFrameCount) return;
                    if (tex != null)
                    {
                        if (MyBase.LowerAnimXFrames > 0 && MyBase.LowerAnimYFrames > 0)
                        {
                            int frameWidth = (int) tex.Width / MyBase.LowerAnimXFrames;
                            int frameHeight = (int) tex.Height / MyBase.LowerAnimYFrames;
                            EditorGraphics.DrawTexture(tex,
                                new RectangleF((mLowerFrame % MyBase.LowerAnimXFrames) * frameWidth,
                                    (float) Math.Floor((double) mLowerFrame / MyBase.LowerAnimXFrames) * frameHeight,
                                    frameWidth,
                                    frameHeight),
                                new RectangleF(mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth,
                                    frameHeight),
                                System.Drawing.Color.White, target, BlendState.NonPremultiplied);
                        }
                    }
                    EditorGraphics.AddLight(
                        Options.MapWidth * Options.TileWidth + (int) mRenderX + MyBase.LowerLights[mLowerFrame].OffsetX,
                        Options.MapHeight * Options.TileHeight + (int) mRenderY +
                        MyBase.LowerLights[mLowerFrame].OffsetY,
                        MyBase.LowerLights[mLowerFrame]);
                }
            }
            else
            {
                //Draw Upper
                Texture2D tex = GameContentManager.GetTexture(GameContentManager.TextureType.Animation,
                    MyBase.UpperAnimSprite);
                if (mShowUpper)
                {
                    if (mUpperFrame >= MyBase.UpperAnimFrameCount) return;
                    if (tex != null)
                    {
                        if (MyBase.UpperAnimXFrames > 0 && MyBase.UpperAnimYFrames > 0)
                        {
                            int frameWidth = (int) tex.Width / MyBase.UpperAnimXFrames;
                            int frameHeight = (int) tex.Height / MyBase.UpperAnimYFrames;
                            EditorGraphics.DrawTexture(tex,
                                new RectangleF((mUpperFrame % MyBase.UpperAnimXFrames) * frameWidth,
                                    (float) Math.Floor((double) mUpperFrame / MyBase.UpperAnimXFrames) * frameHeight,
                                    frameWidth,
                                    frameHeight),
                                new RectangleF(mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth,
                                    frameHeight),
                                System.Drawing.Color.White, target, BlendState.NonPremultiplied);
                        }
                    }
                    EditorGraphics.AddLight(
                        Options.MapWidth * Options.TileWidth + (int) mRenderX + MyBase.UpperLights[mUpperFrame].OffsetX,
                        Options.MapHeight * Options.TileHeight + (int) mRenderY +
                        MyBase.UpperLights[mUpperFrame].OffsetY,
                        MyBase.UpperLights[mUpperFrame]);
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
                if (mLowerFrame >= MyBase.LowerAnimFrameCount)
                {
                    mLowerLoop--;
                    mLowerFrame = 0;
                    if (mLowerLoop < 0)
                    {
                        if (mInfiniteLoop)
                        {
                            mLowerLoop = MyBase.LowerAnimLoopCount;
                        }
                        else
                        {
                            mShowLower = false;
                        }
                    }
                }
                mLowerTimer = Globals.System.GetTimeMs() + MyBase.LowerAnimFrameSpeed;
            }
            if (mUpperTimer < Globals.System.GetTimeMs() && mShowUpper)
            {
                mUpperFrame++;
                if (mUpperFrame >= MyBase.UpperAnimFrameCount)
                {
                    mUpperLoop--;
                    mUpperFrame = 0;
                    if (mUpperLoop < 0)
                    {
                        if (mInfiniteLoop)
                        {
                            mUpperLoop = MyBase.UpperAnimLoopCount;
                        }
                        else
                        {
                            mShowUpper = false;
                        }
                    }
                }
                mUpperTimer = Globals.System.GetTimeMs() + MyBase.UpperAnimFrameSpeed;
            }
        }
    }
}