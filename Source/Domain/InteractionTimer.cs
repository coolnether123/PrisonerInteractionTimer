using System;

namespace PrisonerInteractionTimer.Domain
{
    public enum TimeDisplayFormat
    {
        HumanReadable = 0,
        GameHours = 1,
        Ticks = 2
    }

    public enum ReadinessBlockReason
    {
        None = 0,
        NotAColonyPrisoner = 1,
        InteractionDisabled = 2,
        Cooldown = 3,
        DailyLimit = 4,
        Asleep = 5,
        Incapacitated = 6,
        MentalState = 7,
        Insecure = 8,
        Unspawned = 9,
        NoEligibleWarden = 10,
        ModeRequirement = 11
    }

    public readonly struct InteractionTimerInput
    {
        public InteractionTimerInput(
            int nowTick,
            int lastInteractionTick,
            int interactionsToday,
            int ticksUntilLocalDayReset)
        {
            NowTick = nowTick;
            LastInteractionTick = lastInteractionTick;
            InteractionsToday = interactionsToday;
            TicksUntilLocalDayReset = ticksUntilLocalDayReset;
        }

        public int NowTick { get; }

        public int LastInteractionTick { get; }

        public int InteractionsToday { get; }

        public int TicksUntilLocalDayReset { get; }
    }

    public readonly struct InteractionTimerResult
    {
        public InteractionTimerResult(
            int remainingTicks,
            int nextEligibleTick,
            bool intervalReady,
            bool dailyLimitReady)
        {
            RemainingTicks = remainingTicks;
            NextEligibleTick = nextEligibleTick;
            IntervalReady = intervalReady;
            DailyLimitReady = dailyLimitReady;
        }

        public int RemainingTicks { get; }

        public int NextEligibleTick { get; }

        public bool IntervalReady { get; }

        public bool DailyLimitReady { get; }

        public bool IsReady => IntervalReady && DailyLimitReady;
    }

    public static class InteractionTimerCalculator
    {
        public const int MinimumInteractionIntervalTicks = 10000;
        public const int MaximumInteractionsPerDay = 2;

        public static InteractionTimerResult Calculate(
            InteractionTimerInput input)
        {
            long intervalEligibleTick =
                (long)input.LastInteractionTick +
                MinimumInteractionIntervalTicks +
                1L;
            int intervalRemaining = ClampRemaining(
                intervalEligibleTick - input.NowTick);

            int dayRemaining = input.InteractionsToday <
                MaximumInteractionsPerDay
                ? 0
                : Math.Max(1, input.TicksUntilLocalDayReset);

            int remaining = Math.Max(intervalRemaining, dayRemaining);
            long nextTick = (long)input.NowTick + remaining;
            return new InteractionTimerResult(
                remaining,
                nextTick > int.MaxValue ? int.MaxValue : (int)nextTick,
                intervalRemaining == 0,
                dayRemaining == 0);
        }

        private static int ClampRemaining(long value)
        {
            if (value <= 0L)
            {
                return 0;
            }

            return value > int.MaxValue ? int.MaxValue : (int)value;
        }
    }

    public static class InteractionTimeFormatter
    {
        public const int TicksPerGameHour = 2500;
        public const int TicksPerGameDay = 60000;

        public static string Format(
            int ticks,
            TimeDisplayFormat format)
        {
            ticks = Math.Max(0, ticks);
            switch (format)
            {
                case TimeDisplayFormat.Ticks:
                    return ticks.ToString("N0") + " ticks";
                case TimeDisplayFormat.GameHours:
                    return (ticks / (double)TicksPerGameHour)
                        .ToString("0.0") + " h";
                default:
                    return FormatHumanReadable(ticks);
            }
        }

        private static string FormatHumanReadable(int ticks)
        {
            if (ticks == 0)
            {
                return "ready now";
            }

            int days = ticks / TicksPerGameDay;
            double hours = (ticks % TicksPerGameDay) /
                (double)TicksPerGameHour;
            if (days > 0)
            {
                return days + " d " + hours.ToString("0.#") + " h";
            }

            return hours.ToString("0.#") + " h";
        }
    }

    public static class ReadyTransitionDetector
    {
        public static bool ShouldNotify(
            bool hasBaseline,
            bool wasReady,
            bool isReady,
            bool modeUnchanged)
        {
            return hasBaseline && modeUnchanged && !wasReady && isReady;
        }
    }

    public readonly struct ReadyNotificationPollResult
    {
        public ReadyNotificationPollResult(
            bool shouldNotify,
            bool hasBaseline,
            bool wasReady)
        {
            ShouldNotify = shouldNotify;
            HasBaseline = hasBaseline;
            WasReady = wasReady;
        }

        public bool ShouldNotify { get; }

        public bool HasBaseline { get; }

        public bool WasReady { get; }
    }

    public static class ReadyNotificationPolicy
    {
        public static ReadyNotificationPollResult Evaluate(
            bool featureEnabled,
            bool notificationsEnabled,
            bool hasBaseline,
            bool wasReady,
            bool isReady,
            bool modeUnchanged)
        {
            bool shouldNotify =
                featureEnabled &&
                notificationsEnabled &&
                ReadyTransitionDetector.ShouldNotify(
                    hasBaseline,
                    wasReady,
                    isReady,
                    modeUnchanged);
            return new ReadyNotificationPollResult(
                shouldNotify,
                modeUnchanged || hasBaseline,
                modeUnchanged ? isReady : wasReady);
        }
    }
}
