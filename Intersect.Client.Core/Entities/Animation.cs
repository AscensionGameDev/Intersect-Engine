using Intersect.Client.Core;
using Intersect.Client.Core.Sounds;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.General;
using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.GameObjects;
using Intersect.GameObjects.Animations;
using Intersect.Utilities;

namespace Intersect.Client.Entities;

public partial class Animation : IAnimation
{
    public bool AutoRotate { get; set; }

    private bool disposed = false;

    public bool Hidden { get; set; }

    public bool InfiniteLoop { get; set; }

    private bool mDisposeNextDraw;

    private int mLowerFrame;

    private readonly int mLowerLoop;

    private readonly IEntity? mParent;

    private Direction mRenderDir;

    private float mRenderX;

    private float mRenderY;

    private bool mShowLower = true;

    private bool mShowUpper = true;

    private MapSound? mSound;

    private readonly long mStartTime = Timing.Global.MillisecondsUtc;

    private int mUpperFrame;

    private readonly int mUpperLoop;

    private bool mUseExternalRotation;

    private float mExternalRotation;

    public AnimationBase? MyBase { get; set; }

    public AnimationSource Source { get; }

    public Point Size => CalculateAnimationSize();

    private int mZDimension = -1;

    public Animation(
        AnimationBase animBase,
        bool loopForever,
        bool autoRotate = false,
        int zDimension = -1,
        IEntity? parent = null,
        AnimationSource source = default
    )
    {
        MyBase = animBase;
        Source = source;
        mParent = parent;
        if (MyBase != null)
        {
            mLowerLoop = animBase.Lower.LoopCount;
            mUpperLoop = animBase.Upper.LoopCount;
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
        if (Hidden || MyBase == default)
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

        if (mUseExternalRotation)
        {
            rotationDegrees = mExternalRotation;
        }

        if ((AutoRotate || mRenderDir != Direction.None) && !dontRotate && !mUseExternalRotation)
        {
            switch (mRenderDir)
            {
                case Direction.Up:
                    rotationDegrees = 0f;

                    break;
                case Direction.Down:
                    rotationDegrees = 180f;

                    break;
                case Direction.Left:
                    rotationDegrees = 270f;

                    break;
                case Direction.Right:
                    rotationDegrees = 90f;

                    break;
                case Direction.UpLeft:
                    rotationDegrees = 315f;

                    break;
                case Direction.UpRight:
                    rotationDegrees = 45f;

                    break;
                case Direction.DownRight:
                    rotationDegrees = 135f;

                    break;
                case Direction.DownLeft:
                    rotationDegrees = 225f;

                    break;
            }
        }

        if (!upper && mShowLower && mZDimension < 1 || !upper && mShowLower && mZDimension > 0)
        {
            //Draw Lower
            var tex = Globals.ContentManager.GetTexture(
                Framework.Content.TextureType.Animation, MyBase.Lower.Sprite
            );

            if (tex != null)
            {
                if (MyBase.Lower.XFrames > 0 && MyBase.Lower.YFrames > 0)
                {
                    var frameWidth = tex.Width / MyBase.Lower.XFrames;
                    var frameHeight = tex.Height / MyBase.Lower.YFrames;
                    Graphics.DrawGameTexture(
                        tex,
                        new FloatRect(
                            mLowerFrame % MyBase.Lower.XFrames * frameWidth,
                            (float)Math.Floor((double)mLowerFrame / MyBase.Lower.XFrames) * frameHeight,
                            frameWidth, frameHeight
                        ),
                        new FloatRect(
                            mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth, frameHeight
                        ), Color.White, null, GameBlendModes.None, null, rotationDegrees
                    );
                }
            }

            var offsetX = MyBase.Lower.Lights[mLowerFrame].OffsetX;
            var offsetY = MyBase.Lower.Lights[mLowerFrame].OffsetY;
            var offset = RotatePoint(
                new Point(offsetX, offsetY), new Point(0, 0), rotationDegrees + 180
            );

            Graphics.AddLight(
                (int)mRenderX - offset.X, (int)mRenderY - offset.Y, MyBase.Lower.Lights[mLowerFrame].Size,
                MyBase.Lower.Lights[mLowerFrame].Intensity, MyBase.Lower.Lights[mLowerFrame].Expand,
                MyBase.Lower.Lights[mLowerFrame].Color
            );
        }

        if (upper && mShowUpper && mZDimension != 0 || upper && mShowUpper && mZDimension == 0)
        {
            //Draw Upper
            var tex = Globals.ContentManager.GetTexture(
                Framework.Content.TextureType.Animation, MyBase.Upper.Sprite
            );

            if (tex != null)
            {
                if (MyBase.Upper.XFrames > 0 && MyBase.Upper.YFrames > 0)
                {
                    var frameWidth = tex.Width / MyBase.Upper.XFrames;
                    var frameHeight = tex.Height / MyBase.Upper.YFrames;

                    Graphics.DrawGameTexture(
                        tex,
                        new FloatRect(
                            mUpperFrame % MyBase.Upper.XFrames * frameWidth,
                            (float)Math.Floor((double)mUpperFrame / MyBase.Upper.XFrames) * frameHeight,
                            frameWidth, frameHeight
                        ),
                        new FloatRect(
                            mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth, frameHeight
                        ), Color.White, null, GameBlendModes.None, null, rotationDegrees
                    );
                }
            }

            var offsetX = MyBase.Upper.Lights[mUpperFrame].OffsetX;
            var offsetY = MyBase.Upper.Lights[mUpperFrame].OffsetY;
            var offset = RotatePoint(
                new Point(offsetX, offsetY), new Point(0, 0), rotationDegrees + 180
            );

            Graphics.AddLight(
                (int)mRenderX - offset.X, (int)mRenderY - offset.Y, MyBase.Upper.Lights[mUpperFrame].Size,
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

        return new Point {
            X = (int)(cosTheta * (pointToRotate.X - centerPoint.X) -
                       sinTheta * (pointToRotate.Y - centerPoint.Y) +
                       centerPoint.X),
            Y = (int)(sinTheta * (pointToRotate.X - centerPoint.X) +
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

    public bool ParentGone() => mParent is { IsDisposed: true };

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
                if (MyBase?.CompleteSound == false)
                {
                    mSound.Stop();
                }

                mSound = null;
            }

            _ = Graphics.LiveAnimations.Remove(this);
            disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    public void DisposeNextDraw()
    {
        mDisposeNextDraw = true;
    }

    public bool IsDisposed => disposed;

    public void SetPosition(float worldX, float worldY, int mapx, int mapy, Guid mapId, Direction dir, int z = 0)
    {
        mRenderX = worldX;
        mRenderY = worldY;
        mSound?.UpdatePosition(mapx, mapy, mapId);

        if (dir > Direction.None)
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
                _ = mSound.Update();
            }

            //Calculate Frames
            var elapsedTime = Timing.Global.MillisecondsUtc - mStartTime;

            //Lower
            if (MyBase.Lower.FrameCount > 0 && MyBase.Lower.FrameSpeed > 0)
            {
                var realFrameCount = Math.Min(MyBase.Lower.FrameCount, MyBase.Lower.XFrames * MyBase.Lower.YFrames);
                var lowerFrame = (int)Math.Floor(elapsedTime / (float)MyBase.Lower.FrameSpeed);
                var lowerLoops = (int)Math.Floor(lowerFrame / (float)realFrameCount);
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
                var upperFrame = (int)Math.Floor(elapsedTime / (float)MyBase.Upper.FrameSpeed);
                var upperLoops = (int)Math.Floor(upperFrame / (float)realFrameCount);
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

    public Point CalculateAnimationSize()
    {
        var size = new Point(0, 0);

        if (MyBase == default)
        {
            return size;
        }

        var tex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Animation, MyBase.Lower.Sprite);
        if (tex != null)
        {
            if (MyBase.Lower.XFrames > 0 && MyBase.Lower.YFrames > 0)
            {
                var frameWidth = tex.Width / MyBase.Lower.XFrames;
                var frameHeight = tex.Height / MyBase.Lower.YFrames;
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

        tex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Animation, MyBase.Upper.Sprite);
        if (tex != null)
        {
            if (MyBase.Upper.XFrames > 0 && MyBase.Upper.YFrames > 0)
            {
                var frameWidth = tex.Width / MyBase.Upper.XFrames;
                var frameHeight = tex.Height / MyBase.Upper.YFrames;
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

    public void SetDir(Direction dir)
    {
        mRenderDir = dir;
    }

    public void SetRotation(float angleInDegrees)
    {
        mUseExternalRotation = true;
        mExternalRotation = angleInDegrees;
    }

    public void SetRotation(bool toggle)
    {
        mUseExternalRotation = toggle;
    }
}
