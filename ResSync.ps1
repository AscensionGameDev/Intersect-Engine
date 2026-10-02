# Synchro des ressources Aetherion (client + editeur) via le VPS - remplace OneDrive pour les ressources.
#   .\ResSync.ps1 Status   -> compare ce PC avec le serveur
#   .\ResSync.ps1 Push     -> envoie les fichiers modifies sur ce PC (incrementiel)
#   .\ResSync.ps1 Pull     -> recupere les fichiers modifies depuis le serveur (incrementiel)
# Prerequis : cle SSH (par defaut ~/.ssh/id_ed25519_aetherion) et OpenSSH (present dans Windows 10/11).
param(
    [Parameter(Position = 0)][ValidateSet('Status', 'Push', 'Pull')][string]$Action = 'Status',
    [switch]$Force,
    [string]$ClientRoot,
    [string]$EditorRoot,
    [string]$StateDir = (Join-Path $env:LOCALAPPDATA 'AetherionResSync'),
    [string]$Key = (Join-Path $env:USERPROFILE '.ssh\id_ed25519_aetherion'),
    [string]$RemoteHost = '159.69.156.73',
    [string]$RemoteUser = 'root',
    [string]$RemoteDir = '/srv/aetherion-dev-sync'
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression, System.IO.Compression.FileSystem

$repo = $PSScriptRoot
if (-not $ClientRoot) { $ClientRoot = Join-Path $repo 'Intersect.Client\bin\Debug\net8.0\resources' }
if (-not $EditorRoot) { $EditorRoot = Join-Path $repo 'Intersect.Editor\bin\Debug\net8.0-windows\win-x64\resources' }

$utf8 = New-Object System.Text.UTF8Encoding($false)
$SharedDirs = @('animations', 'entities', 'faces', 'fogs', 'items', 'paperdolls', 'spells', 'tilesets')
$Trees = @(
    @{ Name = 'client'; Root = $ClientRoot; SkipDirs = ($SharedDirs + 'logs') },
    @{ Name = 'editor'; Root = $EditorRoot; SkipDirs = @('logs') }
)
$SkipTopFiles = @('config.json')
$SkipFilePatterns = @('*.log', 'mapcache*', 'Thumbs.db', 'desktop.ini')

New-Item -ItemType Directory -Force $StateDir | Out-Null
$tmp = Join-Path $env:TEMP 'aetherion-ressync'
New-Item -ItemType Directory -Force $tmp | Out-Null

$sshOpts = @('-i', $Key, '-o', 'BatchMode=yes', '-o', 'StrictHostKeyChecking=accept-new', '-o', 'ConnectTimeout=15')
$target = "$RemoteUser@$RemoteHost"

function Write-Info($message, $color = 'Gray') { Write-Host $message -ForegroundColor $color }

function Invoke-Native([scriptblock]$Block) {
    $previous = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try { & $Block 2>&1 | ForEach-Object { if ($_ -is [System.Management.Automation.ErrorRecord]) { $null } else { $_ } } }
    finally { $ErrorActionPreference = $previous }
}

function Invoke-Ssh([string]$Command) {
    Invoke-Native { ssh @sshOpts $target $Command } | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "Commande distante en echec : $Command" }
}

function Copy-ToRemote([string]$Local, [string]$Remote) {
    Invoke-Native { scp -q @sshOpts $Local "${target}:$Remote" } | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "Envoi impossible : $Local" }
}

function Copy-FromRemote([string]$Remote, [string]$Local) {
    Invoke-Native { scp -q @sshOpts "${target}:$Remote" $Local } | Out-Null
    return ($LASTEXITCODE -eq 0)
}

function Write-TextFile([string]$Path, [string]$Text) {
    [System.IO.File]::WriteAllText($Path, ($Text -replace "`r`n", "`n"), $utf8)
}

function Get-FileSha256([string]$Path) {
    $sha = [System.Security.Cryptography.SHA256]::Create()
    $stream = [System.IO.File]::OpenRead($Path)
    try { return ([BitConverter]::ToString($sha.ComputeHash($stream))).Replace('-', '').ToLowerInvariant() }
    finally { $stream.Dispose(); $sha.Dispose() }
}

