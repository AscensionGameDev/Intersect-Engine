using System;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.Entities
{
    public class AnimationInstance
    {
        private int mRenderDir;
        private float mRenderX;
        private float mRenderY;
        public bool AutoRotate;
        public bool Hidden;
        public bool InfiniteLoop;
        private int mLowerFrame;
        private int mLowerLoop;
        private long mLowerTimer;
        public AnimationBase MyBase;
        private bool mShowLower = true;
        private bool mShowUpper = true;
        private MapSound mSound;
        private int mUpperFrame;
        private int mUpperLoop;
        private long mUpperTimer;
        private int mZDimension = -1;
        private Entity mParent;

        public AnimationInstance(AnimationBase animBase, bool loopForever, bool autoRotate = false, int zDimension = -1, Entity parent = null)
        {
            MyBase = animBase;
            mParent = parent;
            if (MyBase != null)
            {
                mLowerLoop = animBase.LowerAnimLoopCount;
                mUpperLoop = animBase.UpperAnimLoopCount;
                mLowerTimer = Globals.System.GetTimeMs() + animBase.LowerAnimFrameSpeed;
                mUpperTimer = Globals.System.GetTimeMs() + animBase.UpperAnimFrameSpeed;
                InfiniteLoop = loopForever;
                AutoRotate = autoRotate;
                mZDimension = zDimension;
                mSound = GameAudio.AddMapSound(MyBase.Sound, 0, 0, 0, loopForever, 12);
                lock (GameGraphics.AnimationLock)
                {
                    GameGraphics.LiveAnimations.Add(this);
                }
            }
            else
            {
                Dispose();
            }
        }

        public void Draw(bool upper = false)
        {
            if (Hidden) return;
            float rotationDegrees = 0f;
            if (AutoRotate || mRenderDir != -1)
            {
                switch (mRenderDir)
                {
                    case 0: //Up
                        rotationDegrees = 0f;
                        break;
                    case 1: //Down
                        rotationDegrees = 180f;
                        break;
                    case 2: //Left
                        rotationDegrees = 270f;
                        break;
                    case 3: //Right
                        rotationDegrees = 90f;
                        break;
                    case 4: //NW
                        rotationDegrees = 315f;
                        break;
                    case 5: //NE
                        rotationDegrees = 45f;
                        break;
                    case 6: //SW
                        rotationDegrees = 225f;
                        break;
                    case 7: //SE
                        rotationDegrees = 135f;
                        break;
                }
            }

            if ((!upper && mShowLower && mZDimension < 1) || (!upper && mShowLower && mZDimension > 0))
            {
                //Draw Lower
                GameTexture tex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Animation,
                    MyBase.LowerAnimSprite);
                if (tex != null)
                {
                    if (MyBase.LowerAnimXFrames > 0 && MyBase.LowerAnimYFrames > 0)
                    {
                        int frameWidth = tex.GetWidth() / MyBase.LowerAnimXFrames;
                        int frameHeight = tex.GetHeight() / MyBase.LowerAnimYFrames;
                        GameGraphics.DrawGameTexture(tex,
                            new FloatRect((mLowerFrame % MyBase.LowerAnimXFrames) * frameWidth,
                                (float) Math.Floor((double) mLowerFrame / MyBase.LowerAnimXFrames) * frameHeight,
                                frameWidth,
                                frameHeight),
                            new FloatRect(mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth,
                                frameHeight),
                            Intersect.Color.White, null, GameBlendModes.None, null, rotationDegrees);
                    }
                }
                int offsetX = MyBase.LowerLights[mLowerFrame].OffsetX;
                int offsetY = MyBase.LowerLights[mLowerFrame].OffsetY;
                var offset = RotatePoint(new Point((int) offsetX, (int) offsetY), new Point(0, 0),
                    rotationDegrees + 180);
                GameGraphics.AddLight((int) mRenderX - offset.X,
                    (int) mRenderY - offset.Y, MyBase.LowerLights[mLowerFrame].Size,
                    MyBase.LowerLights[mLowerFrame].Intensity, MyBase.LowerLights[mLowerFrame].Expand,
                    MyBase.LowerLights[mLowerFrame].Color);
            }

            if ((upper && mShowUpper && mZDimension != 0) || (upper && mShowUpper && mZDimension == 0))
            {
                //Draw Upper
                GameTexture tex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Animation,
                    MyBase.UpperAnimSprite);
                if (tex != null)
                {
                    if (MyBase.UpperAnimXFrames > 0 && MyBase.UpperAnimYFrames > 0)
                    {
                        int frameWidth = tex.GetWidth() / MyBase.UpperAnimXFrames;
                        int frameHeight = tex.GetHeight() / MyBase.UpperAnimYFrames;

                        GameGraphics.DrawGameTexture(tex,
                            new FloatRect((mUpperFrame % MyBase.UpperAnimXFrames) * frameWidth,
                                (float) Math.Floor((double) mUpperFrame / MyBase.UpperAnimXFrames) * frameHeight,
                                frameWidth,
                                frameHeight),
                            new FloatRect(mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth,
                                frameHeight),
                            Intersect.Color.White, null, GameBlendModes.None, null, rotationDegrees);
                    }
                }
                int offsetX = MyBase.UpperLights[mUpperFrame].OffsetX;
                int offsetY = MyBase.UpperLights[mUpperFrame].OffsetY;
                var offset = RotatePoint(new Point((int) offsetX, (int) offsetY), new Point(0, 0),
                    rotationDegrees + 180);
                GameGraphics.AddLight((int) mRenderX - offset.X,
                    (int) mRenderY - offset.Y, MyBase.UpperLights[mUpperFrame].Size,
                    MyBase.UpperLights[mUpperFrame].Intensity, MyBase.UpperLights[mUpperFrame].Expand,
                    MyBase.UpperLights[mUpperFrame].Color);
            }
        }

        static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                     sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                     cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        public void Hide()
        {
            Hidden = true;
        }

        public void Show()
        {
            Hidden = false;
        }

        public bool ParentGone()
        {
            if (mParent != null && mParent.IsDisposed())
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            lock (GameGraphics.AnimationLock)
            {
                if (mSound != null)
                {
                    mSound.Stop();
                    mSound = null;
                }
                GameGraphics.LiveAnimations.Remove(this);
            }
        }

        public void SetPosition(float worldX, float worldY, int mapx, int mapy, int map, int dir, int z = 0)
        {
            mRenderX = worldX;
            mRenderY = worldY;
            if (mSound != null)
            {
                mSound.UpdatePosition(mapx, mapy, map);
            }
            if (dir > -1) mRenderDir = dir;
            mZDimension = z;
        }

        public void Update()
        {
            if (MyBase != null)
            {
                if (mSound != null)
                {
                    mSound.Update();
                }
                if (mLowerTimer < Globals.System.GetTimeMs() && mShowLower)
                {
                    mLowerFrame++;
                    if (mLowerFrame >= MyBase.LowerAnimFrameCount)
                    {
                        mLowerLoop--;
                        mLowerFrame = 0;
                        if (mLowerLoop < 0)
                        {
                            if (InfiniteLoop)
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
                            if (InfiniteLoop)
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
                if (!mShowLower && !mShowUpper)
                {
                    Dispose();
                }
            }
        }

        public void SetDir(int dir)
        {
            mRenderDir = dir;
        }
    }
}