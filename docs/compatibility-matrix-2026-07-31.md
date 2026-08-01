# Prisoner Interaction Timer compatibility matrix

All runtime rows use RimWorld `1.6.4871 rev574` (harness manifest
`1.6.4871 rev573`) from the canonical H-drive install with Core only.

| Target / combination | Exact relevant order | Scenario | Classification | Evidence |
|---|---|---|---|---|
| PIT `CoolNether123.PrisonerInteractionTimer` commit `68fcf664` | Core, Harmony, Agent, Spine, PIT | Recruit countdown; settings; watch; save/reload; 26 watched prisoners | **Compatible** on exercised Core behavior, UI scale 1, and save surface; performance separately inconclusive | `compat-prisoner-timer-e84ecace7c00404285e77e156a68c6df` |
| PIT removed from copied save | Core, Harmony, Agent, Spine | Load watched save without PIT; inspect prisoner; resave | **Compatible** for tested copied Core save | `compat-prisoner-timer-removal-adf453399a204e02985e89c16955ac82` |
| Custom Prisoner Interactions `Mlie.CustomPrisonerInteractions` 1.6.2 alone | Core, Harmony, Agent, CPI | Startup and Harmony ownership only | **Inconclusive** behavior baseline | `compat-pit-cpi-alone-c2c747fc22c34b528bd1bae0e29243f1` |
| PIT + Custom Prisoner Interactions 1.6.2 | Core, Harmony, Agent, Spine, PIT, CPI | CPI Release options stay untimed; Recruit remains timed; UI; save/reload; patches | **Compatible with a documented limitation**: Core and this order only | `compat-pit-cpi-a-da396246ba194a2ab7c7d9e385c58568` |
| PIT + CPI, reverse order | Not run | Required second-order test | **Inconclusive** | Pending |
| PIT + Prisoner Utils | Not run | Custom combined modes and gate ownership | **Integration opportunity / inconclusive** | Pending grouped full-DLC pass |
| PIT + Prison Labor | Not run | Custom work modes, timing gate, panel overlap | **Integration opportunity / inconclusive** | Pending grouped full-DLC pass |
| PIT + CPI + Prisoner Utils + Prison Labor | Not run | Known overlapping custom-mode surface | **Inconclusive** | Pending grouped full-DLC pass |
| Locks, Prison Commons, Prisoner Recreation, Prisoners Need Recruiting, alerts and ransom mods | Not run | Security, needs, alerts and lifecycle surfaces | **Inconclusive** | Pending |
| RimHUD, Numbers, Character Editor, Better Pawn Control, inspect-tab extensions | Not run | Panel composition and unusual state setup | **Inconclusive** | Pending |
| DLC modes and special prisoner states | Not run on canonical install | Full mode/state/lifecycle matrix | **Inconclusive** | Pending grouped full-DLC pass |
| 25-watch performance sample | Core, Harmony, Agent, Spine, PIT | 0/1/25 watches; sequential samples | **Inconclusive** due inverse equal-population control | Baseline session above |

No tested row produced a confirmed failure, hard conflict, target defect,
external-mod defect, or patch-required result. Unknown custom timing modes must
remain untimed until their actual gate is proved; any resulting integration is
PIT-domain code, not a Spine service.

