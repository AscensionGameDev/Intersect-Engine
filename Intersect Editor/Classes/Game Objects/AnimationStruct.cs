/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SFML.Graphics;
using Color = SFML.Graphics.Color;

namespace Intersect_Editor.Classes
{
    public class AnimationStruct
    {
        public const string Version = "0.0.0.1";
        public string Name = "";
        public string Sound = "";

        //Lower Animation
        public string LowerAnimSprite = "";
        public int LowerAnimXFrames = 1;
        public int LowerAnimYFrames = 1;
        public int LowerAnimFrameCount = 1;
        public int LowerAnimFrameSpeed = 100;
        public int LowerAnimLoopCount = 1;
        public Light[] LowerLights;

        //Upper Animation
        public string UpperAnimSprite = "";
        public int UpperAnimXFrames = 1;
        public int UpperAnimYFrames = 1;
        public int UpperAnimFrameCount = 1;
        public int UpperAnimFrameSpeed = 100;
        public int UpperAnimLoopCount = 1;
        public Light[] UpperLights;

        public void Load(byte[] packet, int index)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);

            string loadedVersion = myBuffer.ReadString();
            if (loadedVersion != Version)
                throw new Exception("Failed to load Animation #" + index + ". Loaded Version: " + loadedVersion + " Expected Version: " + Version);

            Name = myBuffer.ReadString();
            Sound = myBuffer.ReadString();

            //Lower Animation
            LowerAnimSprite = myBuffer.ReadString();
            LowerAnimXFrames = myBuffer.ReadInteger();
            LowerAnimYFrames = myBuffer.ReadInteger();
            LowerAnimFrameCount = myBuffer.ReadInteger();
            LowerAnimFrameSpeed = myBuffer.ReadInteger();
            LowerAnimLoopCount = myBuffer.ReadInteger();
            LowerLights = new Light[LowerAnimFrameCount];
            for (int i = 0; i < LowerAnimFrameCount; i++)
            {
                LowerLights[i] = new Light(myBuffer);
            }

            //Upper Animation
            UpperAnimSprite = myBuffer.ReadString();
            UpperAnimXFrames = myBuffer.ReadInteger();
            UpperAnimYFrames = myBuffer.ReadInteger();
            UpperAnimFrameCount = myBuffer.ReadInteger();
            UpperAnimFrameSpeed = myBuffer.ReadInteger();
            UpperAnimLoopCount = myBuffer.ReadInteger();
            UpperLights = new Light[UpperAnimFrameCount];
            for (int i = 0; i < UpperAnimFrameCount; i++)
            {
                UpperLights[i] = new Light(myBuffer);
            }

