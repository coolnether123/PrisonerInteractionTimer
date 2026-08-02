# Harmony patches

## `ITab_Pawn_Visitor.DoPrisonerTab`

Type: postfix.

Reason: this is the narrowest point at which the selected prisoner's vanilla
`Listing_Standard` is active. The postfix adds the timer panel before vanilla
ends the listing and computes tab height.

Safety:

- target is resolved by declaring type, exact method name, and exact parameter
  types;
- applied through the shared consumer installer's `PatchTypeOnce`;
- exactly one patched method is required, otherwise a controlled error is
  logged;
- the postfix does not change arguments, return values, policy state, jobs, or
  timing;
- no transpiler is used.

## `Game.ExposeSmallComponents`

Type: postfix.

Reason: persists the optional notification-watch list as a plain child of
vanilla game data. No custom `GameComponent` type is written into the
components list, so removing the mod does not leave a missing class for
RimWorld to instantiate.

## `GameComponentUtility.GameComponentTick`

Type: postfix.

Reason: advances the game-scoped notification service. The postfix performs a
constant-time interval check on ordinary ticks and evaluates only explicitly
watched prisoner/mode pairs every 250 ticks.

Both lifecycle targets are resolved by exact declaring type and method name.
The same installer and Harmony owner are used for the tab and notification
patch classes; each named install has target-specific failure diagnostics.
