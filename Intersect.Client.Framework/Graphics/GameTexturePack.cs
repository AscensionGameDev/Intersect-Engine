using System.Collections.Generic;
using System.Linq;

namespace Intersect.Client.Framework.Graphics
{
    public static class GameTexturePacks
    {
        private static List<GameTexturePackFrame> mFrames = new List<GameTexturePackFrame>();

        private static Dictionary<string, List<GameTexturePackFrame>> mFrameTypes =
            new Dictionary<string, List<GameTexturePackFrame>>();

        public static void AddFrame(GameTexturePackFrame frame)
        {
            mFrames.Add(frame);

            //find the sub folder
            var sep = new char[] {'/', '\\'};
            var subFolder = frame.Name.Split(sep)[1].ToLower();
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
            return mFrames.Where(p => p.Name.ToLower() == filename).FirstOrDefault();
        }
    }
}
