using System;

using Intersect.Client.Core;
using Intersect.Client.Core.Sounds;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.GameObjects;

namespace Intersect.Client.Entities
{

    public partial class Animation
    {

        public bool AutoRotate;

        private bool disposed = false;

        public bool Hidden;

        public bool InfiniteLoop;

        private bool mDisposeNextDraw;

        private int mLowerFrame;

        private int mLowerLoop;

        private long mLowerTimer;

        private Entity mParent;

        private int mRenderDir;

        private float mRenderX;

        private float mRenderY;

        private bool mShowLower = true;

        private bool mShowUpper = true;

        private MapSound mSound;

        private long mStartTime = Globals.System.GetTimeMs();

        private int mUpperFrame;

        private int mUpperLoop;

        private long mUpperTimer;

        public AnimationBase MyBase;

        private int mZDimension = -1;

        public Animation(
            AnimationBase animBase,
            bool loopForever,
            bool autoRotate = false,
            int zDimension = -1,
            Entity parent = null
        )
        {
            MyBase = animBase;
            mParent = parent;
            if (MyBase != null)
            {
                mLowerLoop = animBase.Lower.LoopCount;
                mUpperLoop = animBase.Upper.LoopCount;
                mLowerTimer = Globals.System.GetTimeMs() + animBase.Lower.FrameSpeed;
                mUpperTimer = Globals.System.GetTimeMs() + animBase.Upper.FrameSpeed;
                InfiniteLoop = loopForever;
                AutoRotate = autoRotate;
                mZDimension = zDimension;
                mSound = Audio.AddMapSound(MyBase.Sound, 0, 0, Guid.Empty, loopForever, 0, 12, parent);
                lock (Graphics.AnimationLock)
                {
                    Graphics.LiveAnimations.Add(this);
                }
            }
            else
            {
                Dispose();
            }
        }

        public void Draw(bool upper = false, bool alternate = false)
        {
            if (Hidden)
            {
                return;
            }

            if (!upper && alternate != MyBase.Lower.AlternateRenderLayer)
            {
                return;
            }

            if (upper && alternate != MyBase.Upper.AlternateRenderLayer)
            {
                return;
            }

            var rotationDegrees = 0f;
            var dontRotate = upper && MyBase.Upper.DisableRotations || !upper && MyBase.Lower.DisableRotations;
            if ((AutoRotate || mRenderDir != -1) && !dontRotate)
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

            if (!upper && mShowLower && mZDimension < 1 || !upper && mShowLower && mZDimension > 0)
            {
                //Draw Lower
                var tex = Globals.ContentManager.GetTexture(
                    GameContentManager.TextureType.Animation, MyBase.Lower.Sprite
                );

                if (tex != null)
                {
                    if (MyBase.Lower.XFrames > 0 && MyBase.Lower.YFrames > 0)
                    {
                        var frameWidth = tex.GetWidth() / MyBase.Lower.XFrames;
                        var frameHeight = tex.GetHeight() / MyBase.Lower.YFrames;
                        Graphics.DrawGameTexture(
                            tex,
                            new FloatRect(
                                mLowerFrame % MyBase.Lower.XFrames * frameWidth,
                                (float) Math.Floor((double) mLowerFrame / MyBase.Lower.XFrames) * frameHeight,
                                frameWidth, frameHeight
                            ),
                            new FloatRect(
                                mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth, frameHeight
                            ), Intersect.Color.White, null, GameBlendModes.None, null, rotationDegrees
                        );
                    }
                }

                var offsetX = MyBase.Lower.Lights[mLowerFrame].OffsetX;
                var offsetY = MyBase.Lower.Lights[mLowerFrame].OffsetY;
                var offset = RotatePoint(
                    new Point((int) offsetX, (int) offsetY), new Point(0, 0), rotationDegrees + 180
                );

                Graphics.AddLight(
                    (int) mRenderX - offset.X, (int) mRenderY - offset.Y, MyBase.Lower.Lights[mLowerFrame].Size,
                    MyBase.Lower.Lights[mLowerFrame].Intensity, MyBase.Lower.Lights[mLowerFrame].Expand,
                    MyBase.Lower.Lights[mLowerFrame].Color
                );
            }

            if (upper && mShowUpper && mZDimension != 0 || upper && mShowUpper && mZDimension == 0)
            {
                //Draw Upper
                var tex = Globals.ContentManager.GetTexture(
                    GameContentManager.TextureType.Animation, MyBase.Upper.Sprite
                );

                if (tex != null)
                {
                    if (MyBase.Upper.XFrames > 0 && MyBase.Upper.YFrames > 0)
                    {
                        var frameWidth = tex.GetWidth() / MyBase.Upper.XFrames;
                        var frameHeight = tex.GetHeight() / MyBase.Upper.YFrames;

                        Graphics.DrawGameTexture(
                            tex,
                            new FloatRect(
                                mUpperFrame % MyBase.Upper.XFrames * frameWidth,
                                (float) Math.Floor((double) mUpperFrame / MyBase.Upper.XFrames) * frameHeight,
                                frameWidth, frameHeight
                            ),
                            new FloatRect(
                                mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth, frameHeight
                            ), Intersect.Color.White, null, GameBlendModes.None, null, rotationDegrees
                        );
                    }
                }

                var offsetX = MyBase.Upper.Lights[mUpperFrame].OffsetX;
                var offsetY = MyBase.Upper.Lights[mUpperFrame].OffsetY;
                var offset = RotatePoint(
                    new Point((int) offsetX, (int) offsetY), new Point(0, 0), rotationDegrees + 180
                );

                Graphics.AddLight(
                    (int) mRenderX - offset.X, (int) mRenderY - offset.Y, MyBase.Upper.Lights[mUpperFrame].Size,
                    MyBase.Upper.Lights[mUpperFrame].Intensity, MyBase.Upper.Lights[mUpperFrame].Expand,
                    MyBase.Upper.Lights[mUpperFrame].Color
                );
            }
        }

