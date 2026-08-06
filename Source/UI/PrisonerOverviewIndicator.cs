using PrisonerInteractionTimer.Readiness;
using Spine.UI.ContextualSettings;
using UnityEngine;
using Verse;

namespace PrisonerInteractionTimer.UI
{
    /// <summary>
    /// Supplies an opt-in compact readiness marker for tables owned by other
    /// mods, avoiding ownership of a replacement prisoner overview.
    /// </summary>
    internal static class PrisonerOverviewIndicator
    {
        public static void Draw(Rect rect, Pawn prisoner)
        {
            if (Bootstrap.PrisonerInteractionTimerMod.Settings?
                .FeatureEnabled != true ||
                prisoner == null ||
                !prisoner.IsPrisonerOfColony)
            {
                return;
            }

            PrisonerReadinessSnapshot snapshot =
                PrisonerReadinessEvaluator.Evaluate(prisoner);
            Color color = snapshot.IsEligibleNow
                ? new Color(0.35f, 0.9f, 0.35f)
                : snapshot.Timer.RemainingTicks > 0
                    ? new Color(1f, 0.72f, 0.2f)
                    : new Color(0.55f, 0.55f, 0.55f);
            Rect marker = new Rect(
                rect.center.x - 5f,
                rect.center.y - 5f,
                10f,
                10f);
            Widgets.DrawBoxSolid(marker, color);
            Widgets.DrawBox(marker);
            Bootstrap.PrisonerInteractionTimerMod.ContextualSettings?.Bind(
                rect,
                ContextualSettingsTarget.Exact(
                    "display.enabled",
                    "display.header"),
                ContextualSettingsBindingOptions.WithTooltip(
                    ReadinessTextResolver.Tooltip(snapshot),
                    priority: 10));
        }
    }
}
