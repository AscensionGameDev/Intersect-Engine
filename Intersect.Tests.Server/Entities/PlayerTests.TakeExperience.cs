using Intersect.Server.Entities;
using NUnit.Framework;

namespace Intersect.Tests.Server.Entities;

public partial class PlayerTests
{
    private static IEnumerable<object[]> DataTakeExperience
    {
        get
        {
            Options.EnsureCreated();

            yield return [1, 0, -1, true, true, 1, 1]; // Gives experience
            yield return [1, 0, 1, true, true, 1, 0];
            yield return [1, 1, 1, true, true, 1, 0];

            yield return [1, 99, -1, true, true, 2, 0]; // Gives experience
            yield return [1, 99, -1, true, false, 2, 0]; // Gives experience
            yield return [1, 99, -1, false, true, 2, 0]; // Gives experience
            yield return [1, 99, -1, false, false, 2, 0]; // Gives experience

            yield return [1, 99, -2, true, true, 2, 1]; // Gives experience
            yield return [1, 99, -2, true, false, 2, 1]; // Gives experience
            yield return [1, 99, -2, false, true, 2, 1]; // Gives experience
            yield return [1, 99, -2, false, false, 2, 1]; // Gives experience

            yield return [2, 0, 1, true, true, 1, 99];
            yield return [2, 0, 1, true, false, 1, 99];
            yield return [2, 0, 1, false, true, 2, 0];
            yield return [2, 0, 1, false, false, 2, 0];

            yield return [2, 1, 1, true, true, 2, 0];
            yield return [2, 1, 1, true, false, 2, 0];
            yield return [2, 1, 1, false, true, 2, 0];
            yield return [2, 1, 1, false, false, 2, 0];

            yield return [2, 1, 2, true, true, 1, 99];
            yield return [2, 1, 2, true, false, 1, 99];
            yield return [2, 1, 2, false, true, 2, 0];
            yield return [2, 1, 2, false, false, 2, 0];

            yield return [Options.Instance.Player.MaxLevel, 0, 1, true, true, Options.Instance.Player.MaxLevel - 1, 99];
            yield return [Options.Instance.Player.MaxLevel, 1, 1000, true, true, Options.Instance.Player.MaxLevel - 10, 1];
        }
    }

    [TestCaseSource(nameof(DataTakeExperience))]
    public void TestTakeExperience(
        int initialLevel,
        int initialExperience,
        long experienceToTake,
        bool enableLosingLevels,
        bool force,
        int expectedLevel,
        long expectedExperience
    )
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

        player.TakeExperience(experienceToTake, enableLosingLevels: enableLosingLevels, force: force);

        Assert.Multiple(
            () =>
            {
                Assert.That(player.Level, Is.EqualTo(expectedLevel));
                Assert.That(player.Exp, Is.EqualTo(expectedExperience));
            }
        );
    }
}