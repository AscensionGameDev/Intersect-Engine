using HarmonyLib;

using Microsoft.Xna.Framework.Audio;

using System.Threading;

namespace Intersect.Client.MonoGame.Audio
{
    // TODO: Remove this when MonoGame/MonoGame#7403 is merged
    [HarmonyPatch(typeof(DynamicSoundEffectInstance))]
    internal class DynamicSoundEffectInstance_Patch
    {
        [HarmonyPatch("PlatformStop"), HarmonyPrefix]
        public static void PlatformStop_Prefix(DynamicSoundEffectInstance __instance) =>
            Monitor.Enter(__instance);

        [HarmonyPatch("PlatformStop"), HarmonyPostfix]
        public static void PlatformStop_Postfix(DynamicSoundEffectInstance __instance) =>
            Monitor.Exit(__instance);

        [HarmonyPatch("PlatformSubmitBuffer"), HarmonyPrefix]
        public static void PlatformSubmitBuffer_Prefix(DynamicSoundEffectInstance __instance) =>
            Monitor.Enter(__instance);

        [HarmonyPatch("PlatformSubmitBuffer"), HarmonyPostfix]
        public static void PlatformSubmitBuffer_Postfix(DynamicSoundEffectInstance __instance) =>
            Monitor.Exit(__instance);

        [HarmonyPatch("PlatformDispose"), HarmonyPrefix]
        public static void PlatformDispose_Prefix(DynamicSoundEffectInstance __instance) =>
            Monitor.Enter(__instance);

        [HarmonyPatch("PlatformDispose"), HarmonyPostfix]
        public static void PlatformDispose_Postfix(DynamicSoundEffectInstance __instance) =>
            Monitor.Exit(__instance);

        [HarmonyPatch("PlatformUpdateQueue"), HarmonyPrefix]
        public static void PlatformUpdateQueue_Prefix(DynamicSoundEffectInstance __instance) =>
            Monitor.Enter(__instance);

        [HarmonyPatch("PlatformUpdateQueue"), HarmonyPostfix]
        public static void PlatformUpdateQueue_Postfix(DynamicSoundEffectInstance __instance) =>
            Monitor.Exit(__instance);
    }
}
