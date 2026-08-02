using System.Reflection;
using HarmonyLib;
using PrisonerInteractionTimer.Notifications;
using Verse;

namespace PrisonerInteractionTimer.Patches
{
    internal static class NotificationHookInstaller
    {
        public static void Install()
        {
            PatchInstaller.Instance.PatchTypeOnce(
                "notification persistence",
                typeof(NotificationPersistencePatch),
                expectedPatchCount: 1);
            PatchInstaller.Instance.PatchTypeOnce(
                "notification polling",
                typeof(NotificationTickPatch),
                expectedPatchCount: 1);
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
