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

using System.IO;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using SFML.Graphics;
using SFML.System;
using Color = IntersectClientExtras.GenericClasses.Color;

namespace Intersect_Client.Classes.Bridges_and_Interfaces.SFML.Graphics
{
    public class SfmlShader : GameShader
    {
        private Shader _shader;
        public SfmlShader(string shaderName) : base(shaderName)
        {
            if (Shader.IsAvailable)
            {
                _shader = Shader.FromString(File.ReadAllText(shaderName + "_vert.shader"),
                    File.ReadAllText(shaderName + "_frag.shader"));
            }
            else
            {
                global::System.Windows.Forms.MessageBox.Show("Shaders are not supported on this machine. Intersect must quit.");
            }
        }

        public override void SetFloat(string key, float val)
        {
            _shader.SetParameter(key, val);
        }

        public override void SetInt(string key, int val)
        {
            _shader.SetParameter(key, val);
        }

        public override void SetColor(string key, Color val)
        {
            _shader.SetParameter(key, new global::SFML.Graphics.Color(val.R,val.G,val.B,val.A));
        }

        public override void SetVector2(string key, Pointf val)
        {
            _shader.SetParameter(key, new Vector2f(val.X,val.Y));
        }

        public override object GetShader()
        {
            return _shader;
        }
    }
}
