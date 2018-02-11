using System.Collections.Generic;
using System.Linq;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_6.Intersect_Convert_Lib.GameObjects
{
    public class AnimationBase : DatabaseObject
    {
        public new const string DATABASE_TABLE = "animations";
        public new const GameObject OBJECT_TYPE = GameObject.Animation;
        protected static Dictionary<int, DatabaseObject> sObjects = new Dictionary<int, DatabaseObject>();
        public int LowerAnimFrameCount = 1;
        public int LowerAnimFrameSpeed = 100;
        public int LowerAnimLoopCount;

        //Lower Animation
        public string LowerAnimSprite = "";

        public int LowerAnimXFrames = 1;
        public int LowerAnimYFrames = 1;
        public LightBase[] LowerLights;

        public string Name = "New Animation";
        public string Sound = "";
        public int UpperAnimFrameCount = 1;
        public int UpperAnimFrameSpeed = 100;
        public int UpperAnimLoopCount;

        //Upper Animation
        public string UpperAnimSprite = "";

        public int UpperAnimXFrames = 1;
        public int UpperAnimYFrames = 1;
        public LightBase[] UpperLights;

        public AnimationBase(int id) : base(id)
        {
            LowerLights = new LightBase[LowerAnimFrameCount];
            for (int i = 0; i < LowerAnimFrameCount; i++)
            {
                LowerLights[i] = new LightBase();
            }
            UpperLights = new LightBase[UpperAnimFrameCount];
            for (int i = 0; i < UpperAnimFrameCount; i++)
            {
                UpperLights[i] = new LightBase();
            }
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);

            Name = myBuffer.ReadString();
            Sound = myBuffer.ReadString();

            //Lower Animation
            LowerAnimSprite = myBuffer.ReadString();
            LowerAnimXFrames = myBuffer.ReadInteger();
            LowerAnimYFrames = myBuffer.ReadInteger();
            LowerAnimFrameCount = myBuffer.ReadInteger();
            LowerAnimFrameSpeed = myBuffer.ReadInteger();
            LowerAnimLoopCount = myBuffer.ReadInteger();
            LowerLights = new LightBase[LowerAnimFrameCount];
            for (int i = 0; i < LowerAnimFrameCount; i++)
            {
                LowerLights[i] = new LightBase(myBuffer);
            }

            //Upper Animation
            UpperAnimSprite = myBuffer.ReadString();
            UpperAnimXFrames = myBuffer.ReadInteger();
            UpperAnimYFrames = myBuffer.ReadInteger();
            UpperAnimFrameCount = myBuffer.ReadInteger();
            UpperAnimFrameSpeed = myBuffer.ReadInteger();
            UpperAnimLoopCount = myBuffer.ReadInteger();
            UpperLights = new LightBase[UpperAnimFrameCount];
            for (int i = 0; i < UpperAnimFrameCount; i++)
            {
                UpperLights[i] = new LightBase(myBuffer);
            }

            myBuffer.Dispose();
        }

        public byte[] AnimData()
        {
            var myBuffer = new ByteBuffer();
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

        public static AnimationBase GetAnim(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return (AnimationBase) sObjects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return ((AnimationBase) sObjects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return AnimData();
        }

        public override string GetTable()
        {
            return DATABASE_TABLE;
        }

        public override GameObject GetGameObjectType()
        {
            return OBJECT_TYPE;
        }

        public static DatabaseObject Get(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return sObjects[index];
            }
            return null;
        }

        public static int ObjectCount()
        {
            return sObjects.Count;
        }

        public static Dictionary<int, AnimationBase> GetObjects()
        {
            Dictionary<int, AnimationBase> objects = sObjects.ToDictionary(k => k.Key, v => (AnimationBase) v.Value);
            return objects;
        }

        public override void Delete()
        {
            sObjects.Remove(GetId());
        }

        public static void ClearObjects()
        {
            sObjects.Clear();
        }

        public static void AddObject(int index, DatabaseObject obj)
        {
            sObjects.Remove(index);
            sObjects.Add(index, obj);
        }
    }
}