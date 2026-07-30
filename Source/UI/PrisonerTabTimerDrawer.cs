using PrisonerInteractionTimer.Notifications;
using PrisonerInteractionTimer.Readiness;
using Spine.UI.Layout;
using UnityEngine;
using Verse;

namespace PrisonerInteractionTimer.UI
{
    internal static class PrisonerTabTimerDrawer
    {
        private static readonly Color ReadyColor =
            new Color(0.45f, 0.9f, 0.45f);
        private static readonly Color CooldownColor =
            new Color(1f, 0.78f, 0.32f);
        private static readonly Color BlockedColor =
            new Color(0.82f, 0.82f, 0.82f);

        public static void Draw(Listing_Standard listing, Pawn prisoner)
        {
            if (Bootstrap.PrisonerInteractionTimerMod.Settings?
                .FeatureEnabled != true)
            {
                return;
            }

            PrisonerReadinessSnapshot snapshot =
                PrisonerReadinessEvaluator.Evaluate(prisoner);
            bool showWatch =
                Bootstrap.PrisonerInteractionTimerMod.Settings
                    .NotificationsEnabled &&
                snapshot.UsesTimer &&
                !string.IsNullOrEmpty(snapshot.ModeSignature);

            listing.Gap(8f);
            Rect outer = listing.GetRect(showWatch ? 112f : 84f)
                .Rounded();
            Widgets.DrawMenuSection(outer);
            Rect content = SpineRectLayout.Pad(outer, 10f, 8f);

            Rect header = SpineRectLayout.WithHeight(
                content,
                24f,
                out Rect remainder,
                2f);
            var columns = SpineRectLayout.Horizontal(
                header,
                6f,
                SpineLayoutTrack.Flex(),
                SpineLayoutTrack.Fixed(92f));

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(columns[0], "PIT_TimerHeader".Translate());

            Color previousColor = GUI.color;
            GUI.color = StatusColor(snapshot);
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(
                columns[1],
                ReadinessTextResolver.Status(snapshot));
            GUI.color = previousColor;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect mode = SpineRectLayout.WithHeight(
                remainder,
                22f,
                out remainder);
            Widgets.Label(
                mode,
                "PIT_ModeLine".Translate(snapshot.ModeLabel));

            Rect next = SpineRectLayout.WithHeight(
                remainder,
                22f,
                out remainder);
            Widgets.Label(
                next,
                "PIT_NextLine".Translate(
                    ReadinessTextResolver.NextEligible(snapshot)));

            if (showWatch)
            {
                bool watched =
                    PrisonerNotificationService.IsWatched(
                        prisoner,
                        snapshot.ModeSignature);
                Rect watch = SpineRectLayout.WithHeight(
                    remainder,
                    24f,
                    out _);
                Widgets.CheckboxLabeled(
                    watch,
                    "PIT_NotifyThisMode".Translate(),
                    ref watched);
                if (watched !=
                    PrisonerNotificationService.IsWatched(
                        prisoner,
                        snapshot.ModeSignature))
                {
                    PrisonerNotificationService.SetWatched(
                        prisoner,
                        snapshot.ModeSignature,
                        watched,
                        snapshot.IsEligibleNow);
                }
            }

            TooltipHandler.TipRegion(
                outer,
                ReadinessTextResolver.Tooltip(snapshot));
            Widgets.DrawHighlightIfMouseover(outer);
        }

        private static Color StatusColor(
            PrisonerReadinessSnapshot snapshot)
        {
            if (snapshot.IsEligibleNow)
            {
                return ReadyColor;
            }
            if (snapshot.Timer.RemainingTicks > 0)
            {
                return CooldownColor;
            }
            return BlockedColor;
        }
    }
}
