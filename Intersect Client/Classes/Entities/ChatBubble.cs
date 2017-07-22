using System;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.UI;
using Color = Intersect.Color;
using Point = IntersectClientExtras.GenericClasses.Point;

namespace Intersect_Client.Classes.Entities
{
    public class ChatBubble
    {
        private GameTexture _bubbleTex;
        private Entity _owner;
        private Color _renderColor;
        private long _renderTimer;
        private Point[,] _texSections;
        private string[] _text;
        private string _sourceText;
        private Rectangle _textBounds;
        private Rectangle _textureBounds;

        public ChatBubble(Entity Owner, string text)
        {
            _owner = Owner;
            _sourceText = text; 
            _renderTimer = Globals.System.GetTimeMS() + 5000;
            _bubbleTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Misc, "chatbubble.png");
        }

        public bool Update()
        {
            if (_renderTimer < Globals.System.GetTimeMS())
            {
                return false;
            }
            return true;
        }

        public float Draw(float yoffset = 0f)
        {
            if (_text == null && _sourceText.Trim().Length > 0)
            {
                _text = Gui.WrapText(_sourceText, 200, GameGraphics.GameFont);
            }
            var y = (int) Math.Ceiling(_owner.GetTopPos());
            var x = (int) Math.Ceiling(_owner.GetCenterPos().X);
            if (_textureBounds.Width == 0)
            {
                //Gotta Calculate Bounds
                for (int i = _text.Length - 1; i > -1; i--)
                {
                    var textSize = GameGraphics.Renderer.MeasureText(_text[i], GameGraphics.GameFont, 1);
                    if (textSize.X > _textureBounds.Width) _textureBounds.Width = (int) textSize.X + 16;
                    _textureBounds.Height += (int) textSize.Y + 2;
                    if (textSize.X > _textBounds.Width) _textBounds.Width = (int) textSize.X;
                    _textBounds.Height += (int) textSize.Y + 2;
                }
                _textureBounds.Height += 16;
                if (_textureBounds.Width < 48) _textureBounds.Width = 48;
                if (_textureBounds.Height < 32) _textureBounds.Height = 32;
                _textureBounds.Width = (int) (Math.Round(_textureBounds.Width / 8.0) * 8.0);
                _textureBounds.Height = (int) (Math.Round(_textureBounds.Height / 8.0) * 8.0);
                if ((_textureBounds.Width / 8) % 2 != 0) _textureBounds.Width += 8;
                _texSections = new Point[_textureBounds.Width / 8, _textureBounds.Height / 8];
                for (int x1 = 0; x1 < _textureBounds.Width / 8; x1++)
                {
                    for (int y1 = 0; y1 < _textureBounds.Height / 8; y1++)
                    {
                        if (x1 == 0) _texSections[x1, y1].X = 0;
                        else if (x1 == 1) _texSections[x1, y1].X = 1;
                        else if (x1 == _textureBounds.Width / 16 - 1) _texSections[x1, y1].X = 3;
                        else if (x1 == _textureBounds.Width / 16) _texSections[x1, y1].X = 4;
                        else if (x1 == _textureBounds.Width / 8 - 1) _texSections[x1, y1].X = 7;
                        else if (x1 == _textureBounds.Width / 8 - 2) _texSections[x1, y1].X = 6;
                        else _texSections[x1, y1].X = 2;

                        if (y1 == 0) _texSections[x1, y1].Y = 0;
                        else if (y1 == 1) _texSections[x1, y1].Y = 1;
                        else if (y1 == _textureBounds.Height / 8 - 1) _texSections[x1, y1].Y = 3;
                        else if (y1 == _textureBounds.Height / 8 - 2) _texSections[x1, y1].Y = 2;
                        else _texSections[x1, y1].Y = 1;
                    }
                }
            }

            if (_bubbleTex != null)
            {
                //Draw Background if available
                //Draw Top Left
                for (int x1 = 0; x1 < _textureBounds.Width / 8; x1++)
                {
                    for (int y1 = 0; y1 < _textureBounds.Height / 8; y1++)
                    {
                        GameGraphics.Renderer.DrawTexture(_bubbleTex,
                            new FloatRect(_texSections[x1, y1].X * 8, _texSections[x1, y1].Y * 8, 8, 8),
                            new FloatRect(x - _textureBounds.Width / 2 + (x1 * 8),
                                y - _textureBounds.Height - yoffset + (y1 * 8), 8, 8),
                            IntersectClientExtras.GenericClasses.Color.White);
                    }
                }
                for (int i = _text.Length - 1; i > -1; i--)
                {
                    var textSize = GameGraphics.Renderer.MeasureText(_text[i], GameGraphics.GameFont, 1);
                    GameGraphics.Renderer.DrawString(_text[i], GameGraphics.GameFont,
                        (int) (x - _textureBounds.Width / 2 + (_textureBounds.Width - textSize.X) / 2f),
                        (int) ((y) - _textureBounds.Height - yoffset + 8 + (i * 16)), 1,
                        IntersectClientExtras.GenericClasses.Color.Black, true, null, false);
                }
            }
            yoffset += _textureBounds.Height;
            return yoffset;
        }
    }
}