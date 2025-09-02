import csv
import os
from pathlib import Path
from typing import Any

from dbfread import DBF


SRC_DIR = Path('Old/1')
OUT_DIR = SRC_DIR / '_export_csv'


def to_str(v: Any) -> str:
    if v is None:
        return ''
    # dates come as datetime.date
    try:
        import datetime as _dt
        if isinstance(v, (_dt.date, _dt.datetime)):
            return v.isoformat()
    except Exception:
        pass
    return str(v)


def export_dbf(src: Path, dest: Path) -> None:
    table = DBF(str(src), ignore_missing_memofile=True, char_decode_errors='ignore')
    fields = [f.name for f in table.fields]
    with dest.open('w', newline='', encoding='utf-8') as f:
        writer = csv.writer(f)
        writer.writerow(fields)
        for rec in table:
            row = [to_str(rec.get(name)) for name in fields]
            writer.writerow(row)


def export_text(src: Path, dest: Path) -> None:
    # Save raw lines to a one-column CSV, preserving content
    with dest.open('w', newline='', encoding='utf-8') as f:
        writer = csv.writer(f)
        writer.writerow(['line'])
        with src.open('r', encoding='utf-8', errors='ignore') as fin:
            for line in fin:
                writer.writerow([line.rstrip('\n')])


def main() -> int:
    OUT_DIR.mkdir(parents=True, exist_ok=True)

    # Export .FIL (DBF) files
    dbf_files = sorted(SRC_DIR.glob('*.FIL'))
    for dbf in dbf_files:
        dest = OUT_DIR / (dbf.stem + '.csv')
        try:
            export_dbf(dbf, dest)
            print(f"Exported DBF: {dbf.name} -> {dest}")
        except Exception as e:
            print(f"FAILED DBF export for {dbf}: {e}")

    # Export .TXT files as raw CSV lines
    txt_files = sorted(SRC_DIR.glob('*.TXT'))
    for txt in txt_files:
        dest = OUT_DIR / (txt.stem + '.csv')
        try:
            export_text(txt, dest)
            print(f"Exported TXT: {txt.name} -> {dest}")
        except Exception as e:
            print(f"FAILED TXT export for {txt}: {e}")

    print(f"\nAll exports written to: {OUT_DIR}")
    return 0


if __name__ == '__main__':
    raise SystemExit(main())

