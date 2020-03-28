namespace Intersect.Network
{

    public enum TransmissionMode
    {

        /// <summary>
        ///     Guarantees that all packets will be transmitted successfully
        ///     and will be received and processed in the order they are sent.
        /// </summary>
        All = 0,

        /// <summary>
        ///     Guarantees that all packets will be transmitted successfully
        ///     but any packets of the same type will be ignored if they
        ///     arrive after a newer packet of their type.
        /// </summary>
        Latest,

        /// <summary>
        ///     Makes no guarantees about anything.
        /// </summary>
        Any

    }

}
