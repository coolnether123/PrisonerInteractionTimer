using System.Reflection;
using HarmonyLib;
using PrisonerInteractionTimer.UI;
using RimWorld;
using Verse;

namespace PrisonerInteractionTimer.Patches
{
    internal static class PrisonerTabPatchInstaller
    {
        public static void Install()
        {
            PatchInstaller.Instance.PatchTypeOnce(
                "prisoner tab",
                typeof(PrisonerTabPatch),
                expectedPatchCount: 1);
        }
    }

    [HarmonyPatch]
    internal static class PrisonerTabPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.DeclaredMethod(
                typeof(ITab_Pawn_Visitor),
                "DoPrisonerTab",
                new[]
                {
                    typeof(bool),
                    typeof(Listing_Standard)
                });
        }

        [HarmonyPostfix]
        private static void Postfix(
            ITab_Pawn_Visitor __instance,
            Listing_Standard listing)
        {
            Pawn prisoner =
                Find.Selector.SingleSelectedThing as Pawn;
            if (prisoner != null &&
                prisoner.IsPrisonerOfColony)
            {
                float additionalHeight =
                    PrisonerTabTimerDrawer.Draw(
                    listing,
                    prisoner);
                if (additionalHeight > 0f)
                {
                    // FillTab calculates the tab's final height from
                    // Listing.CurHeight after this method returns. Reserve
                    // the panel's space in the completed listing so vanilla
                    // sizes the tab around it on this and future frames.
                    // This runs only after vanilla has drawn its policy rows.
                    listing.Gap(additionalHeight);
                }
            }
        }
    }
}
