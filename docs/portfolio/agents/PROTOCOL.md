# Fleet protocol (fork side)

The GitHub repo `derprito64bit/derprito64bit.github.io` is the fleet's durable work log:
- **Issues are crew briefs:** acceptance criteria, owned globs, the gate and `after[]` dependencies.
- **The branch and draft PR hold the work. Comments hold the state.** A replacement agent must be able to resume from
  the issue and branch alone.

Unity crew rules: [`blocks/rules.unity.md`](blocks/rules.unity.md). **Pass `-R derprito64bit/derprito64bit.github.io`
to every `gh` command:** the repo has an `upstream` remote that `gh` may pick. Nothing goes to Lets-be-strategic-here/project.ion.

## 1. Claim an issue

1. Read it: `gh issue view <n> -R <repo> --comments` (every acceptance criterion, the owned globs, the gate).
2. Claim it only if it has `status:ready` and every `after[]` issue is closed:
   `gh issue edit <n> -R <repo> --add-label status:in-progress --remove-label status:ready`, then post a checkpoint.
3. Already `status:in-progress`? If the last checkpoint is under 45 minutes old, stop and report it. If it is older,
   resume (section 8).

| Label | Set by | Meaning |
|---|---|---|
| `status:ready` | dispatcher | brief complete, every `after[]` issue closed |
| `status:in-progress` | crew, on claim | work started |
| `status:needs-review` | crew | gate green, PR marked ready |
| `status:changes-requested` | reviewer | fix, then set `status:in-progress` again |
| `status:blocked` | anyone | the comment names the blocking `#n` |
| `status:done` | orchestrator | merged |

## 2. Checkpoints

Post one after each meaningful push, when blocked, and before you stop. Post **at most one new comment per 15
minutes**; inside that window, update the last one with `gh issue comment <n> -R <repo> --edit-last --body-file cp.md`.

```
**Checkpoint** <role> · <YYYY-MM-DD HH:MM UTC> · `<short sha>` on `crew/<id>` · PR #<pr>
Done: <what now works, with evidence: test summary line, screenshot path>
Next: <the next concrete step>
Blockers: none | #<issue> (<one line>)
Gate: `<exact command to rerun>` -> <result>
```

## 3. Branches and pull requests

- Branch: `crew/<id>` from `origin/main` (branch protocol in the rules block). There are no `up/*` branches any more (D-022).
- **Draft PR at your first commit:** fill in a copy of [`PR_TEMPLATE.md`](PR_TEMPLATE.md), then run
  `gh pr create -R <repo> --draft --base main --head crew/<id> --title "<id>: <outcome>" --body-file pr.md`.
  The body contains `Closes #<n>`.
- One crew, one issue, one PR, about 800 changed lines at most (not counting `.meta`). Larger? Ask in the issue to split it.
- **Ready:** the full gate is green and the PR body has current evidence. Run `gh pr ready <pr> -R <repo>`, then set
  `status:needs-review`.
- Never merge your own PR. The orchestrator serialises merges.

## 4. Gates (run them yourself; paste the output into the PR)

- **Iterate:** `powershell -File scripts/ion.ps1 test-play -Filter <YourTests>` (or `test-edit`), with
  `run_in_background`. Poll `Build/logs/<step>.log`; results land in `Build/results-<platform>.xml`.
- **Before review:** run `powershell -File scripts/ion.ps1 test` once and paste its passed/failed summary lines.
- **Ownership:** after `git fetch origin`, `powershell -File scripts/fork/ownership-check.ps1 -Base origin/main -Owned '<glob>','<glob>'` with your
  issue's globs. It must print `PASS` and exit 0.
  - Exception rows come from `scripts/fork/ownership-exceptions.txt`. Only the orchestrator edits that file.
- **Evidence:** `TourShots` PNGs and the per-zone batch and triangle JSON, at the PR head SHA.

## 5. Review

The **gate agent** re-runs the gates at the PR head SHA and shoots its own screenshots; it never trusts the author's
images. The **manager** then reviews three axes:
1. **Correctness and budgets:** acceptance criteria met; tests pass at the SHA; test count not lower; `ArtPlayTests`
   constants unchanged; at most 120 batches and 60k triangles per zone; no new textures; local lights Ultra-only;
   ownership check passes.
2. **Accessibility and performance:** DOM overlays meet WCAG 2.2 AA contrast and are keyboard reachable; pointer
   lock and Esc behave; reduced motion is honoured; cost is measured, not guessed.
3. **Art direction against rendered evidence:** the gate agent's screenshots against the direction doc and the art
   bible; every placeholder visibly marked; nothing generic.

**Verdict:** run `gh pr review <pr> -R <repo> --comment --body-file review.md`, then set exactly one of
`review:approved` or `review:changes-requested` and remove the other. Labels carry the verdict because every agent posts as
one account and an author cannot approve its own PR. In `review.md`, one finding per line:
`blocker|should|nit · file:line · problem · evidence`. After 3 rounds without approval, set `status:blocked` and `needs:orchestrator`.

## 6. Issue requests (the only channel between agents)

Need a change outside your globs, or found a bug there? Never edit it yourself. File a request and keep working
around it:

```
gh issue create -R <repo> --title "<needs-role>: <one-line ask>" --label "type:request,needs:<role>,from:<your role>,track:<M|G>" --body-file req.md
```

- Use `type:bug` for defects.
- The body gives What, Why (which acceptance criterion needs it), Where (`file:line`), the proposed change, and your
  workaround meanwhile. Link the request under Blockers in your checkpoint.
- Route it:

  | Need | Label |
  |---|---|
  | an engine file another crew owns | `needs:<that crew>` |
  | a new `Mat` material | `needs:<the palette owner, see the M plan>` |
  | an owner fact | `needs:orchestrator`; use a typed placeholder until answered |

## 7. Rate-limit hygiene

The fleet shares one account: about 80 content writes per minute, 500 per hour.
- Only the dispatcher creates issues in bulk.
- Batch label changes into one `gh issue edit`. Read with `--json <fields>`. Never poll faster than once a minute.
- **On HTTP 403/429 (secondary limit):** wait 60 s × attempt + random 0–30 s, at most 5 attempts. Then keep the
  text in the PR body or a local file, carry on, and post it at the next checkpoint.

## 8. Resume as a replacement agent

1. `gh issue view <n> -R <repo> --comments`: read the brief and the last checkpoint (SHA, Next, Gate).
2. `git fetch origin; git reset --hard origin/crew/<id>`. Check the head is the checkpoint SHA. If it is ahead, read
   `git log <sha>..HEAD`.
3. `gh pr view crew/<id> -R <repo> --comments`: read the open review findings.
4. Re-run the checkpoint's Gate command before changing anything.
5. Post a `Resumed by <role>` checkpoint, then continue from **Next**.

Do not redo work a checkpoint marks Done unless the gate shows it is broken.
