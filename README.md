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
- Legacy data should be placed under `Old/` (excluded from Git).
- App databases are created under `%LocalAppData%\FocusModern\` as `focus_branch{1..3}.db`.

## Project Layout
- App project: `FocusModern/FocusModern`
- Docs: `FocusModern/Docs`
- App readme: `FocusModern/README.md`

## Key Docs
- `FocusModern/Docs/README.md` – index of specs/plans
- `FocusModern/Docs/FINAL_PROJECT_SPECIFICATION.md` – main spec
- `FocusModern/Docs/DATA_MIGRATION_STRATEGY.md` – migration plan
- `FocusModern/Docs/MODERNIZATION_PROJECT_PLAN.md` – phased plan

## Next Steps
- Implement migration utility for `Old/{1,2,3}` `.FIL/.NTX`
- Build vehicle management and payment processing
- Recreate A4 reports (Recovery, Day Book, Statements)

---
GitHub: https://github.com/kta136/ModernDosApp
