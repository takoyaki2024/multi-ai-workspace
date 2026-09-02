# TASKS

## IN_PROGRESS

### T-003 — Evaluate executable anomaly tracking during real work
Related requirements: REQ-007, REQ-008

Acceptance criteria:
- Use the objective thresholds already fixed in `SPEC.md`.
- Do not add a complex tracker before there are real retry/Codex events to measure.
- On the first real threshold event, record enough structured data to determine whether a small executable tracker is justified.
- If implementation is justified, keep it deterministic and repository-local.

## TODO

None.

## DONE

### T-002 — Validate CI state writer failure path safely
Related requirements: REQ-003, REQ-007

Completed evidence:
- Shared state-writer logic was extracted to `scripts/update-project-state.ps1`.
- CI validates a simulated Build failure against a temporary copy of `PROJECT_STATE.md`.
- The validation asserts `Build: FAIL` and `Tests: NOT_RUN` are written correctly.
- The validation asserts ChatGPT-owned judgment fields are unchanged.
- The validation asserts the real `PROJECT_STATE.md` is not modified by the simulation.
- GitHub Actions run 33591357926 passed Restore, Build, Test, safe failure-state validation, PROJECT_STATE update, and final enforcement.

### T-001 — Establish ChatGPT-centered V1 foundation
Related requirements: REQ-001..REQ-010

Completed evidence:
- Durable project-memory files exist.
- CI writes objective Build/Test facts into `PROJECT_STATE.md` without overwriting judgment fields.
- CI state-only commit did not recursively trigger CI.
- Codex return validation gate is documented.
- STOP routes and objective anomaly thresholds are documented.
- GitHub Actions run 33590695021 passed Restore, Build, Test, PROJECT_STATE update, and final enforcement.
