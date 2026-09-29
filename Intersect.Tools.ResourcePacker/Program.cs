using System.Security.Cryptography;
using Intersect.Compression;

// Bundles loose client assets (item/entity/spell/animation/face/fog/paperdoll/tileset/misc/map-resource art,
// plus sound effects and music) into opaque .pack blobs + a gzip-compressed .index, the same format already
// used upstream for sound/music packs. This does NOT encrypt anything - it just stops a player from browsing
// a folder of loose files. Someone who knows the pack format (gzip'd JSON index with byte offsets into a
// gzip'd concatenated blob) can still recover the originals. Its only purpose is to deter casual copying, not
// to be unbreakable DRM.
//
// IMPORTANT: do NOT run this against Intersect.Client's own bin/Debug/.../resources folder anymore.
// items/entities/spells/animations/faces/fogs/paperdolls/tilesets are directory junctions into the Editor's
// resources folder (so adding art in the Editor makes it show up in the Client instantly, with nothing to
// copy by hand) - packing would move the Editor's real files out from under it. This tool is meant to run
// against a disposable COPY of resources made specifically for shipping - see publish-client.ps1, which
// does exactly that as part of building the player-facing zip.
//
// Usage: Intersect Resource Packer.exe <path to a resources folder COPY, not the live dev one>
// Defaults to ../Intersect.Client/bin/Debug/net8.0/resources relative to this tool's own folder, which is
// only correct before the Editor<->Client junctions exist (a fresh checkout/first build).
//
// For each category, this: packs it, verifies every file reads back byte-identical from the pack, and only
// then moves the loose files out of "resources" (into a sibling "source_assets" folder) so nothing manual is
// left to do - the shipped resources folder ends up with just the .pack/.index blobs. Originals are moved,
// not deleted, so nothing is lost if something looks wrong afterward. The client already checks
// packs/<index>.index before falling back to loose files for every one of these categories (see
// MonoContentManager's LoadTextureGroup/LoadTilesets/LoadSounds/LoadMusic), so this is safe to re-run
// incrementally as you add new content - just drop new files back into the matching "source_assets" folder
// first.

var resourcesDir = args.Length > 0
    ? args[0]
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..",
        "Intersect.Client", "bin", "Debug", "net8.0", "resources"));

if (!Directory.Exists(resourcesDir))
{
    Console.WriteLine($"Resources folder not found: {resourcesDir}");
    Console.WriteLine("Pass the path to the client's \"resources\" folder as an argument.");
    return 1;
}

var packsDir = Path.Combine(resourcesDir, "packs");
Directory.CreateDirectory(packsDir);

// Sits next to "resources", not inside it, so it never gets shipped/zipped along with the game.
var sourceBackupDir = Path.Combine(resourcesDir, "..", "source_assets");

// Folder (relative to resources), file filter, and pack index base name. Index name only differs from the
// folder name for "sounds" -> "sound.index", matching the upstream naming MonoContentManager.LoadSounds
// already expects.
(string Folder, string Filter, string IndexName)[] categories =
[
    ("items", "*.png", "items"),
    ("entities", "*.png", "entities"),
    ("spells", "*.png", "spells"),
    ("animations", "*.png", "animations"),
    ("faces", "*.png", "faces"),
    ("images", "*.png", "images"),
    ("fogs", "*.png", "fogs"),
    ("paperdolls", "*.png", "paperdolls"),
    ("gui", "*.png", "gui"),
    ("tilesets", "*.png", "tilesets"),
    ("misc", "*.png", "misc"),
    ("resources", "*.png", "resources"),
    ("sounds", "*.wav", "sound"),
    ("music", "*.ogg", "music"),
];

Console.WriteLine($"Resources folder: {resourcesDir}");
Console.WriteLine($"Packs output:     {packsDir}");
Console.WriteLine($"Originals moved to: {Path.GetFullPath(sourceBackupDir)}");
Console.WriteLine();

var packedCategories = new List<string>();
var failedCategories = new List<string>();

