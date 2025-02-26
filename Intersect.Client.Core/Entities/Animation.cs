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

    public AnimationDescriptor? Descriptor { get; set; }

    public AnimationSource Source { get; }

    public Point Size => CalculateAnimationSize();

    private int mZDimension = -1;

    public Animation(
        AnimationDescriptor animationDescriptor,
        bool loopForever,
        bool autoRotate = false,
        int zDimension = -1,
        IEntity? parent = null,
        AnimationSource source = default
    )
    {
        Descriptor = animationDescriptor;
        Source = source;
        mParent = parent;
        if (Descriptor != null)
        {
            mLowerLoop = animationDescriptor.Lower.LoopCount;
            mUpperLoop = animationDescriptor.Upper.LoopCount;
            InfiniteLoop = loopForever;
            AutoRotate = autoRotate;
            mZDimension = zDimension;
            mSound = Audio.AddMapSound(Descriptor.Sound, 0, 0, Guid.Empty, loopForever, 0, 12, parent);
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
        if (Hidden || Descriptor == default)
        {
            return;
        }

        if (!upper && alternate != Descriptor.Lower.AlternateRenderLayer)
        {
            return;
        }

        if (upper && alternate != Descriptor.Upper.AlternateRenderLayer)
        {
            return;
        }

        var rotationDegrees = 0f;
        var dontRotate = upper && Descriptor.Upper.DisableRotations || !upper && Descriptor.Lower.DisableRotations;

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
                Framework.Content.TextureType.Animation, Descriptor.Lower.Sprite
            );

            if (tex != null)
            {
                if (Descriptor.Lower.XFrames > 0 && Descriptor.Lower.YFrames > 0)
                {
                    var frameWidth = tex.Width / Descriptor.Lower.XFrames;
                    var frameHeight = tex.Height / Descriptor.Lower.YFrames;
                    Graphics.DrawGameTexture(
                        tex,
                        new FloatRect(
                            mLowerFrame % Descriptor.Lower.XFrames * frameWidth,
                            (float)Math.Floor((double)mLowerFrame / Descriptor.Lower.XFrames) * frameHeight,
                            frameWidth, frameHeight
                        ),
                        new FloatRect(
                            mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth, frameHeight
                        ), Color.White, null, GameBlendModes.None, null, rotationDegrees
                    );
                }
            }

            var offsetX = Descriptor.Lower.Lights[mLowerFrame].OffsetX;
            var offsetY = Descriptor.Lower.Lights[mLowerFrame].OffsetY;
            var offset = RotatePoint(
                new Point(offsetX, offsetY), new Point(0, 0), rotationDegrees + 180
            );

            Graphics.AddLight(
                (int)mRenderX - offset.X, (int)mRenderY - offset.Y, Descriptor.Lower.Lights[mLowerFrame].Size,
                Descriptor.Lower.Lights[mLowerFrame].Intensity, Descriptor.Lower.Lights[mLowerFrame].Expand,
                Descriptor.Lower.Lights[mLowerFrame].Color
            );
        }

        if (upper && mShowUpper && mZDimension != 0 || upper && mShowUpper && mZDimension == 0)
        {
            //Draw Upper
            var tex = Globals.ContentManager.GetTexture(
                Framework.Content.TextureType.Animation, Descriptor.Upper.Sprite
            );

            if (tex != null)
            {
                if (Descriptor.Upper.XFrames > 0 && Descriptor.Upper.YFrames > 0)
                {
                    var frameWidth = tex.Width / Descriptor.Upper.XFrames;
                    var frameHeight = tex.Height / Descriptor.Upper.YFrames;

                    Graphics.DrawGameTexture(
                        tex,
                        new FloatRect(
                            mUpperFrame % Descriptor.Upper.XFrames * frameWidth,
                            (float)Math.Floor((double)mUpperFrame / Descriptor.Upper.XFrames) * frameHeight,
                            frameWidth, frameHeight
                        ),
                        new FloatRect(
                            mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth, frameHeight
                        ), Color.White, null, GameBlendModes.None, null, rotationDegrees
                    );
                }
            }

            var offsetX = Descriptor.Upper.Lights[mUpperFrame].OffsetX;
            var offsetY = Descriptor.Upper.Lights[mUpperFrame].OffsetY;
            var offset = RotatePoint(
                new Point(offsetX, offsetY), new Point(0, 0), rotationDegrees + 180
            );

            Graphics.AddLight(
                (int)mRenderX - offset.X, (int)mRenderY - offset.Y, Descriptor.Upper.Lights[mUpperFrame].Size,
                Descriptor.Upper.Lights[mUpperFrame].Intensity, Descriptor.Upper.Lights[mUpperFrame].Expand,
                Descriptor.Upper.Lights[mUpperFrame].Color
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
                if (Descriptor?.CompleteSound == false)
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

        if (Descriptor != null)
        {
            if (mSound != null)
            {
                _ = mSound.Update();
            }

            //Calculate Frames
            var elapsedTime = Timing.Global.MillisecondsUtc - mStartTime;

            //Lower
            if (Descriptor.Lower.FrameCount > 0 && Descriptor.Lower.FrameSpeed > 0)
            {
                var realFrameCount = Math.Min(Descriptor.Lower.FrameCount, Descriptor.Lower.XFrames * Descriptor.Lower.YFrames);
                var lowerFrame = (int)Math.Floor(elapsedTime / (float)Descriptor.Lower.FrameSpeed);
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
            if (Descriptor.Upper.FrameCount > 0 && Descriptor.Upper.FrameSpeed > 0)
            {
                var realFrameCount = Math.Min(Descriptor.Upper.FrameCount, Descriptor.Upper.XFrames * Descriptor.Upper.YFrames);
                var upperFrame = (int)Math.Floor(elapsedTime / (float)Descriptor.Upper.FrameSpeed);
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

        if (Descriptor == default)
        {
            return size;
        }

        var tex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Animation, Descriptor.Lower.Sprite);
        if (tex != null)
        {
            if (Descriptor.Lower.XFrames > 0 && Descriptor.Lower.YFrames > 0)
            {
                var frameWidth = tex.Width / Descriptor.Lower.XFrames;
                var frameHeight = tex.Height / Descriptor.Lower.YFrames;
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

        tex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Animation, Descriptor.Upper.Sprite);
        if (tex != null)
        {
            if (Descriptor.Upper.XFrames > 0 && Descriptor.Upper.YFrames > 0)
            {
                var frameWidth = tex.Width / Descriptor.Upper.XFrames;
                var frameHeight = tex.Height / Descriptor.Upper.YFrames;
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

        foreach (var light in Descriptor.Lower.Lights)
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

        foreach (var light in Descriptor.Upper.Lights)
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
