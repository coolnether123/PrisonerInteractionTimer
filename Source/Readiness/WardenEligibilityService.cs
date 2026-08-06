using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PrisonerInteractionTimer.Domain;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace PrisonerInteractionTimer.Readiness
{
    /// <summary>
    /// Approximates vanilla warden availability with a short-lived cache so UI
    /// draws do not create jobs or repeatedly run pathfinding.
    /// </summary>
    internal static class WardenEligibilityService
    {
        private const int CacheDurationTicks = 120;

        /// <summary>
        /// Distinguishes general warden availability from mode-specific
        /// requirements so the UI can explain the useful blocker.
        /// </summary>
        internal readonly struct Result
        {
            public Result(bool hasBaseWarden, bool hasModeEligibleWarden)
            {
                HasBaseWarden = hasBaseWarden;
                HasModeEligibleWarden = hasModeEligibleWarden;
            }

            public bool HasBaseWarden { get; }

            public bool HasModeEligibleWarden { get; }
        }

        /// <summary>
        /// Retains one prisoner's bounded scan result without extending the
        /// pawn's lifetime.
        /// </summary>
        private sealed class CacheEntry
        {
            public bool HasValue;
            public int EvaluatedTick;
            public int MapId;
            public string ModeSignature;
            public Result Result;
        }

        private static readonly ConditionalWeakTable<Pawn, CacheEntry> Cache =
            new ConditionalWeakTable<Pawn, CacheEntry>();

        public static Result Evaluate(
            Pawn prisoner,
            IReadOnlyList<PrisonerInteractionModeDef> modes,
            string modeSignature)
        {
            int now = Find.TickManager.TicksGame;
            int mapId = prisoner.MapHeld?.uniqueID ?? -1;
            CacheEntry entry = Cache.GetValue(
                prisoner,
                _ => new CacheEntry());
            if (entry.HasValue &&
                entry.MapId == mapId &&
                entry.ModeSignature == modeSignature &&
                now >= entry.EvaluatedTick &&
                now - entry.EvaluatedTick < CacheDurationTicks)
            {
                return entry.Result;
            }

            Result result = FindEligibleWarden(prisoner, modes);
            entry.HasValue = true;
            entry.EvaluatedTick = now;
            entry.MapId = mapId;
            entry.ModeSignature = modeSignature;
            entry.Result = result;
            return result;
        }

        private static Result FindEligibleWarden(
            Pawn prisoner,
            IReadOnlyList<PrisonerInteractionModeDef> modes)
        {
            Map map = prisoner.MapHeld;
            if (map == null)
            {
                return new Result(false, false);
            }

            bool hasBaseWarden = false;
            foreach (Pawn warden in map.mapPawns.FreeColonistsSpawned)
            {
                if (!IsBaseEligible(warden, prisoner))
                {
                    continue;
                }

                hasBaseWarden = true;
                for (int index = 0; index < modes.Count; index++)
                {
                    if (CanPerformMode(warden, prisoner, modes[index]))
                    {
                        return new Result(true, true);
                    }
                }
            }

            return new Result(hasBaseWarden, false);
        }

        private static bool IsBaseEligible(Pawn warden, Pawn prisoner)
        {
            return warden != null &&
                !warden.Dead &&
                !warden.Downed &&
                warden.Spawned &&
                !warden.InMentalState &&
                warden.workSettings != null &&
                warden.workSettings.WorkIsActive(WorkTypeDefOf.Warden) &&
                warden.health.capacities.CapableOf(PawnCapacityDefOf.Talking) &&
                !prisoner.IsForbidden(warden) &&
                !prisoner.IsFormingCaravan() &&
                warden.CanReserveAndReach(
                    prisoner,
                    PathEndMode.OnCell,
                    warden.NormalMaxDanger(),
                    1,
                    -1,
                    null,
                    false);
        }

        private static bool CanPerformMode(
            Pawn warden,
            Pawn prisoner,
            PrisonerInteractionModeDef mode)
        {
            switch (mode.defName)
            {
                case "ReduceResistance":
                    return prisoner.guest.Resistance > 0f;
                case "Convert":
                    return prisoner.guest.ideoForConversion != null &&
                        prisoner.Ideo != prisoner.guest.ideoForConversion &&
                        warden.Ideo == prisoner.guest.ideoForConversion;
                case "ReduceWill":
                    return prisoner.guest.will > 0f &&
                        WillingToEnslave(warden);
                case "Enslave":
                    return WillingToEnslave(warden);
                default:
                    return true;
            }
        }

        private static bool WillingToEnslave(Pawn warden)
        {
            return new HistoryEvent(
                HistoryEventDefOf.EnslavedPrisoner,
                warden.Named(HistoryEventArgsNames.Doer))
                .DoerWillingToDo();
        }
    }
}
