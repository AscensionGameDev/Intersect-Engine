<#
.SYNOPSIS
    Builds a clean, player-ready copy of the Intersect Client: one self-contained .exe (no loose
    dependency DLLs) plus a packed "resources" folder, zipped up and ready to send out.

.DESCRIPTION
    A raw `dotnet build` output folder (bin/Debug/...) ships 60+ loose files: every dependency DLL,
    XML doc-comment files (IDE-only, never read at runtime), debug symbols, and (for items/entities/
    spells/animations/faces/fogs/paperdolls/tilesets) directory junctions straight into the Editor's own
    resources folder, so the two apps always share one live copy of the art during development. None of
    that belongs in what a player downloads.

    `dotnet publish -c Release` already collapses almost all of the dependency clutter automatically: the
    client project has PublishSingleFile + SelfContained enabled for Release builds, so publishing bundles
    every managed dependency into one compressed .exe and drops the satellite-language/multi-platform
    runtime folders entirely. What's left after that (a handful of .xml/.pdb dev files) gets deleted here.

    The "resources" folder gets copied alongside (Copy-Item follows the dev junctions and copies the real
    files, so the publish output is a normal standalone folder, never links), skipping logs/source_assets.
    Intersect.Tools.ResourcePacker then runs against THIS COPY ONLY - never against the dev bin/Debug
    folder, which must stay loose so the Editor<->Client junction keeps working - bundling
    items/entities/spells/animations/faces/fogs/paperdolls/tilesets into opaque .pack files and moving the
    loose PNGs out of the shipped copy before it's zipped.

    Finally, this mirrors the clean build into the SERVER's own assets/client folder. The server already has
    a full, working auto-update system (Intersect.Server\Web\Controllers\AssetManagement\AssetsController.cs,
    enabled by default via appsettings.json's UpdateServer section) that generates an update manifest by
    scanning that folder and lets already-installed clients download only what's missing/changed and delete
    whatever's no longer there - nothing extra needed to build for that, it already exists and just needed
    something to serve. This is why the server must be restarted after running this script for updates to
    reach existing players (it caches the generated manifest for a while, but a restart guarantees a fresh
    one), and why the SHIPPED client's config.json needs "UpdateUrl" pointed at it - see the printed
    instructions at the end of a run for the exact value.

.PARAMETER OutputDir
    Where to place the clean build before zipping. Defaults to publish\client-release next to this
    script.

.PARAMETER Zip
    Also produce publish\Aetherion-Client.zip from the clean build. Default: true.

.PARAMETER ServerAssetsDir
    Where the server serves client updates from. Defaults to
    Intersect.Server\bin\Debug\net8.0\assets\client next to this script - point this at your real server's
    folder if you build here but run the server elsewhere.

.PARAMETER PublishGitHubRelease
    Also publish a new GitHub release with the zip, which is what "Aetherion Launcher.exe" (the actual
    player-facing app - see Intersect.Launcher\LauncherForm.cs) checks for updates against, completely
    separately from the server's own auto-update system above. It compares a plain "version.txt" asset
    against a locally-cached copy and, if different, downloads "Aetherion-Client.zip" fresh. Default: true.
    Requires `gh` authenticated with push access to GitHubRepo.

.PARAMETER GitHubRepo
    The releases-only repo the Launcher pulls from. Default: Yorudan210/aetherion-releases.

.PARAMETER PublicHost
.PARAMETER PublicPort
    The Host/Port a downloaded player client should connect to (the Aetherion VM's public IP and the WAN
    port your Freebox forwards to it - 45400 -> 5400, not the VM's own internal port). These get written
    into ONLY the shipped copy's config.json, never into
    Intersect.Client\bin\Debug\net8.0\resources\config.json - that one stays "localhost"/5400 so local
    testing (client+server both on this PC) keeps working. Publishing used to overwrite the dev config in
    place, which is why local testing showed "server offline" right after a publish run - don't reintroduce
    that by editing the dev config.json directly for this again.
#>
param(
    [string]$OutputDir = (Join-Path $PSScriptRoot "publish\client-release"),
    [bool]$Zip = $true,
    [string]$ServerAssetsDir = (Join-Path $PSScriptRoot "Intersect.Server\bin\Debug\net8.0\assets\client"),
    [bool]$PublishGitHubRelease = $true,
    [string]$GitHubRepo = "Yorudan210/aetherion-releases",
    [string]$PublicHost = "88.188.93.215",
    [int]$PublicPort = 45400
)

$ErrorActionPreference = "Stop"

$repoRoot = $PSScriptRoot
$clientProject = Join-Path $repoRoot "Intersect.Client\Intersect.Client.csproj"
$devResourcesDir = Join-Path $repoRoot "Intersect.Client\bin\Debug\net8.0\resources"
$zipPath = Join-Path $repoRoot "publish\Aetherion-Client.zip"

if (-not (Test-Path $devResourcesDir)) {
    throw "Resources folder not found at '$devResourcesDir'. Build the client in Debug at least once first."
}

Write-Host "Publishing client (Release, win-x64, self-contained, single-file)..." -ForegroundColor Cyan
if (Test-Path $OutputDir) {
    Remove-Item $OutputDir -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputDir | Out-Null

dotnet publish $clientProject -c Release -r win-x64 --self-contained true -o $OutputDir
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE"
}

Write-Host "Stripping dev-only files (doc XML, debug symbols)..." -ForegroundColor Cyan
Get-ChildItem $OutputDir -Filter "*.xml" | Remove-Item -Force
Get-ChildItem $OutputDir -Filter "*.pdb" | Remove-Item -Force

Write-Host "Copying resources (config, translations, art/sound)..." -ForegroundColor Cyan
$outResourcesDir = Join-Path $OutputDir "resources"
Copy-Item $devResourcesDir $outResourcesDir -Recurse

Write-Host "Pointing the shipped config.json at $PublicHost`:$PublicPort (dev config.json left untouched)..." -ForegroundColor Cyan
$shippedConfigPath = Join-Path $outResourcesDir "config.json"
$shippedConfig = Get-Content $shippedConfigPath -Raw | ConvertFrom-Json
$shippedConfig.Host = $PublicHost
$shippedConfig.Port = $PublicPort
$shippedConfig | ConvertTo-Json -Depth 10 | Set-Content -Path $shippedConfigPath -Encoding utf8

Write-Host "Packing art into opaque .pack blobs (deters casual copying)..." -ForegroundColor Cyan
$packerProject = Join-Path $repoRoot "Intersect.Tools.ResourcePacker\Intersect.Tools.ResourcePacker.csproj"
dotnet run --project $packerProject -c Release -- $outResourcesDir
if ($LASTEXITCODE -ne 0) {
    throw "Resource packer failed with exit code $LASTEXITCODE"
}

# The packer moves originals into resources/../source_assets inside the PUBLISH copy - fold that back into
# packs so nothing extra ships, then drop the empty backup folder.
$publishSourceAssets = Join-Path $OutputDir "source_assets"
if (Test-Path $publishSourceAssets) {
    Remove-Item $publishSourceAssets -Recurse -Force
}

$fileCount = (Get-ChildItem $OutputDir -Recurse -File).Count
$sizeMb = [math]::Round(((Get-ChildItem $OutputDir -Recurse -File | Measure-Object Length -Sum).Sum / 1MB), 1)
Write-Host "Clean build ready: $OutputDir ($fileCount files, $sizeMb MB)" -ForegroundColor Green

if ($Zip) {
    Write-Host "Zipping to $zipPath..." -ForegroundColor Cyan
    if (Test-Path $zipPath) {
        Remove-Item $zipPath -Force
    }
    Compress-Archive -Path (Join-Path $OutputDir "*") -DestinationPath $zipPath -CompressionLevel Optimal
    $zipSizeMb = [math]::Round(((Get-Item $zipPath).Length / 1MB), 1)
    Write-Host "Done: $zipPath ($zipSizeMb MB)" -ForegroundColor Green
}

Write-Host "Mirroring build into the server's update folder ($ServerAssetsDir)..." -ForegroundColor Cyan
if (Test-Path $ServerAssetsDir) {
    Remove-Item $ServerAssetsDir -Recurse -Force
}
New-Item -ItemType Directory -Path $ServerAssetsDir -Force | Out-Null
Copy-Item (Join-Path $OutputDir "*") $ServerAssetsDir -Recurse

Write-Host ""
Write-Host "Done. Already-installed players update automatically on next launch, once:" -ForegroundColor Green
Write-Host "  1. Their client's resources\config.json has an UpdateUrl set (only needs doing once, in the"
Write-Host "     very first build you send out). The server redirects plain HTTP to HTTPS (port 5443 by"
Write-Host "     default), so point at the HTTPS port directly, e.g. for example.com:"
Write-Host "       `"UpdateUrl`": `"https://example.com:5443/assets/#appmanifest`""
Write-Host "     Verified working against the bundled self-signed cert for localhost/LAN IPs. For real"
Write-Host "     players over the internet on a public host/domain, that cert must be a properly trusted"
Write-Host "     one (e.g. Let's Encrypt) - the client's HTTP client only auto-trusts self-signed certs for"
Write-Host "     loopback/private addresses (see IntersectHttpClient.cs)."
Write-Host "  2. The server is restarted (or its update-manifest cache expires) so it picks up this build."

if ($PublishGitHubRelease) {
    Write-Host ""
    Write-Host "Publishing a new GitHub release to $GitHubRepo (what Aetherion Launcher.exe actually checks)..." -ForegroundColor Cyan

    $gh = Get-Command gh -ErrorAction SilentlyContinue
    if (-not $gh) {
        Write-Host "  SKIPPED: `gh` CLI not found on PATH." -ForegroundColor Yellow
    }
    else {
        $currentVersionRaw = gh release download -R $GitHubRepo --pattern "version.txt" -O - 2>$null
        $currentVersion = 0
        if ($LASTEXITCODE -eq 0 -and [int]::TryParse(($currentVersionRaw | Out-String).Trim(), [ref]$currentVersion)) {
            # got it
        }
        else {
            $currentVersion = 0
        }

        $newVersion = $currentVersion + 1
        $versionFilePath = Join-Path $OutputDir "..\version.txt"
        Set-Content -Path $versionFilePath -Value "$newVersion" -NoNewline

        gh release create "v$newVersion" $zipPath $versionFilePath `
            --repo $GitHubRepo `
            --title "Version $newVersion" `
            --notes "Automated build via publish-client.ps1"

        if ($LASTEXITCODE -eq 0) {
            Write-Host "  Published v$newVersion - Aetherion Launcher.exe will pick it up on next launch." -ForegroundColor Green
        }
        else {
            Write-Host "  FAILED to publish the GitHub release (see output above)." -ForegroundColor Red
        }
    }
}
