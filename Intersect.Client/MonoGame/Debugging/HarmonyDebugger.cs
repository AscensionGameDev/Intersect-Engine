using HarmonyLib;

using Intersect.Client.MonoGame.Debugging.Patches;
using Intersect.Logging;

using Microsoft.Xna.Framework.Audio;

using System.Text;

namespace Intersect.Client.MonoGame.Debugging
{
    internal class HarmonyDebugger
    {
        private Logger Logger { get; }

        internal HarmonyDebugger(Logger logger)
        {
            Logger = logger;
        }

        public void Start()
        {
            var harmony = new Harmony(typeof(IntersectGame).FullName);
            harmony.PatchAll();
        }

        public void Stop()
        {
            Logger.Debug(DumpDSEMetrics());
        }

        private string DumpDSEMetrics()
        {
            var stringBuilder = new StringBuilder($"{{\n\t\"name\": \"{nameof(DynamicSoundEffectInstance)}\",\n\t\"data\": [\n");

            while (DynamicSoundEffectInstancePatches.Metrics.Count > 0)
            {
                var metric = DynamicSoundEffectInstancePatches.Metrics.Dequeue();
                stringBuilder.AppendLine($"\t\t{metric.ToString()}");
            }

            stringBuilder.Append("\t]\n}");
            return stringBuilder.ToString();
        }
    }
}
