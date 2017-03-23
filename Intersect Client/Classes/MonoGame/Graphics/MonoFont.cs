using System;
using Intersect.Logging;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect_Client_MonoGame.Classes.SFML.Graphics
{
    public class MonoFont : GameFont
    {
        private SpriteFont font;

        public MonoFont(string fontName, string fileName, int fontSize, ContentManager contentManager)
            : base(fontName, fontSize)
        {
            try
            {
                fileName = GameContentManager.RemoveExtension(fileName);
                font = contentManager.Load<SpriteFont>(fileName);
            }
            catch (Exception ex)
            {
                Log.Trace(ex);
            }
        }

        public override object GetFont()
        {
            return font;
        }
    }
}