using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Framework.Core.Network.Packets.Security;
using Intersect.Framework.Core.Security;
using Intersect.Network;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.Networking;

public partial class Client
{
    private PermissionSet _permissionSet = PermissionSet.Default;

    [NotMapped] // TODO: Not sure why this was even necessary, why does EF think that Client is going into the DB?
    public PermissionSet PermissionSet
    {
        get => _permissionSet;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            if (value == _permissionSet)
            {
                return;
            }

            _permissionSet = value;
            SendActivePermissionSet();
        }
    }

    [NotMapped] // TODO: Not sure why this was even necessary, why does EF think that Client is going into the DB?
    public string PermissionSetName
    {
        get => _permissionSet.Name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

            if (value == _permissionSet.Name)
            {
                return;
            }

            if (PermissionSet.TryGet(value, out var permissionSet))
            {
                PermissionSet = permissionSet;
                SendActivePermissionSet();
            }
            else
            {
                ApplicationContext.Logger.LogError(
                    "Failed to set permission set to {PermissionSetName} on client '{ClientName}' ({ClientId}) because the permission set does not exist",
                    value,
                    Name,
                    Id
                );
            }
        }
    }

    private void OnPermissionSetUpdated(PermissionSet permissionSet)
    {
        SendPermissionSet(permissionSet);
    }

    public void SendActivePermissionSet()
    {
        Send(new ActivePermissionSetPacket(PermissionSetName), TransmissionMode.All);
    }

    public void SendPermissionSet(string permissionSetName)
    {
        if (PermissionSet.TryGet(permissionSetName, out var permissionSet))
        {
            SendPermissionSet(permissionSet);
        }
        else
        {
            ApplicationContext.Logger.LogError(
                "Failed to send permission set '{PermissionSetName}' to client '{ClientName}' ({ClientId}) because the permission set does not exist",
                permissionSetName,
                Name,
                Id
            );
        }
    }

    public void SendPermissionSet(PermissionSet permissionSet)
    {
        Send(new PermissionSetPacket(permissionSet), TransmissionMode.All);
    }

    public void SynchronizePermissions()
    {
        foreach (var permissionSet in PermissionSet.PermissionSets)
        {
            SendPermissionSet(permissionSet);
        }

        SendActivePermissionSet();

        if (!PermissionSet.HasPermissionSetUpdatedHandler(OnPermissionSetUpdated))
        {
            PermissionSet.PermissionSetUpdated += OnPermissionSetUpdated;
        }
    }
}