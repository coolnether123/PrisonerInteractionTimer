using Verse;

namespace PrisonerInteractionTimer.Settings
{
    public sealed class PrisonerInteractionTimerSettings : ModSettings
    {
        public bool FeatureEnabled;

        public override void ExposeData()
        {
            Scribe_Values.Look(
                ref FeatureEnabled,
                "featureEnabled",
                false);
            base.ExposeData();
        }
    }
}
