using System.Diagnostics.CodeAnalysis;

namespace Intersect.Server.Entities;

public partial class Player
{
    public bool TryLoadAvatarName([NotNullWhen(true)] out string? avatarName, out bool isFace)
    {
        if (!string.IsNullOrWhiteSpace(Face))
        {
            avatarName = Face;
            isFace = true;
            return true;
        }

        isFace = false;

        if (!string.IsNullOrWhiteSpace(Sprite))
        {
            avatarName = Sprite;
            return true;
        }

        avatarName = default;
        return false;
    }
}