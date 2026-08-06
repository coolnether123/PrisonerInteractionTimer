using System.Reflection;
using HarmonyLib;
using PrisonerInteractionTimer.Notifications;
using Verse;

namespace PrisonerInteractionTimer.Patches
{
    /// <summary>
    /// Installs the two lifecycle hooks needed for removal-safe subscription
    /// persistence and bounded polling.
    /// </summary>
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
    /// <summary>
    /// Stores subscriptions inside vanilla game data so saves do not retain a
    /// missing component type when the mod is removed.
    /// </summary>
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
    /// <summary>
    /// Borrows vanilla's game-component tick to poll without introducing a
    /// save-visible component of this mod's own.
    /// </summary>
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
