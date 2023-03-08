using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.Models;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{

    public partial class ProjectileBase : DatabaseObject<ProjectileBase>, IFolderable
    {

        public const int MAX_PROJECTILE_DIRECTIONS = 8;
        
        public static readonly int[] ProjectileRotationDir =
        {
            0, 1, 2, 3, 4, 5, 6, 7, // Up
            1, 0, 3, 2, 7, 6, 4, 5, // Down
            2, 3, 1, 0, 7, 4, 5, 6, // Left
            3, 2, 0, 1, 5, 6, 7, 4, // Right
            4, 6, 7, 5, 2, 0, 3, 1, // UpLeft
            5, 7, 4, 6, 0, 3, 1, 2, // UpRight
            6, 4, 5, 7, 3, 1, 2, 0, // DownRight
            7, 5, 6, 4, 1, 2, 0, 3 // DownLeft
        };

        public const int SPAWN_LOCATIONS_HEIGHT = 5;

        public const int SPAWN_LOCATIONS_WIDTH = 5;

        [NotMapped] public List<ProjectileAnimation> Animations = new List<ProjectileAnimation>();

        [NotMapped] public Location[,] SpawnLocations = new Location[SPAWN_LOCATIONS_WIDTH, SPAWN_LOCATIONS_HEIGHT];

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

        [Column("Ammo")]
        public Guid AmmoItemId { get; set; } = Guid.Empty;

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

        public int Delay { get; set; } = 1;

        //this one is not used anymore
        public bool GrappleHook { get; set; }

        public bool IgnoreActiveResources { get; set; }

        public bool IgnoreExhaustedResources { get; set; }

        public bool IgnoreMapBlocks { get; set; }

        public bool IgnoreZDimension { get; set; }

        public bool PierceTarget { get; set; }

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
        public List<GrappleOption> GrappleHookOptions = new List<GrappleOption>();

        [JsonIgnore]
        [Column("GrappleHookOptions")]
        public string GrappleHookOptionsJson
        {
            get => JsonConvert.SerializeObject(GrappleHookOptions);
            set
            {
                GrappleHookOptions = JsonConvert.DeserializeObject<List<GrappleOption>>(value ?? "") ?? new List<GrappleOption>();
            }
        }

        public int Speed { get; set; } = 1;

        [Column("Spell")]
        public Guid SpellId { get; set; } = Guid.Empty;

        [NotMapped]
        [JsonIgnore]
        public SpellBase Spell
        {
            get => SpellBase.Get(SpellId);
            set => SpellId = value?.Id ?? Guid.Empty;
        }

        /// <inheritdoc />
        public string Folder { get; set; } = "";

    }

    public partial class Location
    {

        public bool[] Directions = new bool[ProjectileBase.MAX_PROJECTILE_DIRECTIONS];

    }

    public partial class ProjectileAnimation
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
