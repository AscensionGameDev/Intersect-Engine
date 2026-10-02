using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Input;
using Intersect.Client.General;
using Intersect.Configuration;
using Intersect.Framework.Core;
using Intersect.Utilities;

namespace Intersect.Client.Core;

/// <summary>
/// Intro narrative affichee avant le menu principal : texte centre qui apparait/disparait en
/// synchro avec la narration audio (resources/music/intro_aetherion.ogg). Passable a tout moment
/// (Espace / Entree / Echap / clic) et automatiquement sautee une fois vue (sauf en build Debug).
/// </summary>
internal static class StoryIntro
{
    private const string MusicName = "intro_aetherion";
    private const string SeenPreference = "StoryIntroSeen";

    private const double AudioSeconds = 69.3;
    private const double EndHoldSeconds = 3.0;
    private const double FadeSeconds = 0.6;
    private const double MinSkipDelaySeconds = 0.4;

    private const int TextSize = 26;
    private const int TitleSize = 40;
    private const int HintSize = 14;
    private const int LineSpacing = 12;

    // Debut (en secondes depuis le lancement de la narration) de chaque passage, cale sur les pauses de la voix.
    private static readonly (double Start, string Text, int Size)[] Chunks =
    [
        (0.3, "Avant les royaumes... avant les rois...", TextSize),
        (3.29, "le monde n'était qu'un souffle.", TextSize),
        (5.26, "Un voile invisible enveloppait Elyndor tout entier — l'Aetherion. Il coulait dans la terre, dans l'eau... dans le sang même des vivants.", TextSize),
        (14.13, "Les Anciens l'apprirent les premiers. Ils bâtirent des cités qui flottaient au-dessus des nuages... des machines qui respiraient comme des êtres vivants... des portails reliant des mondes que nul n'aurait dû voir se toucher.", TextSize),
        (26.82, "Mais la sagesse a une limite. Et l'orgueil, aucune.", TextSize),
        (30.78, "Ils voulurent plier l'Aetherion à leur volonté... le forcer à obéir.", TextSize),
        (35.25, "Le voile se déchira.", TextSize),
        (36.82, "Les cités tombèrent. La terre s'ouvrit sur des failles sans fond. Et dans les profondeurs déchirées... quelque chose s'éveilla.", TextSize),
        (43.99, "Nyxaroth.", TitleSize),
        (45.79, "Trois siècles ont passé depuis la Rupture. Les royaumes se sont relevés sur les cendres des Anciens. Mais l'Aetherion... n'a jamais cessé de saigner.", TextSize),
        (54.13, "Les failles se rouvrent.", TextSize),
        (55.98, "Et quelque part... dans un village qu'aucune carte ne connaît encore... un pouvoir que l'on croyait perdu s'apprête à se réveiller.", TextSize),
        (65.22, "Bienvenue à Elyndor.", TitleSize),
        (67.16, "L'Âge des Cendres touche à sa fin.", TextSize),
    ];

    private static readonly Dictionary<int, string[]> WrappedLines = [];

    private static bool _started;
    private static bool _finished;
    private static long _startMs;
    private static int _wrappedForWidth;

    public static bool IsActive => _started && !_finished;

    public static bool ShouldRun => !_finished && (_started || CanPlay());

    private static bool CanPlay()
    {
#if DEBUG
        return true;
#else
        return !Globals.Database.HasPreference(SeenPreference);
#endif
    }

    public static void Update()
    {
        if (!_started)
        {
            _started = true;
            _startMs = Timing.Global.MillisecondsUtc;
            var fade = ClientConfiguration.Instance.MusicFadeTimer;
            Audio.PlayMusic(MusicName, fade, fade, false);
            return;
        }

        var elapsed = ElapsedSeconds();
        if (elapsed >= MinSkipDelaySeconds && SkipRequested())
        {
            Finish();
            return;
        }

        if (elapsed >= AudioSeconds + EndHoldSeconds)
        {
            Finish();
        }
    }

