using Spine.Api;
using Spine.Harmony;

namespace PrisonerInteractionTimer.Patches
{
    /// <summary>
    /// Provides one Harmony owner so every patch uses Spine's duplicate and
    /// failure safeguards consistently.
    /// </summary>
    internal static class PatchInstaller
    {
        internal static readonly IHarmonyPatchInstaller Instance =
            SpineApi.Patching.CreateInstaller(
                "CoolNether123.PrisonerInteractionTimer",
                "[Prisoner Interaction Timer]");
    }
}
