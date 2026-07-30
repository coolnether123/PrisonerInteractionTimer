# Architecture

## Scope and invariants

The mod is read-only with respect to gameplay. It never writes
`lastAssignedInteractTime`, `interactionsToday`, a prisoner's policy, jobs,
interaction chances, or cooldown constants. Its only save data is optional
notification subscriptions and their transition baselines.

The implementation is separated into:

- `Domain`: pure cooldown math, time formatting, and ready-transition logic.
- `Compatibility`: the registry of interaction modes known to use vanilla's
  `ScheduledForInteraction` gate.
- `Readiness`: active-mode discovery, vanilla gate sampling, blocker
  resolution, and bounded warden capability checks.
- `UI`: the appended Prisoner-tab panel and opt-in compact overview renderer.
- `Notifications`: per-prisoner/per-mode subscriptions held by a game-scoped
  service and scribed as an optional vanilla `Game` data node.
- `Patches`: three narrow postfixes with direct installers, applied through
  Spine's guarded Harmony helper.
- `Settings`: Spine settings definitions, automatic scribing, and drawer.

## Vanilla authority

`Pawn_GuestTracker.ScheduledForInteraction` remains the authoritative ready
signal. Pure calculation mirrors its strict interval and daily cap only to
produce a countdown:

- interaction is allowed after, not at, 10,000 ticks since
  `lastAssignedInteractTime`;
- no more than two assigned interactions occur per local day;
- the local-day counter resets through `Pawn_MindState`.

The UI samples this state every draw, so game-speed changes and interaction
mode changes require no wall-clock timers or cache invalidation.

## Removal-safe notification persistence

Notification watches are not stored as a custom `GameComponent`. RimWorld
writes the service's list as an optional child of its existing game data
through an `ExposeSmallComponents` postfix. When the mod is absent, RimWorld
ignores that unknown child instead of trying to instantiate a missing custom
component class. This keeps saves loadable without red errors after removal.

The service binds its in-memory list to the current `Game` reference, so a
return to the main menu or a different save cannot leak watch state. A
`GameComponentUtility.GameComponentTick` postfix supplies the same bounded
250-tick polling cadence without adding a save-visible component type.

## Blocker resolution

Cheap prisoner conditions are checked directly each draw. A warden scan is
only performed after the time gate and prisoner conditions are ready. Its
path/reservation result is cached for 120 game ticks and keyed by prisoner,
map, and active-mode signature. Mode changes therefore bypass an old result.

The scan mirrors the relevant 1.6 work-giver conditions without calling
`JobOnThing`, avoiding job creation, `JobFailReason` mutation, or other
work-giver side effects.

## Compatibility API

`PrisonerInteractionTimer.Api.PrisonerInteractionTimerApi` allows another mod
to:

- register a mode defName as timed or untimed;
- query the current readiness snapshot;
- draw a compact indicator inside an overview cell it already owns.

Unknown modded modes are not guessed. A mod that uses
`Pawn_GuestTracker.ScheduledForInteraction` registers its defName explicitly.
This avoids claiming that release, execution, medical, or other untimed
policies share the recruitment cooldown.

Vanilla has no general prisoner overview table in 1.6. The mod therefore does
not replace a whole interface just to add a column. The compact renderer is an
opt-in API for existing tables.

## One-caller helpers

`PrisonerOverviewIndicator.Draw` currently has one in-assembly caller, the
public overview API. This is intentional: it isolates optional table drawing
while leaving table ownership with the consumer. It should remain local until
at least one real overview integration requires a broader abstraction.

The private `StatusColor` and `ClampRemaining` helpers each have one direct
caller. They remain private because they isolate palette policy and
overflow-safe arithmetic respectively; neither belongs in shared
infrastructure. `FormatHumanReadable`, `BuildDefinitions`,
`FindEligibleWarden`, and `IsBaseEligible` also each have one direct source
caller. They remain private, local steps because they isolate a format branch,
declarative settings construction, and the two levels of a bounded warden
query; promoting any of them would add a public abstraction without another
consumer. Harmony calls `TargetMethod` by convention rather than by a visible
source reference.

Empty scaffold registries for patch and compatibility modules were removed.
The direct installers now cover one UI patch and two notification lifecycle
hooks, while the real compatibility surface remains the interaction-mode
registry. If a second warden-query consumer is added later, reassess the local
boundary then rather than centralizing it speculatively now.
