using PrisonerInteractionTimer.Patches;
using PrisonerInteractionTimer.Settings;
using Spine.UI.SettingsFramework;
using UnityEngine;
using Verse;

namespace PrisonerInteractionTimer.Bootstrap
{
    public sealed class PrisonerInteractionTimerMod : Mod
    {
        private readonly PrisonerInteractionTimerSettings settings;
        private readonly SettingsListDrawer settingsDrawer;
        private SettingsViewMode settingsViewMode =
            SettingsViewMode.Simple;

        public PrisonerInteractionTimerMod(ModContentPack content)
            : base(content)
        {
            settings = GetSettings<PrisonerInteractionTimerSettings>();
            Settings = settings;
            settingsDrawer = new SettingsListDrawer(
                new SettingsHierarchy(
                    PrisonerInteractionTimerSettings.Definitions));

            PrisonerTabPatchInstaller.Install();
            NotificationHookInstaller.Install();
        }

        public static PrisonerInteractionTimerSettings Settings { get; private set; }

        public override string SettingsCategory()
        {
            return "PIT_ModName".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settingsDrawer.Draw(
                inRect,
                settings,
                ref settingsViewMode,
                WriteSettings);
        }
    }
}
