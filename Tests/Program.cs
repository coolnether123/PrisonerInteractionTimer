using System;
using System.Collections.Generic;
using PrisonerInteractionTimer.Domain;
using PrisonerInteractionTimer.UI;
using static RimWorld.ModTestSupport.Test;

namespace PrisonerInteractionTimer.Tests
{
    internal static class Program
    {
        private static int Main()
        {
            Start("Prisoner Interaction Timer contracts");
            Run("strict vanilla cooldown boundary", TestStrictVanillaCooldownBoundary);
            Run("cooldown already ready", TestCooldownAlreadyReady);
            Run("daily limit extends countdown", TestDailyLimitExtendsCountdown);
            Run("cooldown can outlast daily reset", TestCooldownCanOutlastDailyReset);
            Run("negative reset input clamps", TestNegativeDayResetInputIsClamped);
            Run("large tick arithmetic", TestLargeTickArithmeticDoesNotOverflow);
            Run("time formats", TestTimeFormats);
            Run("ready transition", TestReadyTransition);
            Run("mode change does not notify", TestModeChangeDoesNotNotify);
            Run("unchanged wait does not notify", TestUnchangedNotReadyDoesNotNotify);
            Run("master disable suppresses notification", TestMasterDisableSuppressesNotification);
            Run("notification disable suppresses notification", TestNotificationDisableSuppressesNotification);
            Run("disabled poll advances baseline", TestDisabledPollAdvancesBaseline);
            Run("interaction counts above limit", TestInteractionCountsAboveLimitRemainBlocked);
            Run("timer panel adds space", TestTimerPanelAddsSpaceBelowVanillaContent);
            Run("timer panel reserves descenders", TestTimerPanelReservesDescenderAndBottomPadding);
            Run("timer panel stacks when narrow", TestTimerPanelStacksHeaderWhenNarrow);
            Run("timer panel fits default layout", TestTimerPanelFitsAfterDefaultPrisonerLayout);
            Run("timer panel fits full DLC layout", TestTimerPanelFitsAfterFullDlcPrisonerLayout);
            return Finish();
        }

        private static void TestStrictVanillaCooldownBoundary()
        {
            InteractionTimerResult atBoundary =
                InteractionTimerCalculator.Calculate(
                    new InteractionTimerInput(
                        11000,
                        1000,
                        1,
                        40000));
            Equal(
                1,
                atBoundary.RemainingTicks,
                "RimWorld uses a strict less-than comparison at 10,000 ticks.");

            InteractionTimerResult afterBoundary =
                InteractionTimerCalculator.Calculate(
                    new InteractionTimerInput(
                        11001,
                        1000,
                        1,
                        40000));
            True(
                afterBoundary.IsReady,
                "The interval must become ready one tick after the boundary.");
        }

        private static void TestCooldownAlreadyReady()
        {
            InteractionTimerResult result =
                InteractionTimerCalculator.Calculate(
                    new InteractionTimerInput(
                        50000,
                        -99999,
                        0,
                        10000));
            Equal(0, result.RemainingTicks, "Fresh prisoners are ready.");
            Equal(50000, result.NextEligibleTick, "Ready tick is now.");
        }

        private static void TestDailyLimitExtendsCountdown()
        {
            InteractionTimerResult result =
                InteractionTimerCalculator.Calculate(
                    new InteractionTimerInput(
                        50000,
                        30000,
                        2,
                        12000));
            Equal(
                12000,
                result.RemainingTicks,
                "Daily limit must wait for the local-day rollover.");
            True(
                !result.DailyLimitReady,
                "Daily limit must be represented separately.");
        }

        private static void TestCooldownCanOutlastDailyReset()
        {
            InteractionTimerResult result =
                InteractionTimerCalculator.Calculate(
                    new InteractionTimerInput(
                        1000,
                        0,
                        2,
                        500));
            Equal(
                9001,
                result.RemainingTicks,
                "The later of interval and day reset controls eligibility.");
        }

