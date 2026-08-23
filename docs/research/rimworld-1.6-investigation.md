# RimWorld 1.6 technical investigation

Investigated against:

- executable:
  `<rimworld-install>\RimWorldWin64.exe`
- managed assembly:
  `<rimworld-install>\RimWorldWin64_Data\Managed\Assembly-CSharp.dll`
- assembly version: `1.6.9676.17238`

No RimWorld or dependency source was copied into this repository.

## Timing source

`Pawn_GuestTracker.ScheduledForInteraction` is the common gate used by the
1.6 warden interaction work givers. It returns true only when:

- `lastAssignedInteractTime < TicksGame - 10000`; and
- `interactionsToday < 2`.

The strict comparison means the first eligible tick is
`lastAssignedInteractTime + 10001`.

`Toils_Interpersonal.SetLastInteractTime` writes the current game tick and
increments `interactionsToday` immediately before the final recruit, convert,
enslave/reduce-will, or interrogation action. `Pawn_MindState` resets the
daily counter when `GenLocalDate.DayTick(pawn)` rolls over.

## Eligibility paths inspected

- `WorkGiver_Warden.ShouldTakeCareOfPrisoner`
- `WorkGiver_Warden_Chat`
- `WorkGiver_Warden_Convert`
- `WorkGiver_Warden_Enslave`
- `WorkGiver_Warden_InterrogateIdentity`
- `JobDriver_ChatWithPrisoner`
- `JobDriver_ConvertPrisoner`
- `JobDriver_EnslaveOrReduceWillPrisoner`
- `JobDriver_InterrogatePrisoner`
- `Toils_Interpersonal`

The shared conditions include prisoner security, spawn state, mental state,
sleep where required, downed-in-bed handling, reservation/reachability, the
Warden work assignment, and Talking capacity. Conversion also needs
a warden of the target ideoligion; reduction-only modes need resistance or
will remaining; enslavement respects the warden's ideological willingness.

The evaluator never calls a work giver's `JobOnThing`; doing so would be a
poor read-only query surface and could mutate transient failure state.

## UI path inspected

`ITab_Pawn_Prisoner` inherits `ITab_Pawn_Visitor`, whose private
`DoPrisonerTab(bool, Listing_Standard)` draws the existing policy UI while its
listing is still active. A postfix appends one panel to that listing. This
preserves vanilla sizing and every existing row and avoids a transpiler or
replacement tab.

## Overview decision

The inspected 1.6 pawn-table system has reusable columns, but vanilla does not
provide a general prisoner overview table. Replacing another tab or building
a new main table would exceed the requested compact integration. A public
cell renderer is supplied for compatible overview mods instead.
