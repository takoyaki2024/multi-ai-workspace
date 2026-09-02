# SPEC

## Purpose
Build a ChatGPT-centered AI development workflow that reaches a reliably working implementation as quickly as practical while keeping human intervention low.

## Confirmed requirements

- REQ-001: Each project keeps durable memory in `project-memory/`.
- REQ-002: `PROJECT_STATE.md` is the first resume point and stays concise.
- REQ-003: GitHub Actions owns factual CI fields; ChatGPT owns judgment fields such as Current Task and Next.
- REQ-004: `DECISIONS.md` records accepted and rejected alternatives with reasons.
- REQ-005: Codex is manually invoked by the user, but should be recommended early when it materially improves speed or success.
- REQ-006: Codex output is never marked DONE until reflected in GitHub and validated by CI.
- REQ-007: Repeated failures follow deterministic STOP and anomaly rules.
- REQ-008: On STOP, ChatGPT presents exactly three routes: Codex escalation, revisit SPEC, or put the feature on hold.
- REQ-009: Project switching requires one confirmation showing repository identity and current task.
- REQ-010: Confirmed SPEC takes precedence over unsolicited AI reinterpretation.

## Initial anomaly thresholds

- Same error signature repeated 2 or more times on the same task: warning.
- Same task reaches 3 failed repair/test cycles: warning and STOP review.
- Codex is invoked 2 or more times for the same task without a passing validation between invocations: warning.

These are initial tunable defaults. Review them using operating data rather than subjective judgment.

## Definition of done for a task
A task can be marked DONE only when its required implementation is present in GitHub and the required Build/Test validation has passed, unless the SPEC explicitly defines another acceptance path.
