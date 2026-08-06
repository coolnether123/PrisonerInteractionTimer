using System.Collections.Generic;
using PrisonerInteractionTimer.Domain;
using Spine.Api;
using Spine.UI.SettingsFramework;
using Verse;

namespace PrisonerInteractionTimer.Settings
{
    /// <summary>
    /// Stores the player choices that control timer presentation and opt-in
    /// readiness alerts through Spine's shared settings framework.
    /// </summary>
    public sealed class PrisonerInteractionTimerSettings : ModSettings
    {
        private static readonly IReadOnlyList<SettingDefinition>
            DefinitionList = BuildDefinitions();

        public bool FeatureEnabled = true;
        public TimeDisplayFormat DisplayFormat =
            TimeDisplayFormat.HumanReadable;
        public bool ShowDetailedReasons = true;
        public bool NotificationsEnabled;

        public static IReadOnlyList<SettingDefinition> Definitions =>
            DefinitionList;

        public override void ExposeData()
        {
            SpineApi.Settings.Scribe(this, DefinitionList);
            base.ExposeData();
        }

        private static IReadOnlyList<SettingDefinition> BuildDefinitions()
        {
            return new[]
            {
                SettingDefinitions.Header(
                    "display.header",
                    "Timer display",
                    "PIT_SettingsDisplayHeader"),
                SettingDefinitions.Toggle(
                    "display.enabled",
                    nameof(FeatureEnabled),
                    "Show prisoner interaction timers",
                    "PIT_SettingsEnabledLabel",
                    "Adds vanilla readiness timing and blockers to the " +
                        "selected prisoner's Prisoner tab.",
                    "PIT_SettingsEnabledTooltip",
                    controlsChildren: true,
                    scribeKey: "featureEnabled"),
                SettingDefinitions.Enum(
                    "display.format",
                    nameof(DisplayFormat),
                    typeof(TimeDisplayFormat),
                    "Countdown format",
                    "PIT_SettingsFormatLabel",
                    "Choose how remaining game time is shown.",
                    "PIT_SettingsFormatTooltip",
                    parentId: "display.enabled",
                    scribeKey: "displayFormat",
                    labelProvider: FormatLabel,
                    descriptionProvider: FormatDescription),
                SettingDefinitions.Toggle(
                    "display.reasons",
                    nameof(ShowDetailedReasons),
                    "Show detailed blocker explanations",
                    "PIT_SettingsReasonsLabel",
                    "Lists all currently known vanilla blockers in the " +
                        "timer tooltip.",
                    "PIT_SettingsReasonsTooltip",
                    parentId: "display.enabled",
                    scribeKey: "showDetailedReasons"),
                SettingDefinitions.Header(
                    "notifications.header",
                    "Notifications",
                    "PIT_SettingsNotificationsHeader"),
                SettingDefinitions.Toggle(
                    "notifications.enabled",
                    nameof(NotificationsEnabled),
                    "Enable per-prisoner ready notifications",
                    "PIT_SettingsNotificationsLabel",
                    "When enabled, the Prisoner tab offers an opt-in alert " +
                        "for the prisoner's current interaction mode. Alerts " +
                        "fire once per not-ready to ready transition.",
                    "PIT_SettingsNotificationsTooltip",
                    scribeKey: "notificationsEnabled")
            };
        }

        private static string FormatLabel(object value)
        {
            switch ((TimeDisplayFormat)value)
            {
                case TimeDisplayFormat.GameHours:
                    return "PIT_SettingsFormatHours".Translate();
                case TimeDisplayFormat.Ticks:
                    return "PIT_SettingsFormatTicks".Translate();
                default:
                    return "PIT_SettingsFormatHuman".Translate();
            }
        }

        private static string FormatDescription(object value)
        {
            switch ((TimeDisplayFormat)value)
            {
                case TimeDisplayFormat.GameHours:
                    return "PIT_SettingsFormatHoursDescription".Translate();
                case TimeDisplayFormat.Ticks:
                    return "PIT_SettingsFormatTicksDescription".Translate();
                default:
                    return "PIT_SettingsFormatHumanDescription".Translate();
            }
        }
    }
}
