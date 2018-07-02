using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class ProjectileBase : DatabaseObject<ProjectileBase>
    {
        public const int SPAWN_LOCATIONS_WIDTH = 5;
        public const int SPAWN_LOCATIONS_HEIGHT = 5;
        public const int MAX_PROJECTILE_DIRECTIONS = 8;

        [Column("Ammo")]
        public Guid AmmoItemId { get; protected set; } = Guid.Empty;
        [NotMapped]
        [JsonIgnore]
        public ItemBase Ammo
        {
            get => ItemBase.Get(AmmoItemId);
            set => AmmoItemId = value?.Id ?? Guid.Empty;
        }
        public int AmmoRequired { get; set; } = 1;

        [Column("Animations")]
        [JsonIgnore]
        public string AnimationsJson
        {
            get => JsonConvert.SerializeObject(Animations);
            set => Animations = JsonConvert.DeserializeObject<List<ProjectileAnimation>>(value);
        }
        [NotMapped]
        public List<ProjectileAnimation> Animations = new List<ProjectileAnimation>();

        public int Delay { get; set; } = 1;
        public bool GrappleHook { get; set; }
        public bool Homing { get; set; }
        public bool IgnoreActiveResources { get; set; }
        public bool IgnoreExhaustedResources { get; set; }
        public bool IgnoreMapBlocks { get; set; }
        public bool IgnoreZDimension { get; set; }
        public int Knockback { get; set; }
        public int Quantity { get; set; } = 1;
        public int Range { get; set; } = 1;

        [Column("SpawnLocations")]
        [JsonIgnore]
        public string SpawnsJson
        {
            get => JsonConvert.SerializeObject(SpawnLocations);
            set => SpawnLocations = JsonConvert.DeserializeObject<Location[,]>(value);
        }
        [NotMapped]
        public Location[,] SpawnLocations = new Location[SPAWN_LOCATIONS_WIDTH, SPAWN_LOCATIONS_HEIGHT];

        public int Speed { get; set; } = 1;

        [Column("Spell")]
        public Guid SpellId { get; protected set; } = Guid.Empty;
        [NotMapped]
        [JsonIgnore]
        public SpellBase Spell
        {
            get => SpellBase.Get(SpellId);
            set => SpellId = value?.Id ?? Guid.Empty;
        }

        //Init
        [JsonConstructor]
        public ProjectileBase(Guid id) : base(id)
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

        //Parameterless for EF
        public ProjectileBase()
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
        public Guid AnimationId;
        public bool AutoRotate;
        public int SpawnRange = 1;

        public ProjectileAnimation(Guid animationId, int spawnRange, bool autoRotate)
        {
            AnimationId = animationId;
            SpawnRange = spawnRange;
            AutoRotate = autoRotate;
        }
    }
}