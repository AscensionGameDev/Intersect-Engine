using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects
{
    public class ProjectileBase : DatabaseObject
    {
        public new const string DATABASE_TABLE = "projectiles";
        public new const GameObject OBJECT_TYPE = GameObject.Projectile;

        public const int SpawnLocationsWidth = 5;
        public const int SpawnLocationsHeight = 5;
        public const int MaxProjectileDirections = 8;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();
        public List<ProjectileAnimation> Animations = new List<ProjectileAnimation>();
        public int Delay = 1;
        public bool GrappleHook;
        public bool Homing;
        public bool IgnoreActiveResources;
        public bool IgnoreExhaustedResources;
        public bool IgnoreMapBlocks;
        public bool IgnoreZDimension;
        public int Knockback;

        public string Name = "New Projectile";
        public int Quantity = 1;
        public int Range = 1;
        public Location[,] SpawnLocations = new Location[SpawnLocationsWidth, SpawnLocationsHeight];
        public int Speed = 1;
        public int Spell;

        //Init
        public ProjectileBase(int id) : base(id)
        {
            for (var x = 0; x < SpawnLocationsWidth; x++)
            {
                for (var y = 0; y < SpawnLocationsHeight; y++)
                {
                    SpawnLocations[x, y] = new Location();
                }
            }
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            Speed = myBuffer.ReadInteger();
            Delay = myBuffer.ReadInteger();
            Quantity = myBuffer.ReadInteger();
            Range = myBuffer.ReadInteger();
            Spell = myBuffer.ReadInteger();
            Knockback = myBuffer.ReadInteger();
            IgnoreMapBlocks = Convert.ToBoolean(myBuffer.ReadInteger());
            IgnoreActiveResources = Convert.ToBoolean(myBuffer.ReadInteger());
            IgnoreExhaustedResources = Convert.ToBoolean(myBuffer.ReadInteger());
            IgnoreZDimension = Convert.ToBoolean(myBuffer.ReadInteger());
            Homing = Convert.ToBoolean(myBuffer.ReadInteger());
            GrappleHook = Convert.ToBoolean(myBuffer.ReadInteger());

            for (var x = 0; x < SpawnLocationsWidth; x++)
            {
                for (var y = 0; y < SpawnLocationsHeight; y++)
                {
                    for (var i = 0; i < MaxProjectileDirections; i++)
                    {
                        SpawnLocations[x, y].Directions[i] = Convert.ToBoolean(myBuffer.ReadInteger());
                    }
                }
            }

            // Load Animations
            Animations.Clear();
            var animCount = myBuffer.ReadInteger();
            for (var i = 0; i < animCount; i++)
            {
                Animations.Add(new ProjectileAnimation(myBuffer.ReadInteger(), myBuffer.ReadInteger(),
                    Convert.ToBoolean(myBuffer.ReadInteger())));
            }

            //If no animations present.
            if (animCount <= 0)
            {
                Animations.Add(new ProjectileAnimation(-1, Quantity, false));
            }

            myBuffer.Dispose();
        }

        public byte[] ProjectileData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteInteger(Speed);
            myBuffer.WriteInteger(Delay);
            myBuffer.WriteInteger(Quantity);
            myBuffer.WriteInteger(Range);
            myBuffer.WriteInteger(Spell);
            myBuffer.WriteInteger(Knockback);
            myBuffer.WriteInteger(Convert.ToInt32(IgnoreMapBlocks));
            myBuffer.WriteInteger(Convert.ToInt32(IgnoreActiveResources));
            myBuffer.WriteInteger(Convert.ToInt32(IgnoreExhaustedResources));
            myBuffer.WriteInteger(Convert.ToInt32(IgnoreZDimension));
            myBuffer.WriteInteger(Convert.ToInt32(Homing));
            myBuffer.WriteInteger(Convert.ToInt32(GrappleHook));

            for (var x = 0; x < SpawnLocationsWidth; x++)
            {
                for (var y = 0; y < SpawnLocationsHeight; y++)
                {
                    for (var i = 0; i < MaxProjectileDirections; i++)
                    {
                        myBuffer.WriteInteger(Convert.ToInt32(SpawnLocations[x, y].Directions[i]));
                    }
                }
            }

            // Save animations
            myBuffer.WriteInteger(Animations.Count);
            for (var i = 0; i < Animations.Count; i++)
            {
                myBuffer.WriteInteger(Animations[i].Animation);
                myBuffer.WriteInteger(Animations[i].SpawnRange);
                myBuffer.WriteInteger(Convert.ToInt32(Animations[i].AutoRotate));
            }

            return myBuffer.ToArray();
        }

        public static ProjectileBase GetProjectile(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (ProjectileBase) Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((ProjectileBase) Objects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return ProjectileData();
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
            if (Objects.ContainsKey(index))
            {
                return Objects[index];
            }
            return null;
        }

        public override void Delete()
        {
            Objects.Remove(GetId());
        }

        public static void ClearObjects()
        {
            Objects.Clear();
        }

        public static void AddObject(int index, DatabaseObject obj)
        {
            Objects.Remove(index);
            Objects.Add(index, obj);
        }

        public static int ObjectCount()
        {
            return Objects.Count;
        }

        public static Dictionary<int, ProjectileBase> GetObjects()
        {
            Dictionary<int, ProjectileBase> objects = Objects.ToDictionary(k => k.Key, v => (ProjectileBase) v.Value);
            return objects;
        }
    }

    public class Location
    {
        public bool[] Directions = new bool[ProjectileBase.MaxProjectileDirections];
    }

    public class ProjectileAnimation
    {
        public int Animation = -1;
        public bool AutoRotate;
        public int SpawnRange = 1;

        public ProjectileAnimation(int animation, int spawnRange, bool autoRotate)
        {
            Animation = animation;
            SpawnRange = spawnRange;
            AutoRotate = autoRotate;
        }
    }
}