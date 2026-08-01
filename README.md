# Prisoner Interaction Timer

Prisoner Interaction Timer exposes RimWorld 1.6's existing prisoner
interaction timing in the selected prisoner's Prisoner tab. It reports the
active timed policy, the next eligible game time, and current vanilla blockers
without changing cooldowns or issuing work.

Runtime dependencies:

- RimWorld 1.6
- Harmony
- Spine

The standalone Spine mod supplies the shared settings UI and guarded Harmony
patch application. Better Work Tab is not a runtime or build dependency.

Alt-click the timer heading, time value, blocker lines, or watch control to
open and highlight the corresponding setting. The Alt-click is consumed before
the watch checkbox can change.

## Installation

Spine does not yet have a public Workshop or download URL, so this verified
build is distributed in the local collection at
`A:\Dev\RimWorld\Releases\1.6\2026-07-30-program-final`. Copy both
`PrisonerInteractionTimer` and `Spine` into RimWorld's `Mods` directory, then
enable Harmony, Spine, and Prisoner Interaction Timer in that order. No other
gameplay mod in the collection is required.

## Build and test

```powershell
dotnet run --project A:\Dev\RimWorld\Mods\PrisonerInteractionTimer\Tests\Mod.Tests.csproj -c Release
```

Use the centralized RimWorld tooling for release builds:

```powershell
Set-ExecutionPolicy -Scope Process Bypass -Force
$repo = 'A:\Dev\RimWorld\Mods\PrisonerInteractionTimer'
& 'A:\Dev\RimWorld\Worktrees\RimWorld-Tooling\phase-a\tools\Invoke-RimWorldBuild.ps1' `
  -Project "$repo\Source\Mod.csproj" `
  -Configuration Release `
  -Version 1.6 `
  -OutputRoot "$repo\Engineering\artifacts\central-build" `
  -Engine MSBuild `
  -Dependency @('harmony', 'spine')
```

See [architecture](docs/architecture.md), the
[RimWorld 1.6 investigation](docs/research/rimworld-1.6-investigation.md), and
[verification record](docs/verification.md).
