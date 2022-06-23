using System;
using System.IO;

using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Logging;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.MonoGame.Graphics
{

    public partial class MonoFont : GameFont
    {

        private SpriteFont mFont;

        public MonoFont(string fontName, string fileName, int fontSize, ContentManager contentManager) : base(
            fontName, fontSize
        )
        {
            try
            {
                var extensionlessFileName = GameContentManager.RemoveExtension(fileName);
                var resolvedFileName = Path.Combine(Environment.CurrentDirectory, extensionlessFileName);
                mFont = contentManager.Load<SpriteFont>(resolvedFileName);
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
