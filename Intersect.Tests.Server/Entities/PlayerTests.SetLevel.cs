using Intersect.Server.Entities;
using NUnit.Framework;

namespace Intersect.Tests.Server.Entities;

public partial class PlayerTests
{
    private static IEnumerable<object[]> DataSetLevel
    {
        get
        {
            Options.EnsureCreated();

            yield return [1, 1, 0, false, 1, 1];
            yield return [1, 1, 0, true, 1, 1];
            yield return [1, 1, 2, false, 2, 1];
            yield return [1, 1, 2, true, 2, 0];
            yield return [2, 1, 1, false, 1, 1];
            yield return [2, 1, 1, true, 1, 0];
            yield return [1, 1, Options.Instance.Player.MaxLevel + 1, false, Options.Instance.Player.MaxLevel, 1];
            yield return [1, 1, Options.Instance.Player.MaxLevel + 1, true, Options.Instance.Player.MaxLevel, 0];
        }
    }

    [TestCaseSource(nameof(DataSetLevel))]
    public void TestSetLevel(int initialLevel, int initialExperience, int newLevel, bool resetExperience, int expectedLevel, long expectedExperience)
    {
        Player player = new()
        {
            Level = initialLevel,
            Exp = initialExperience,
            MapId = _mapId,
        };

        Assert.Multiple(
            () =>
            {
                Assert.That(player.Level, Is.EqualTo(initialLevel));
                Assert.That(player.Exp, Is.EqualTo(initialExperience));
            }
        );

        player.SetLevel(newLevel: newLevel, resetExperience: resetExperience);

        Assert.Multiple(
            () =>
            {
                Assert.That(player.Level, Is.EqualTo(expectedLevel));
                Assert.That(player.Exp, Is.EqualTo(expectedExperience));
            }
        );
    }
}