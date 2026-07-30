using System.Collections.Generic;
using System.Linq;
using PrisonerInteractionTimer.Domain;
using PrisonerInteractionTimer.Readiness;
using PrisonerInteractionTimer.Settings;
using Verse;

namespace PrisonerInteractionTimer.UI
{
    internal static class ReadinessTextResolver
    {
        public static string Status(PrisonerReadinessSnapshot snapshot)
        {
            if (!snapshot.UsesTimer)
            {
                return "PIT_StatusDisabled".Translate();
            }
            if (snapshot.IsEligibleNow)
            {
                return "PIT_StatusReady".Translate();
            }
            if (snapshot.Timer.RemainingTicks > 0)
            {
                return "PIT_StatusCooldown".Translate(
                    Format(snapshot.Timer.RemainingTicks));
            }

            switch (snapshot.Blockers.FirstOrDefault())
            {
                case ReadinessBlockReason.NotAColonyPrisoner:
                    return "PIT_StatusNotPrisoner".Translate();
                case ReadinessBlockReason.InteractionDisabled:
                    return "PIT_StatusDisabled".Translate();
                case ReadinessBlockReason.DailyLimit:
                    return "PIT_StatusDailyLimit".Translate();
                case ReadinessBlockReason.Asleep:
                    return "PIT_StatusAsleep".Translate();
                case ReadinessBlockReason.Incapacitated:
                    return "PIT_StatusIncapacitated".Translate();
                case ReadinessBlockReason.MentalState:
                    return "PIT_StatusMentalState".Translate();
                case ReadinessBlockReason.Insecure:
                    return "PIT_StatusInsecure".Translate();
                case ReadinessBlockReason.Unspawned:
                    return "PIT_StatusUnavailable".Translate();
                case ReadinessBlockReason.NoEligibleWarden:
                    return "PIT_StatusNoWarden".Translate();
                case ReadinessBlockReason.ModeRequirement:
                    return "PIT_StatusModeBlocked".Translate();
                default:
                    return "PIT_StatusUnavailable".Translate();
            }
        }

        public static string NextEligible(
            PrisonerReadinessSnapshot snapshot)
        {
            if (!snapshot.UsesTimer)
            {
                return "PIT_NotScheduled".Translate();
            }
            if (snapshot.IsEligibleNow)
            {
                return "PIT_ReadyNow".Translate();
            }
            if (snapshot.Timer.RemainingTicks > 0)
            {
                return Format(snapshot.Timer.RemainingTicks);
            }

            return "PIT_WhenConditionsMet".Translate();
        }

        public static string Tooltip(
            PrisonerReadinessSnapshot snapshot)
        {
            var lines = new List<string>
            {
                "PIT_TooltipVanilla".Translate(),
                string.Empty,
                "PIT_ModeLine".Translate(snapshot.ModeLabel),
                "PIT_NextLine".Translate(NextEligible(snapshot))
            };

            PrisonerInteractionTimerSettings settings =
                Bootstrap.PrisonerInteractionTimerMod.Settings;
            if (settings?.ShowDetailedReasons != false &&
                snapshot.Blockers.Count > 0)
            {
                lines.Add(string.Empty);
                lines.Add("PIT_BlockersHeader".Translate());
                foreach (ReadinessBlockReason blocker in snapshot.Blockers)
                {
                    lines.Add("  - " + Reason(blocker));
                }
            }

            return string.Join("\n", lines);
        }

        public static string Reason(ReadinessBlockReason reason)
        {
            switch (reason)
            {
                case ReadinessBlockReason.NotAColonyPrisoner:
                    return "PIT_ReasonNotPrisoner".Translate();
                case ReadinessBlockReason.InteractionDisabled:
                    return "PIT_ReasonDisabled".Translate();
                case ReadinessBlockReason.Cooldown:
                    return "PIT_ReasonCooldown".Translate();
                case ReadinessBlockReason.DailyLimit:
                    return "PIT_ReasonDailyLimit".Translate();
                case ReadinessBlockReason.Asleep:
                    return "PIT_ReasonAsleep".Translate();
                case ReadinessBlockReason.Incapacitated:
                    return "PIT_ReasonIncapacitated".Translate();
                case ReadinessBlockReason.MentalState:
                    return "PIT_ReasonMentalState".Translate();
                case ReadinessBlockReason.Insecure:
                    return "PIT_ReasonInsecure".Translate();
                case ReadinessBlockReason.Unspawned:
                    return "PIT_ReasonUnspawned".Translate();
                case ReadinessBlockReason.NoEligibleWarden:
                    return "PIT_ReasonNoWarden".Translate();
                case ReadinessBlockReason.ModeRequirement:
                    return "PIT_ReasonModeRequirement".Translate();
                default:
                    return "PIT_ReasonUnknown".Translate();
            }
        }

        private static string Format(int ticks)
        {
            TimeDisplayFormat format =
                Bootstrap.PrisonerInteractionTimerMod.Settings?
                    .DisplayFormat ??
                TimeDisplayFormat.HumanReadable;
            return InteractionTimeFormatter.Format(ticks, format);
        }
    }
}
