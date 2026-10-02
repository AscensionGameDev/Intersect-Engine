using Newtonsoft.Json;

namespace Intersect.Server.Core;

/// <summary>
/// Mode maintenance : quand actif, seuls les comptes avec le droit Editor peuvent se connecter.
/// Pilote via un fichier resources/maintenance.json (modifiable a chaud par le panel web d'admin, sans
/// avoir besoin de redemarrer/redeployer le serveur).
/// </summary>
public static class MaintenanceMode
{
    private sealed class State
    {
        public bool Enabled { get; set; }

        public string Message { get; set; } = "Le serveur est actuellement en maintenance. Reessayez plus tard.";
    }

    private static string FilePath => Path.Combine(ServerContext.ResourceDirectory, "maintenance.json");

    public static bool Enabled => Read().Enabled;

    public static string Message => Read().Message;

    private static State Read()
    {
        try
        {
            var path = FilePath;
            if (!File.Exists(path))
            {
                return new State();
            }

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<State>(json) ?? new State();
        }
        catch
        {
            // Si le fichier est absent/corrompu, on ne bloque jamais les joueurs par accident.
            return new State();
        }
    }
}
