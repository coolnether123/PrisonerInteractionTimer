using System;
using System.Collections.Generic;
using PrisonerInteractionTimer.Domain;

namespace PrisonerInteractionTimer.Tests
{
    internal static class Program
    {
        private static int assertions;

        private static int Main()
        {
            TestStrictVanillaCooldownBoundary();
            TestCooldownAlreadyReady();
            TestDailyLimitExtendsCountdown();
            TestCooldownCanOutlastDailyReset();
            TestNegativeDayResetInputIsClamped();
            TestLargeTickArithmeticDoesNotOverflow();
            TestTimeFormats();
            TestReadyTransition();
            TestModeChangeDoesNotNotify();
            TestUnchangedNotReadyDoesNotNotify();
            TestMasterDisableSuppressesNotification();
            TestNotificationDisableSuppressesNotification();
            TestDisabledPollAdvancesBaseline();
            TestInteractionCountsAboveLimitRemainBlocked();

            Console.WriteLine(
                "PASS: " + assertions +
                " prisoner timer contract assertions");
            return 0;
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

        private static void True(bool condition, string message)
        {
            assertions++;
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        private static void Equal<T>(
            T expected,
            T actual,
            string message)
        {
            assertions++;
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new InvalidOperationException(
                    message + " Expected " + expected +
                    ", got " + actual + ".");
            }
        }
    }
}
