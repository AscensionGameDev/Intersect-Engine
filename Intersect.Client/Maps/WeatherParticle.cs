using System;
using System.Collections.Generic;

using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Framework;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.General;
using Intersect.GameObjects;

namespace Intersect.Client.Maps
{

    public class WeatherParticle
    {

        private List<WeatherParticle> _RemoveParticle;

        private Animation animInstance;

        private Rectangle bounds;

        private float cameraSpawnX;

        private float cameraSpawnY;

        private int originalX;

        private int originalY;

        private Point partSize;

        private long TransmittionTimer;

        public float X;

        private int xVelocity;

        public float Y;

        private int yVelocity;

        public WeatherParticle(IGameContext gameContext, List<WeatherParticle> RemoveParticle, int xvelocity, int yvelocity, AnimationBase anim)
        {
            TransmittionTimer = Globals.System.GetTimeMs();
            bounds = new Rectangle(0, 0, Graphics.GameRenderer.ScreenWidth, Graphics.GameRenderer.ScreenHeight);

            xVelocity = xvelocity;
            yVelocity = yvelocity;

            animInstance = new Animation(gameContext, anim, true, false);
            var animSize = animInstance.AnimationSize();
            partSize = animSize;

            if (xVelocity > 0)
            {
                originalX = Globals.Random.Next(
                    -Graphics.GameRenderer.ScreenWidth / 4 - animSize.X,
                    Graphics.GameRenderer.ScreenWidth + animSize.X
                );
            }
            else if (xVelocity < 0)
            {
                originalX = Globals.Random.Next(
                    animSize.X, (int) (Graphics.GameRenderer.ScreenWidth * 1.25f) + animSize.X
                );
            }
            else
            {
                originalX = Globals.Random.Next(-animSize.X, Graphics.GameRenderer.ScreenWidth + animSize.X);
            }

            if (yVelocity > 0)
            {
                originalY = -animSize.Y;
            }
            else if (yVelocity < 0)
            {
                originalY = Graphics.GameRenderer.ScreenHeight + animSize.Y;
            }
            else
            {
                originalY = Globals.Random.Next(-animSize.Y, Graphics.GameRenderer.ScreenHeight + animSize.Y);
                if (xVelocity > 0)
                {
                    originalX = -animSize.X;
                }
                else if (xVelocity < 0)
                {
                    originalX = Graphics.GameRenderer.ScreenWidth;
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

            if (originalY > Graphics.GameRenderer.ScreenHeight)
            {
                bounds.Height = originalY;
            }

            if (originalX > Graphics.GameRenderer.ScreenWidth)
            {
                bounds.Width = originalX;
            }

            bounds.X -= animSize.X;
            bounds.Width += animSize.X * 2;
            bounds.Y -= animSize.Y;
            bounds.Height += animSize.Y * 2;

            X = originalX;
            Y = originalY;
            cameraSpawnX = Graphics.GameRenderer.GetView().Left;
            cameraSpawnY = Graphics.GameRenderer.GetView().Top;
            _RemoveParticle = RemoveParticle;
        }

        public void Update()
        {
            //Check if out of bounds
            var newBounds = new Rectangle(
                bounds.X + ((int) Graphics.GameRenderer.GetView().Left - (int) cameraSpawnX),
                bounds.Y + ((int) Graphics.GameRenderer.GetView().Top - (int) cameraSpawnY), bounds.Width, bounds.Height
            );

            if (!newBounds.IntersectsWith(new Rectangle((int) X, (int) Y, partSize.X, partSize.Y)))
            {
                _RemoveParticle.Add(this);
            }
            else
            {
                X = originalX + xVelocity * (int) ((Globals.System.GetTimeMs() - TransmittionTimer) / 10f);
                Y = originalY + yVelocity * (int) ((Globals.System.GetTimeMs() - TransmittionTimer) / 10f);
                animInstance.SetPosition(cameraSpawnX + X, cameraSpawnY + Y, -1, -1, Guid.Empty, -1, 0);
                animInstance.Update();
            }
        }

        public void Dispose()
        {
            animInstance.Dispose();
        }

        ~WeatherParticle()
        {
            Dispose();
        }

    }

}
