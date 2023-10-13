using Intersect.Network.Packets.Client;
using Intersect.Server.Admin.Actions;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Localization;
using Intersect.Server.Notifications;
using Intersect.Utilities;

namespace Intersect.Server.Networking;

internal sealed partial class NetworkedPacketHandler
{
    //AdminActionPacket
    public void HandlePacket(Client client, AdminActionPacket packet)
    {
        var player = client?.Entity;
        if (player == null || client.Power == UserRights.None)
        {
            return;
        }

        ActionProcessing.ProcessAction(player, (dynamic) packet.Action);
    }

    //RequestPasswordResetPacket
    public void HandlePacket(Client client, RequestPasswordResetPacket packet)
    {
        if (client.TimeoutMs > Timing.Global.Milliseconds)
        {
            PacketSender.SendError(client, Strings.Errors.errortimeout);
            client.ResetTimeout();

            return;
        }

        if (Options.Instance.SmtpValid)
        {
            //Find account with that name or email
            var user = User.FindFromNameOrEmail(packet.NameOrEmail.Trim());
            if (user != null)
            {
                var email = new PasswordResetEmail(user);
                if (!email.Send())
                {
                    PacketSender.SendError(client, Strings.Account.emailfail);
                }
            }
            else
            {
                client.FailedAttempt();
            }
        }
        else
        {
            client.FailedAttempt();
        }
    }
}