using Newtonsoft.Json;

namespace Intersect.Config;

public partial class PassabilityOptions
{
    private bool[]? _cache;
    private bool _arena;
    private bool _normal;
    private bool _safe = true;

    public bool Arena
    {
        get => _arena;
        set
        {
            if (value == _arena)
            {
                return;
            }

            _arena = value;
            _cache = null;
        }
    }

    //Can players move through each other on the following map types/moralities
    public bool Normal
    {
        get => _normal;
        set
        {
            if (value == _arena)
            {
                return;
            }

            _normal = value;
            _cache = null;
        }
    }

    public bool Safe
    {
        get => _safe;
        set
        {
            if (value == _arena)
            {
                return;
            }

            _safe = value;
            _cache = null;
        }
    }

    [JsonIgnore]
    public bool[] Passable => _cache ??= [Normal, Safe, Arena];
}
