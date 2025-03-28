using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Maps;
using Intersect.Client.General;
using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Client.Maps;

public partial class WeatherParticle : IWeatherParticle, IDisposable
{
    private readonly List<IWeatherParticle> _RemoveParticle;

    private readonly Animation animInstance;

    private readonly Rectangle bounds;

    private readonly float cameraSpawnX;

    private readonly float cameraSpawnY;

    private readonly int originalX;

    private readonly int originalY;

    private readonly Point partSize;

    private readonly long TransmittionTimer;

    private readonly int xVelocity;

    private readonly int yVelocity;

    public float X { get; set; }

    public float Y { get; set; }

    public WeatherParticle(List<IWeatherParticle> RemoveParticle, int xvelocity, int yvelocity, AnimationDescriptor anim)
    {
        TransmittionTimer = Timing.Global.MillisecondsUtc;
        bounds = new Rectangle(0, 0, Graphics.Renderer.ScreenWidth, Graphics.Renderer.ScreenHeight);

        xVelocity = xvelocity;
        yVelocity = yvelocity;

        animInstance = new Animation(anim, true, false);
        var animSize = animInstance.CalculateAnimationSize();
        partSize = animSize;

        if (xVelocity > 0)
        {
            originalX = Globals.Random.Next(
                -Graphics.Renderer.ScreenWidth / 4 - animSize.X,
                Graphics.Renderer.ScreenWidth + animSize.X
            );
        }
        else if (xVelocity < 0)
        {
            originalX = Globals.Random.Next(
                animSize.X, (int)(Graphics.Renderer.ScreenWidth * 1.25f) + animSize.X
            );
        }
        else
        {
            originalX = Globals.Random.Next(-animSize.X, Graphics.Renderer.ScreenWidth + animSize.X);
        }

        if (yVelocity > 0)
        {
            originalY = -animSize.Y;
        }
        else if (yVelocity < 0)
        {
            originalY = Graphics.Renderer.ScreenHeight + animSize.Y;
        }
        else
        {
            originalY = Globals.Random.Next(-animSize.Y, Graphics.Renderer.ScreenHeight + animSize.Y);
            if (xVelocity > 0)
            {
                originalX = -animSize.X;
            }
            else if (xVelocity < 0)
            {
                originalX = Graphics.Renderer.ScreenWidth;
            }
        }

        if (originalX < 0)
        {
            bounds.X = originalX;
            bounds.Width += Math.Abs(originalX);
        }

        if (originalY < 0)
        {
            bounds.Y = originalY;
            bounds.Height += Math.Abs(originalY);
        }

        if (originalY > Graphics.Renderer.ScreenHeight)
        {
            bounds.Height = originalY;
        }

        if (originalX > Graphics.Renderer.ScreenWidth)
        {
            bounds.Width = originalX;
        }

        bounds.X -= animSize.X;
        bounds.Width += animSize.X * 2;
        bounds.Y -= animSize.Y;
        bounds.Height += animSize.Y * 2;

        X = originalX;
        Y = originalY;
        cameraSpawnX = Graphics.Renderer.GetView().Left;
        cameraSpawnY = Graphics.Renderer.GetView().Top;
        _RemoveParticle = RemoveParticle;
    }

    public void Update()
    {
        //Check if out of bounds
        var newBounds = new Rectangle(
            bounds.X + ((int)Graphics.Renderer.GetView().Left - (int)cameraSpawnX),
            bounds.Y + ((int)Graphics.Renderer.GetView().Top - (int)cameraSpawnY), bounds.Width, bounds.Height
        );

        if (!newBounds.IntersectsWith(new Rectangle((int)X, (int)Y, partSize.X, partSize.Y)))
        {
            if (_RemoveParticle.Contains(this))
            {
                throw new Exception();
            }
            _RemoveParticle.Add(this);
        }
        else
        {
            var timeScale = (Timing.Global.MillisecondsUtc - TransmittionTimer) / 10f;
            X = originalX + xVelocity * timeScale;
            Y = originalY + yVelocity * timeScale;
            animInstance.SetPosition(cameraSpawnX + X, cameraSpawnY + Y, -1, -1, Guid.Empty, Direction.None, 0);
            animInstance.Update();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();

        if (!disposing)
        {
            return;
        }

        animInstance.Dispose();
        ReleaseManagedResources();
    }

    protected virtual void ReleaseManagedResources()
    {
    }

    protected virtual void ReleaseUnmanagedResources()
    {
        // TODO release unmanaged resources here
    }

    ~WeatherParticle()
    {
        Dispose(false);
    }
}
