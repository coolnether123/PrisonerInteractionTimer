using PrisonerInteractionTimer.Patches;
using PrisonerInteractionTimer.Settings;
using Spine.Api;
using Spine.UI.SettingsFramework;
using Verse;

namespace PrisonerInteractionTimer.Bootstrap
{
    public sealed class PrisonerInteractionTimerMod :
        SpineMod<PrisonerInteractionTimerSettings>
    {
        public PrisonerInteractionTimerMod(ModContentPack content)
            : base(
                content,
                "CoolNether123.PrisonerInteractionTimer",
                new SemanticVersion(1, 0, 0),
                PrisonerInteractionTimerSettings.Definitions,
                SpineCapability.HarmonyPatching)
        {
            PrisonerTabPatchInstaller.Install();
            NotificationHookInstaller.Install();
        }

        protected override string SettingsCategoryLabel =>
            "PIT_ModName".Translate();
    }
}
