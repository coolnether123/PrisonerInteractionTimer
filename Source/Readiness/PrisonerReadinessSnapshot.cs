using System;
using System.Collections.Generic;
using System.Linq;
using PrisonerInteractionTimer.Domain;
using RimWorld;
using Verse;

namespace PrisonerInteractionTimer.Readiness
{
    /// <summary>
    /// Freezes the timing, modes, and blockers from one evaluation so every UI
    /// surface presents a coherent readiness state.
    /// </summary>
    public sealed class PrisonerReadinessSnapshot
    {
        public PrisonerReadinessSnapshot(
            Pawn prisoner,
            IReadOnlyList<PrisonerInteractionModeDef> modes,
            InteractionTimerResult timer,
            bool usesTimer,
            IReadOnlyList<ReadinessBlockReason> blockers)
        {
            Prisoner = prisoner;
            Modes = modes ?? Array.Empty<PrisonerInteractionModeDef>();
            Timer = timer;
            UsesTimer = usesTimer;
            Blockers = blockers ?? Array.Empty<ReadinessBlockReason>();
        }

        public Pawn Prisoner { get; }

        public IReadOnlyList<PrisonerInteractionModeDef> Modes { get; }

        public InteractionTimerResult Timer { get; }

        public bool UsesTimer { get; }

        public IReadOnlyList<ReadinessBlockReason> Blockers { get; }

        public bool IsEligibleNow =>
            UsesTimer && Blockers.Count == 0;

        public string ModeSignature =>
            string.Join(
                "|",
                Modes.Select(mode => mode.defName));

        public string ModeLabel
        {
            get
            {
                if (Modes.Count > 0)
                {
                    return string.Join(
                        ", ",
                        Modes.Select(mode =>
                            mode.LabelCap.ToString()));
                }

#if PIT_HAS_EXCLUSIVE_MODE
                return Prisoner?.guest?.ExclusiveInteractionMode?
                    .LabelCap.ToString() ?? "None".Translate();
#else
                return "None".Translate();
#endif
            }
        }
    }
}