            myBuffer.Dispose();
        }

        public byte[] AnimData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Version);
            myBuffer.WriteString(Name);
            myBuffer.WriteString(Sound);

            //Lower Animation
            myBuffer.WriteString(LowerAnimSprite);
            myBuffer.WriteInteger(LowerAnimXFrames);
            myBuffer.WriteInteger(LowerAnimYFrames);
            myBuffer.WriteInteger(LowerAnimFrameCount);
            myBuffer.WriteInteger(LowerAnimFrameSpeed);
            myBuffer.WriteInteger(LowerAnimLoopCount);
            for (int i = 0; i < LowerAnimFrameCount; i++)
            {
                myBuffer.WriteBytes(LowerLights[i].LightData());
            }

            //Upper Animation
            myBuffer.WriteString(UpperAnimSprite);
            myBuffer.WriteInteger(UpperAnimXFrames);
            myBuffer.WriteInteger(UpperAnimYFrames);
            myBuffer.WriteInteger(UpperAnimFrameCount);
            myBuffer.WriteInteger(UpperAnimFrameSpeed);
            myBuffer.WriteInteger(UpperAnimLoopCount);
            for (int i = 0; i < UpperAnimFrameCount; i++)
            {
                myBuffer.WriteBytes(UpperLights[i].LightData());
            }

            return myBuffer.ToArray();
        }
    }

    public class AnimationInstance
    {
        public AnimationStruct myBase;
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
        public AnimationInstance(AnimationStruct animBase, bool loopForever)
        {
            myBase = animBase;
            lowerLoop = animBase.LowerAnimLoopCount;
            upperLoop = animBase.UpperAnimLoopCount;
            lowerTimer = Environment.TickCount + animBase.LowerAnimFrameSpeed;
            upperTimer = Environment.TickCount + animBase.UpperAnimFrameSpeed;
            infiniteLoop = loopForever;
        }

        public void Draw(RenderTarget target, bool upper = false)
        {
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

            if (!upper)
            {
                //Draw Lower
                if (showLower && EditorGraphics.AnimationFileNames.IndexOf(myBase.LowerAnimSprite) > -1)
                {
                    if (myBase.LowerAnimXFrames > 0 && myBase.LowerAnimYFrames > 0)
                    {
                        Texture tex =
                            EditorGraphics.AnimationTextures[
                                EditorGraphics.AnimationFileNames.IndexOf(myBase.LowerAnimSprite)];
                        int frameWidth = (int)tex.Size.X / myBase.LowerAnimXFrames;
                        int frameHeight = (int)tex.Size.Y / myBase.LowerAnimYFrames;
                        EditorGraphics.RenderTexture(tex,
                            new RectangleF((lowerFrame % myBase.LowerAnimXFrames) * frameWidth,
                                (float)Math.Floor((double)lowerFrame / myBase.LowerAnimXFrames) * frameHeight, frameWidth,
                                frameHeight),
                            new RectangleF(_renderX - frameWidth / 2, _renderY - frameHeight / 2, frameWidth, frameHeight),
                             target, BlendMode.Alpha);
                        EditorGraphics.DrawLight((int)_renderX + myBase.LowerLights[lowerFrame].OffsetX,
                            (int)_renderY + myBase.LowerLights[lowerFrame].OffsetY, myBase.LowerLights[lowerFrame].Size,
                            myBase.LowerLights[lowerFrame].Intensity, myBase.LowerLights[lowerFrame].Expand,
                            myBase.LowerLights[lowerFrame].Color);
                    }
                }
            }
            else
            {
                //Draw Upper
                if (showUpper && EditorGraphics.AnimationFileNames.IndexOf(myBase.UpperAnimSprite) > -1)
                {
                    if (myBase.UpperAnimXFrames > 0 && myBase.UpperAnimYFrames > 0)
                    {
                        Texture tex =
                            EditorGraphics.AnimationTextures[
                                EditorGraphics.AnimationFileNames.IndexOf(myBase.UpperAnimSprite)];
                        int frameWidth = (int)tex.Size.X / myBase.UpperAnimXFrames;
                        int frameHeight = (int)tex.Size.Y / myBase.UpperAnimYFrames;
                        EditorGraphics.RenderTexture(tex,
                            new RectangleF((upperFrame%myBase.UpperAnimXFrames)*frameWidth,
                                (float) Math.Floor((double) upperFrame/myBase.UpperAnimXFrames)*frameHeight, frameWidth,
                                frameHeight),
                            new RectangleF(_renderX - frameWidth/2, _renderY - frameHeight/2, frameWidth, frameHeight),
                            target, BlendMode.Alpha);
                        EditorGraphics.DrawLight((int)_renderX + myBase.UpperLights[lowerFrame].OffsetX,
                            (int)_renderY + myBase.UpperLights[lowerFrame].OffsetY, myBase.UpperLights[lowerFrame].Size,
                            myBase.UpperLights[lowerFrame].Intensity, myBase.UpperLights[lowerFrame].Expand,
                            myBase.UpperLights[lowerFrame].Color);
                    }
                }
            }
        }

        public void SetPosition(float x, float y, int dir)
        {
            _renderX = x;
            _renderY = y;
            _renderDir = dir;
        }

        public void Update()
        {
            if (lowerTimer < Environment.TickCount && showLower)
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
                    lowerTimer = Environment.TickCount + myBase.LowerAnimFrameSpeed;
                }
            }
            if (upperTimer < Environment.TickCount && showUpper)
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
                    upperTimer = Environment.TickCount + myBase.UpperAnimFrameSpeed;
                }
            }
        }
    }
}
