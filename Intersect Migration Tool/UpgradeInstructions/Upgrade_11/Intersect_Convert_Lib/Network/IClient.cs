namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Network
{
    public interface IClient : INetwork
    {
        bool IsConnected { get; }
        bool IsServerOnline { get; }
        int Ping { get; }
        bool Connect();
    }
}