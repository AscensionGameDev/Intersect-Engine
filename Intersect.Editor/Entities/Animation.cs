using Intersect.Editor.Content;
using Intersect.Framework.Core;
using Intersect.GameObjects;
using Intersect.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.Entities;


public partial class Animation
{

    private bool mInfiniteLoop;

    private int mLowerFrame;

    private int mLowerLoop;

    private long mLowerTimer;

    private int mRenderDir;

    private float mRenderX;

    private float mRenderY;

    private bool mShowLower = true;

    private bool mShowUpper = true;

    private int mUpperFrame;

    private int mUpperLoop;

    private long mUpperTimer;

    public AnimationDescriptor Descriptor;

    public Animation(AnimationDescriptor animationDescriptor, bool loopForever)
    {
        Descriptor = animationDescriptor;
        mLowerLoop = animationDescriptor.Lower.LoopCount;
        mUpperLoop = animationDescriptor.Upper.LoopCount;
        mLowerTimer = Timing.Global.MillisecondsUtc + animationDescriptor.Lower.FrameSpeed;
        mUpperTimer = Timing.Global.MillisecondsUtc + animationDescriptor.Upper.FrameSpeed;
        mInfiniteLoop = loopForever;
    }

    public void Draw(RenderTarget2D target, bool upper = false, bool alternate = false)
    {
        if (!upper && alternate != Descriptor.Lower.AlternateRenderLayer)
        {
            return;
        }

        if (upper && alternate != Descriptor.Upper.AlternateRenderLayer)
        {
            return;
        }

        if (!upper)
        {
            //Draw Lower
            var tex = GameContentManager.GetTexture(GameContentManager.TextureType.Animation, Descriptor.Lower.Sprite);
            if (mShowLower)
            {
                if (mLowerFrame >= Descriptor.Lower.FrameCount)
                {
                    return;
                }

                if (tex != null)
                {
                    if (Descriptor.Lower.XFrames > 0 && Descriptor.Lower.YFrames > 0)
                    {
                        var frameWidth = (int) tex.Width / Descriptor.Lower.XFrames;
                        var frameHeight = (int) tex.Height / Descriptor.Lower.YFrames;
                        Core.Graphics.DrawTexture(
                            tex,
                            new RectangleF(
                                mLowerFrame % Descriptor.Lower.XFrames * frameWidth,
                                (float) Math.Floor((double) mLowerFrame / Descriptor.Lower.XFrames) * frameHeight,
                                frameWidth, frameHeight
                            ),
                            new RectangleF(
                                mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth, frameHeight
                            ), System.Drawing.Color.White, target, BlendState.NonPremultiplied
                        );
                    }
                }

                Core.Graphics.AddLight(
                    Options.Instance.Map.MapWidth * Options.Instance.Map.TileWidth -
                    Core.Graphics.CurrentView.Left +
                    (int) mRenderX +
                    Descriptor.Lower.Lights[mLowerFrame].OffsetX,
                    Options.Instance.Map.MapHeight * Options.Instance.Map.TileHeight -
                    Core.Graphics.CurrentView.Top +
                    (int) mRenderY +
                    Descriptor.Lower.Lights[mLowerFrame].OffsetY, Descriptor.Lower.Lights[mLowerFrame]
                );
            }
        }
        else
        {
            //Draw Upper
            var tex = GameContentManager.GetTexture(GameContentManager.TextureType.Animation, Descriptor.Upper.Sprite);
            if (mShowUpper)
            {
                if (mUpperFrame >= Descriptor.Upper.FrameCount)
                {
                    return;
                }

                if (tex != null)
                {
                    if (Descriptor.Upper.XFrames > 0 && Descriptor.Upper.YFrames > 0)
                    {
                        var frameWidth = (int) tex.Width / Descriptor.Upper.XFrames;
                        var frameHeight = (int) tex.Height / Descriptor.Upper.YFrames;
                        Core.Graphics.DrawTexture(
                            tex,
                            new RectangleF(
                                mUpperFrame % Descriptor.Upper.XFrames * frameWidth,
                                (float) Math.Floor((double) mUpperFrame / Descriptor.Upper.XFrames) * frameHeight,
                                frameWidth, frameHeight
                            ),
                            new RectangleF(
                                mRenderX - frameWidth / 2, mRenderY - frameHeight / 2, frameWidth, frameHeight
                            ), System.Drawing.Color.White, target, BlendState.NonPremultiplied
                        );
                    }
                }

                Core.Graphics.AddLight(
                    Options.Instance.Map.MapWidth * Options.Instance.Map.TileWidth -
                    Core.Graphics.CurrentView.Left +
                    (int) mRenderX +
                    Descriptor.Upper.Lights[mUpperFrame].OffsetX,
                    Options.Instance.Map.MapHeight * Options.Instance.Map.TileHeight -
                    Core.Graphics.CurrentView.Top +
                    (int) mRenderY +
                    Descriptor.Upper.Lights[mUpperFrame].OffsetY, Descriptor.Upper.Lights[mUpperFrame]
                );
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
        if (mLowerTimer < Timing.Global.MillisecondsUtc && mShowLower)
        {
            mLowerFrame++;
            if (mLowerFrame >= Descriptor.Lower.FrameCount)
            {
                mLowerLoop--;
                mLowerFrame = 0;
                if (mLowerLoop < 0)
                {
                    if (mInfiniteLoop)
                    {
                        mLowerLoop = Descriptor.Lower.LoopCount;
                    }
                    else
                    {
                        mShowLower = false;
                    }
                }
            }

            mLowerTimer = Timing.Global.MillisecondsUtc + Descriptor.Lower.FrameSpeed;
        }

        if (mUpperTimer < Timing.Global.MillisecondsUtc && mShowUpper)
        {
            mUpperFrame++;
            if (mUpperFrame >= Descriptor.Upper.FrameCount)
            {
                mUpperLoop--;
                mUpperFrame = 0;
                if (mUpperLoop < 0)
                {
                    if (mInfiniteLoop)
                    {
                        mUpperLoop = Descriptor.Upper.LoopCount;
                    }
                    else
                    {
                        mShowUpper = false;
                    }
                }
            }

            mUpperTimer = Timing.Global.MillisecondsUtc + Descriptor.Upper.FrameSpeed;
        }
    }

}