foreach (var (folder, filter, indexName) in categories)
{
    var inputDir = Path.Combine(resourcesDir, folder);
    if (!Directory.Exists(inputDir) || Directory.GetFiles(inputDir, filter).Length == 0)
    {
        Console.WriteLine($"[skip] {folder}: no {filter} files found in {inputDir}");
        continue;
    }

    var fileCount = Directory.GetFiles(inputDir, filter).Length;
    Console.WriteLine($"[pack] {folder}: packing {fileCount} file(s)...");

    AssetPacker.PackageAssets(
        inputDir,
        filter,
        packsDir,
        $"{indexName}.index",
        indexName,
        ".pack",
        batchSize: 64
    );

    Console.WriteLine($"[pack] {folder}: verifying...");

    if (!VerifyPack(inputDir, packsDir, filter, indexName))
    {
        Console.WriteLine($"[FAIL] {folder}: pack verification failed - loose files were left in place, nothing moved.");
        failedCategories.Add(folder);
        continue;
    }

    // Move only the packed files, never the whole directory - some categories (e.g. "gui") keep non-asset
    // content alongside (like the JSON UI layouts under gui/layouts) that the game still reads straight out
    // of "resources" at runtime and must NOT be moved out.
    var backupDir = Path.Combine(sourceBackupDir, folder);
    Directory.CreateDirectory(backupDir);

    foreach (var file in Directory.GetFiles(inputDir, filter))
    {
        var destination = Path.Combine(backupDir, Path.GetFileName(file));
        if (File.Exists(destination))
        {
            File.Delete(destination);
        }

        File.Move(file, destination);
    }

    // Only remove the category folder itself if packing it away emptied it out completely.
    if (Directory.GetFileSystemEntries(inputDir).Length == 0)
    {
        Directory.Delete(inputDir);
    }

    Console.WriteLine($"[done] {folder}: verified and moved {folder}'s {filter} files to {backupDir}");

    packedCategories.Add(folder);
}

Console.WriteLine();

if (packedCategories.Count == 0 && failedCategories.Count == 0)
{
    Console.WriteLine("Nothing was packed - no matching resource folders/files were found.");
    return 0;
}

if (packedCategories.Count > 0)
{
    Console.WriteLine($"Packed and hid {packedCategories.Count} categor{(packedCategories.Count == 1 ? "y" : "ies")}: {string.Join(", ", packedCategories)}");
}

if (failedCategories.Count > 0)
{
    Console.WriteLine($"Failed verification (left untouched): {string.Join(", ", failedCategories)}");
    return 1;
}

return 0;

// Reads every file back out of the freshly-written pack and compares it byte-for-byte (via MD5) against the
// original on disk, so we never move/hide originals behind a pack that's silently corrupt or incomplete.
static bool VerifyPack(string inputDir, string packsDir, string filter, string indexName)
{
    var indexFile = Path.Combine(packsDir, $"{indexName}.index");
    if (!File.Exists(indexFile))
    {
        return false;
    }

    using var pack = new AssetPacker(indexFile, packsDir);

    var originalFiles = Directory.GetFiles(inputDir, filter)
        .Select(Path.GetFileName)
        .Where(name => name is not null)
        .Cast<string>()
        .ToList();

    if (originalFiles.Count == 0 || pack.FileList.Count != originalFiles.Count)
    {
        return false;
    }

    foreach (var fileName in originalFiles)
    {
        if (!pack.Contains(fileName))
        {
            return false;
        }

        var originalBytes = File.ReadAllBytes(Path.Combine(inputDir, fileName));
        using var packedStream = pack.GetAsset(fileName);
        using var packedMemory = new MemoryStream();
        packedStream.CopyTo(packedMemory);
        var packedBytes = packedMemory.ToArray();

        if (!MD5.HashData(originalBytes).AsSpan().SequenceEqual(MD5.HashData(packedBytes)))
        {
            return false;
        }
    }

    return true;
}
