using System;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Logging;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.MonoGame.Graphics
{

    public class MonoFont : GameFont
    {

        private SpriteFont mFont;

        public MonoFont(string fontName, string fileName, int fontSize, ContentManager contentManager) : base(
            fontName, fontSize
        )
        {
            try
            {
                fileName = GameContentManager.RemoveExtension(fileName);
                mFont = contentManager.Load<SpriteFont>(fileName);
            }
            catch (Exception ex)
            {
                Log.Trace(ex);
            }
        }

        public override object GetFont()
        {
            return mFont;
        }

    }

}
