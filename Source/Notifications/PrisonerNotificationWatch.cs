using Verse;

namespace PrisonerInteractionTimer.Notifications
{
    public sealed class PrisonerNotificationWatch : IExposable
    {
        public Pawn Prisoner;
        public string ModeSignature;
        public bool HasBaseline;
        public bool WasReady;

        public void ExposeData()
        {
            Scribe_References.Look(ref Prisoner, "prisoner");
            Scribe_Values.Look(
                ref ModeSignature,
                "modeSignature");
            Scribe_Values.Look(
                ref HasBaseline,
                "hasBaseline",
                false);
            Scribe_Values.Look(
                ref WasReady,
                "wasReady",
                false);
        }
    }
}
