using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace PrisonerInteractionTimer.Compatibility
{
    /// <summary>
    /// Compatibility surface for interaction modes that use RimWorld's
    /// Pawn_GuestTracker.ScheduledForInteraction gate.
    /// </summary>
    public static class InteractionModeRegistry
    {
        private static readonly HashSet<string> TimedModes =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "AttemptRecruit",
                "ReduceResistance",
                "Convert",
                "Enslave",
                "ReduceWill",
                "Interrogate"
            };

        private static readonly HashSet<string> UntimedModes =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "MaintainOnly",
                "Release",
                "Execution",
                "HemogenFarm"
            };

        public static void RegisterTimedMode(string defName)
        {
            ValidateDefName(defName);
            UntimedModes.Remove(defName);
            TimedModes.Add(defName);
        }

        public static void RegisterUntimedMode(string defName)
        {
            ValidateDefName(defName);
            TimedModes.Remove(defName);
            UntimedModes.Add(defName);
        }

        public static bool UsesVanillaScheduledGate(
            PrisonerInteractionModeDef mode)
        {
            if (mode == null || string.IsNullOrEmpty(mode.defName))
            {
                return false;
            }

            return TimedModes.Contains(mode.defName) &&
                !UntimedModes.Contains(mode.defName);
        }

        public static IReadOnlyList<PrisonerInteractionModeDef>
            GetActiveTimedModes(Pawn prisoner)
        {
            if (prisoner?.guest == null)
            {
                return Array.Empty<PrisonerInteractionModeDef>();
            }

            return DefDatabase<PrisonerInteractionModeDef>
                .AllDefsListForReading
                .Where(mode =>
                    UsesVanillaScheduledGate(mode) &&
                    prisoner.guest.IsInteractionEnabled(mode))
                .OrderBy(mode => mode.listOrder)
                .ThenBy(mode => mode.defName, StringComparer.Ordinal)
                .ToList();
        }

        private static void ValidateDefName(string defName)
        {
            if (string.IsNullOrWhiteSpace(defName))
            {
                throw new ArgumentException(
                    "A prisoner interaction mode defName is required.",
                    nameof(defName));
            }
        }
    }
}
