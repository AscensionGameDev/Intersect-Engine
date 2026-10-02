# Publie le CONTENU fait dans l'editeur (serveur de TEST) vers la PRODUCTION : objets, sorts, recettes, cartes modifiees...
# Fusion sure : ajoute ce qui est nouveau et met a jour ce qui est plus recent cote test, ne supprime RIEN en prod.
# Ne touche jamais aux comptes joueurs (playerdata.db). Sauvegarde automatique juste avant, redemarrage garanti.
#   .\PublishContent.ps1            -> apercu, confirmation, puis application
#   .\PublishContent.ps1 -Preview   -> apercu seulement (rien n'est modifie)
param(
    [switch]$Preview,
    [string]$Key = (Join-Path $env:USERPROFILE '.ssh\id_ed25519_aetherion'),
    [string]$RemoteHost = '159.69.156.73',
    [string]$RemoteUser = 'root'
)

$ErrorActionPreference = 'Stop'
$target = "$RemoteUser@$RemoteHost"
$sshOpts = @('-i', $Key, '-o', 'BatchMode=yes', '-o', 'StrictHostKeyChecking=accept-new', '-o', 'ConnectTimeout=15')
$utf8 = New-Object System.Text.UTF8Encoding($false)
$tmp = Join-Path $env:TEMP 'aetherion-publish-content'
New-Item -ItemType Directory -Force $tmp | Out-Null

function Invoke-Remote([string]$Command) {
    $previous = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        & ssh @sshOpts $target $Command 2>&1 | ForEach-Object { if ($_ -isnot [System.Management.Automation.ErrorRecord]) { $_ } }
    } finally { $ErrorActionPreference = $previous }
    if ($LASTEXITCODE -ne 0) { throw "La commande distante a echoue (code $LASTEXITCODE)." }
}

function Send-File([string]$Local, [string]$Remote) {
    $previous = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try { & scp -q @sshOpts $Local "${target}:$Remote" 2>&1 | Out-Null } finally { $ErrorActionPreference = $previous }
    if ($LASTEXITCODE -ne 0) { throw "Envoi impossible : $Local" }
}

$mergeScript = Join-Path $PSScriptRoot 'tools\merge_content.py'
if (-not (Test-Path $Key)) { throw "Cle SSH introuvable : $Key" }
if (-not (Test-Path $mergeScript)) { throw "Script de fusion introuvable : $mergeScript" }

$remoteScript = @'
set -e
PROD=/home/aetherion/intersect-server/resources
TEST=/home/aetherion/intersect-server-test/resources
MODE="$1"

if [ "$(sqlite3 $TEST/gamedata.db 'PRAGMA integrity_check;')" != "ok" ]; then
  echo "La base du serveur de test est abimee : abandon."; exit 1
fi

if [ "$MODE" = "preview" ]; then
  cp $PROD/gamedata.db /tmp/pc-prod.db
  cp $TEST/gamedata.db /tmp/pc-test.db
  python3 /tmp/pc-merge.py /tmp/pc-prod.db /tmp/pc-test.db
  echo "cartes en prod apres fusion : $(sqlite3 /tmp/pc-prod.db 'SELECT COUNT(*) FROM Maps;') (aucune n'est supprimee)"
  rm -f /tmp/pc-prod.db /tmp/pc-test.db /tmp/pc-merge.py
  exit 0
fi

BACKUP="$PROD/gamedata.db.bak_contenu_$(date +%Y%m%d_%H%M)"
systemctl stop intersect-server
trap 'systemctl start intersect-server' EXIT
cp $PROD/gamedata.db "$BACKUP"
cp $TEST/gamedata.db /tmp/pc-test.db
python3 /tmp/pc-merge.py $PROD/gamedata.db /tmp/pc-test.db
chown aetherion:aetherion $PROD/gamedata.db*
chmod g+w $PROD/gamedata.db
rm -f /tmp/pc-test.db /tmp/pc-merge.py
echo "sauvegarde : $BACKUP"
'@

try {
    Write-Host "`n=== Publication du contenu : TEST -> PRODUCTION ===" -ForegroundColor Cyan
    Write-Host "(ne touche pas aux comptes joueurs, ne supprime rien en prod)`n"

    Send-File $mergeScript '/tmp/pc-merge.py'
    [System.IO.File]::WriteAllText((Join-Path $tmp 'publish.sh'), ($remoteScript -replace "`r`n", "`n"), $utf8)
    Send-File (Join-Path $tmp 'publish.sh') '/tmp/pc-publish.sh'

    Write-Host "--- Apercu de ce qui serait applique en production :" -ForegroundColor Yellow
    Invoke-Remote 'bash /tmp/pc-publish.sh preview'

    if ($Preview) {
        Invoke-Remote 'rm -f /tmp/pc-publish.sh /tmp/pc-merge.py'
        Write-Host "`nApercu seulement : rien n'a ete modifie." -ForegroundColor Green
        return
    }

    Write-Host ""
    $answer = Read-Host "Appliquer a la PRODUCTION ? Le serveur sera coupe quelques secondes (joueurs deconnectes). (o/n)"
    if ($answer -ne 'o') {
        Invoke-Remote 'rm -f /tmp/pc-publish.sh /tmp/pc-merge.py'
        Write-Host "Annule : rien n'a ete modifie." -ForegroundColor Yellow
        return
    }

    Write-Host "`nApplication (arret, sauvegarde, fusion, redemarrage)..." -ForegroundColor Cyan
    Invoke-Remote 'bash /tmp/pc-publish.sh apply'

    Write-Host "`nVerification..." -ForegroundColor Cyan
    Start-Sleep -Seconds 10
    Invoke-Remote 'echo "serveur : $(systemctl is-active intersect-server)"; R=/home/aetherion/intersect-server/resources; echo "integrite de la base : $(sqlite3 $R/gamedata.db "PRAGMA integrity_check;")"; echo "cartes : $(sqlite3 $R/gamedata.db "SELECT COUNT(*) FROM Maps;")  objets : $(sqlite3 $R/gamedata.db "SELECT COUNT(*) FROM Items;")  sorts : $(sqlite3 $R/gamedata.db "SELECT COUNT(*) FROM Spells;")  recettes : $(sqlite3 $R/gamedata.db "SELECT COUNT(*) FROM Crafts;")"; echo "comptes (inchanges) : $(sqlite3 $R/playerdata.db "SELECT COUNT(*) FROM Users;")"; rm -f /tmp/pc-publish.sh'
    Write-Host "`nTermine : le contenu du test est maintenant en production." -ForegroundColor Green
    Write-Host "Rappel : les images liees arrivent chez les joueurs avec 'Publier le client'." -ForegroundColor Yellow
}
catch {
    Write-Host ("ERREUR : " + $_.Exception.Message) -ForegroundColor Red
    Write-Host "Si le serveur a ete coupe, il est relance automatiquement. Verifie son etat avec le panel web." -ForegroundColor Yellow
    exit 1
}
