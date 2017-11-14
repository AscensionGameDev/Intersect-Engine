using System;
using System.Collections.Generic;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib.GameObjects
{
    public class ProjectileBase : DatabaseObject<ProjectileBase>
    {
        public const int SpawnLocationsWidth = 5;
        public const int SpawnLocationsHeight = 5;
        public const int MaxProjectileDirections = 8;
        public int Ammo = -1;
        public int AmmoRequired = 1;
        public List<ProjectileAnimation> Animations = new List<ProjectileAnimation>();
        public int Delay = 1;
        public bool GrappleHook;
        public bool Homing;
        public bool IgnoreActiveResources;
        public bool IgnoreExhaustedResources;
        public bool IgnoreMapBlocks;
        public bool IgnoreZDimension;
        public int Knockback;
        public int Quantity = 1;
        public int Range = 1;
        public Location[,] SpawnLocations = new Location[SpawnLocationsWidth, SpawnLocationsHeight];

        public int Speed = 1;
        public int Spell;

        //Init
        public ProjectileBase(int id) : base(id)
        {
            Name = "New Projectile";
            for (var x = 0; x < SpawnLocationsWidth; x++)
            {
                for (var y = 0; y < SpawnLocationsHeight; y++)
                {
                    SpawnLocations[x, y] = new Location();
                }
            }
        }

        public override byte[] BinaryData => ProjectileData();

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
            Ammo = myBuffer.ReadInteger();
            AmmoRequired = myBuffer.ReadInteger();

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
            myBuffer.WriteInteger(Ammo);
            myBuffer.WriteInteger(AmmoRequired);

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