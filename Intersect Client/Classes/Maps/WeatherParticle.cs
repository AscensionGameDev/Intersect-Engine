using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Client.Classes.Core;
using Intersect.GameObjects;
using IntersectClientExtras.GenericClasses;
using Intersect_Client.Classes.Entities;
using Intersect_Client.Classes.General;

namespace Intersect.Client.Classes.Maps
{
    public class WeatherParticle
    {
        private long TransmittionTimer;

        private List<WeatherParticle> _RemoveParticle;

        private int originalX;
        private int originalY;
        private float cameraSpawnX;
        private float cameraSpawnY;
        private int xVelocity;
        private int yVelocity;
        private IntersectClientExtras.GenericClasses.Point partSize;
        private Rectangle bounds;
        private AnimationInstance animInstance;

        public float X;
        public float Y;

        public WeatherParticle(List<WeatherParticle> RemoveParticle, int xvelocity, int yvelocity, AnimationBase anim)
        {
            TransmittionTimer = Globals.System.GetTimeMs();
            bounds = new Rectangle(0, 0, GameGraphics.Renderer.GetScreenWidth(), GameGraphics.Renderer.GetScreenHeight());

            xVelocity = xvelocity;
            yVelocity = yvelocity;

            animInstance = new AnimationInstance(anim, true, false);
            var animSize = animInstance.AnimationSize();
            partSize = animSize;

            if (xVelocity > 0)
            {
                originalX = Globals.Random.Next(-GameGraphics.Renderer.GetScreenWidth() / 4 - animSize.X, GameGraphics.Renderer.GetScreenWidth() + animSize.X);
            }
            else if (xVelocity < 0)
            {
                originalX = Globals.Random.Next(animSize.X, (int)(GameGraphics.Renderer.GetScreenWidth() * 1.25f) + animSize.X);
            }
            else
            {
                originalX = Globals.Random.Next(-animSize.X, GameGraphics.Renderer.GetScreenWidth() + animSize.X);
            }

            if (yVelocity > 0)
            {
                originalY = -animSize.Y;
            }
            else if (yVelocity < 0)
            {
                originalY = GameGraphics.Renderer.GetScreenHeight() + animSize.Y;
            }
            else
            {
                originalY = Globals.Random.Next(-animSize.Y, GameGraphics.Renderer.GetScreenHeight() + animSize.Y);
                if (xVelocity > 0)
                {
                    originalX = -animSize.X;
                }
                else if (xVelocity < 0)
                {
                    originalX = GameGraphics.Renderer.GetScreenWidth();
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

            if (originalY > GameGraphics.Renderer.GetScreenHeight())
            {
                bounds.Height = originalY;
            }

            if (originalX > GameGraphics.Renderer.GetScreenWidth())
            {
                bounds.Width = originalX;
            }


            bounds.X -= animSize.X;
            bounds.Width += animSize.X * 2;
            bounds.Y -= animSize.Y;
            bounds.Height += animSize.Y * 2;



            X = originalX;
            Y = originalY;
            cameraSpawnX = GameGraphics.Renderer.GetView().Left;
            cameraSpawnY = GameGraphics.Renderer.GetView().Top;
            _RemoveParticle = RemoveParticle;
        }

        public void Update()
        {
            //Check if out of bounds
            var newBounds = new Rectangle(bounds.X + ((int)GameGraphics.Renderer.GetView().Left - (int)cameraSpawnX), bounds.Y + ((int)GameGraphics.Renderer.GetView().Top - (int)cameraSpawnY), bounds.Width, bounds.Height);
            if (!newBounds.IntersectsWith(new Rectangle((int)X, (int)Y, partSize.X, partSize.Y)))
            {
                _RemoveParticle.Add(this);
            }
            else
            {
                X = originalX + (xVelocity * (int)((Globals.System.GetTimeMs() - TransmittionTimer) / 10f));
                Y = originalY + (yVelocity * (int)((Globals.System.GetTimeMs() - TransmittionTimer) / 10f));
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
