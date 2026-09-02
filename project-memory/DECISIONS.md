# DECISIONS

## DEC-001 — Durable memory lives in each repository
Status: Accepted

Accepted option:
Use a repository-local `project-memory/` directory as the source of truth.

Why:
It travels with the code, is versioned, and can be read by ChatGPT and other tools.

Rejected alternatives:
- Chat history only — rejected because resume state can become incomplete or ambiguous.
- External memory database in V1 — rejected because it adds complexity before it is needed.

Reconsider when:
Repository-local Markdown becomes too large or cross-project semantic recall becomes a proven bottleneck.

## DEC-002 — CI owns factual Build/Test state
Status: Accepted

Accepted option:
GitHub Actions writes objective CI facts to a machine-owned block in `PROJECT_STATE.md`; ChatGPT writes judgment fields.

Why:
This prevents the project save-state from relying on human or AI recollection of CI results.

Rejected alternatives:
- ChatGPT manually copying all CI results — rejected because updates can be forgotten or misstated.
- CI owning the entire PROJECT_STATE file — rejected because CI cannot reliably decide Current Task or Next.

Reconsider when:
A dedicated state service replaces Markdown.

## DEC-003 — Codex is an early manual escalation tool
Status: Accepted

Accepted option:
Recommend Codex whenever it is likely to materially improve speed or success; user invokes it manually.

Why:
Fast, reliable delivery is higher priority than minimizing model usage.

Rejected alternatives:
- Avoid Codex until all cheaper approaches fail — rejected because it can waste time.
- Automatically invoke Codex — rejected for V1 because manual control is retained.

Reconsider when:
A safe, auditable automatic invocation path is deliberately approved.

## DEC-004 — Objective anomaly thresholds
Status: Accepted

Accepted option:
Use initial numeric warning thresholds defined in SPEC rather than subjective AI judgment.

Rejected alternatives:
- Let ChatGPT decide whether usage 'feels abnormal' — rejected because it is not objective or reproducible.

Reconsider when:
Operating data supports better threshold values.
