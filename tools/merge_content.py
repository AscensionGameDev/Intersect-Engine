import sqlite3
import sys

target_path, source_path = sys.argv[1], sys.argv[2]

con = sqlite3.connect(target_path)
con.execute("ATTACH DATABASE ? AS src", (source_path,))
cur = con.cursor()

# Tables de contenu a fusionner (jamais les comptes : ce script ne connait que gamedata.db).
TABLES = ["Spells", "Items", "Crafts", "CraftingTables", "Animations", "Events", "Maps"]
# NON fusionne : MapFolders (l'arborescence listerait en moins les cartes qui n'existent que en prod).

report = []
for table in TABLES:
    cols = [c[1] for c in cur.execute("PRAGMA main.table_info(%s)" % table)]
    src_cols = [c[1] for c in cur.execute("PRAGMA src.table_info(%s)" % table)]
    if cols != src_cols:
        raise SystemExit("Structure differente pour %s : on s'arrete." % table)

    col_list = ",".join('"%s"' % c for c in cols)
    cond_new = "s.Id NOT IN (SELECT Id FROM main.%s)" % table
    if table == "Maps":
        cond_changed = "s.Id IN (SELECT m.Id FROM main.Maps m JOIN src.Maps x ON x.Id = m.Id WHERE x.Revision > m.Revision)"
    else:
        diff = " OR ".join('m."%s" IS NOT x."%s"' % (c, c) for c in cols if c != "Id")
        cond_changed = "s.Id IN (SELECT m.Id FROM main.%s m JOIN src.%s x ON x.Id = m.Id WHERE %s)" % (table, table, diff)

    new_count = cur.execute("SELECT COUNT(*) FROM src.%s s WHERE %s" % (table, cond_new)).fetchone()[0]
    changed_count = cur.execute("SELECT COUNT(*) FROM src.%s s WHERE %s" % (table, cond_changed)).fetchone()[0]

    cur.execute(
        "INSERT OR REPLACE INTO main.%s (%s) SELECT %s FROM src.%s s WHERE (%s) OR (%s)"
        % (table, col_list, ",".join('s."%s"' % c for c in cols), table, cond_new, cond_changed)
    )
    report.append((table, new_count, changed_count))

con.commit()
for table, new_count, changed_count in report:
    print("%-16s ajoutes: %3d   mis a jour: %3d" % (table, new_count, changed_count))
print("integrite :", con.execute("PRAGMA integrity_check").fetchone()[0])
