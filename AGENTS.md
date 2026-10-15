# Prisoner Interaction Timer Agent Guide

Own only this repository. Expose vanilla timing and eligibility without
changing cooldowns or issuing jobs. Keep readiness evaluation, UI, notification
transition state, patches, and settings separated; use game ticks.

Draw the timer panel after vanilla Prisoner-tab rows, then reserve only its
measured height in the completed listing. Never consume, stretch, or reposition
a vanilla policy row. Measure localized row heights and preserve descender
padding.

Use `SpineMod<PrisonerInteractionTimerSettings>` and its contextual lease. Bind
the panel before the watch checkbox so contextual input cannot mutate watch
state.

Metadata, `Tools\CascadeManifest.json`, project files, and `Tests` define
support and verification. Follow `H:\Dev\RimWorld\AGENTS.md` for shared
build/runtime work.
