# Prisoner Interaction Timer compatibility investigation

Date: 2026-07-31 local / 2026-08-01 UTC  
Target commit: `68fcf664dfa5af06910b81efd244bdc1d0a5327d`  
Target DLL SHA-256: `1C6E59BBCAE689827221D04054A6C534CB3E05F19DB2764B47424FB4D0AEAD08`

## Scope and environment

This checkpoint records only results obtained through the canonical isolated
H-drive harness. The executable was
`H:\Games\RimWorld1-6-4871Win64\RimWorldWin64.exe`; the runtime log identifies
RimWorld `1.6.4871 rev574`, while the harness manifest names build
`1.6.4871 rev573`. Core was the only active Ludeon package. Results from a
non-canonical D-drive attempt were discarded and support no conclusion.

The target was a local source checkout, so a download date does not apply. Its
source commit is dated `2026-07-30T20:25:18-05:00`. The tested external mod was
Custom Prisoner Interactions, Workshop item `2841231775`, package
`Mlie.CustomPrisonerInteractions`, author Mlie, version `1.6.2`. SteamCMD
acquired it at `2026-08-01T00:38:47.7150995Z`; its 1.6 DLL SHA-256 is
`E48BEF84924045084A97F294687D43922B98945B53CF7D2E92A311208AA5B03B`.

Harness dependencies in the load-order records are test infrastructure, not
part of either mod's compatibility claim. Every owned process exited with code
0 and `ForcedTermination=false`.

## Findings

### Target alone — compatible on the exercised Core surface

Exact order: Core; Harmony (`brrainz.harmony`); RimWorld Agent
(`Coolnether123.RimWorldAgent`); Spine (`CoolNether123.Spine`); Prisoner
Interaction Timer (`CoolNether123.PrisonerInteractionTimer`).

Session `compat-prisoner-timer-e84ecace7c00404285e77e156a68c6df`
created a secure prisoner, exercised Recruit with a 5,000-tick cooldown,
changed all three settings, enabled a watch through the visible checkbox,
saved, reloaded, and repeated the load with 26 watched prisoners. The panel
showed a countdown only for the timed Recruit mode. Display format, detailed
reason, and notification settings persisted. The watched state also persisted.
At 1920x1080 and UI scale 1, neither the prisoner panel nor settings page
overlapped vanilla controls.

Harmony ownership was narrow: three postfixes and no prefixes, transpilers, or
finalizers. The patched methods were
`RimWorld.ITab_Pawn_Visitor.DoPrisonerTab`,
`Verse.Game.ExposeSmallComponents`, and
`Verse.GameComponentUtility.GameComponentTick`. No target exception was
observed. Searches that appeared to mention `Exception` were the agent log
echoing its own search term, not game exceptions.

Evidence:

- Recruit countdown: `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\compat-prisoner-timer-e84ecace7c00404285e77e156a68c6df\ipc\captures\pit-core-recruit-cooldown-20260801-004758-278.png`
- Settings: `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\compat-prisoner-timer-e84ecace7c00404285e77e156a68c6df\ipc\captures\pit-core-settings-20260801-004804-461.png`
- Watched-state UI: `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\compat-prisoner-timer-e84ecace7c00404285e77e156a68c6df\ipc\captures\pit-core-ticks-notify-20260801-004907-934.png`
- Reloaded watch: `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\compat-prisoner-timer-e84ecace7c00404285e77e156a68c6df\ipc\captures\pit-core-watch-reloaded-20260801-005649-443.png`
- Manifest and log: `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\compat-prisoner-timer-e84ecace7c00404285e77e156a68c6df\lane.json` and `logs\Player.log`
- Baseline save SHA-256: `3A342313B2973E716D1148BA8856A483429DDB050481A6A194586DF71BAA2E0C`
- 26-watch save SHA-256: `C00DFC4803684EB9C143D9027CAEBD0692CE2B62A9A8D330351449C213736CEA`

