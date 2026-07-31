using PrisonerInteractionTimer.Notifications;
using PrisonerInteractionTimer.Readiness;
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

        public static float Draw(Listing_Standard listing, Pawn prisoner)
        {
            if (Bootstrap.PrisonerInteractionTimerMod.Settings?
                .FeatureEnabled != true)
            {
                return 0f;
            }

            PrisonerReadinessSnapshot snapshot =
                PrisonerReadinessEvaluator.Evaluate(prisoner);
            bool showWatch =
                Bootstrap.PrisonerInteractionTimerMod.Settings
                    .NotificationsEnabled &&
                snapshot.UsesTimer &&
                !string.IsNullOrEmpty(snapshot.ModeSignature);

            string headerText = "PIT_TimerHeader".Translate();
            string statusText = ReadinessTextResolver.Status(snapshot);
            string modeText = "PIT_ModeLine".Translate(snapshot.ModeLabel);
            string nextText = "PIT_NextLine".Translate(
                ReadinessTextResolver.NextEligible(snapshot));
            string watchText = "PIT_NotifyThisMode".Translate();

            GameFont previousFont = Text.Font;
            TextAnchor previousAnchor = Text.Anchor;
            bool previousWordWrap = Text.WordWrap;
            Color previousColor = GUI.color;

            Text.Font = GameFont.Small;
            Text.WordWrap = true;
            float contentWidth = Mathf.Max(
                1f,
                listing.ColumnWidth -
                2f * PrisonerTimerPanelLayoutCalculator.HorizontalPadding);
            PrisonerTimerPanelLayout layout =
                PrisonerTimerPanelLayoutCalculator.Measure(
                    listing.ColumnWidth,
                    Text.CalcSize(headerText).x,
                    Text.CalcSize(statusText).x,
                    Text.CalcHeight(headerText, contentWidth),
                    Text.CalcHeight(statusText, contentWidth),
                    Text.CalcHeight(modeText, contentWidth),
                    Text.CalcHeight(nextText, contentWidth),
                    Text.CalcHeight(watchText, contentWidth),
                    showWatch);

            Rect outer = new Rect(
                0f,
                listing.CurHeight +
                    PrisonerTimerPanelLayoutCalculator.PanelGap,
                listing.ColumnWidth,
                layout.OuterHeight).Rounded();
            Widgets.DrawMenuSection(outer);
            Rect content = outer.ContractedBy(
                PrisonerTimerPanelLayoutCalculator.HorizontalPadding,
                PrisonerTimerPanelLayoutCalculator.TopPadding);
            content.yMax = outer.yMax -
                PrisonerTimerPanelLayoutCalculator.BottomPadding;

            float cursorY = content.y;
            DrawHeader(
                content,
                ref cursorY,
                layout,
                headerText,
                statusText,
                snapshot);
            GUI.color = previousColor;
            cursorY += PrisonerTimerPanelLayoutCalculator.RowGap;

            DrawTextRow(
                content,
                ref cursorY,
                layout.ModeRowHeight,
                modeText);
            cursorY += PrisonerTimerPanelLayoutCalculator.RowGap;
            DrawTextRow(
                content,
                ref cursorY,
                layout.NextRowHeight,
                nextText);

            if (showWatch)
            {
                cursorY += PrisonerTimerPanelLayoutCalculator.RowGap;
                bool watched =
                    PrisonerNotificationService.IsWatched(
                        prisoner,
                        snapshot.ModeSignature);
                Rect watch = new Rect(
                    content.x,
                    cursorY,
                    content.width,
                    layout.WatchRowHeight);
                Text.Anchor = TextAnchor.UpperLeft;
                Widgets.CheckboxLabeled(watch, watchText, ref watched);
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

            GUI.color = previousColor;
            Text.Anchor = previousAnchor;
            Text.WordWrap = previousWordWrap;
            Text.Font = previousFont;
            return PrisonerTimerPanelLayoutCalculator.AdditionalTabHeight(
                layout);
        }

        private static void DrawHeader(
            Rect content,
            ref float cursorY,
            PrisonerTimerPanelLayout layout,
            string headerText,
            string statusText,
            PrisonerReadinessSnapshot snapshot)
        {
            Rect headerLabel = new Rect(
                content.x,
                cursorY,
                layout.HeaderLabelWidth,
                layout.HeaderLabelRowHeight);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(headerLabel, headerText);

            if (layout.StackHeader)
            {
                cursorY += layout.HeaderLabelRowHeight +
                    PrisonerTimerPanelLayoutCalculator.RowGap;
            }

            Rect status = new Rect(
                layout.StackHeader ? content.x : headerLabel.xMax +
                    PrisonerTimerPanelLayoutCalculator.HeaderGap,
                cursorY,
                layout.StatusWidth,
                layout.StatusRowHeight);
            GUI.color = StatusColor(snapshot);
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(status, statusText);
            cursorY += layout.StatusRowHeight;
        }

        private static void DrawTextRow(
            Rect content,
            ref float cursorY,
            float rowHeight,
            string text)
        {
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.Label(new Rect(
                content.x,
                cursorY,
                content.width,
                rowHeight), text);
            cursorY += rowHeight;
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