        private static void TestNegativeDayResetInputIsClamped()
        {
            InteractionTimerResult result =
                InteractionTimerCalculator.Calculate(
                    new InteractionTimerInput(
                        20000,
                        0,
                        2,
                        -10));
            Equal(
                1,
                result.RemainingTicks,
                "A stale local-day sample must never claim immediate readiness.");
        }

        private static void TestLargeTickArithmeticDoesNotOverflow()
        {
            InteractionTimerResult result =
                InteractionTimerCalculator.Calculate(
                    new InteractionTimerInput(
                        int.MaxValue - 5,
                        int.MaxValue - 2,
                        0,
                        10));
            Equal(
                int.MaxValue,
                result.NextEligibleTick,
                "Next tick must saturate instead of wrapping negative.");
            True(
                result.RemainingTicks > 0,
                "Overflow protection must preserve an active cooldown.");
        }

        private static void TestTimeFormats()
        {
            Equal(
                "1.0 h",
                InteractionTimeFormatter.Format(
                    2500,
                    TimeDisplayFormat.GameHours),
                "Game-hour format uses 2,500 ticks per hour.");
            Equal(
                "1 d 1 h",
                InteractionTimeFormatter.Format(
                    62500,
                    TimeDisplayFormat.HumanReadable),
                "Human format combines days and hours.");
            True(
                InteractionTimeFormatter.Format(
                    1234,
                    TimeDisplayFormat.Ticks)
                    .EndsWith(" ticks", StringComparison.Ordinal),
                "Tick format labels its unit.");
        }

        private static void TestReadyTransition()
        {
            True(
                ReadyTransitionDetector.ShouldNotify(
                    true,
                    false,
                    true,
                    true),
                "A stable-mode false-to-true transition should notify.");
            True(
                !ReadyTransitionDetector.ShouldNotify(
                    true,
                    true,
                    true,
                    true),
                "Remaining ready must not repeat notifications.");
            True(
                !ReadyTransitionDetector.ShouldNotify(
                    false,
                    false,
                    true,
                    true),
                "First observation establishes a baseline.");
        }

        private static void TestModeChangeDoesNotNotify()
        {
            True(
                !ReadyTransitionDetector.ShouldNotify(
                    true,
                    false,
                    true,
                    false),
                "Changing interaction type must not create a false transition.");
        }

        private static void TestUnchangedNotReadyDoesNotNotify()
        {
            True(
                !ReadyTransitionDetector.ShouldNotify(
                    true,
                    false,
                    false,
                    true),
                "A prisoner that remains blocked must not notify.");
        }

        private static void TestMasterDisableSuppressesNotification()
        {
            ReadyNotificationPollResult result =
                ReadyNotificationPolicy.Evaluate(
                    false,
                    true,
                    true,
                    false,
                    true,
                    true);
            True(
                !result.ShouldNotify,
                "The master feature switch must suppress ready alerts.");
        }

        private static void TestNotificationDisableSuppressesNotification()
        {
            ReadyNotificationPollResult result =
                ReadyNotificationPolicy.Evaluate(
                    true,
                    false,
                    true,
                    false,
                    true,
                    true);
            True(
                !result.ShouldNotify,
                "The notification switch must suppress ready alerts.");
        }

        private static void TestDisabledPollAdvancesBaseline()
        {
            ReadyNotificationPollResult disabledPoll =
                ReadyNotificationPolicy.Evaluate(
                    false,
                    true,
                    true,
                    false,
                    true,
                    true);
            True(
                disabledPoll.HasBaseline && disabledPoll.WasReady,
                "A disabled poll must still record the current ready state.");

            ReadyNotificationPollResult reenabledPoll =
                ReadyNotificationPolicy.Evaluate(
                    true,
                    true,
                    disabledPoll.HasBaseline,
                    disabledPoll.WasReady,
                    true,
                    true);
            True(
                !reenabledPoll.ShouldNotify,
                "Re-enabling must not emit a stale catch-up alert.");
        }

        private static void TestInteractionCountsAboveLimitRemainBlocked()
        {
            InteractionTimerResult result =
                InteractionTimerCalculator.Calculate(
                    new InteractionTimerInput(
                        50000,
                        0,
                        3,
                        250));
            Equal(
                250,
                result.RemainingTicks,
                "Counts beyond the vanilla cap still wait for day reset.");
            True(
                !result.IsReady,
                "A count beyond the cap must not be treated as ready.");
        }

