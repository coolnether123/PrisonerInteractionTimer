using System.Collections.Generic;
using PrisonerInteractionTimer.Domain;
using PrisonerInteractionTimer.Readiness;
using PrisonerInteractionTimer.Settings;
using RimWorld;
using Verse;

namespace PrisonerInteractionTimer.Notifications
{
    /// <summary>
    /// Owns game-scoped subscriptions and bounded polling so alerts remain
    /// removal-safe and cannot leak between saves.
    /// </summary>
    internal static class PrisonerNotificationService
    {
        private const int CheckIntervalTicks = 250;
        private static Game owningGame;
        private static List<PrisonerNotificationWatch> watches =
            new List<PrisonerNotificationWatch>();

        public static bool IsWatched(Pawn prisoner, string modeSignature)
        {
            EnsureGame(Current.Game);
            return FindWatchIndex(prisoner, modeSignature) >= 0;
        }

        public static void SetWatched(
            Pawn prisoner,
            string modeSignature,
            bool watched,
            bool baselineReady)
        {
            EnsureGame(Current.Game);
            int existingIndex = FindWatchIndex(prisoner, modeSignature);
            PrisonerNotificationWatch existing = existingIndex < 0
                ? null
                : watches[existingIndex];
            if (!watched)
            {
                if (existing != null)
                {
                    watches.Remove(existing);
                }
                return;
            }

            if (existing == null)
            {
                watches.Add(new PrisonerNotificationWatch
                {
                    Prisoner = prisoner,
                    ModeSignature = modeSignature,
                    HasBaseline = true,
                    WasReady = baselineReady
                });
            }
        }

        public static void Tick()
        {
            EnsureGame(Current.Game);
            if (Find.TickManager.TicksGame %
                CheckIntervalTicks != 0)
            {
                return;
            }

            PrisonerInteractionTimerSettings settings =
                Bootstrap.PrisonerInteractionTimerMod.Settings;
            for (int index = watches.Count - 1;
                index >= 0;
                index--)
            {
                PrisonerNotificationWatch watch = watches[index];
                if (watch?.Prisoner == null ||
                    watch.Prisoner.Dead ||
                    !watch.Prisoner.IsPrisonerOfColony)
                {
                    watches.RemoveAt(index);
                    continue;
                }

                PrisonerReadinessSnapshot snapshot =
                    PrisonerReadinessEvaluator.Evaluate(
                        watch.Prisoner);
                bool sameMode =
                    snapshot.ModeSignature ==
                    watch.ModeSignature;
                bool isReady = sameMode &&
                    snapshot.IsEligibleNow;
                ReadyNotificationPollResult poll =
                    ReadyNotificationPolicy.Evaluate(
                        settings?.FeatureEnabled == true,
                        settings?.NotificationsEnabled == true,
                        watch.HasBaseline,
                        watch.WasReady,
                        isReady,
                        sameMode);
                // Baselines advance even while alerts are disabled; otherwise
                // re-enabling them would emit an old transition.
                if (poll.ShouldNotify)
                {
                    Messages.Message(
                        "PIT_ReadyMessage".Translate(
                            watch.Prisoner.LabelShort,
                            snapshot.ModeLabel),
                        watch.Prisoner,
                        MessageTypeDefOf.PositiveEvent,
                        false);
                }

                watch.HasBaseline = poll.HasBaseline;
                watch.WasReady = poll.WasReady;
            }
        }

        public static void ExposeData(Game game)
        {
            EnsureGame(game);
            Scribe_Collections.Look(
                ref watches,
                "prisonerInteractionTimerWatches",
                LookMode.Deep);
            if (watches == null)
            {
                watches = new List<PrisonerNotificationWatch>();
            }
        }

        private static void EnsureGame(Game game)
        {
            if (owningGame == game)
            {
                if (watches == null)
                {
                    watches =
                        new List<PrisonerNotificationWatch>();
                }
                return;
            }

            owningGame = game;
            watches = new List<PrisonerNotificationWatch>();
        }

        private static int FindWatchIndex(
            Pawn prisoner,
            string modeSignature)
        {
            for (int index = 0; index < watches.Count; index++)
            {
                PrisonerNotificationWatch watch = watches[index];
                if (watch?.Prisoner == prisoner &&
                    watch.ModeSignature == modeSignature)
                {
                    return index;
                }
            }

            return -1;
        }
    }
}
