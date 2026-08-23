# Prisoner Interaction Timer

Prisoner Interaction Timer shows RimWorld 1.6's prisoner interaction timing in
the selected prisoner's Prisoner tab. It lists the active timed policy, the
next eligible game time, and anything in the base game that is blocking the
interaction. It never changes cooldowns or assigns work.

The current public release supports RimWorld 1.6 only. Older support branches
and payload directories remain in Git as archives, but they are not part of
the current package.

Runtime dependencies:

- RimWorld 1.6
- Harmony
- SpineLib

The standalone SpineLib mod supplies the shared settings UI and guarded Harmony
patch application. Better Work Tab is not a runtime or build dependency.

Alt-click the timer heading, time value, blocker lines, or watch control to
open the matching setting. Doing this on the watch control will not change the
checkbox.

## Installation

Prisoner Interaction Timer needs Harmony and [SpineLib](https://steamcommunity.com/sharedfiles/filedetails/?id=3778463813). Harmony and SpineLib are available on the Steam Workshop; repository builds are at [coolnether123/Spine](https://github.com/coolnether123/Spine).

Copy both `PrisonerInteractionTimer` and `Spine` into RimWorld's `Mods`
directory, then enable Harmony, SpineLib, and Prisoner Interaction Timer in that
order. No other mod is required.

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

See [architecture](docs/architecture.md), the
[RimWorld 1.6 investigation](docs/research/rimworld-1.6-investigation.md), and
[verification record](docs/verification.md).
