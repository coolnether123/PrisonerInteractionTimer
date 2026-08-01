# Verification record

The 2026-07-31 compatibility checkpoint is recorded in
`compatibility-investigation-2026-07-31.md` with its compact companion matrix
in `compatibility-matrix-2026-07-31.md`.

This file records commands and evidence. A build alone is not treated as
end-to-end completion.

## Automated contracts

```powershell
dotnet run --project A:\Dev\RimWorld\Mods\PrisonerInteractionTimer\Tests\Mod.Tests.csproj -c Release
```

Result on 2026-07-30: `PASS: 24 prisoner timer contract assertions`.

Covered contracts include the strict vanilla cooldown boundary, fresh-pawn
readiness, local-day cap, max-of-gates behavior, overflow safety, display
formats, notification edge detection, mode-change suppression, deterministic
blocked-state suppression, master/notification switch suppression, disabled
baseline advancement, stale-alert prevention on re-enable, and interaction
counts beyond the daily cap.
Both XML files parse successfully, and all 51 `PIT_*` source translation keys
have exactly one corresponding English keyed entry with no missing or unused
keys.

## Central build

Built with RimWorld Tooling using RimWorld 1.6 plus `harmony` and `spine`.
The successful build output records:

- RimWorld managed assembly version: `1.6.9676.17238`
- Spine SHA-256:
  `2441959E82AA5CAC5C96E7456213B21D1FB67881E314F85F54373A4DB8C0E2AA`
- final packaged mod DLL SHA-256:
  `1C6E59BBCAE689827221D04054A6C534CB3E05F19DB2764B47424FB4D0AEAD08`

The final build resolves Spine commit
`650fb95835d187777fae314e1de361b8991b33ee`. Package validation returned
`RWT-BUILD-PACKAGE-VALID` for the single 31,744-byte assembly.

## Initial runtime candidate

The initial feature candidate DLL
`D020BA2F33CD27ED96228DCBA820768E2D9BFB1F679804B81294EAEE208EC232`
loaded through the isolated agentic harness with Core, Harmony, RimWorld
Agent, Spine, and Prisoner Interaction Timer in RimWorld
`1.6.4871 rev574`.

Session `PrisonerInteractionTimer-abbf441c3fa248b89083c8167fb15de1`
reported exactly three owned methods and three postfixes, with zero prefixes,
transpilers, or finalizers. A harness-created prisoner remained secure in its
enclosed cell. The ordinary Prisoner tab rendered cooldown, ready, asleep, and
no-eligible-warden states. With notifications enabled and the per-mode
checkbox selected, the mod emitted exactly one `Camisa is ready for Recruit.`
message on the not-ready to ready transition.

`PIT_RemovalSafe_Watched` saved through RimWorld's normal save path and loaded
to completion (`completedLoadGeneration=1`). The selected Recruit-mode watch
remained checked after load. Error-filtered logs contained no matching in-game
or output-log messages. The process exited normally through the harness
(`ExitCode=0`, `ForcedTermination=false`).

Evidence:

- notification and persisted-watch capture:
  `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\PrisonerInteractionTimer-abbf441c3fa248b89083c8167fb15de1\ipc\captures\pit-removal-safe-notification-20260730-205055-654.png`
- preserved exact-candidate evidence:
  `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\PrisonerInteractionTimer-abbf441c3fa248b89083c8167fb15de1\evidence`

## Exact-final master-switch regression

Session `PrisonerInteractionTimer-a5d19fe002ef495e8f2b2c1a6f2cf8f5`
loaded the final `1C6E59...` assembly. A watched Recruit prisoner was first
polled while not ready. With notifications enabled, the harness then disabled
the master `FeatureEnabled` setting, forced the prisoner to vanilla-ready, and
advanced 250 paused ticks across a notification poll. The prisoner remained
`ScheduledForInteraction=True`, and no message appeared.

The harness re-enabled the master setting and advanced another 250 paused
ticks across the next poll. The Prisoner tab rendered `Ready now`, the
per-mode watch remained checked, and no stale catch-up message appeared.
Harmony reported exactly three owned postfixes; error-filtered logs contained
no matches. The harness released the process normally
(`ExitCode=0`, `ForcedTermination=false`).

Evidence:

- disabled transition:
  `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\PrisonerInteractionTimer-a5d19fe002ef495e8f2b2c1a6f2cf8f5\ipc\captures\pit-master-disabled-ready-paused-20260730-213945-012.png`
- re-enabled ready state with no catch-up alert:
  `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\PrisonerInteractionTimer-a5d19fe002ef495e8f2b2c1a6f2cf8f5\ipc\captures\pit-master-reenabled-ready-no-catchup-paused-20260730-214004-670.png`

## Removal and add-back compatibility

Session `Spine-4f553189a4bc41adb123c0e269b50a34` loaded
`PIT_RemovalSafe_Watched` with Prisoner Interaction Timer absent. Prisoner
`44542` remained present, secure, and configured for Recruit. Error- and
exception-filtered logs contained no matches. RimWorld then wrote
`PIT_NoMod_Resaved` normally, and the harness released the process without a
forced termination.

Session `PrisonerInteractionTimer-84bf135fd2974669ade2ed41d03c0da4`
loaded that mod-free resave after adding Prisoner Interaction Timer. The same
prisoner remained intact and secure, the timer panel rendered `0.1 h` for
Recruit, Harmony again reported exactly three owned postfixes, and
exception-filtered logs contained no matches. The harness released the process
normally.

Evidence:

- no-mod removal capture:
  `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\Spine-4f553189a4bc41adb123c0e269b50a34\ipc\captures\pit-removal-safe-no-mod-20260730-205404-913.png`
- add-back timer capture:
  `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\PrisonerInteractionTimer-84bf135fd2974669ade2ed41d03c0da4\ipc\captures\pit-existing-save-add-back-visible-20260730-205916-147.png`

The only runtime warning observed was shared metadata validation noting that
the local Spine dependency has no public download or Workshop URL. This mod
does not fabricate one.
