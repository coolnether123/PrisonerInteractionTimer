using System;
using System.Reflection;
using HarmonyLib;
using PrisonerInteractionTimer.UI;
using RimWorld;
using Spine.Harmony;
using UnityEngine;
using Verse;

namespace PrisonerInteractionTimer.Patches
{
    internal static class PrisonerTabPatchInstaller
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
                            "[Prisoner Interaction Timer] Patch failure: " +
                            result);
                    }
                }
            };
            var patched = HarmonyUtil.PatchType(
                harmony,
                typeof(PrisonerTabPatch),
                options);
            if (patched.Count != 1)
            {
                Log.Error(
                    "[Prisoner Interaction Timer] Expected one Prisoner " +
                    "tab patch, but applied " + patched.Count + ".");
            }
        }
    }

    [HarmonyPatch]
    internal static class PrisonerTabPatch
    {
        private static readonly FieldInfo TabSizeField =
            AccessTools.Field(typeof(ITab_Pawn_Visitor), "size");

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
                    Vector2 tabSize = (Vector2)TabSizeField.GetValue(
                        __instance);
                    tabSize.y += additionalHeight;
                    TabSizeField.SetValue(__instance, tabSize);
                }
            }
        }
    }
}
