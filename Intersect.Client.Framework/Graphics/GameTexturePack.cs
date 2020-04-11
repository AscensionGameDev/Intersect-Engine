using System.Collections.Generic;
using System.Linq;

using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Graphics
{

    public class GameTexturePacks
    {

        private static List<GameTexturePackFrame> mFrames = new List<GameTexturePackFrame>();

        private static Dictionary<string, List<GameTexturePackFrame>> mFrameTypes =
            new Dictionary<string, List<GameTexturePackFrame>>();

        public static void AddFrame(GameTexturePackFrame frame)
        {
            mFrames.Add(frame);

            //find the sub folder
            var sep = new char[] {'/', '\\'};
            var subFolder = frame.Filename.Split(sep)[1].ToLower();
            if (!mFrameTypes.ContainsKey(subFolder))
            {
                mFrameTypes.Add(subFolder, new List<GameTexturePackFrame>());
            }

            if (!mFrameTypes[subFolder].Contains(frame))
            {
                mFrameTypes[subFolder].Add(frame);
            }
        }

        public static GameTexturePackFrame[] GetFolderFrames(string folder)
        {
            if (mFrameTypes.ContainsKey(folder.ToLower()))
            {
                return mFrameTypes[folder.ToLower()].ToArray();
            }

            return null;
        }

        public static GameTexturePackFrame GetFrame(string filename)
        {
            filename = filename.Replace("\\", "/");
            return mFrames.Where(p => p.Filename.ToLower() == filename).FirstOrDefault();
        }

    }

    public class GameTexturePackFrame
    {

        public GameTexturePackFrame(
            string filename,
            Rectangle rect,
            bool rotated,
            Rectangle sourceSpriteRect,
            GameTexture packTexture
        )
        {
            Filename = filename.Replace('\\', '/');
            Rect = rect;
            Rotated = rotated;
            SourceRect = sourceSpriteRect;
            PackTexture = packTexture;
        }

        public string Filename { get; set; }

        public Rectangle Rect { get; set; }

        public bool Rotated { get; set; }

        public Rectangle SourceRect { get; set; }

        public GameTexture PackTexture { get; set; }

    }

}
