# TASKS

## IN_PROGRESS

### T-001 — Establish ChatGPT-centered V1 foundation
Related requirements: REQ-001..REQ-010

Acceptance criteria:
- Durable project-memory files exist.
- CI writes objective Build/Test facts into `PROJECT_STATE.md` without overwriting judgment fields.
- CI state updates do not recursively trigger CI.
- Codex return validation gate is documented.
- STOP routes and anomaly thresholds are documented.

## TODO

### T-002 — Validate CI state writer in GitHub Actions
Run CI on the committed foundation and verify the machine-owned block updates correctly on both success and failure paths.

### T-003 — Add executable anomaly tracking if operational data shows the documentation-only guard is insufficient
Keep V1 simple until this is justified by real usage.

## DONE

None yet.
