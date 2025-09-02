# ModernDosApp (FOCUS Modern)

Windows desktop modernization of a legacy DOS/Clipper vehicle finance system. The app targets .NET 8 (Windows) with Windows Forms and uses per-branch SQLite databases.

## Quick Start

- Requirements: Windows 10/11, Visual Studio 2022, .NET 8 SDK
- Open solution: `FocusModern/FocusModern.sln`

### Run in Visual Studio
1. Open `FocusModern/FocusModern.sln`
2. Restore NuGet packages
3. Build and Start (F5)

### Run from CLI
```
cd FocusModern/FocusModern
dotnet restore
dotnet build
dotnet run
```

## Data & Storage
- Legacy data goes under `Old/` (excluded from Git). Expected layout per branch: `Old/1`, `Old/2`, `Old/3` containing `ACCOUNT.FIL`, `CASH.FIL`, and optional text reports like `OUTPUT.TXT`, `F.TXT`, `FOCU.TXT`, `CMC.TXT`.
- App databases are now created locally under `Data/` by default as `focus_branch{1..3}.db` (configurable via `App.config` key `DatabasePath`). On first run it will migrate any existing DBs from `%LocalAppData%\FocusModern\`.

## Project Layout
- App project: `FocusModern/FocusModern`
- Import CLI: `FocusModern/ImportCli`
- Docs: `FocusModern/Docs`
- App readme: `FocusModern/README.md`

## Legacy Import (CLI)
- Build single-file CLI:
  - `dotnet publish FocusModern/ImportCli/ImportCli.csproj -c Release -p:PublishProfile=Properties/PublishProfiles/SingleFile-win-x64.pubxml`
- Run import for a branch (stages files, imports customers from `ACCOUNT.FIL`, transactions from `CASH.FIL`, vehicles from text reports when available):
  - `FocusModern/ImportCli/bin/SingleFile/win-x64/ImportCli.exe Old\1 --branch 1`

Notes:
- Vehicles are parsed from text reports (e.g., `OUTPUT.TXT`) by detecting patterns like `UP-25E / T-8036`. If a branch only has consolidated summaries, vehicle count may be zero.
- Loans are not inferred (no reliable legacy loan master files were found). Provide loan sources to import.

## Single-file Publish (App)
- Build one-EXE app:
  - `dotnet publish FocusModern/FocusModern/FocusModern.csproj -c Release -p:PublishProfile=Properties/PublishProfiles/SingleFile-win-x64.pubxml`
- Output: `FocusModern/FocusModern/bin/SingleFile/win-x64/FocusModern.exe`

## Key Docs
- `FocusModern/Docs/README.md` – index of specs/plans
- `FocusModern/Docs/FINAL_PROJECT_SPECIFICATION.md` – main spec
- `FocusModern/Docs/DATA_MIGRATION_STRATEGY.md` – migration plan
- `FocusModern/Docs/MODERNIZATION_PROJECT_PLAN.md` – phased plan

## Next Steps
 
### Current Status
- Customers: Implemented (CRUD forms, validation, search).
- Vehicles: Implemented (list/edit forms, validation, statistics).
- Loans: Implemented create/edit and details; list wired; Make Payment integrated to entry.
- Payments: Implemented entry and details (with cancel/reversal), list with search and working date range picker; vouchers use per-branch sequence; day-book transaction created on payment/cancel.
- Day Book: Implemented viewer (date range + search). Temporarily bound to F6.
- Reports: Implemented basic UI (daily payments, monthly collection, loan statement) with CSV export; print/PDF pending.
- Reports: Implemented basic UI (daily payments, monthly collection, loan statement) with CSV export and Print/PDF (via Microsoft Print to PDF).
- Backup/Settings: Not implemented.
 - Backup/Settings: Implemented basic Backup/Restore (ZIP) and Settings dialog.
- Migration tooling: Not implemented.

### Remaining Work
- Reports: polish visuals and layouts; add branding to printouts.
- Legacy data migration tool for `Old/{1,2,3}` (.FIL/.NTX) per branch.

---
GitHub: https://github.com/kta136/ModernDosApp
