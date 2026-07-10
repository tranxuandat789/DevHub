# Implementation Plan: Replace Dashboard Bar Chart with Job Stats Table

**Feature:** Recruiter Dashboard — replace "Ứng viên theo tin tuyển dụng" bar chart
**Scope:** `RecruiterDashboardController`, `RecruiterDashboardService`, `RecruiterDashboardRepository`, dashboard ViewModel, `RecruiterDashboard/Index.cshtml`
**No schema changes required** (all data derivable from existing `job_post` / `application` / `interview` tables), **except** the Hired definition — see Decision #2.

---

## 0. Open decisions (must be confirmed before implementation starts)

### Decision #1 — Does the new table replace "Tin tuyển dụng gần đây" or sit alongside it?

Current dashboard has two job-related blocks:
- Bar chart "Ứng viên theo tin tuyển dụng" (target of this change) — top 5 approved jobs, 1 metric (ApplicantCount)
- Table "Tin tuyển dụng gần đây" (2-column layout, right side) — same top-5 approved jobs, 3 columns (Vị trí / Ứng viên / Trạng thái)

**Recommendation: consolidate.** Delete the bar chart section entirely and expand "Tin tuyển dụng gần đây" into the new full table (all approved jobs, not just 5, with Pending/Approved/Hired columns + filter/sort). Keeping both would mean two blocks describing the same jobs with overlapping columns — same duplication problem the bar chart already had with the old table.

Consequence: the "Xem tất cả" link on that block becomes redundant (the table already shows everything, paginated) — remove it once merged.

**If this recommendation is rejected**, the plan below still applies but Section 3/6 target a *new* block instead of replacing the existing one — flag this explicitly before implementation.

### Decision #2 — Definition of "Hired"

No code path in the current codebase sets `Application.Status = "HIRED"`. `RecruiterApplicationService.ApproveAsync/RejectAsync` only transition PENDING → APPROVED/REJECTED. `CountByStatusAsync` already treats `HIRED` as a distinct status group, but nothing populates it.

Two options:

