using PrisonerInteractionTimer.Domain;
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
        internal static readonly SettingsSchema<
            PrisonerInteractionTimerSettings> Schema =
            new SettingsSchema<PrisonerInteractionTimerSettings>(
                SettingsSchemaConventions.LowerCamelCase);

        static PrisonerInteractionTimerSettings()
        {
            var display = Schema.Section(
                "display.header", "Timer display", "PIT_SettingsDisplayHeader");
            display.Toggle("display.enabled", settings => settings.FeatureEnabled,
                "Show prisoner interaction timers",
                "Adds vanilla readiness timing and blockers to the selected prisoner's Prisoner tab.")
                .ControlsChildren()
                .Localized("PIT_SettingsEnabledLabel", "PIT_SettingsEnabledTooltip");

            var children = Schema.Under("display.enabled");
            children.Enum("display.format", settings => settings.DisplayFormat,
                "Countdown format", "Choose how remaining game time is shown.",
                labelProvider: FormatLabel, descriptionProvider: FormatDescription)
                .Localized("PIT_SettingsFormatLabel", "PIT_SettingsFormatTooltip");
            children.Toggle("display.reasons", settings => settings.ShowDetailedReasons,
                "Show detailed blocker explanations",
                "Lists all currently known vanilla blockers in the timer tooltip.")
                .Localized("PIT_SettingsReasonsLabel", "PIT_SettingsReasonsTooltip");

            var notifications = Schema.Section(
                "notifications.header", "Notifications", "PIT_SettingsNotificationsHeader");
            notifications.Toggle("notifications.enabled",
                settings => settings.NotificationsEnabled,
                "Enable per-prisoner ready notifications",
                "When enabled, the Prisoner tab offers an opt-in alert for the prisoner's current interaction mode. Alerts fire once per not-ready to ready transition.")
                .Localized("PIT_SettingsNotificationsLabel", "PIT_SettingsNotificationsTooltip");
        }

        public bool FeatureEnabled = true;
        public TimeDisplayFormat DisplayFormat = TimeDisplayFormat.HumanReadable;
        public bool ShowDetailedReasons = true;
        public bool NotificationsEnabled;

        public override void ExposeData()
        {
            Schema.Scribe(this);
            base.ExposeData();
        }

        private static string FormatLabel(TimeDisplayFormat value)
        {
            switch (value)
            {
                case TimeDisplayFormat.GameHours:
                    return "PIT_SettingsFormatHours".Translate();
                case TimeDisplayFormat.Ticks:
                    return "PIT_SettingsFormatTicks".Translate();
                default:
                    return "PIT_SettingsFormatHuman".Translate();
            }
        }

        private static string FormatDescription(TimeDisplayFormat value)
        {
            switch (value)
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
