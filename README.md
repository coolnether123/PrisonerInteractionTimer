# Prisoner Interaction Timer

Prisoner Interaction Timer exposes RimWorld 1.6's existing prisoner interaction
timing in the selected prisoner's Prisoner tab. It reports the active timed
policy, the next eligible game time, and current vanilla blockers without
changing cooldowns or issuing work.

## Player features

- The selected prisoner's live vanilla interaction cooldown and next eligible
  time, measured in RimWorld game ticks.
- The currently active interaction modes for that prisoner.
- Current blockers, such as sleep, incapacity, or the absence of an eligible
  warden.
- Optional per-prisoner and per-mode notifications that fire once when a
  watched prisoner becomes ready.

The mod never changes cooldowns, assigns wardens, or performs interactions. It
reads and displays vanilla state only.

Alt-click the timer heading, time value, blocker lines, or watch control to
open and highlight the corresponding setting. The Alt-click is consumed before
the watch checkbox can change.

## Requirements

- RimWorld 1.6
- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
- [Spine](https://github.com/coolnether123/Spine) — the shared runtime used by
  CoolNether123 mods

Spine supplies the shared settings UI and guarded Harmony patch application.
Better Work Tab is not a runtime or build dependency.

## Installation

Install Harmony and Spine, copy `PrisonerInteractionTimer` into RimWorld's
`Mods` folder, then enable Harmony, Spine, and Prisoner Interaction Timer in
that order.

The mod stores only global display preferences and adds no game-save
component, so it is safe to add to or remove from an existing save.

## Build and test

```powershell
dotnet run --project .\Tests\Mod.Tests.csproj -c Release
```

Use the centralized RimWorld tooling for release builds:

```powershell
Set-ExecutionPolicy -Scope Process Bypass -Force
$repo = $PWD.Path
$toolingRoot = $env:RIMWORLD_TOOLING_ROOT
if (-not $toolingRoot) { throw 'Set RIMWORLD_TOOLING_ROOT first.' }
& "$toolingRoot\tools\Invoke-RimWorldBuild.ps1" `
  -Project "$repo\Source\Mod.csproj" `
  -Configuration Release `
  -Version 1.6 `
  -OutputRoot "$repo\Engineering\artifacts\central-build" `
  -Engine MSBuild `
  -Dependency @('harmony', 'spine')
```

## Documentation

- [Architecture](docs/architecture.md)
- [Compatibility and performance](docs/compatibility-and-performance.md)
- [Patch inventory](docs/patches.md)
- [RimWorld 1.6 investigation](docs/research/rimworld-1.6-investigation.md)
- [Verification record](docs/verification.md)

## License

Released under the [MIT License](LICENSE). Harmony and Spine are used under
their own licenses.