| Option | Source | Risk |
|---|---|---|
| A. `Application.Status == "HIRED"` | Same source as the rest of the table (Application) | Column will show 0 for every job until the Hired-transition flow is built elsewhere — table ships "empty" |
| B. `Interview.Status IN ('FINISHED')` per job, counted via Application → Interview join | Data exists today (interviews already get marked FINISHED per the dashboard's own `CompletedInterviews` logic) | Conflates "interview completed" with "hired" — a completed interview doesn't always mean hired; may mislead |

**Recommendation:** ship the table with **Option A** (correct long-term source), accept that it reads 0 everywhere until the Hired-transition workflow is implemented (separate, already-known backlog item per project notes), and label the column clearly (e.g. "Trúng tuyển" with a tooltip) rather than approximate with Option B, which would need to be silently swapped out later and could create a false impression of accuracy in the meantime.

This must be confirmed with the team before coding, since it affects whether the feature is "done" or "partially done, pending another feature."

### Decision #3 — Full page reload vs partial (AJAX) filter/sort

The dashboard `Index` action currently loads everything in one call (`GetDashboardAsync`): KPIs, package info, expiring jobs, recent applications, interview lists, and (soon to be removed) chart series. Adding filter/sort/pagination to the new table means either:

- **A. Full GET reload** — filter/sort controls submit as query params on `/recruiter/dashboard`, controller re-runs `GetDashboardAsync` with the new params. Simple, consistent with the rest of the codebase (`JobPostController.Index`, `RecruiterApplicationController.Index` both work this way — no AJAX exists anywhere in the reviewed code). Downside: every table filter change re-queries and re-renders KPIs, interviews, package widget, etc. — wasted work.
- **B. Dedicated AJAX endpoint** — new action (e.g. `GET /recruiter/dashboard/job-stats`) returns a partial view; JS fetches and swaps only the table on filter/sort/page change. Cheaper per interaction, but introduces the first AJAX pattern in this part of the app (net-new convention to maintain).

**Recommendation:** **Option A** for the initial implementation — matches existing project conventions, avoids introducing a new pattern for one table, and the dashboard's overall query cost is already dominated by other sections, not this table. Revisit Option B only if the table's filter usage turns out to be frequent enough that full reloads become a UX complaint.

---

## 1. Data layer — `RecruiterDashboardRepository`

### 1.1 New method: `GetJobStatsAsync`

Signature (indicative, not final code):
```
Task<(List<JobStatsRow> Items, int TotalCount)> GetJobStatsAsync(
    int recruiterId, string? statusFilter, string? keyword,
    string? sortBy, int page, int pageSize)
```

**Query construction:**
- Base: `JobPosts.Where(j => j.RecruiterId == recruiterId)`
- Per the recommendation in Decision #1, no longer hard-filtered to `Status == "APPROVED"` at the repository level — the status filter becomes a *parameter* (default could still be "APPROVED" to match current behavior, but the column itself is generic).
- Keyword filter on `Title` (reuse the same trim/lowercase/collapse-whitespace normalization already used in `RecruiterJobPostService.NormalizeText` — factor it into a shared helper if not already accessible from this layer).
- **Counts per job, avoiding N+1:** project pending/approved/hired counts as correlated subqueries within a single `Select`, e.g. conceptually:
  - `PendingCount = j.Applications.Count(a => a.Status == "PENDING")`
  - `ApprovedCount = j.Applications.Count(a => a.Status == "APPROVED" || a.Status == "FINISHED")` (mirrors the existing tab-grouping logic in `RecruiterApplicationRepository.MapStatusGroup` — reuse that mapping instead of duplicating the raw status list)
  - `HiredCount = j.Applications.Count(a => a.Status == "HIRED")` (per Decision #2, Option A)

  EF Core translates this pattern into scalar subqueries in a single SQL statement — no manual `GroupBy` + in-memory join needed, and no per-job round trip. This is the same approach already proven to work in this codebase's simpler count-by-status query, just extended to multiple jobs at once instead of one.

- **Sort:** apply `OrderBy`/`OrderByDescending` on the *computed* fields (e.g. `PendingCount` descending as the default, matching the "most urgent first" recommendation from the dashboard review) before `Skip/Take`. Confirm this translates correctly to SQL `ORDER BY (subquery)` — this is generally supported, but validate with a quick EF Core query-plan check during implementation, since project notes already flag group-by/filtered-include translation as a spot with version-specific pitfalls in this codebase's EF Core version.
- **Pagination:** `Skip((page-1)*pageSize).Take(pageSize)` after `OrderBy`, consistent with the pattern in `RecruiterApplicationRepository.GetApplicantsAsync`.
- **TotalCount:** separate `CountAsync()` on the filtered (pre-paged) query, same pattern as existing repositories.

### 1.2 Retire (or keep, depending on Decision #1)

If consolidating: `GetJobPostsAsync` (used only to build `JobPostApplicantCounts` currently) may become unused for the dashboard and can be removed once the new method is wired in — confirm no other consumer depends on it before deleting.

---

## 2. Service layer — `RecruiterDashboardService`

### 2.1 `GetDashboardAsync` signature change

Extend with the new filter/sort/page parameters (or accept a small `JobStatsFilter` parameter object, mirroring the existing `ApplicantFilter` pattern used in the RecruiterApplication feature — preferred, for consistency and to avoid a long parameter list).

### 2.2 Remove obsolete pieces

- Delete the call to build `jobApplicantCounts` from `_dashboardRepo.GetJobPostsAsync` + `GetApplicantAvatarsByJobAsync` (the avatar-stacking feature was specific to the bar-chart-adjacent table; confirm whether avatars are still wanted as a column in the new table — if yes, keep `GetApplicantAvatarsByJobAsync`, otherwise remove it as dead weight).
- Delete `BuildStatsSeriesAsync` and the `GetApplicationDatesAsync` call **only if** the line chart removal (discussed separately) is implemented in the same pass. If the line chart is being kept for now, leave this untouched — this plan is scoped to the bar chart only.

### 2.3 New method call

Add a call to the new repository method, mapped into the ViewModel (Section 3).

---

## 3. ViewModel changes

Two options depending on whether the broader dashboard-model cleanup (Section "1. Vấn đề cần xử lý trước" from the earlier review — `Models.RecruiterDashboard` vs the unused `ViewModels.Recruiter.RecruiterDashboardViewModel`) happens first:

- **If done inline (minimal change):** add to `Models.RecruiterDashboard`:
  - `List<JobStatsRow> JobStats`
  - `int JobStatsTotalCount`, `int JobStatsPage`, `int JobStatsTotalPages`
  - `string? JobStatsFilter`, `string? JobStatsKeyword`, `string? JobStatsSort` (echo current filter state back to the view for building pagination links, same as `ApplicantListViewModel.Filter` does)
- **If done properly (recommended, matches the rest of the codebase's ViewModel discipline):** introduce a small `JobStatsTableViewModel` (or reuse the naming convention of `ApplicantListViewModel`/`ApplicantFilter`) and pass it as a nested property, rather than flattening more fields onto the already-large dashboard model. This is the better long-term shape but is a larger refactor — flag as a call to make alongside Decision #1, since it touches the same object.

New row type: `JobStatsRow { JobId, Title, Status, CreatedAt, PendingCount, ApprovedCount, HiredCount, TotalApplicationCount }`.

---

## 4. Controller changes — `RecruiterDashboardController`

- `Index(string? range, ...)` gains new query parameters for the table: e.g. `jobStatus`, `jobQ`, `jobSort`, `jobPage` (prefixed to avoid colliding with the existing `range` param used by the line chart, and to avoid colliding with `q`/`status`/`page` used elsewhere in the app for different resources if this ever gets partial-rendered).
- Pass these through to `_dashboardService.GetDashboardAsync(...)`.
- No new `[HttpPost]` actions needed — filter/sort/page are all GET-based per Decision #3 Option A.

---

## 5. View changes — `RecruiterDashboard/Index.cshtml`

### 5.1 Remove
- `<div>` block containing `<canvas id="jobApplicantChart">` (lines ~225-232 in current file).
- The `[#9] Bar chart` JS block (`document.addEventListener` for `jobApplicantChart`, lines ~651-689).
- If consolidating per Decision #1: remove the existing "Tin tuyển dụng gần đây" block's current 3-column table body (title/applicants/status) and its "Xem tất cả" link.

### 5.2 Add: new table

Structure, in the same visual slot as the old two-column "Tin tuyển dụng gần đây" card (full-width now, since it's absorbing the chart's space too):

- **Header row:** title + filter controls
  - Status filter: dropdown (Tất cả / Đang tuyển / Chờ duyệt / Đã đóng...) — submits via GET, same UX as the status tabs in `RecruiterApplication/Index.cshtml`
  - Keyword search: text input on job title
  - Sort: dropdown (Nhiều đơn chờ nhất — default / Mới đăng nhất / Cũ nhất / Tên A-Z), or clickable column headers if preferred — dropdown is simpler and matches the existing `Sort` select pattern already used in `RecruiterApplication/Index.cshtml`
- **Columns:** Vị trí (title, linked to `JobPost/Detail/{id}`) · Ngày đăng · Trạng thái tin (badge, reuse the badge-mapping switch already in the current view) · Pending (badge, linked to `RecruiterApplication?jobId={id}&Status=PENDING`) · Approved (linked, `Status=APPROVED`) · Hired (linked, `Status=HIRED`) · Tổng ứng viên (plain count, not linked — redundant with the other three)
- **Empty state:** reuse the existing "Chưa có tin tuyển dụng nào" pattern already present in the current table.
- **Pagination:** reuse the numbered-page-link pattern from `RecruiterApplication/Index.cshtml` (`BuildUrl` helper generating `?...&Page=n`), adapted to the new query param names from Section 4.

### 5.3 Filter/sort form

Model the `<form method="get">` after the one in `RecruiterApplication/Index.cshtml` — GET form preserving other dashboard query params (in particular, don't lose the chart's `range` param if the line chart is still present at this point) via hidden inputs.

---

## 6. Testing considerations

Since this introduces a new grouped/counted query, and the project has already noted EF Core group-by translation as a place prior bugs originated:

- **Unit test** the repository method against a real MySQL instance (Testcontainers, per existing project convention for MySQL-dependent logic) rather than EF Core InMemory — correlated subquery translation and `ORDER BY` on computed columns are exactly the kind of thing that behaves differently between InMemory and MySQL.
- Cover: job with zero applications (counts should be 0, not null/error), job with applications in every status (counts don't leak across categories — especially verify Approved correctly includes `FINISHED` per the shared status-group mapping), sort stability when two jobs have equal `PendingCount`, and keyword filter matching partial/whitespace-normalized titles.
- **Integration/UI check**: confirm pagination links preserve filter state (a common source of bugs in GET-based filter forms — verify against the existing `BuildUrl` pattern's known-good behavior in `RecruiterApplication/Index.cshtml`).

---

## 7. Sequencing (suggested implementation order)

1. Confirm Decisions #1, #2, #3 with the team.
2. Repository method + its test (against Testcontainers MySQL) — get this correct in isolation before touching the view.
3. Service layer wiring (extend `GetDashboardAsync`, remove obsolete bar-chart data building).
4. ViewModel additions.
5. Controller param plumbing.
6. View: remove chart, add table + filter form, verify links.
7. Manual QA pass specifically on the link-completeness gaps identified in the earlier dashboard review (this table closes two of them: job-level Pending/Approved/Hired links were previously missing entirely).

---

## Explicitly out of scope for this plan

- The line chart ("Thống kê ứng tuyển & Phỏng vấn") — separate decision, separate plan if pursued.
- The "Recent Applicants" card and "Lịch phỏng vấn" card missing links — separate, unrelated fixes noted in the earlier review.
- The `Company` entity normalization referenced by `RecruiterDashboardController`/`RecruiterDashboardService` (`recruiter.Company`, `CompanyId`) — this plan assumes that migration is either already done or done in parallel; if not, the repository/service code in this plan needs its field references adjusted to match whichever `Recruiter` shape is live at implementation time.
