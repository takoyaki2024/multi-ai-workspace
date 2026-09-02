# ARCHITECTURE

## V1 components

- ChatGPT: commander, requirement/decision owner, task routing, state judgment.
- GitHub: source code, history, durable project memory.
- GitHub Actions: objective Build/Test execution and CI fact recording.
- Codex: manually invoked escalation tool for difficult implementation/debugging.

## Resume order

1. `PROJECT_STATE.md`
2. relevant task in `TASKS.md`
3. relevant requirements in `SPEC.md`
4. relevant decisions in `DECISIONS.md`
5. `ARCHITECTURE.md` only when needed
6. relevant code only

## PROJECT_STATE ownership

Human/ChatGPT-owned fields:
- Status
- Current Task
- Last Completed
- Next
- Recommended Action

CI-owned fields inside the machine block:
- Build
- Tests
- Run ID
- Run URL
- Commit SHA
- Updated At UTC

CI must not rewrite judgment fields.

## CI self-trigger protection

The CI workflow ignores commits that change only `project-memory/PROJECT_STATE.md`. The CI writer also commits only the machine-owned state block. This prevents the CI state commit from recursively triggering the same workflow.

## Codex return gate

Codex -> code reflected in GitHub -> GitHub Actions -> Build/Test PASS -> TASKS/PROJECT_STATE judgment update -> DONE.

A Codex response by itself is never completion evidence.

## STOP decision

When STOP is reached, ChatGPT presents:
1. Escalate to Codex
2. Revisit SPEC
3. Put the feature on hold

## Cost policy

Delivery speed and reliable operation outrank cost minimization. Cost monitoring exists only as an anomaly guard, not as the primary router.
