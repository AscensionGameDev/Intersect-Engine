using Newtonsoft.Json;

namespace Intersect.Config
{
    /// <summary>
    /// Contains configurable options pertaining to the way sprites are rendered within the engine
    /// </summary>
    public partial class SpriteOptions
    {
        /// <summary>
        /// Defines the number of frames there will be in attacking sprite sheets.
        /// </summary>
        public int AttackFrames { get; set; } = 4;

        /// <summary>
        /// Defines the number of frames there will be in casting sprite sheets.
        /// </summary>
        public int CastFrames { get; set; } = 4;

        /// <summary>
        /// Defines the number of frames there will be in idling sprite sheets.
        /// </summary>
        public int IdleFrames { get; set; } = 4;

        /// <summary>
        /// Defines the duration (in milliseconds) for transitioning between consecutive idling frames.
        /// </summary>
        public int IdleFrameDuration { get; set; } = 200;

        /// <summary>
        /// Defines how long (in milliseconds) a player must idle before the idling sprite starts to render.
        /// </summary>
        public int IdleStartDelay { get; set; } = 4000;

        /// <summary>
        /// Defines the number of frames there will be in normal (walking) sprite sheets.
        /// </summary>
        public int NormalFrames { get; set; } = 4;

        /// <summary>
        /// Defines a single frame from the normal sprite sheet to show for attacking when there is no designated sheet for attack.
        /// </summary>
        public int NormalAttackFrame { get; set; } = 3;

        /// <summary>
        /// Defines a single frame from the normal sprite sheet to show when blocking.
        /// </summary>
        public int NormalBlockFrame { get; set; } = 2;

        /// <summary>
        /// Defines a single frame from the normal sprite sheet to show for casting when there is no designated sheet for cast.
        /// </summary>
        public int NormalCastFrame { get; set; } = 2;

        /// <summary>
        /// Defines a single frame from the normal sprite sheet to show when dashing or sliding.
        /// </summary>
        public int NormalDashFrame { get; set; } = 1;

        /// <summary>
        /// Defines the duration (in milliseconds) for transitioning between consecutive walking frames.
        /// </summary>
        public int MovingFrameDuration { get; set; } = 200;

        /// <summary>
        /// Defines the number of frames there will be in shooting sprite sheets.
        /// </summary>
        public int ShootFrames { get; set; } = 4;

        /// <summary>
        /// Defines the number of frames there will be in weapon attacking sprite sheets.
        /// </summary>
        public int WeaponFrames { get; set; } = 4;

        /// <summary>
        /// The number of rows in the sprite sheet that correspond to the number of directions supported in the game.
        /// Currently, Intersect only supports 4 rows of frames for directions.
        /// </summary>
        [JsonIgnore]
        public int Directions => 4;
    }
}
