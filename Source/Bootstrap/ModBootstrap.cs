using PrisonerInteractionTimer.Patches;
using PrisonerInteractionTimer.Settings;
using Spine.Api;
using Spine.UI.SettingsFramework;
using Verse;

namespace PrisonerInteractionTimer.Bootstrap
{
    /// <summary>
    /// Connects the mod to Spine so settings and guarded patches share one
    /// lifecycle owner.
    /// </summary>
    public sealed class PrisonerInteractionTimerMod :
        SpineMod<PrisonerInteractionTimerSettings>
    {
        public PrisonerInteractionTimerMod(ModContentPack content)
            : base(
                content,
                "CoolNether123.PrisonerInteractionTimer",
                new SemanticVersion(1, 1, 0),
                PrisonerInteractionTimerSettings.Schema.Definitions,
                SpineCapability.HarmonyPatching |
                SpineCapability.SettingsSchema)
        {
            PrisonerTabPatchInstaller.Install();
            NotificationHookInstaller.Install();
        }

        protected override string SettingsCategoryLabel =>
            "PIT_ModName".Translate();
    }
}