### Removal from a copied save — compatible on the exercised Core surface

Exact order: Core; Harmony; RimWorld Agent; Spine. Prisoner Interaction Timer
was absent. Session
`compat-prisoner-timer-removal-adf453399a204e02985e89c16955ac82`
loaded the copied 26-watch save, retained prisoner `36960` as a secure colony
prisoner in Recruit mode, and saved again normally. No in-game exception was
observed. The resave SHA-256 is
`8B2C775CF6585253AA081235E5BA4BADF160005A78D6CFC2578E38C1273E8049`.

Evidence:

- Screenshot: `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\compat-prisoner-timer-removal-adf453399a204e02985e89c16955ac82\ipc\captures\pit-removal-no-mod-20260801-010117-618.png`
- Manifest and log: `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\compat-prisoner-timer-removal-adf453399a204e02985e89c16955ac82\lane.json` and `logs\Player.log`

This proves removal safety for the copied Core-only state exercised here. It
does not supersede another mod author's removal instructions for that mod's
custom interaction definitions.

### Custom Prisoner Interactions alone — inconclusive behavior baseline

Exact order: Core; Harmony; RimWorld Agent; Custom Prisoner Interactions.
Session `compat-pit-cpi-alone-c2c747fc22c34b528bd1bae0e29243f1`
reached startup and exposed eleven prisoner-related Harmony patches, including
a prefix and postfix on `ITab_Pawn_Visitor.FillTab`. No exception was observed,
but the short run did not complete a useful prisoner UI scenario. This is an
assembly/startup baseline only, not a behavior compatibility result.

Evidence: `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\compat-pit-cpi-alone-c2c747fc22c34b528bd1bae0e29243f1\lane.json`
and its `logs\Player.log`.

### Target then Custom Prisoner Interactions — compatible on tested Core modes

Exact order: Core; Harmony; RimWorld Agent; Spine; Prisoner Interaction Timer;
Custom Prisoner Interactions. Session
`compat-pit-cpi-a-da396246ba194a2ab7c7d9e385c58568` exercised the shared
Prisoner-tab surface. Release, including CPI's visible Options row, remained
untimed and the PIT panel correctly said `Not scheduled`. The same prisoner in
Recruit mode showed a countdown. The panels did not overlap and no duplicate
controls or tooltips were visible. Saving and reloading completed without an
observed exception.

Patch ownership was compositional in the tested order: PIT patches
`DoPrisonerTab`; CPI surrounds `FillTab`. No conflict was observed on those
methods. The reverse order and CPI's Ideology/Biotech behaviors were not tested,
so the classification is limited to Core and the exact order above.

Evidence:

- Untimed Release: `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\compat-pit-cpi-a-da396246ba194a2ab7c7d9e385c58568\ipc\captures\pit-cpi-release-untimed-20260801-010421-903.png`
- Timed Recruit: `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\compat-pit-cpi-a-da396246ba194a2ab7c7d9e385c58568\ipc\captures\pit-cpi-recruit-timed-20260801-010426-704.png`
- Manifest and log: `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\compat-pit-cpi-a-da396246ba194a2ab7c7d9e385c58568\lane.json` and `logs\Player.log`

## Performance checkpoint — inconclusive

Recorded samples were 758.894 TPS / 1.318 ms wall per tick / 2.152 ms CPU per
tick with one watch, 746.873 / 1.339 / 2.240 with zero watches in an earlier
state, and 732.642 / 1.365 / 2.130 with 25 watches. An equal-population control
after reload and watch removal measured 486.462 / 2.056 / 3.300. That inverse
movement demonstrates run-state noise; it does not show that watches improve
performance. There was no positive regression signal in the watched sample,
but a defensible cost estimate requires repeated A/B/A trials from one frozen
save. This item is classified `inconclusive`, not `compatible`.

## Mode semantics and smallest defensible response

