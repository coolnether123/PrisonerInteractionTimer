using PrisonerInteractionTimer.Compatibility;
using PrisonerInteractionTimer.Readiness;
using PrisonerInteractionTimer.UI;
using UnityEngine;
using Verse;

namespace PrisonerInteractionTimer.Api
{
    /// <summary>
    /// Gives other mods a stable way to declare timed modes and reuse the
    /// readiness model without depending on this mod's UI internals.
    /// </summary>
    public static class PrisonerInteractionTimerApi
    {
        public static void RegisterTimedMode(string defName)
        {
            InteractionModeRegistry.RegisterTimedMode(defName);
        }

        public static void RegisterUntimedMode(string defName)
        {
            InteractionModeRegistry.RegisterUntimedMode(defName);
        }

        public static PrisonerReadinessSnapshot GetReadiness(Pawn prisoner)
        {
            return PrisonerReadinessEvaluator.Evaluate(prisoner);
        }

        /// <summary>
        /// Optional integration point for prisoner overview tables. The host
        /// retains ownership of its table and supplies the cell rectangle.
        /// </summary>
        public static void DrawCompactIndicator(Rect rect, Pawn prisoner)
        {
            PrisonerOverviewIndicator.Draw(rect, prisoner);
        }
    }
}
