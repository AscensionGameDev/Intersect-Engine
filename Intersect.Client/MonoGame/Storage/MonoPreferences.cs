using Intersect.Client.Framework;
using Intersect.Client.Framework.Storage;
using Intersect.Client.MonoGame.Storage;

namespace Intersect.Client.MonoGame.Database
{
    public class MonoPreferences : GamePreferences
    {
        protected IGameContext GameContext { get; }

        public MonoPreferences(IGameContext gameContext)
        {
            GameContext = gameContext;

            AddSerializer(new JsonPreferencesSerializer(gameContext));
            AddSerializer(new RegistryPreferencesSerializer(gameContext));
        }
    }
}
