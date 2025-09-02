import sys
from pathlib import Path

try:
    from dbfread import DBF
except Exception as e:
    print("ERROR: dbfread is not installed or failed to import:", e)
    sys.exit(2)


def dump_dbf(path: Path, limit: int = 20) -> int:
    print(f"\n=== {path} ===")
    try:
        table = DBF(str(path), ignore_missing_memofile=True, char_decode_errors="ignore")
    except Exception as e:
        print(f"Failed to open as DBF: {e}")
        return 1

    fields = [f.name for f in table.fields]
    print(f"Fields ({len(fields)}): {', '.join(fields)}")

    count = 0
    try:
        for rec in table:
            if count == 0:
                # Print header
                pass
            # Render a concise line per record
            preview = {k: rec.get(k) for k in fields[:8]}
            print(preview)
            count += 1
            if count >= limit:
                break
    except Exception as e:
        print(f"Error iterating records: {e}")
        return 1

    print(f"Shown {count} records (of unknown total without full scan)")
    return 0


def main(argv: list[str]) -> int:
    if len(argv) < 2:
        print("Usage: python scripts/dbf_dump.py <file1> [file2 ...] [--limit N]")
        return 1

    limit = 20
    files = []
    i = 1
    while i < len(argv):
        if argv[i] == "--limit" and i + 1 < len(argv):
            try:
                limit = int(argv[i + 1])
            except ValueError:
                print("Invalid limit; using default 20")
            i += 2
            continue
        files.append(Path(argv[i]))
        i += 1

    rc = 0
    for p in files:
        if not p.exists():
            print(f"File not found: {p}")
            rc = 1
            continue
        rc |= dump_dbf(p, limit=limit)
    return rc


if __name__ == "__main__":
    raise SystemExit(main(sys.argv))

