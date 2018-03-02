using System;
using System.Collections.Generic;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class ProjectileBase : DatabaseObject<ProjectileBase>
    {
        public const int SPAWN_LOCATIONS_WIDTH = 5;
        public const int SPAWN_LOCATIONS_HEIGHT = 5;
        public const int MAX_PROJECTILE_DIRECTIONS = 8;
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
        public Location[,] SpawnLocations = new Location[SPAWN_LOCATIONS_WIDTH, SPAWN_LOCATIONS_HEIGHT];

        public int Speed = 1;
        public int Spell;

        //Init
        [JsonConstructor]
        public ProjectileBase(int index) : base(index)
        {
            Name = "New Projectile";
            for (var x = 0; x < SPAWN_LOCATIONS_WIDTH; x++)
            {
                for (var y = 0; y < SPAWN_LOCATIONS_HEIGHT; y++)
                {
                    SpawnLocations[x, y] = new Location();
                }
            }
        }
    }

    public class Location
    {
        public bool[] Directions = new bool[ProjectileBase.MAX_PROJECTILE_DIRECTIONS];
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