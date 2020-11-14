using System;

using HarmonyLib;

using Microsoft.Xna.Framework.Audio;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Intersect.Logging;

namespace Intersect.Client.MonoGame.Debugging.Patches
{
    internal static class DynamicSoundEffectInstancePatches
    {
        internal struct Metric
        {
            public string Name { get; set; }

            public int ALBuffers { get; set; }

            public int QueuedBuffers { get; set; }

            public override string ToString() => $"{{ \"Name\": \"{Name}\", \"ALBuffers\": {ALBuffers}, \"QueuedBuffers\": {QueuedBuffers} }}";
        }

        internal static Queue<Metric> Metrics = new Queue<Metric>();

        private static Type type_AL { get; }

        private static Type type_ALGetSourcei { get; }

        private static FieldInfo info_DynamicSoundEffectInstance__format { get; }
        private static FieldInfo info_DynamicSoundEffectInstance__queuedBuffers { get; }

        private static FieldInfo info_ALGetSourcei_BuffersProcessed { get; }
        private static FieldInfo info_AL_GetSource { get; }
        private static FieldInfo info_DynamicSoundEffectInstance_SourceId { get; }

        private static MulticastDelegate field_AL_GetSource { get; }
        private static object field_ALGetSourcei_BuffersProcessed { get; }

        static DynamicSoundEffectInstancePatches()
        {
            type_AL = AccessTools.TypeByName("AL");
            type_ALGetSourcei = AccessTools.TypeByName("ALGetSourcei");

            info_DynamicSoundEffectInstance__format = AccessTools.Field(typeof(DynamicSoundEffectInstance), "_format");
            info_DynamicSoundEffectInstance__queuedBuffers = AccessTools.Field(typeof(DynamicSoundEffectInstance), "_queuedBuffers");
            info_ALGetSourcei_BuffersProcessed = AccessTools.Field(type_ALGetSourcei, "BuffersProcessed");
            info_AL_GetSource = AccessTools.Field(type_AL, "GetSource");
            info_DynamicSoundEffectInstance_SourceId = AccessTools.Field(typeof(SoundEffectInstance), "SourceId");

            field_AL_GetSource = info_AL_GetSource.GetValue(null) as MulticastDelegate;
            field_ALGetSourcei_BuffersProcessed = info_ALGetSourcei_BuffersProcessed.GetValue(null);
        }

        internal abstract class MetricsCollector<TPatch> where TPatch : MetricsCollector<TPatch>
        {
            private static string Name => $"{nameof(DynamicSoundEffectInstance)}.{typeof(TPatch).Name}";

            protected static void Collect(DynamicSoundEffectInstance __instance, string subkey)
            {
                var queuedBuffersEnumerable = info_DynamicSoundEffectInstance__queuedBuffers.GetValue(__instance) as IEnumerable;

                var queuedBuffers = queuedBuffersEnumerable?.Cast<object>()?.ToList();
                Metrics.Enqueue(
                    new Metric
                    {
                        Name = $"{Name}_{subkey}",
                        ALBuffers = GetALBufferCount(__instance),
                        QueuedBuffers = queuedBuffers?.Count ?? int.MinValue
                    }
                );

                if (Metrics.Count > 50)
                {
                    Metrics.Dequeue();
                }
            }

            protected static int GetALBufferCount(DynamicSoundEffectInstance __instance)
            {
                try
                {
                    var numBuffers = int.MinValue;
                    var SourceId = info_DynamicSoundEffectInstance_SourceId.GetValue(__instance);
                    var GetSource_params = new object[] {SourceId, field_ALGetSourcei_BuffersProcessed, numBuffers};
                    field_AL_GetSource.DynamicInvoke(GetSource_params);
                    return (int) GetSource_params[2];
                }
                catch (Exception exception)
                {
                    Log.Debug(exception);
                    return int.MinValue;
                }
            }
        }

        [HarmonyPatch(typeof(DynamicSoundEffectInstance), "PlatformCreate")]
        internal class PlatformCreate : MetricsCollector<PlatformCreate>
        {
            public static void Prefix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Prefix));

            public static void Postfix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Postfix));
        }

        [HarmonyPatch(typeof(DynamicSoundEffectInstance), "PlatformPlay")]
        internal class PlatformPlay : MetricsCollector<PlatformPlay>
        {
            public static void Prefix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Prefix));

            public static void Postfix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Postfix));
        }

        [HarmonyPatch(typeof(DynamicSoundEffectInstance), "PlatformPause")]
        internal class PlatformPause : MetricsCollector<PlatformPause>
        {
            public static void Prefix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Prefix));

            public static void Postfix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Postfix));
        }

        [HarmonyPatch(typeof(DynamicSoundEffectInstance), "PlatformResume")]
        internal class PlatformResume : MetricsCollector<PlatformResume>
        {
            public static void Prefix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Prefix));

            public static void Postfix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Postfix));
        }

        [HarmonyPatch(typeof(DynamicSoundEffectInstance), "PlatformStop")]
        internal class PlatformStop : MetricsCollector<PlatformStop>
        {
            public static void Prefix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Prefix));

            public static void Postfix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Postfix));
        }

        [HarmonyPatch(typeof(DynamicSoundEffectInstance), "PlatformSubmitBuffer")]
        internal class PlatformSubmitBuffer : MetricsCollector<PlatformSubmitBuffer>
        {
            public static void Prefix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Prefix));

            public static void Postfix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Postfix));
        }

        [HarmonyPatch(typeof(DynamicSoundEffectInstance), "PlatformDispose")]
        internal class PlatformDispose : MetricsCollector<PlatformDispose>
        {
            public static void Prefix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Prefix));

            public static void Postfix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Postfix));
        }

        [HarmonyPatch(typeof(DynamicSoundEffectInstance), "PlatformUpdateQueue")]
        internal class PlatformUpdateQueue : MetricsCollector<PlatformUpdateQueue>
        {
            public static void Prefix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Prefix));

            public static void Postfix(DynamicSoundEffectInstance __instance) => Collect(__instance, nameof(Postfix));
        }
    }
}
