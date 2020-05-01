namespace Intersect.Enums
{
    
    /// <summary>
    /// Contains the definitions to be used on how to handle giving or taking away items to and from players.
    /// </summary>
    public enum ItemHandling
    {
        /// <summary>
        /// Give and take away items like normal, no special rules.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Allow the inventory to overflow, dropping items on the map when the inventory reaches capacity.
        /// NOTE: Does not apply to taking away items!
        /// </summary>
        Overflow,

        /// <summary>
        /// Give or Take as many items as possible until the user either runs out of items or space.
        /// NOTE: Requires at least one change to be successful!
        /// </summary>
        UpTo,

    }

}
