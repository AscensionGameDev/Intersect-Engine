using Newtonsoft.Json;

namespace Intersect.Config
{ 
    /// <summary>
    /// Contains configurable options pertaining to the way sprites are rendered within the engine
    /// </summary>
    public class SpriteOptions
    {
        /// <summary>
        /// Defines the number of frames there will be in idling sprite sheets
        /// </summary>
        public int IdleFrames = 4;

        /// <summary>
        /// Defines the number of frames there will be in normal (walking) sprite sheets
        /// </summary>
        public int NormalFrames = 4;

        /// <summary>
        /// Defines the number of frames there will be in casting sprite sheets
        /// </summary>
        public int CastFrames = 4;

        /// <summary>
        /// Defines the number of frames there will be in attacking sprite sheets
        /// </summary>
        public int AttackFrames = 4;

        /// <summary>
        /// Defines the number of frames there will be in shooting sprite sheets
        /// </summary>
        public int ShootFrames = 4;

        /// <summary>
        /// Defines the number of frames there will be in weapon attacking sprite sheets
        /// </summary>
        public int WeaponFrames = 4;

        /// <summary>
        /// The frame on the normal sprite sheet to show when attacking when there is no designated sheet for attack.
        /// </summary>
        public int NormalSheetAttackFrame = 3;

        /// <summary>
        /// The frame on the normal sprite sheet to show when dashing or sliding.
        /// </summary>
        public int NormalSheetDashFrame = 1;

        /// <summary>
        /// Defines how long (in ms) between walking frames
        /// </summary>
        public int MovingFrameDuration = 200;

        /// <summary>
        /// Defines how long (in ms) between idling frames
        /// </summary>
        public int IdleFrameDuration = 200;

        /// <summary>
        /// Defines how long (in ms) a player must idle before the idling sprite starts to render
        /// </summary>
        public int TimeBeforeIdle = 4000;

        /// <summary>
        /// Defines the number of rows in sprite sheets which will correlate to the number of directions in the game (Intersect is programmed by default with only 4 directions)
        /// </summary>
        [JsonIgnore] public int Directions => 4;
    }
}
