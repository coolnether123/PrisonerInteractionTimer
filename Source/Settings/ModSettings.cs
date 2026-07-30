using System.Collections.Generic;
using PrisonerInteractionTimer.Domain;
using Spine.UI.SettingsFramework;
using Verse;

namespace PrisonerInteractionTimer.Settings
{
    public sealed class PrisonerInteractionTimerSettings : ModSettings
    {
        private static readonly IReadOnlyList<SettingDefinition>
            SettingDefinitions = BuildDefinitions();

        public bool FeatureEnabled = true;
        public TimeDisplayFormat DisplayFormat =
            TimeDisplayFormat.HumanReadable;
        public bool ShowDetailedReasons = true;
        public bool NotificationsEnabled;

        public static IReadOnlyList<SettingDefinition> Definitions =>
            SettingDefinitions;

        public override void ExposeData()
        {
            SettingsScribe.ScribeAll(this, SettingDefinitions);
            base.ExposeData();
        }

        private static IReadOnlyList<SettingDefinition> BuildDefinitions()
        {
            return new[]
            {
                new SettingDefinition
                {
                    Id = "display.header",
                    Type = SettingType.Header,
                    Label = "Timer display",
                    LabelKey = "PIT_SettingsDisplayHeader",
                    SortOrder = 0,
                    ShowInSimpleView = true
                },
                new SettingDefinition
                {
                    Id = "display.enabled",
                    FieldName = nameof(FeatureEnabled),
                    ScribeKey = "featureEnabled",
                    Type = SettingType.Bool,
                    Label = "Show prisoner interaction timers",
                    LabelKey = "PIT_SettingsEnabledLabel",
                    Tooltip = "Adds vanilla readiness timing and blockers " +
                        "to the selected prisoner's Prisoner tab.",
                    TooltipKey = "PIT_SettingsEnabledTooltip",
                    DefaultValue = true,
                    SortOrder = 10,
                    ShowInSimpleView = true,
                    ControlsChildVisibility = true
                },
                new SettingDefinition
                {
                    Id = "display.format",
                    ParentId = "display.enabled",
                    FieldName = nameof(DisplayFormat),
                    ScribeKey = "displayFormat",
                    Type = SettingType.Enum,
                    EnumType = typeof(TimeDisplayFormat),
                    EnumLabelProvider = FormatLabel,
                    EnumDescriptionProvider = FormatDescription,
                    Label = "Countdown format",
                    LabelKey = "PIT_SettingsFormatLabel",
                    Tooltip = "Choose how remaining game time is shown.",
                    TooltipKey = "PIT_SettingsFormatTooltip",
                    DefaultValue = TimeDisplayFormat.HumanReadable,
                    SortOrder = 20,
                    ShowInSimpleView = true
                },
                new SettingDefinition
                {
                    Id = "display.reasons",
                    ParentId = "display.enabled",
                    FieldName = nameof(ShowDetailedReasons),
                    ScribeKey = "showDetailedReasons",
                    Type = SettingType.Bool,
                    Label = "Show detailed blocker explanations",
                    LabelKey = "PIT_SettingsReasonsLabel",
                    Tooltip = "Lists all currently known vanilla blockers " +
                        "in the timer tooltip.",
                    TooltipKey = "PIT_SettingsReasonsTooltip",
                    DefaultValue = true,
                    SortOrder = 30,
                    ShowInSimpleView = true
                },
                new SettingDefinition
                {
                    Id = "notifications.header",
                    Type = SettingType.Header,
                    Label = "Notifications",
                    LabelKey = "PIT_SettingsNotificationsHeader",
                    SortOrder = 100,
                    ShowInSimpleView = true
                },
                new SettingDefinition
                {
                    Id = "notifications.enabled",
                    FieldName = nameof(NotificationsEnabled),
                    ScribeKey = "notificationsEnabled",
                    Type = SettingType.Bool,
                    Label = "Enable per-prisoner ready notifications",
                    LabelKey = "PIT_SettingsNotificationsLabel",
                    Tooltip = "When enabled, the Prisoner tab offers an " +
                        "opt-in alert for the prisoner's current interaction " +
                        "mode. Alerts fire once per not-ready to ready transition.",
                    TooltipKey = "PIT_SettingsNotificationsTooltip",
                    DefaultValue = false,
                    SortOrder = 110,
                    ShowInSimpleView = true
                }
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
