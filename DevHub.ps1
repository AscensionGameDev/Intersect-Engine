Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing
[System.Windows.Forms.Application]::EnableVisualStyles()

$Root = $PSScriptRoot
$ClientExe = Join-Path $Root 'Intersect.Client\bin\Debug\net8.0\Intersect Client.exe'
$EditorExe = Join-Path $Root 'Intersect.Editor\bin\Debug\net8.0-windows\win-x64\Intersect Editor.exe'
$ClientConfig = Join-Path $Root 'Intersect.Client\bin\Debug\net8.0\resources\config.json'
$SyncBat = Join-Path $env:OneDrive 'IntersectSync\Sync Intersect.bat'
$PanelUrl = 'http://159.69.156.73/admin/'

$cBg = [System.Drawing.Color]::FromArgb(16, 18, 28)
$cPanel = [System.Drawing.Color]::FromArgb(28, 31, 46)
$cGold = [System.Drawing.Color]::FromArgb(235, 196, 110)
$cText = [System.Drawing.Color]::FromArgb(214, 214, 224)
$cMuted = [System.Drawing.Color]::FromArgb(140, 140, 158)
$cGreen = [System.Drawing.Color]::FromArgb(120, 200, 140)
$cRed = [System.Drawing.Color]::FromArgb(230, 100, 100)

$form = New-Object System.Windows.Forms.Form
$form.Text = 'Aetherion Dev Hub'
$form.ClientSize = New-Object System.Drawing.Size(560, 560)
$form.StartPosition = 'CenterScreen'
$form.BackColor = $cBg
$form.ForeColor = $cText
$form.FormBorderStyle = 'FixedSingle'
$form.MaximizeBox = $false
$form.Font = New-Object System.Drawing.Font('Segoe UI', 10)

function New-Label($text, $x, $y, $w, $h, $size, $bold, $color) {
    $l = New-Object System.Windows.Forms.Label
    $l.Text = $text
    $l.Location = New-Object System.Drawing.Point($x, $y)
    $l.Size = New-Object System.Drawing.Size($w, $h)
    $style = if ($bold) { [System.Drawing.FontStyle]::Bold } else { [System.Drawing.FontStyle]::Regular }
    $l.Font = New-Object System.Drawing.Font('Segoe UI', $size, $style)
    $l.ForeColor = $color
    $form.Controls.Add($l)
    return $l
}

function Write-Log($message) {
    $logBox.AppendText(('[{0}] {1}{2}' -f (Get-Date -Format 'HH:mm:ss'), $message, [Environment]::NewLine))
}

function New-HubButton($text, $x, $y, $w, $primary, $onClick) {
    $b = New-Object System.Windows.Forms.Button
    $b.Text = $text
    $b.Location = New-Object System.Drawing.Point($x, $y)
    $b.Size = New-Object System.Drawing.Size($w, 44)
    $b.FlatStyle = 'Flat'
    $b.Cursor = [System.Windows.Forms.Cursors]::Hand
    $b.Font = New-Object System.Drawing.Font('Segoe UI', 10, [System.Drawing.FontStyle]::Bold)
    if ($primary) {
        $b.BackColor = $cGold
        $b.ForeColor = [System.Drawing.Color]::FromArgb(24, 20, 14)
        $b.FlatAppearance.BorderSize = 0
    } else {
        $b.BackColor = $cPanel
        $b.ForeColor = $cText
        $b.FlatAppearance.BorderColor = [System.Drawing.Color]::FromArgb(58, 52, 74)
    }
    $b.Add_Click($onClick)
    $form.Controls.Add($b)
    return $b
}

function Start-App($name, $exe) {
    if (-not (Test-Path $exe)) {
        Write-Log "$name introuvable : compile d'abord (bouton Compiler tout)."
        return
    }
    Start-Process -FilePath $exe -WorkingDirectory (Split-Path $exe)
    Write-Log "$name lance."
}

function Stop-Apps {
    Get-Process -Name 'Intersect Client', 'Intersect Editor', 'Intersect Server' -ErrorAction SilentlyContinue | Stop-Process -Force
    Write-Log 'Client / Editeur / Serveur local arretes.'
}

function Start-Console($script) {
    $full = "Set-Location '$Root'; $script; Write-Host ''; Read-Host 'Termine - appuie sur Entree pour fermer'"
    Start-Process powershell -ArgumentList '-NoProfile', '-ExecutionPolicy', 'Bypass', '-Command', $full
}

