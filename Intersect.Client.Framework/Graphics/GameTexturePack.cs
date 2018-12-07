using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Graphics
{
    public class GameTexturePacks
    {
        private static List<GameTexturePackFrame> mFrames = new List<GameTexturePackFrame>();

        public static void AddFrame(GameTexturePackFrame frame)
        {
            mFrames.Add(frame);
        }

        public static GameTexturePackFrame GetFrame(string filename)
        {
            return mFrames.Where(p => p.Filename.ToLower() == filename).FirstOrDefault();
        }
    }

    public class GameTexturePackFrame
    {
        public string Filename { get; set; }
        public Rectangle Rect { get; set; }
        public bool Rotated { get; set; }
        public bool Trimmed { get; set; }
        public Rectangle SourceRect { get; set; }
        public GameTexture PackTexture { get; set; }

        public GameTexturePackFrame(string filename, Rectangle rect, bool rotated, bool trimmed, Rectangle sourceSpriteRect, GameTexture packTexture)
        {
            Filename = filename.Replace('/','\\');
            Rect = rect;
            Rotated = rotated;
            Trimmed = trimmed;
            SourceRect = sourceSpriteRect;
            PackTexture = packTexture;
        }
    }
}