        public void EndDraw()
        {
            if (mDisposeNextDraw)
            {
                Dispose();
            }
        }

        static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            var angleInRadians = angleInDegrees * (Math.PI / 180);
            var cosTheta = Math.Cos(angleInRadians);
            var sinTheta = Math.Sin(angleInRadians);

            return new Point
            {
                X = (int) (cosTheta * (pointToRotate.X - centerPoint.X) -
                           sinTheta * (pointToRotate.Y - centerPoint.Y) +
                           centerPoint.X),
                Y = (int) (sinTheta * (pointToRotate.X - centerPoint.X) +
                           cosTheta * (pointToRotate.Y - centerPoint.Y) +
                           centerPoint.Y)
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
            if (disposed)
            {
                return;
            }

            lock (Graphics.AnimationLock)
            {
                if (mSound != null)
                {
                    mSound.Loop = false;
                    if (!MyBase.CompleteSound)
                    {
                        mSound.Stop();
                    }

                    mSound = null;
                }

                Graphics.LiveAnimations.Remove(this);
                disposed = true;
            }
        }

        public void DisposeNextDraw()
        {
            mDisposeNextDraw = true;
        }

        public bool Disposed()
        {
            return disposed;
        }

        public void SetPosition(float worldX, float worldY, int mapx, int mapy, Guid mapId, int dir, int z = 0)
        {
            mRenderX = worldX;
            mRenderY = worldY;
            if (mSound != null)
            {
                mSound.UpdatePosition(mapx, mapy, mapId);
            }

            if (dir > -1)
            {
                mRenderDir = dir;
            }

            mZDimension = z;
        }

        public void Update()
        {
            if (disposed)
            {
                return;
            }

            if (MyBase != null)
            {
                if (mSound != null)
                {
                    mSound.Update();
                }

                //Calculate Frames
                var elapsedTime = Globals.System.GetTimeMs() - mStartTime;

                //Lower
                if (MyBase.Lower.FrameCount > 0 && MyBase.Lower.FrameSpeed > 0)
                {
                    var realFrameCount = Math.Min(MyBase.Lower.FrameCount, MyBase.Lower.XFrames * MyBase.Lower.YFrames);
                    var lowerFrame = (int) Math.Floor(elapsedTime / (float) MyBase.Lower.FrameSpeed);
                    var lowerLoops = (int) Math.Floor(lowerFrame / (float) realFrameCount);
                    if (lowerLoops > mLowerLoop && !InfiniteLoop)
                    {
                        mShowLower = false;
                    }
                    else
                    {
                        mLowerFrame = lowerFrame - lowerLoops * realFrameCount;
                    }
                }

                //Upper
                if (MyBase.Upper.FrameCount > 0 && MyBase.Upper.FrameSpeed > 0)
                {
                    var realFrameCount = Math.Min(MyBase.Upper.FrameCount, MyBase.Upper.XFrames * MyBase.Upper.YFrames);
                    var upperFrame = (int) Math.Floor(elapsedTime / (float) MyBase.Upper.FrameSpeed);
                    var upperLoops = (int) Math.Floor(upperFrame / (float) realFrameCount);
                    if (upperLoops > mUpperLoop && !InfiniteLoop)
                    {
                        mShowUpper = false;
                    }
                    else
                    {
                        mUpperFrame = upperFrame - upperLoops * realFrameCount;
                    }
                }

                if (!mShowLower && !mShowUpper)
                {
                    Dispose();
                }
            }
        }

        public Point AnimationSize()
        {
            var size = new Point(0, 0);

            var tex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Animation, MyBase.Lower.Sprite);
            if (tex != null)
            {
                if (MyBase.Lower.XFrames > 0 && MyBase.Lower.YFrames > 0)
                {
                    var frameWidth = tex.GetWidth() / MyBase.Lower.XFrames;
                    var frameHeight = tex.GetHeight() / MyBase.Lower.YFrames;
                    if (frameWidth > size.X)
                    {
                        size.X = frameWidth;
                    }

                    if (frameHeight > size.Y)
                    {
                        size.Y = frameHeight;
                    }
                }
            }

            tex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Animation, MyBase.Upper.Sprite);
            if (tex != null)
            {
                if (MyBase.Upper.XFrames > 0 && MyBase.Upper.YFrames > 0)
                {
                    var frameWidth = tex.GetWidth() / MyBase.Upper.XFrames;
                    var frameHeight = tex.GetHeight() / MyBase.Upper.YFrames;
                    if (frameWidth > size.X)
                    {
                        size.X = frameWidth;
                    }

                    if (frameHeight > size.Y)
                    {
                        size.Y = frameHeight;
                    }
                }
            }

            foreach (var light in MyBase.Lower.Lights)
            {
                if (light != null)
                {
                    if (light.Size + Math.Abs(light.OffsetX) > size.X)
                    {
                        size.X = light.Size + light.OffsetX;
                    }

                    if (light.Size + Math.Abs(light.OffsetY) > size.Y)
                    {
                        size.Y = light.Size + light.OffsetY;
                    }
                }
            }

            foreach (var light in MyBase.Upper.Lights)
            {
                if (light != null)
                {
                    if (light.Size + Math.Abs(light.OffsetX) > size.X)
                    {
                        size.X = light.Size + light.OffsetX;
                    }

                    if (light.Size + Math.Abs(light.OffsetY) > size.Y)
                    {
                        size.Y = light.Size + light.OffsetY;
                    }
                }
            }

            return size;
        }

        public void SetDir(int dir)
        {
            mRenderDir = dir;
        }

    }

}
