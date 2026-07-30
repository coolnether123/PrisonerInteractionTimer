using UnityEngine;
using Verse;
using PrisonerInteractionTimer.Compatibility;
using PrisonerInteractionTimer.Patches;
using PrisonerInteractionTimer.Settings;

namespace PrisonerInteractionTimer.Bootstrap
{
    public sealed class PrisonerInteractionTimerMod : Mod
    {
        private readonly PrisonerInteractionTimerSettings settings;

        public PrisonerInteractionTimerMod(ModContentPack content)
            : base(content)
        {
            settings = GetSettings<PrisonerInteractionTimerSettings>();
            CompatibilityRegistry.InitializeAll();
            PatchInstaller.InstallAll();
        }

        public override string SettingsCategory()
        {
            return "Prisoner Interaction Timer";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled(
                "Feature enabled",
                ref settings.FeatureEnabled);
            listing.End();
        }
    }
}
