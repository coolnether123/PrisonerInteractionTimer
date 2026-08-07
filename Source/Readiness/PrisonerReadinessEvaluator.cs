using System;
using System.Collections.Generic;
using System.Linq;
using PrisonerInteractionTimer.Compatibility;
using PrisonerInteractionTimer.Domain;
using RimWorld;
using Verse;

namespace PrisonerInteractionTimer.Readiness
{
    /// <summary>
    /// Samples vanilla prisoner state and resolves explainable blockers
    /// without creating jobs or changing interaction eligibility.
    /// </summary>
    public static class PrisonerReadinessEvaluator
    {
        public static PrisonerReadinessSnapshot Evaluate(Pawn prisoner)
        {
            var blockers = new List<ReadinessBlockReason>();
            if (prisoner?.guest == null ||
                !prisoner.IsPrisonerOfColony)
            {
                blockers.Add(
                    ReadinessBlockReason.NotAColonyPrisoner);
                return new PrisonerReadinessSnapshot(
                    prisoner,
                    Array.Empty<PrisonerInteractionModeDef>(),
                    default,
                    false,
                    blockers);
            }

            IReadOnlyList<PrisonerInteractionModeDef> modes =
                InteractionModeRegistry.GetActiveTimedModes(prisoner);
            if (modes.Count == 0)
            {
                blockers.Add(
                    ReadinessBlockReason.InteractionDisabled);
                return new PrisonerReadinessSnapshot(
                    prisoner,
                    modes,
                    default,
                    false,
                    blockers);
            }

            int now = Find.TickManager.TicksGame;
            int ticksUntilDayReset = Math.Max(
                1,
                GenDate.TicksPerDay -
                    GenLocalDate.DayTick(prisoner));
            InteractionTimerResult timer =
                InteractionTimerCalculator.Calculate(
                    new InteractionTimerInput(
                        now,
                        prisoner.mindState.lastAssignedInteractTime,
                        prisoner.mindState.interactionsToday,
                        ticksUntilDayReset));

            bool vanillaTimerReady =
                prisoner.guest.ScheduledForInteraction;
            if (!vanillaTimerReady)
            {
                blockers.Add(
                    prisoner.mindState.interactionsToday >=
                        InteractionTimerCalculator
                            .MaximumInteractionsPerDay
                        ? ReadinessBlockReason.DailyLimit
                        : ReadinessBlockReason.Cooldown);
            }

            if (!prisoner.Spawned)
            {
                blockers.Add(ReadinessBlockReason.Unspawned);
            }
            if (prisoner.InMentalState)
            {
                blockers.Add(ReadinessBlockReason.MentalState);
            }
            if (prisoner.Downed && !prisoner.InBed())
            {
                blockers.Add(
                    ReadinessBlockReason.Incapacitated);
            }
#if PIT_HAS_MUST_BE_AWAKE
            if (modes.All(mode => mode.mustBeAwake) &&
                !prisoner.Awake())
            {
                blockers.Add(ReadinessBlockReason.Asleep);
            }
#endif
            if (!prisoner.guest.PrisonerIsSecure)
            {
                blockers.Add(ReadinessBlockReason.Insecure);
            }

            bool canCheckWarden =
                vanillaTimerReady &&
                !blockers.Contains(
                    ReadinessBlockReason.Unspawned) &&
                !blockers.Contains(
                    ReadinessBlockReason.MentalState) &&
                !blockers.Contains(
                    ReadinessBlockReason.Incapacitated) &&
                !blockers.Contains(
                    ReadinessBlockReason.Asleep) &&
                !blockers.Contains(
                    ReadinessBlockReason.Insecure);

            // Path and reservation checks are the expensive part, so they are
            // useful only after every cheap vanilla gate has passed.
            string signature = string.Join(
                "|",
                modes.Select(mode => mode.defName));
            if (canCheckWarden)
            {
                WardenEligibilityService.Result warden =
                    WardenEligibilityService.Evaluate(
                    prisoner,
                    modes,
                    signature);
                if (!warden.HasBaseWarden)
                {
                    blockers.Add(
                        ReadinessBlockReason.NoEligibleWarden);
                }
                else if (!warden.HasModeEligibleWarden)
                {
                    blockers.Add(
                        ReadinessBlockReason.ModeRequirement);
                }
            }

            return new PrisonerReadinessSnapshot(
                prisoner,
                modes,
                timer,
                true,
                blockers.Distinct().ToList());
        }
    }
}
