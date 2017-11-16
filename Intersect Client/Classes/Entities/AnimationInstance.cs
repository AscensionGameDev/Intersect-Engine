using System;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;

namespace Intersect_Client.Classes.Entities
{
    public class AnimationInstance
    {
        private int _renderDir;
        private float _renderX;
        private float _renderY;
        public bool AutoRotate;
        public bool Hidden;
        public bool InfiniteLoop;
        private int lowerFrame;
        private int lowerLoop;
        private long lowerTimer;
        public AnimationBase MyBase;
        private bool showLower = true;
        private bool showUpper = true;
        private MapSound sound;
        private int upperFrame;
        private int upperLoop;
        private long upperTimer;
        private int ZDimension = -1;
        private Entity _parent;

        public AnimationInstance(AnimationBase animBase, bool loopForever, bool autoRotate = false, int zDimension = -1, Entity parent = null)
        {
            MyBase = animBase;
            _parent = parent;
            if (MyBase != null)
            {
                lowerLoop = animBase.LowerAnimLoopCount;
                upperLoop = animBase.UpperAnimLoopCount;
                lowerTimer = Globals.System.GetTimeMS() + animBase.LowerAnimFrameSpeed;
                upperTimer = Globals.System.GetTimeMS() + animBase.UpperAnimFrameSpeed;
                InfiniteLoop = loopForever;
                AutoRotate = autoRotate;
                ZDimension = zDimension;
                sound = GameAudio.AddMapSound(MyBase.Sound, 0, 0, 0, loopForever, 12);
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
            if (AutoRotate || _renderDir != -1)
            {
                switch (_renderDir)
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

            if ((!upper && showLower && ZDimension < 1) || (!upper && showLower && ZDimension > 0))
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
                            new FloatRect((lowerFrame % MyBase.LowerAnimXFrames) * frameWidth,
                                (float) Math.Floor((double) lowerFrame / MyBase.LowerAnimXFrames) * frameHeight,
                                frameWidth,
                                frameHeight),
                            new FloatRect(_renderX - frameWidth / 2, _renderY - frameHeight / 2, frameWidth,
                                frameHeight),
                            Intersect.Color.White, null, GameBlendModes.None, null, rotationDegrees);
                    }
                }
                int offsetX = MyBase.LowerLights[lowerFrame].OffsetX;
                int offsetY = MyBase.LowerLights[lowerFrame].OffsetY;
                var offset = RotatePoint(new Point((int) offsetX, (int) offsetY), new Point(0, 0),
                    rotationDegrees + 180);
                GameGraphics.AddLight((int) _renderX - offset.X,
                    (int) _renderY - offset.Y, MyBase.LowerLights[lowerFrame].Size,
                    MyBase.LowerLights[lowerFrame].Intensity, MyBase.LowerLights[lowerFrame].Expand,
                    MyBase.LowerLights[lowerFrame].Color);
            }

            if ((upper && showUpper && ZDimension != 0) || (upper && showUpper && ZDimension == 0))
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
                            new FloatRect((upperFrame % MyBase.UpperAnimXFrames) * frameWidth,
                                (float) Math.Floor((double) upperFrame / MyBase.UpperAnimXFrames) * frameHeight,
                                frameWidth,
                                frameHeight),
                            new FloatRect(_renderX - frameWidth / 2, _renderY - frameHeight / 2, frameWidth,
                                frameHeight),
                            Intersect.Color.White, null, GameBlendModes.None, null, rotationDegrees);
                    }
                }
                int offsetX = MyBase.UpperLights[upperFrame].OffsetX;
                int offsetY = MyBase.UpperLights[upperFrame].OffsetY;
                var offset = RotatePoint(new Point((int) offsetX, (int) offsetY), new Point(0, 0),
                    rotationDegrees + 180);
                GameGraphics.AddLight((int) _renderX - offset.X,
                    (int) _renderY - offset.Y, MyBase.UpperLights[upperFrame].Size,
                    MyBase.UpperLights[upperFrame].Intensity, MyBase.UpperLights[upperFrame].Expand,
                    MyBase.UpperLights[upperFrame].Color);
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
            if (_parent != null && _parent.IsDisposed())
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            lock (GameGraphics.AnimationLock)
            {
                if (sound != null)
                {
                    sound.Stop();
                    sound = null;
                }
                GameGraphics.LiveAnimations.Remove(this);
            }
        }

        public void SetPosition(float worldX, float worldY, int mapx, int mapy, int map, int dir, int z = 0)
        {
            _renderX = worldX;
            _renderY = worldY;
            if (sound != null)
            {
                sound.UpdatePosition(mapx, mapy, map);
            }
            if (dir > -1) _renderDir = dir;
            ZDimension = z;
        }

        public void Update()
        {
            if (MyBase != null)
            {
                if (sound != null)
                {
                    sound.Update();
                }
                if (lowerTimer < Globals.System.GetTimeMS() && showLower)
                {
                    lowerFrame++;
                    if (lowerFrame >= MyBase.LowerAnimFrameCount)
                    {
                        lowerLoop--;
                        lowerFrame = 0;
                        if (lowerLoop < 0)
                        {
                            if (InfiniteLoop)
                            {
                                lowerLoop = MyBase.LowerAnimLoopCount;
                            }
                            else
                            {
                                showLower = false;
                            }
                        }
                    }
                    lowerTimer = Globals.System.GetTimeMS() + MyBase.LowerAnimFrameSpeed;
                }
                if (upperTimer < Globals.System.GetTimeMS() && showUpper)
                {
                    upperFrame++;
                    if (upperFrame >= MyBase.UpperAnimFrameCount)
                    {
                        upperLoop--;
                        upperFrame = 0;
                        if (upperLoop < 0)
                        {
                            if (InfiniteLoop)
                            {
                                upperLoop = MyBase.UpperAnimLoopCount;
                            }
                            else
                            {
                                showUpper = false;
                            }
                        }
                    }
                    upperTimer = Globals.System.GetTimeMS() + MyBase.UpperAnimFrameSpeed;
                }
                if (!showLower && !showUpper)
                {
                    Dispose();
                }
            }
        }

        public void SetDir(int dir)
        {
            _renderDir = dir;
        }
    }
}