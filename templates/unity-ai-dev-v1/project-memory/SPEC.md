# SPEC

## Purpose
Deliver a reliably working Unity project with minimal human effort and minimal Codex usage by using free/local AI and Unity MCP for normal work, deterministic tests for validation, and Codex only for difficult escalations.

## Confirmed requirements

- REQ-001: Durable memory lives in `project-memory/`.
- REQ-002: `PROJECT_STATE.md` is the concise resume point.
- REQ-003: Factual Build/Test results are machine-owned; judgment fields are human/ChatGPT-owned.
- REQ-004: `DECISIONS.md` records accepted and rejected alternatives with reasons.
- REQ-005: Normal Unity implementation/testing should prefer free AI + Unity MCP.
- REQ-006: Codex is manually invoked only when escalation materially improves success.
- REQ-007: Codex output is never DONE until reflected in Git and validated.
- REQ-008: Repeated failures follow deterministic STOP rules.
- REQ-009: Confirmed SPEC takes precedence over unsolicited AI reinterpretation.
- REQ-010: UNKNOWN/CONFLICT requirements are not silently guessed into final behavior.

## Initial anomaly thresholds

- Same normalized error signature repeated 2+ times on one task: warn and stop blind repetition.
- Same task reaches 3 failed repair/test cycles: STOP review.
- Codex invoked 2+ times on one task without a passing validation between invocations: STOP review.

These are tunable defaults.

## Definition of done
A task is DONE only when the required implementation is present in Git and its acceptance validation has passed. For Unity implementation tasks, default validation is compile PASS + required test PASS + Console Error 0, unless the task specifies otherwise.
