using HarmonyLib;
#if INTERSECT_DIAGNOSTIC
using Intersect.Client.MonoGame.Debugging.DiagnosticPatches;
#endif
using Intersect.Logging;

#if INTERSECT_DIAGNOSTIC
using Microsoft.Xna.Framework.Audio;

using System.Text;
#endif

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
        }

        public void Stop()
        {
            DumpDSEMetrics();
        }

        private void DumpDSEMetrics()
        {
#if INTERSECT_DIAGNOSTIC
            var stringBuilder = new StringBuilder($"{{\n\t\"name\": \"{nameof(DynamicSoundEffectInstance)}\",\n\t\"data\": [\n");

            while (DynamicSoundEffectInstancePatches.Metrics.Count > 0)
            {
                var metric = DynamicSoundEffectInstancePatches.Metrics.Dequeue();
                stringBuilder.AppendLine($"\t\t{metric.ToString()}");
            }

            stringBuilder.Append("\t]\n}");
            Logger.Debug(stringBuilder.ToString());
#endif
        }
    }
}
