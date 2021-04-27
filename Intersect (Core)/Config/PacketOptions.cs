namespace Intersect.Config
{
    public class PacketOptions
    {
        #region "Packet Batching"
        /// <summary>
        /// If this value is true, player movements will be sent in the batch with npc/event movement when maps update instead of in realtime which will reduce workload on the network pool at the expense of being slightly delayed
        /// </summary>
        public bool BatchPlayerMovementPackets { get; set; } = true;

        /// <summary>
        /// If this value is true, action messages will be sent in a batch maps update instead of in realtime which will reduce workload on the network pool at the expense of being slightly delayed 
        /// </summary>
        public bool BatchActionMessagePackets { get; set; } = true;

        /// <summary>
        /// If this value is true, animation packetswill be sent in a batch maps update instead of in realtime which will reduce workload on the network pool at the expense of being slightly delayed 
        /// </summary>
        public bool BatchAnimationPackets { get; set; } = true;
        #endregion
    }
}