$null = New-Label 'AETHERION' 24 16 300 36 20 $true $cGold
$null = New-Label 'Outils de developpement' 26 54 300 22 10 $false $cMuted

$null = New-Label 'Cible du client :' 24 92 120 22 10 $false $cMuted
$lblTarget = New-Label '-' 150 92 380 22 10 $true $cText
$null = New-Label 'Client :' 24 118 120 22 10 $false $cMuted
$lblClient = New-Label '-' 150 118 380 22 10 $true $cText
$null = New-Label 'Editeur :' 24 144 120 22 10 $false $cMuted
$lblEditor = New-Label '-' 150 144 380 22 10 $true $cText

$null = New-HubButton 'Lancer Client + Editeur' 24 184 512 $true {
    Start-App 'Client' $ClientExe
    Start-App 'Editeur' $EditorExe
}
$null = New-HubButton 'Lancer le Client' 24 238 250 $false { Start-App 'Client' $ClientExe }
$null = New-HubButton "Lancer l'Editeur" 286 238 250 $false { Start-App 'Editeur' $EditorExe }
$null = New-HubButton 'Tout arreter' 24 292 250 $false { Stop-Apps }
$null = New-HubButton 'Panel web du serveur' 286 292 250 $false {
    Start-Process $PanelUrl
    Write-Log 'Panel web ouvert.'
}
$null = New-HubButton 'Compiler tout (ordre sur)' 24 346 250 $false {
    Stop-Apps
    Write-Log 'Compilation lancee dans une console (Network, Server, Client, Editor).'
    Start-Console ('$ok = $true; foreach ($p in "Intersect.Network/Intersect.Network.csproj","Intersect.Server/Intersect.Server.csproj","Intersect.Client/Intersect.Client.csproj","Intersect.Editor/Intersect.Editor.csproj") { if ($ok) { Write-Host "=== $p" -ForegroundColor Cyan; dotnet build $p -c Debug; if ($LASTEXITCODE -ne 0) { $ok = $false; Write-Host "ECHEC sur $p" -ForegroundColor Red } } }; if ($ok) { Write-Host "Tout est compile." -ForegroundColor Green }')
}
$null = New-HubButton 'Mettre a jour (git pull)' 286 346 250 $false {
    Write-Log 'git pull + sous-modules lances dans une console.'
    Start-Console 'git pull; git submodule update --init --recursive'
}
$null = New-HubButton 'Synchro OneDrive' 24 400 250 $false {
    if (Test-Path $SyncBat) {
        Start-Process $SyncBat
        Write-Log 'Synchro OneDrive lancee.'
    } else {
        Write-Log "Sync Intersect.bat introuvable ($SyncBat)."
    }
}
$null = New-HubButton 'Ouvrir le dossier du projet' 286 400 250 $false { Start-Process explorer.exe $Root }

$logBox = New-Object System.Windows.Forms.TextBox
$logBox.Multiline = $true
$logBox.ReadOnly = $true
$logBox.ScrollBars = 'Vertical'
$logBox.BorderStyle = 'None'
$logBox.BackColor = $cPanel
$logBox.ForeColor = $cMuted
$logBox.Font = New-Object System.Drawing.Font('Consolas', 9)
$logBox.Location = New-Object System.Drawing.Point(24, 460)
$logBox.Size = New-Object System.Drawing.Size(512, 80)
$form.Controls.Add($logBox)

function Update-Status {
    foreach ($entry in @(@('Intersect Client', $lblClient), @('Intersect Editor', $lblEditor))) {
        $running = $null -ne (Get-Process -Name $entry[0] -ErrorAction SilentlyContinue)
        $entry[1].Text = if ($running) { 'en cours' } else { 'arrete' }
        $entry[1].ForeColor = if ($running) { $cGreen } else { $cMuted }
    }

    $target = 'config.json introuvable (compile / synchronise d abord)'
    if (Test-Path $ClientConfig) {
        try {
            $cfg = Get-Content $ClientConfig -Raw | ConvertFrom-Json
            $target = '{0}:{1}' -f $cfg.Host, $cfg.Port
        } catch {
            $target = 'config.json illisible'
        }
    }
    $lblTarget.Text = $target
}

$timer = New-Object System.Windows.Forms.Timer
$timer.Interval = 2000
$timer.Add_Tick({ Update-Status })
$timer.Start()

Update-Status
Write-Log "Projet : $Root"
[void]$form.ShowDialog()
