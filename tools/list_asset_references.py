"""Liste les fichiers (images, sons, musiques) que le contenu du jeu reference.

Utilise par update-aetherion.ps1 avant de publier le client : chaque ligne affichee est un fichier qui DOIT se trouver dans
le dossier resources du client, sinon les joueurs voient une case vide / un sprite manquant / pas de musique.
Format d'une ligne :  type|nom de l'element|dossier/fichier
"""
import json
import sqlite3
import sys

db = sqlite3.connect("file:%s?mode=ro" % sys.argv[1], uri=True)
db.text_factory = lambda raw: raw.decode("utf-8", errors="replace")


def table_columns(table):
    return [row[1] for row in db.execute("PRAGMA table_info(%s)" % table)]


def emit(kind, name, folder, value):
    if value is None:
        return
    value = str(value).strip()
    if not value or value.lower() in ("none", "null"):
        return
    print("%s|%s|%s/%s" % (kind, name, folder, value))


def simple(table, kind, columns):
    existing = table_columns(table)
    if not existing:
        return
    wanted = [(column, folder) for column, folder in columns if column in existing]
    if not wanted:
        return
    select = ", ".join(['"%s"' % column for column, _ in wanted])
    for row in db.execute('SELECT Name, %s FROM %s' % (select, table)):
        for (column, folder), value in zip(wanted, row[1:]):
            emit(kind, row[0], folder, value)


simple("Items", "Objet", [("Icon", "items"), ("MalePaperdoll", "paperdolls"), ("FemalePaperdoll", "paperdolls")])
simple("Spells", "Sort", [("Icon", "spells")])
simple("Npcs", "Monstre", [("Sprite", "entities")])
simple("Mounts", "Monture", [("Sprite", "entities")])
simple("Pets", "Familier", [("Sprite", "entities")])
simple("Animations", "Animation", [("Lower_Sprite", "animations"), ("Upper_Sprite", "animations")])
simple("Tilesets", "Tileset", [("Name", "tilesets")])
simple("Maps", "Carte", [
    ("Music", "music"), ("Sound", "sounds"), ("Panorama", "panoramas"), ("Fog", "fogs"), ("OverlayGraphic", "overlays"),
])

# Classes : une liste JSON de sprites/visages.
if table_columns("Classes"):
    for name, sprites in db.execute("SELECT Name, Sprites FROM Classes"):
        try:
            for sprite in json.loads(sprites or "[]"):
                emit("Classe", name, "entities", sprite.get("Sprite"))
                emit("Classe", name, "faces", sprite.get("Face"))
        except (ValueError, AttributeError):
            pass