PIT's registry times only `AttemptRecruit`, `ReduceResistance`, `Convert`,
`Enslave`, `ReduceWill`, and `Interrogate`. It explicitly leaves
`MaintainOnly`, `Release`, `Execution`, and `HemogenFarm` untimed. Unknown modes
are untimed unless an integration registers them. The Core CPI result confirms
the important Release behavior at runtime.

Prisoner Utils and Prison Labor add domain-specific interaction modes. Their
presence is an integration opportunity, not proof of a defect. No compatibility
registration should be added until runtime and source tracing prove that a mode
really uses the supported vanilla scheduled-interaction gate. If justified,
that integration belongs in PIT or an optional prisoner-compatibility assembly,
not Spine. No patch is justified by the completed lanes.

## Pending and untested

- CPI-before-PIT load order; CPI's Convert chains and Biotech Hemogen option.
- Prisoner Utils, Prison Labor, and their pair/triple overlap with CPI.
- Locks, Prison Commons Continued, Prisoner Recreation, Prisoners Need
  Recruiting, Prisoner No Interaction Alert, ransom/release/interrogation/
  slavery mods, RimHUD, Numbers, Character Editor, Better Pawn Control, and a
  maintained prisoner-tab replacement.
- Reduce Resistance and all DLC modes; unwavering, quest and entity prisoners;
  sleeping, downed, mental-state and inaccessible prisoners.
- Death, departure, recruitment, enslavement, release, caravan travel, multiple
  colonies/maps, notification transitions across those states, and external-mod
  removal from copied saves.
- UI scales other than 1, resolutions other than 1920x1080, language fallback,
  30-minute accelerated simulation, and repeatable 0/1/25/100-watch profiling.

### Grouped full-DLC checklist

1. Record exact active packages for Royalty, Ideology, Biotech, Anomaly, and
   Odyssey and repeat both load orders where dependencies permit.
2. Prove Recruit, Reduce Resistance, Convert, Reduce Will, Enslave, and
   Interrogate are timed only when enabled and actually gated by
   `ScheduledForInteraction`; prove Maintain, Release, Execution, and Hemogen
   Farm remain untimed with no watch control.
3. Verify Prisoner Utils combined modes remain N/A unless their real timing gate
   is proved and registered; never invent a countdown.
4. Exercise CPI options around Convert and Release, including transitions; no
   stale notification may survive an option or base-mode change.
5. Exercise Prison Labor Work, Work and Recruit, Work and Convert, and Work and
   Enslave modes alone and in the CPI/Utils/PL triple stack. Separate external
   missing-definition defects from PIT behavior.
6. Cross every supported mode with awake, asleep, downed in/out of bed, mental
   state, inaccessible, no eligible warden, unwavering, quest prisoner, and
   entity states.
7. Verify Hemogen Farm and CPI auto-enable remain untimed and do not conflict
   with Prisoner No Interaction Alert.
8. Test death, departure, recruitment, enslavement, release, caravan travel,
   another map, a second colony, and gravship transition for watch cleanup and
   isolation.
9. Save/reload and remove PIT from a copy. For external custom-mode removal,
   follow that author's removal instructions and verify no missing Def remains.
10. Check 1280x720, 1920x1080, and 2560x1440 at supported 0.75, 1, 1.25, and
    1.5 UI scales with RimHUD and inspect-tab extensions.
11. Capture Harmony ownership and logs, then profile 0, 1, 25, and 100 watches
    using frozen-save A/B/A repetitions.

## Release triage

Highest-value unresolved checks are the full-DLC mode matrix, the
CPI/Prisoner Utils/Prison Labor overlap, and multi-scale panel layout. Repeated
performance trials and lifecycle/map transitions follow. The tested CPI Core
pair and PIT removal path produced no release blocker. There are no confirmed
hard conflicts, patch-required findings, target defects, or external defects in
the completed lanes. There is one integration opportunity—verified custom
timed modes—but it should wait for evidence and remain prisoner-domain code.