        private static void TestTimerPanelAddsSpaceBelowVanillaContent()
        {
            PrisonerTimerPanelLayout layout =
                PrisonerTimerPanelLayoutCalculator.Measure(
                    260f,
                    90f,
                    60f,
                    18f,
                    18f,
                    18f,
                    18f,
                    18f,
                    false);
            Equal(
                PrisonerTimerPanelLayoutCalculator.PanelGap +
                    layout.OuterHeight,
                PrisonerTimerPanelLayoutCalculator.AdditionalTabHeight(
                    layout),
                "The timer panel must extend the tab below vanilla content instead of consuming its listing rows.");
        }

        private static void TestTimerPanelReservesDescenderAndBottomPadding()
        {
            PrisonerTimerPanelLayout layout =
                PrisonerTimerPanelLayoutCalculator.Measure(
                    260f,
                    90f,
                    60f,
                    18f,
                    18f,
                    26f,
                    26f,
                    18f,
                    true);
            True(
                layout.ModeRowHeight >= 30f &&
                layout.NextRowHeight >= 30f,
                "Measured multi-line text must retain bottom space for descenders.");
            True(
                layout.OuterHeight >=
                    PrisonerTimerPanelLayoutCalculator.TopPadding +
                    layout.HeaderHeight +
                    PrisonerTimerPanelLayoutCalculator.RowGap +
                    layout.ModeRowHeight +
                    PrisonerTimerPanelLayoutCalculator.RowGap +
                    layout.NextRowHeight +
                    PrisonerTimerPanelLayoutCalculator.RowGap +
                    layout.WatchRowHeight +
                    PrisonerTimerPanelLayoutCalculator.BottomPadding,
                "The measured panel must retain bottom padding after its final row.");
        }

        private static void TestTimerPanelStacksHeaderWhenNarrow()
        {
            PrisonerTimerPanelLayout layout =
                PrisonerTimerPanelLayoutCalculator.Measure(
                    130f,
                    70f,
                    50f,
                    18f,
                    18f,
                    18f,
                    18f,
                    18f,
                    false);
            True(
                layout.StackHeader,
                "A narrow prisoner tab must stack header text rather than clip either header column.");
            Equal(
                layout.ContentWidth,
                layout.HeaderLabelWidth,
                "A stacked header label must use the measured content width.");
            Equal(
                layout.ContentWidth,
                layout.StatusWidth,
                "A stacked status must use the measured content width.");
        }

        private static void TestTimerPanelFitsAfterDefaultPrisonerLayout()
        {
            AssertPanelReservationFitsCompletedTab(
                184f,
                false,
                "A default prisoner layout must reserve the timer panel " +
                "below vanilla's completed rows.");
        }

        private static void TestTimerPanelFitsAfterFullDlcPrisonerLayout()
        {
            AssertPanelReservationFitsCompletedTab(
                452f,
                true,
                "A full-DLC prisoner layout must reserve the timer panel " +
                "below vanilla's completed rows instead of drawing below " +
                "the tab.");
        }

        private static void AssertPanelReservationFitsCompletedTab(
            float vanillaListingHeight,
            bool showWatch,
            string message)
        {
            PrisonerTimerPanelLayout layout =
                PrisonerTimerPanelLayoutCalculator.Measure(
                    260f,
                    90f,
                    60f,
                    18f,
                    18f,
                    18f,
                    18f,
                    18f,
                    showWatch);
            float panelBottom = vanillaListingHeight +
                PrisonerTimerPanelLayoutCalculator.PanelGap +
                layout.OuterHeight;
            float completedListingHeight = vanillaListingHeight +
                PrisonerTimerPanelLayoutCalculator.AdditionalTabHeight(
                    layout);
            float completedTabHeight = completedListingHeight + 20f;

            Equal(
                panelBottom,
                completedListingHeight,
                message);
            True(
                completedTabHeight > panelBottom,
                "Vanilla's final tab padding must remain after the timer " +
                "panel in every supported prisoner layout.");
        }

    }
}
