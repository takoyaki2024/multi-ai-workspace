# Unity AI Dev Template V1

Reusable, project-local workflow for Unity development with low Codex usage.

## Core flow

1. Collect facts locally with Unity MCP / scripts / FFDec / Git.
2. Use Codex once for initial architecture and task decomposition when useful.
3. Execute normal tasks with free AI workers, preferably Antigravity + Unity MCP.
4. Validate with Unity compile/tests and Console Error = 0.
5. Escalate to Codex only after repeated failure, unresolved conflict/unknown, or a major architecture decision.
6. Do not mark work DONE until the implementation is in Git and the required validation passes.

## Install into another Unity project

Copy these folders/files into the Unity repository root:

- `project-memory/`
- `scripts/`
- `ai-dev.config.json`

Then edit `ai-dev.config.json` for the project and fill in `project-memory/SPEC.md` and `TASKS.md`.

## Resume order

1. `project-memory/PROJECT_STATE.md`
2. Current task in `project-memory/TASKS.md`
3. Relevant `SPEC.md`
4. Relevant `DECISIONS.md`
5. `ARCHITECTURE.md` only if needed
6. Relevant code only

## Safety defaults

- Same error signature twice on one task: warn/stop automated repetition.
- Three failed repair/test cycles on one task: STOP review.
- Two Codex invocations on one task without a passing validation between them: STOP review.
- On STOP, choose one: escalate to Codex, revisit SPEC, or put the feature on hold.
- Rejected alternatives must be recorded in `DECISIONS.md` with reasons.
- Existing user changes must never be destroyed automatically.

These thresholds are tunable defaults and should be revised from operating data.
