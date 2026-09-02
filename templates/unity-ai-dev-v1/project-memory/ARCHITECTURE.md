# ARCHITECTURE

## Roles

- ChatGPT: commander, requirement/decision owner, task selection, state judgment.
- GitHub: source, history, durable memory, optional CI.
- Free AI workers: normal analysis, implementation, review.
- Antigravity + Unity MCP: preferred Unity implementation, compile, Play Mode, Console and Test Runner path.
- Codex: manually invoked escalation for difficult debugging, reverse engineering or major architecture work.
- Scripts/Git/FFDec: deterministic work that should not consume AI usage.

## Normal route
Task -> free AI / Unity MCP -> compile/test -> PASS -> Git -> next task.

## Escalation route
Repeated failure / UNKNOWN / CONFLICT / major architecture decision -> `CODEX_HANDOFF.md` -> human invokes Codex -> change reflected in Git -> required validation PASS -> task may be DONE.

## Resume order
1. PROJECT_STATE
2. current TASK
3. relevant SPEC
4. DECISIONS
5. ARCHITECTURE only if needed
6. relevant code only

## State ownership
Human/ChatGPT: Status, Current Task, Last Completed, Next, Recommended Action.
Machine/CI: Build, Tests, run metadata, commit SHA, timestamp.

Machine writers must never overwrite judgment fields.