    public static void Finish()
    {
        if (_finished)
        {
            return;
        }

        _finished = true;
        Globals.Database.SavePreference(SeenPreference, true);

        var fade = ClientConfiguration.Instance.MusicFadeTimer;
        Audio.PlayMusic(ClientConfiguration.Instance.MenuMusic, fade, fade, true);
        Fade.FadeIn(ClientConfiguration.Instance.FadeDurationMs);
    }

    private static double ElapsedSeconds() => (Timing.Global.MillisecondsUtc - _startMs) / 1000.0;

    private static bool SkipRequested()
    {
        var input = Globals.InputManager;
        return (input.IsKeyDown(Keys.Space) && !input.WasKeyDown(Keys.Space)) ||
               (input.IsKeyDown(Keys.Enter) && !input.WasKeyDown(Keys.Enter)) ||
               (input.IsMouseButtonDown(MouseButton.Left) && !input.WasMouseButtonDown(MouseButton.Left));
    }

    public static void Draw()
    {
        var renderer = Graphics.Renderer;
        var font = Graphics.UIFont;
        var screenWidth = renderer.ScreenWidth;
        var screenHeight = renderer.ScreenHeight;
        var elapsed = ElapsedSeconds();

        if (_wrappedForWidth != screenWidth)
        {
            WrappedLines.Clear();
            _wrappedForWidth = screenWidth;
        }

        var index = -1;
        for (var i = 0; i < Chunks.Length; i++)
        {
            if (Chunks[i].Start <= elapsed)
            {
                index = i;
            }
        }

        if (index >= 0)
        {
            var (start, text, size) = Chunks[index];
            var end = index + 1 < Chunks.Length ? Chunks[index + 1].Start : AudioSeconds + EndHoldSeconds;

            var alpha = Math.Min((elapsed - start) / FadeSeconds, (end - elapsed) / FadeSeconds);
            alpha = Math.Clamp(alpha, 0, 1);

            if (!WrappedLines.TryGetValue(index, out var lines))
            {
                lines = Wrap(text, font, size, screenWidth * 0.7f);
                WrappedLines[index] = lines;
            }

            var lineHeight = renderer.MeasureText("Ag", font, size, 1).Y + LineSpacing;
            var top = (screenHeight - lineHeight * lines.Length) / 2f;
            var color = new Color((int)(alpha * 255), 235, 226, 205);
            var shadow = new Color((int)(alpha * 200), 0, 0, 0);

            for (var i = 0; i < lines.Length; i++)
            {
                var width = renderer.MeasureText(lines[i], font, size, 1).X;
                renderer.DrawString(
                    lines[i],
                    font,
                    size,
                    (int)((screenWidth - width) / 2f),
                    (int)(top + i * lineHeight),
                    1,
                    color,
                    false,
                    null,
                    shadow
                );
            }
        }

        const string hint = "Espace / Entrée / clic : passer l'intro";
        var hintWidth = renderer.MeasureText(hint, font, HintSize, 1).X;
        renderer.DrawString(
            hint,
            font,
            HintSize,
            (int)((screenWidth - hintWidth) / 2f),
            screenHeight - 40,
            1,
            new Color(140, 200, 200, 200),
            false,
            null,
            null
        );
    }

    private static string[] Wrap(string text, IFont? font, int size, float maxWidth)
    {
        var renderer = Graphics.Renderer;
        var lines = new List<string>();
        var current = string.Empty;

        foreach (var word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            var candidate = current.Length == 0 ? word : $"{current} {word}";
            if (current.Length > 0 && renderer.MeasureText(candidate, font, size, 1).X > maxWidth)
            {
                lines.Add(current);
                current = word;
            }
            else
            {
                current = candidate;
            }
        }

        if (current.Length > 0)
        {
            lines.Add(current);
        }

        return [.. lines];
    }
}