function Read-ManifestFile([string]$Path) {
    $table = @{}
    if (Test-Path $Path) {
        foreach ($line in [System.IO.File]::ReadAllLines($Path, $utf8)) {
            $parts = $line.Split("`t")
            if ($parts.Length -eq 3) { $table[$parts[2]] = $parts[0] + '|' + $parts[1] }
        }
    }
    return $table
}

function Write-ManifestFile([hashtable]$Table, [string]$Path) {
    $lines = foreach ($key in ($Table.Keys | Sort-Object)) {
        $value = $Table[$key].Split('|')
        '{0}{3}{1}{3}{2}' -f $value[0], $value[1], $key, "`t"
    }
    Write-TextFile $Path (($lines -join "`n") + "`n")
}

function Get-LocalVersion {
    $path = Join-Path $StateDir 'version.txt'
    if (Test-Path $path) { return [int](Get-Content $path -Raw).Trim() }
    return 0
}

function Set-LocalState([int]$Version, [hashtable]$Manifest) {
    Set-Content -Path (Join-Path $StateDir 'version.txt') -Value $Version -NoNewline
    Write-ManifestFile $Manifest (Join-Path $StateDir 'last-manifest.txt')
}

function Get-LocalManifest {
    $cacheFile = Join-Path $StateDir 'hashcache.txt'
    $cache = @{}
    if (Test-Path $cacheFile) {
        foreach ($line in [System.IO.File]::ReadAllLines($cacheFile, $utf8)) {
            $parts = $line.Split("`t")
            if ($parts.Length -eq 4) { $cache[$parts[0]] = $parts }
        }
    }

    $manifest = @{}
    $newCache = New-Object System.Collections.Generic.List[string]
    foreach ($tree in $Trees) {
        if (-not (Test-Path $tree.Root)) { continue }
        $rootFull = (Resolve-Path $tree.Root).Path.TrimEnd('\')
        $files = New-Object System.Collections.Generic.List[object]
        $children = @(Get-ChildItem $rootFull -Force)
        if ($children.Count -eq 0) { continue }
        # Les FullName sont en forme longue meme si le dossier racine est donne en 8.3 : on part d'eux.
        $rootFull = Split-Path $children[0].FullName -Parent
        foreach ($child in $children) {
            if ($child.PSIsContainer) {
                if ($tree.SkipDirs -contains $child.Name) { continue }
                foreach ($f in (Get-ChildItem $child.FullName -Recurse -File -Force)) { $files.Add($f) }
            } elseif ($SkipTopFiles -notcontains $child.Name) {
                $files.Add($child)
            }
        }

        foreach ($f in $files) {
            $skip = $false
            foreach ($pattern in $SkipFilePatterns) { if ($f.Name -like $pattern) { $skip = $true; break } }
            if ($skip) { continue }

            $rel = $tree.Name + '/' + $f.FullName.Substring($rootFull.Length + 1).Replace('\', '/')
            $ticks = $f.LastWriteTimeUtc.Ticks
            $cached = $cache[$rel]
            if ($cached -and $cached[1] -eq [string]$f.Length -and $cached[2] -eq [string]$ticks) {
                $hash = $cached[3]
            } else {
                try { $hash = Get-FileSha256 $f.FullName }
                catch {
                    Write-Info "  Fichier illisible, ignore : $rel" 'Yellow'
                    continue
                }
            }
            $manifest[$rel] = $hash + '|' + $f.Length
            $newCache.Add(($rel, $f.Length, $ticks, $hash) -join "`t")
        }
    }
    [System.IO.File]::WriteAllLines($cacheFile, $newCache.ToArray(), $utf8)
    return $manifest
}

function Get-RemoteState {
    $manifestTmp = Join-Path $tmp 'remote-manifest.txt'
    $versionTmp = Join-Path $tmp 'remote-version.txt'
    Remove-Item $manifestTmp, $versionTmp -ErrorAction SilentlyContinue
    $version = 0
    if (Copy-FromRemote "$RemoteDir/version.txt" $versionTmp) { $version = [int](Get-Content $versionTmp -Raw).Trim() }
    $manifest = @{}
    if ($version -gt 0 -and (Copy-FromRemote "$RemoteDir/manifest.txt" $manifestTmp)) { $manifest = Read-ManifestFile $manifestTmp }
    return @{ Version = $version; Manifest = $manifest }
}

function Get-Size([hashtable]$Manifest, $Paths) {
    $total = 0
    foreach ($p in $Paths) { $total += [long]$Manifest[$p].Split('|')[1] }
    return [math]::Round($total / 1MB, 1)
}

function Resolve-LocalPath([string]$Rel) {
    if ($Rel.Contains('..')) { throw "Chemin refuse : $Rel" }
    foreach ($tree in $Trees) {
        if ($Rel.StartsWith($tree.Name + '/')) { return Join-Path $tree.Root $Rel.Substring($tree.Name.Length + 1).Replace('/', '\') }
    }
    throw "Dossier inconnu : $Rel"
}

function Initialize-Junctions {
    $clientRoot = $Trees[0].Root
    $editorRoot = $Trees[1].Root
    if (-not (Test-Path $editorRoot)) { return }
    New-Item -ItemType Directory -Force $clientRoot | Out-Null
    foreach ($dir in $SharedDirs) {
        $link = Join-Path $clientRoot $dir
        $dest = Join-Path $editorRoot $dir
        if (-not (Test-Path $dest)) { continue }
        if (Test-Path $link) {
            if ((Get-Item $link -Force).Attributes -band [IO.FileAttributes]::ReparsePoint) { continue }
            try { [System.IO.Directory]::Delete($link) } catch { Write-Info "  $dir : dossier client non vide, lien non cree." 'Yellow'; continue }
        }
        New-Item -ItemType Junction -Path $link -Target $dest | Out-Null
        Write-Info "  Lien cree : client\$dir -> editeur\$dir"
    }
}

function Invoke-Status {
    $remote = Get-RemoteState
    $localVersion = Get-LocalVersion
    $last = Read-ManifestFile (Join-Path $StateDir 'last-manifest.txt')
    $local = Get-LocalManifest

    $localChanges = @($local.Keys | Where-Object { -not $last.ContainsKey($_) -or $last[$_] -ne $local[$_] }).Count
    $localChanges += @($last.Keys | Where-Object { -not $local.ContainsKey($_) }).Count
    $remoteNews = @($remote.Manifest.Keys | Where-Object { -not $local.ContainsKey($_) -or $local[$_] -ne $remote.Manifest[$_] }).Count

    Write-Info "Version de ce PC : $localVersion   |   Version du serveur : $($remote.Version)"
    Write-Info "Modifications locales pas encore envoyees : $localChanges fichier(s)"
    Write-Info "Differences avec le serveur : $remoteNews fichier(s)"

    if ($remote.Version -eq 0) { Write-Output 'STATUS:EMPTY' }
    elseif ($localVersion -lt $remote.Version) { Write-Output "STATUS:REMOTE_NEWER $($remote.Version)" }
    elseif ($localChanges -gt 0) { Write-Output "STATUS:LOCAL_CHANGES $localChanges" }
    else { Write-Output 'STATUS:UPTODATE' }
}

function Invoke-Push {
    foreach ($tree in $Trees) { if (-not (Test-Path $tree.Root)) { throw "Dossier introuvable : $($tree.Root)" } }

    $remote = Get-RemoteState
    $localVersion = Get-LocalVersion
    if ($remote.Version -gt 0 -and $localVersion -ne $remote.Version) {
        throw "Version differente (serveur $($remote.Version), ce PC $localVersion) : fais d'abord 'Recuperer' pour ne rien ecraser."
    }

    Write-Info 'Analyse des fichiers locaux...'
    $last = Read-ManifestFile (Join-Path $StateDir 'last-manifest.txt')
    $local = Get-LocalManifest

    $upload = @($local.Keys | Where-Object { -not $remote.Manifest.ContainsKey($_) -or $remote.Manifest[$_] -ne $local[$_] } | Sort-Object)
    $delete = @($remote.Manifest.Keys | Where-Object { -not $local.ContainsKey($_) -and $last.ContainsKey($_) } | Sort-Object)

    if ($upload.Count -eq 0 -and $delete.Count -eq 0) { Write-Info 'Rien a envoyer : le serveur est deja a jour.' 'Green'; return }
    if (-not $Force -and $remote.Manifest.Count -gt 20 -and $delete.Count -gt ($remote.Manifest.Count / 2)) {
        throw "$($delete.Count) suppressions sur $($remote.Manifest.Count) fichiers : suspect, abandon (relance avec -Force si c'est voulu)."
    }

    Write-Info ("A envoyer : {0} fichier(s), {1} Mo   |   A supprimer sur le serveur : {2}" -f $upload.Count, (Get-Size $local $upload), $delete.Count) 'Cyan'

    $zipPath = Join-Path $tmp 'upload.zip'
    Remove-Item $zipPath -ErrorAction SilentlyContinue
    if ($upload.Count -gt 0) {
        $zip = [System.IO.Compression.ZipFile]::Open($zipPath, 'Create')
        try {
            foreach ($rel in $upload) {
                $abs = Resolve-LocalPath $rel
                [void][System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $abs, $rel, [System.IO.Compression.CompressionLevel]::Fastest)
            }
        } finally { $zip.Dispose() }
    }

    $newVersion = $remote.Version + 1
    $newManifest = @{}
    foreach ($k in $remote.Manifest.Keys) { $newManifest[$k] = $remote.Manifest[$k] }
    foreach ($k in $upload) { $newManifest[$k] = $local[$k] }
    foreach ($k in $delete) { $newManifest.Remove($k) }

    Write-ManifestFile $newManifest (Join-Path $tmp 'new-manifest.txt')
    Write-TextFile (Join-Path $tmp 'new-version.txt') "$newVersion"
    Write-TextFile (Join-Path $tmp 'delete.txt') (($delete -join "`n") + "`n")
    Write-TextFile (Join-Path $tmp 'apply.sh') @'
set -e
D="$1"
mkdir -p "$D/files"
if [ -s /tmp/ressync-upload.zip ]; then unzip -oq /tmp/ressync-upload.zip -d "$D/files"; fi
while IFS= read -r f; do if [ -n "$f" ]; then rm -f "$D/files/$f"; fi; done < /tmp/ressync-delete.txt
mv /tmp/ressync-manifest.txt "$D/manifest.txt"
mv /tmp/ressync-version.txt "$D/version.txt"
echo "envoi $(date -Is) depuis $2" > "$D/meta.txt"
rm -f /tmp/ressync-upload.zip /tmp/ressync-delete.txt /tmp/ressync-apply.sh
'@

    Write-Info 'Transfert vers le serveur...'
    Invoke-Ssh "rm -f /tmp/ressync-upload.zip; mkdir -p $RemoteDir/files"
    if ($upload.Count -gt 0) { Copy-ToRemote $zipPath '/tmp/ressync-upload.zip' }
    Copy-ToRemote (Join-Path $tmp 'delete.txt') '/tmp/ressync-delete.txt'
    Copy-ToRemote (Join-Path $tmp 'new-manifest.txt') '/tmp/ressync-manifest.txt'
    Copy-ToRemote (Join-Path $tmp 'new-version.txt') '/tmp/ressync-version.txt'
    Copy-ToRemote (Join-Path $tmp 'apply.sh') '/tmp/ressync-apply.sh'
    Invoke-Ssh "bash /tmp/ressync-apply.sh $RemoteDir $env:COMPUTERNAME"

    Set-LocalState $newVersion $newManifest
    Write-Info "Envoi termine : le serveur est maintenant en version $newVersion." 'Green'
}

function Invoke-Pull {
    $remote = Get-RemoteState
    if ($remote.Version -eq 0) { throw "Le serveur n'a encore aucune ressource : fais 'Envoyer' depuis le PC qui les possede." }

    Write-Info 'Analyse des fichiers locaux...'
    $firstSync = -not (Test-Path (Join-Path $StateDir 'last-manifest.txt'))
    $last = Read-ManifestFile (Join-Path $StateDir 'last-manifest.txt')
    $local = Get-LocalManifest

    $download = @($remote.Manifest.Keys | Where-Object { -not $local.ContainsKey($_) -or $local[$_] -ne $remote.Manifest[$_] } | Sort-Object)
    $removeLocal = @($last.Keys | Where-Object { -not $remote.Manifest.ContainsKey($_) -and $local.ContainsKey($_) } | Sort-Object)

    if (-not $firstSync) {
        $conflicts = @($download | Where-Object { $local.ContainsKey($_) -and (-not $last.ContainsKey($_) -or $last[$_] -ne $local[$_]) })
        if ($conflicts.Count -gt 0 -and -not $Force) {
            Write-Info "$($conflicts.Count) fichier(s) modifie(s) sur ce PC seraient ecrases par ceux du serveur :" 'Yellow'
            $conflicts | Select-Object -First 10 | ForEach-Object { Write-Info "   $_" 'Yellow' }
            $answer = Read-Host 'Ecraser ? (o/n)'
            if ($answer -ne 'o') { throw 'Annule : rien n a ete modifie.' }
        }
    }

    Write-Info ("A telecharger : {0} fichier(s), {1} Mo   |   A supprimer ici : {2}" -f $download.Count, (Get-Size $remote.Manifest $download), $removeLocal.Count) 'Cyan'

    if ($download.Count -gt 0) {
        Write-TextFile (Join-Path $tmp 'list.txt') (($download -join "`n") + "`n")
        Write-TextFile (Join-Path $tmp 'mkzip.py') @'
import sys, zipfile
lst, out, root = sys.argv[1], sys.argv[2], sys.argv[3].rstrip('/')
with zipfile.ZipFile(out, 'w', zipfile.ZIP_DEFLATED, compresslevel=1) as z:
    for line in open(lst, encoding='utf-8'):
        rel = line.rstrip('\r\n')
        if rel:
            z.write(root + '/' + rel, rel)
'@
        Write-Info 'Preparation de l archive sur le serveur...'
        Copy-ToRemote (Join-Path $tmp 'list.txt') '/tmp/ressync-list.txt'
        Copy-ToRemote (Join-Path $tmp 'mkzip.py') '/tmp/ressync-mkzip.py'
        Invoke-Ssh "python3 /tmp/ressync-mkzip.py /tmp/ressync-list.txt /tmp/ressync-pull.zip $RemoteDir/files"

        Write-Info 'Telechargement...'
        $zipPath = Join-Path $tmp 'pull.zip'
        Remove-Item $zipPath -ErrorAction SilentlyContinue
        if (-not (Copy-FromRemote '/tmp/ressync-pull.zip' $zipPath)) { throw 'Telechargement impossible.' }
        Invoke-Ssh 'rm -f /tmp/ressync-pull.zip /tmp/ressync-list.txt /tmp/ressync-mkzip.py'

        Write-Info 'Extraction...'
        foreach ($tree in $Trees) { New-Item -ItemType Directory -Force $tree.Root | Out-Null }
        $zip = [System.IO.Compression.ZipFile]::OpenRead($zipPath)
        try {
            foreach ($entry in $zip.Entries) {
                if ($entry.FullName.EndsWith('/')) { continue }
                $dest = Resolve-LocalPath $entry.FullName
                New-Item -ItemType Directory -Force (Split-Path $dest) | Out-Null
                [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $dest, $true)
            }
        } finally { $zip.Dispose() }
    }

    foreach ($rel in $removeLocal) {
        $path = Resolve-LocalPath $rel
        if (Test-Path $path) { Remove-Item $path -Force }
    }

    Initialize-Junctions
    Get-LocalManifest | Out-Null
    Set-LocalState $remote.Version $remote.Manifest
    Write-Info "Recuperation terminee : ce PC est en version $($remote.Version)." 'Green'
}

try {
    if (-not (Test-Path $Key)) { throw "Cle SSH introuvable : $Key" }
    switch ($Action) {
        'Status' { Invoke-Status }
        'Push' { Invoke-Push }
        'Pull' { Invoke-Pull }
    }
} catch {
    Write-Host ('ERREUR : ' + $_.Exception.Message) -ForegroundColor Red
    if ($Action -eq 'Status') { Write-Output ('STATUS:ERROR ' + $_.Exception.Message) }
    exit 1
}
