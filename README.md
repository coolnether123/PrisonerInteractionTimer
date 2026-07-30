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
