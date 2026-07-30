# Compatibility and performance

## Compatibility

Built-in timed defNames are recruitment, resistance reduction, conversion,
enslavement, will reduction, and interrogation. Core untimed policies are
explicitly excluded. Other mods can register modes using the public API when
they use the same vanilla scheduling gate.

The patch appends to vanilla's existing listing rather than replacing the
Prisoner tab. Mods that also postfix the same method remain compatible under
normal Harmony composition; layout order follows Harmony patch order.

Ideology- and Anomaly-specific modes are discovered by defName and public
definitions, so their absence does not create a hard expansion dependency.

## Performance

- Cooldown arithmetic and blocker checks are allocation-light and only occur
  while the selected Prisoner tab or an opted-in overview cell is drawn.
- Reachability/reservation checks occur only when the time gate and prisoner
  conditions are ready.
- Warden results are cached for 120 game ticks per prisoner/map/mode
  signature.
- A constant-time notification hook runs with vanilla game components;
  readiness polling occurs every 250 game ticks and only evaluates explicitly
  watched prisoner/mode pairs.
- Notification messages require both the master feature switch and the
  notification switch. Disabled polling advances the readiness baseline so
  re-enabling cannot emit an old transition.
- No per-frame or per-tick work runs when the UI is closed and no notification
  watches exist, apart from the notification hook's constant-time interval
  guard.

The 120-tick warden cache may delay a changed reachability explanation by at
most two in-game seconds at normal speed. Timing itself is never cached.
