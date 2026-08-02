using Spine.Api;
using Spine.Harmony;

namespace PrisonerInteractionTimer.Patches
{
    internal static class PatchInstaller
    {
        internal static readonly IHarmonyPatchInstaller Instance =
            SpineApi.Patching.CreateInstaller(
                "CoolNether123.PrisonerInteractionTimer",
                "[Prisoner Interaction Timer]");
    }
}
