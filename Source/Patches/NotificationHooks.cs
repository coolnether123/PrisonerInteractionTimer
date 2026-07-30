using System;
using System.Reflection;
using HarmonyLib;
using PrisonerInteractionTimer.Notifications;
using Spine.Harmony;
using Verse;

namespace PrisonerInteractionTimer.Patches
{
    internal static class NotificationHookInstaller
    {
        public static void Install()
        {
            var harmony = new Harmony(
                "CoolNether123.PrisonerInteractionTimer");
            var options = new HarmonyUtil.PatchOptions
            {
                OnResult = (target, result) =>
                {
                    if (result.StartsWith(
                        "error:",
                        StringComparison.Ordinal))
                    {
                        Log.Error(
                            "[Prisoner Interaction Timer] Notification " +
                            "hook failure: " + result);
                    }
                }
            };
            var persistencePatches = HarmonyUtil.PatchType(
                harmony,
                typeof(NotificationPersistencePatch),
                options);
            var tickPatches = HarmonyUtil.PatchType(
                harmony,
                typeof(NotificationTickPatch),
                options);
            int patchCount =
                persistencePatches.Count +
                tickPatches.Count;
            if (patchCount != 2)
            {
                Log.Error(
                    "[Prisoner Interaction Timer] Expected two " +
                    "notification hooks, but applied " +
                    patchCount + ".");
            }
        }
    }

    [HarmonyPatch]
    internal static class NotificationPersistencePatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.DeclaredMethod(
                typeof(Game),
                "ExposeSmallComponents");
        }

        [HarmonyPostfix]
        private static void Postfix(Game __instance)
        {
            PrisonerNotificationService.ExposeData(__instance);
        }
    }

    [HarmonyPatch]
    internal static class NotificationTickPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.DeclaredMethod(
                typeof(GameComponentUtility),
                nameof(GameComponentUtility.GameComponentTick));
        }

        [HarmonyPostfix]
        private static void Postfix()
        {
            PrisonerNotificationService.Tick();
        }
    }
}
