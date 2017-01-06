/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Library.GameObjects;

namespace Intersect_Client.Classes.Entities
{


    public class AnimationInstance
    {
        private AnimationBase myBase;
        private float _renderX = 0;
        private float _renderY = 0;
        private int _renderDir = 0;
        private int lowerFrame;
        private int upperFrame;
        private int lowerLoop;
        private int upperLoop;
        private long lowerTimer;
        private long upperTimer;
        private bool infiniteLoop = false;
        private bool showLower = true;
        private bool showUpper = true;
        private int ZDimension = -1;
        public bool AutoRotate = false;
        public bool Hidden = false;
        public AnimationInstance(AnimationBase animBase, bool loopForever, bool autoRotate = false, int zDimension = -1)
        {
            myBase = animBase;
            if (myBase != null)
            {
                lowerLoop = animBase.LowerAnimLoopCount;
                upperLoop = animBase.UpperAnimLoopCount;
                lowerTimer = Globals.System.GetTimeMS() + animBase.LowerAnimFrameSpeed;
                upperTimer = Globals.System.GetTimeMS() + animBase.UpperAnimFrameSpeed;
                infiniteLoop = loopForever;
                AutoRotate = autoRotate;
                ZDimension = zDimension;
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

            if ((!upper && showLower && ZDimension < 1) || (!upper && showLower && ZDimension > 0))
            {
                //Draw Lower
                GameTexture tex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Animation, myBase.LowerAnimSprite);
                if (tex != null)
                {
                    if (myBase.LowerAnimXFrames > 0 && myBase.LowerAnimYFrames > 0)
                    {
                        int frameWidth = tex.GetWidth()/myBase.LowerAnimXFrames;
                        int frameHeight = tex.GetHeight()/myBase.LowerAnimYFrames;
                        GameGraphics.DrawGameTexture(tex,
                            new FloatRect((lowerFrame%myBase.LowerAnimXFrames)*frameWidth,
                                (float) Math.Floor((double) lowerFrame/myBase.LowerAnimXFrames)*frameHeight,
                                frameWidth,
                                frameHeight),
                            new FloatRect(_renderX - frameWidth/2, _renderY - frameHeight/2, frameWidth, frameHeight),
                            Color.White, null, GameBlendModes.None, null, rotationDegrees);
                    }
                }
                int offsetX = myBase.LowerLights[lowerFrame].OffsetX;
                int offsetY = myBase.LowerLights[lowerFrame].OffsetY;
                var rotationRadians = (float)((Math.PI / 180) * rotationDegrees);
                offsetX = (int)(Math.Cos(rotationRadians) * ((double)offsetX) - (double)Math.Sin(rotationRadians) * ((double)offsetY));
                offsetY = (int)(Math.Sin(rotationRadians) * ((double)offsetX) + (double)Math.Cos(rotationRadians) * ((double)offsetY));
                GameGraphics.AddLight((int)_renderX + offsetX,
                    (int)_renderY + offsetY, myBase.LowerLights[lowerFrame].Size,
                    myBase.LowerLights[lowerFrame].Intensity, myBase.LowerLights[lowerFrame].Expand,
                    myBase.LowerLights[lowerFrame].Color);
            }

            if ((upper && showUpper && ZDimension != 0) || (upper && showUpper && ZDimension == 0))
            {
                //Draw Upper
                GameTexture tex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Animation, myBase.UpperAnimSprite);
                if (tex != null)
                {
                    if (myBase.UpperAnimXFrames > 0 && myBase.UpperAnimYFrames > 0)
                    {
                        int frameWidth = tex.GetWidth()/myBase.UpperAnimXFrames;
                        int frameHeight = tex.GetHeight()/myBase.UpperAnimYFrames;

                        GameGraphics.DrawGameTexture(tex,
                            new FloatRect((upperFrame%myBase.UpperAnimXFrames)*frameWidth,
                                (float) Math.Floor((double) upperFrame/myBase.UpperAnimXFrames)*frameHeight,
                                frameWidth,
                                frameHeight),
                            new FloatRect(_renderX - frameWidth/2, _renderY - frameHeight/2, frameWidth, frameHeight),
                            Color.White, null, GameBlendModes.None, null, rotationDegrees);
                    }
                }
                int offsetX = myBase.LowerLights[lowerFrame].OffsetX;
                int offsetY = myBase.LowerLights[lowerFrame].OffsetY;
                var rotationRadians = (float)((Math.PI / 180) * rotationDegrees);
                offsetX =
                    (int)
                        (Math.Cos(rotationRadians) * ((double)offsetX) -
                            (double)Math.Sin(rotationRadians) * ((double)offsetY));
                offsetY =
                    (int)
                        (Math.Sin(rotationRadians) * ((double)offsetX) +
                            (double)Math.Cos(rotationRadians) * ((double)offsetY));
                GameGraphics.AddLight((int)_renderX + offsetX,
                        (int)_renderY + offsetY, myBase.UpperLights[upperFrame].Size,
                        myBase.UpperLights[upperFrame].Intensity, myBase.UpperLights[upperFrame].Expand,
                        myBase.UpperLights[upperFrame].Color);
            }
        }

        public void Hide()
        {
            Hidden = true;
        }

        public void Show()
        {
            Hidden = false;
        }

        public void Dispose()
        {
            lock (GameGraphics.AnimationLock)
            {
                GameGraphics.LiveAnimations.Remove(this);
            }
        }

        public void SetPosition(float x, float y, int dir, int z = 0)
        {
            _renderX = x;
            _renderY = y;
            if (dir > -1) _renderDir = dir;
            ZDimension = z;
        }

        public void Update()
        {
            if (myBase != null)
            {
                if (lowerTimer < Globals.System.GetTimeMS() && showLower)
                {
                    lowerFrame++;
                    if (lowerFrame >= myBase.LowerAnimFrameCount)
                    {
                        lowerLoop--;
                        lowerFrame = 0;
                        if (lowerLoop < 0)
                        {
                            if (infiniteLoop)
                            {
                                lowerLoop = myBase.LowerAnimLoopCount;
                            }
                            else
                            {
                                showLower = false;
                            }
                        }
                    }
                    lowerTimer = Globals.System.GetTimeMS() + myBase.LowerAnimFrameSpeed;
                }
                if (upperTimer < Globals.System.GetTimeMS() && showUpper)
                {
                    upperFrame++;
                    if (upperFrame >= myBase.UpperAnimFrameCount)
                    {
                        upperLoop--;
                        upperFrame = 0;
                        if (upperLoop < 0)
                        {
                            if (infiniteLoop)
                            {
                                upperLoop = myBase.UpperAnimLoopCount;

                            }
                            else
                            {
                                showUpper = false;
                            }
                        }
                    }
                    upperTimer = Globals.System.GetTimeMS() + myBase.UpperAnimFrameSpeed;
                }
            }
        }

        public void SetDir(int dir)
        {
            _renderDir = dir;
        }
    }
}
