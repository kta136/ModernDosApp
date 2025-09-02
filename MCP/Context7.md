# Context 7 – Project Snapshot (MCP)

This context file summarizes the essential state of the ModernDosApp (FOCUS Modern) project for quick loading into MCP-enabled tools.

## Project Summary
- Goal: Modernize a DOS/Clipper vehicle finance system to a Windows desktop app.
- Mode: Single-user, offline; A4 printing only; no networking.
- Data: Separate SQLite database per branch (1, 2, 3).

## Tech Stack
- Runtime: .NET 8 (Windows)
- UI: Windows Forms (WinForms)
- DB: SQLite via System.Data.SQLite
- Printing: Windows printing; Crystal Reports/ReportViewer planned
- Deployment: Standard .NET 8 build (single-file optional)

## Repo Pointers
- Solution: `FocusModern/FocusModern.sln`
- App project: `FocusModern/FocusModern`
- Docs: `FocusModern/Docs`
- Legacy data: `Old/` (ignored in Git)

## Implemented
- Branch selection and main dashboard shell
- Customer management end-to-end (CRUD, validation, search)
- Per-branch DB initialization and isolation
- Logging utility and app configuration

## Remaining (High Priority)
- Vehicle management (forms, repository, service)
- Payment processing (entry, allocation, receipts)
- Migration utility for legacy `.FIL/.NTX/.DBF` per branch
- Reports: Recovery, Day Book, Customer Statements (A4)

## Legacy Data (Inventory)
- Branch 1: `Old/1` – Active (2024) – `ACCOUNT.FIL`, `CASH.FIL`, `CONTROL.FIL`, indexes `.NTX`
- Branch 2: `Old/2` – Historical – larger `CASH.FIL`
- Branch 3: `Old/3` – Historical – similar layout
- Root: `CNTRL.DBF`, `COMPANY.DBF`, `FOCUS.EXE`, `OUTPUT.TXT` and report files

## Validation Sources
- `Old/{1,2,3}/OUTPUT.TXT` and various `FO*.TXT/PDF` for reconciliation

## Next Actions
1) Decide final target: stay on .NET 8 Windows (current) vs .NET Framework 4.8 (docs option)
2) Scaffold migration reader stubs (ACCOUNT, CASH) and probe formats
3) Implement vehicle and payment features per plan
4) Start reproducing reports with print preview

— Maintained for MCP tools to load concise project context.

