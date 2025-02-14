using Intersect.Server.Maps;
using NUnit.Framework;

namespace Intersect.Tests.Server.Entities;

[TestFixture]
public partial class PlayerTests
{
    private Guid _mapId;

    [SetUp]
    public void Setup()
    {
        Options.EnsureCreated();

        _mapId = Guid.NewGuid();
        MapController.Lookup.Set(_mapId, new MapController(_mapId));
    }
}