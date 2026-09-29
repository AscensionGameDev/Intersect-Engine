using System.IO.Compression;
using System.Net.Security;

namespace Intersect.Launcher;

public sealed class LauncherForm : Form
{
    private const string ServerBaseUrl = "https://github.com/Yorudan210/aetherion-releases/releases/latest/download/";

    private const string ClientExeName = "Intersect Client.exe";
    private const string GameFolderName = "game";
    private const string GameVersionAssetName = "version.txt";
    private const string GameZipAssetName = "Aetherion-Client.zip";
    private const string LocalGameVersionFileName = "launcher-version.txt";

    private const string LauncherExeName = "Aetherion Launcher.exe";
    private const string LauncherVersionAssetName = "Launcher-Version.txt";
    private const string LauncherZipAssetName = "Aetherion-Launcher.zip";
    private const string LocalLauncherVersionFileName = "launcher-self-version.txt";

    private const string LogoFileName = "logo.png";

    private static readonly Color ColorBackground = Color.FromArgb(16, 18, 28);
    private static readonly Color ColorPanel = Color.FromArgb(22, 24, 36);
    private static readonly Color ColorBorder = Color.FromArgb(58, 52, 74);
    private static readonly Color ColorGold = Color.FromArgb(235, 196, 110);
    private static readonly Color ColorGoldDim = Color.FromArgb(150, 130, 90);
    private static readonly Color ColorText = Color.FromArgb(214, 214, 224);

    private readonly Label _statusLabel;
    private readonly Label _versionLabel;
    private readonly SimpleProgressBar _progressBar;
    private readonly Button _playButton;
    private readonly PictureBox? _logoBox;

    private readonly string _baseDirectory;
    private readonly string _gameDirectory;
    private readonly string _localGameVersionFilePath;
    private readonly string _localLauncherVersionFilePath;

    private bool _dragging;
    private Point _dragCursorOffset;

    public LauncherForm()
    {
        _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _gameDirectory = Path.Combine(_baseDirectory, GameFolderName);
        _localGameVersionFilePath = Path.Combine(_baseDirectory, LocalGameVersionFileName);
        _localLauncherVersionFilePath = Path.Combine(_baseDirectory, LocalLauncherVersionFileName);

        Text = "Aetherion Launcher";
        FormBorderStyle = FormBorderStyle.None;
        ClientSize = new Size(520, 300);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = ColorBackground;
        DoubleBuffered = true;

        // Barre de titre custom (fenêtre sans bordure système) --------------
        var titleBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 32,
            BackColor = ColorPanel,
        };
        titleBar.MouseDown += TitleBar_MouseDown;
        titleBar.MouseMove += TitleBar_MouseMove;
        titleBar.MouseUp += (_, _) => _dragging = false;

        var titleText = new Label
        {
            Text = "AETHERION LAUNCHER",
            ForeColor = ColorGoldDim,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Left,
            Width = 300,
            Padding = new Padding(12, 0, 0, 0),
        };
        titleText.MouseDown += TitleBar_MouseDown;
        titleText.MouseMove += TitleBar_MouseMove;

        var closeButton = new Label
        {
            Text = "✕",
            ForeColor = ColorText,
            Font = new Font("Segoe UI", 10),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Right,
            Width = 40,
            Cursor = Cursors.Hand,
        };
        closeButton.MouseEnter += (_, _) => closeButton.ForeColor = Color.FromArgb(230, 90, 90);
        closeButton.MouseLeave += (_, _) => closeButton.ForeColor = ColorText;
        closeButton.Click += (_, _) => Close();

        titleBar.Controls.Add(titleText);
        titleBar.Controls.Add(closeButton);

        // Logo (optionnel, fourni par l'utilisateur : <dossier launcher>/logo.png) ---
        var logoPath = Path.Combine(_baseDirectory, LogoFileName);
        var contentTop = 44;

        if (File.Exists(logoPath))
        {
            try
            {
                using var stream = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
                var image = Image.FromStream(stream);
                _logoBox = new PictureBox
                {
                    Image = image,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Location = new Point(0, 44),
                    Size = new Size(520, 140),
                    BackColor = Color.Transparent,
                };
                Controls.Add(_logoBox);
                contentTop = 192;
            }
            catch
            {
                // Fichier logo invalide/corrompu : on se rabat sur le titre texte ci-dessous.
                _logoBox = null;
            }
        }

        if (_logoBox is null)
        {
            var fallbackTitle = new Label
            {
                Text = "AETHERION",
                Font = new Font("Georgia", 26, FontStyle.Bold),
                ForeColor = ColorGold,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 60),
                Size = new Size(520, 60),
            };
            Controls.Add(fallbackTitle);
            contentTop = 140;
        }

        _statusLabel = new Label
        {
            Text = "Prêt à démarrer...",
            ForeColor = ColorText,
            Font = new Font("Segoe UI", 9.5f),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(30, contentTop),
            Size = new Size(460, 22),
        };

        _progressBar = new SimpleProgressBar
        {
            Location = new Point(30, contentTop + 28),
            Size = new Size(460, 8),
        };

        _playButton = new Button
        {
            Text = "JOUER",
            Location = new Point(170, contentTop + 50),
            Size = new Size(180, 38),
            Enabled = false,
            BackColor = ColorGold,
            ForeColor = Color.FromArgb(24, 20, 14),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Cursor = Cursors.Hand,
        };
        _playButton.FlatAppearance.BorderSize = 0;
        _playButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 212, 140);
        _playButton.Click += (_, _) => LaunchGame();

        _versionLabel = new Label
        {
            Text = string.Empty,
            ForeColor = Color.FromArgb(110, 108, 122),
            Font = new Font("Segoe UI", 7.5f),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleRight,
            Dock = DockStyle.Bottom,
            Height = 20,
            Padding = new Padding(0, 0, 10, 0),
        };

        Controls.Add(titleBar);
        Controls.Add(_statusLabel);
        Controls.Add(_progressBar);
        Controls.Add(_playButton);
        Controls.Add(_versionLabel);

        Load += async (_, _) => await RunStartupSequenceAsync();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var pen = new Pen(ColorBorder);
        e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
    }

    private void TitleBar_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        _dragging = true;
        _dragCursorOffset = e.Location;
    }

    private void TitleBar_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!_dragging)
        {
            return;
        }

        var screenPoint = ((Control)sender!).PointToScreen(e.Location);
        Location = new Point(screenPoint.X - _dragCursorOffset.X, screenPoint.Y - _dragCursorOffset.Y);
    }

    private static HttpClient CreateHttpClient()
    {
        var handler = new SocketsHttpHandler
        {
            // Le serveur utilise un certificat auto-signé (installation privée) -
            // on l'accepte explicitement puisqu'on contacte toujours cette adresse connue.
            SslOptions = { RemoteCertificateValidationCallback = (_, _, _, _) => true },
            // Ignore tout proxy système / VPN virtuel (Hamachi, etc.) qui pourrait
            // intercepter ou mal router la requête vers le serveur de jeu.
            UseProxy = false,
            Proxy = null,
            ConnectTimeout = TimeSpan.FromSeconds(10),
        };
        return new HttpClient(handler)
        {
            Timeout = TimeSpan.FromMinutes(10),
        };
    }

    private async Task RunStartupSequenceAsync()
    {
        using var httpClient = CreateHttpClient();

        var selfUpdated = await CheckForLauncherSelfUpdateAsync(httpClient);
        if (selfUpdated)
        {
            // Le processus va être remplacé par le script de relais, on ne continue pas ici.
            return;
        }

        await CheckForGameUpdatesAsync(httpClient);
    }

    /// <summary>
    /// Vérifie si une nouvelle version du LAUNCHER LUI-MÊME est disponible. Si oui,
    /// télécharge le nouvel exécutable, prépare un petit script qui attend la fermeture
    /// du processus courant, remplace le fichier et relance le launcher, puis quitte.
    /// </summary>
    /// <returns>true si une mise à jour a été déclenchée (le processus va se terminer).</returns>
    private async Task<bool> CheckForLauncherSelfUpdateAsync(HttpClient httpClient)
    {
        try
        {
            _statusLabel.Text = "Vérification du launcher...";

            var remoteVersion = (await httpClient.GetStringAsync($"{ServerBaseUrl}{LauncherVersionAssetName}")).Trim();
            var localVersion = File.Exists(_localLauncherVersionFilePath)
                ? (await File.ReadAllTextAsync(_localLauncherVersionFilePath)).Trim()
                : string.Empty;

            if (string.IsNullOrEmpty(remoteVersion) || string.Equals(remoteVersion, localVersion, StringComparison.Ordinal))
            {
                return false;
            }

            _statusLabel.Text = "Mise à jour du launcher...";

            var updateId = Guid.NewGuid().ToString("N");
            var extractDirectory = Path.Combine(Path.GetTempPath(), $"aetherion-launcher-update-{updateId}");
            Directory.CreateDirectory(extractDirectory);
            var tempZipPath = Path.Combine(extractDirectory, LauncherZipAssetName);

            using (var response = await httpClient.GetAsync($"{ServerBaseUrl}{LauncherZipAssetName}"))
            {
                response.EnsureSuccessStatusCode();
                await using var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None);
                await response.Content.CopyToAsync(fileStream);
            }

            var extractedContentDirectory = Path.Combine(extractDirectory, "content");
            ZipFile.ExtractToDirectory(tempZipPath, extractedContentDirectory);

            var newExePath = Path.Combine(extractedContentDirectory, LauncherExeName);
            if (!File.Exists(newExePath))
            {
                // Paquet invalide : on ignore silencieusement la mise à jour du launcher
                // et on continue avec la version actuelle.
                Directory.Delete(extractDirectory, recursive: true);
                return false;
            }

            var currentExePath = Environment.ProcessPath ?? Path.Combine(_baseDirectory, LauncherExeName);
            var scriptPath = Path.Combine(Path.GetTempPath(), $"aetherion-launcher-update-{updateId}.cmd");
            var pid = Environment.ProcessId;

            var newLogoPath = Path.Combine(extractedContentDirectory, LogoFileName);
            var logoCopyLine = File.Exists(newLogoPath)
                ? $"copy /Y \"{newLogoPath}\" \"{Path.Combine(_baseDirectory, LogoFileName)}\" >NUL\r\n"
                : string.Empty;

            var scriptContent =
                "@echo off\r\n" +
                ":wait\r\n" +
                $"tasklist /FI \"PID eq {pid}\" 2>NUL | find \"{pid}\" >NUL\r\n" +
                "if not errorlevel 1 (\r\n" +
                "    timeout /t 1 /nobreak >NUL\r\n" +
                "    goto wait\r\n" +
                ")\r\n" +
                $"copy /Y \"{newExePath}\" \"{currentExePath}\" >NUL\r\n" +
                logoCopyLine +
                $"rmdir /S /Q \"{extractDirectory}\"\r\n" +
                $"start \"\" \"{currentExePath}\"\r\n" +
                "del \"%~f0\"\r\n";

            await File.WriteAllTextAsync(scriptPath, scriptContent);
            await File.WriteAllTextAsync(_localLauncherVersionFilePath, remoteVersion);

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{scriptPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
            });

            Application.Exit();
            return true;
        }
        catch
        {
            // On ne bloque jamais le lancement du jeu si la vérification/mise à jour
            // du launcher lui-même échoue (ex: pas de connexion) - on continue simplement
            // avec la version actuelle.
            return false;
        }
    }

    private async Task CheckForGameUpdatesAsync(HttpClient httpClient)
    {
        try
        {
            _statusLabel.Text = "Vérification des mises à jour...";

            var remoteVersion = (await httpClient.GetStringAsync($"{ServerBaseUrl}{GameVersionAssetName}")).Trim();
            var localVersion = File.Exists(_localGameVersionFilePath)
                ? (await File.ReadAllTextAsync(_localGameVersionFilePath)).Trim()
                : string.Empty;

            _versionLabel.Text = $"v{remoteVersion}";

            var clientExists = File.Exists(Path.Combine(_gameDirectory, ClientExeName));

            if (clientExists && string.Equals(remoteVersion, localVersion, StringComparison.Ordinal))
            {
                _statusLabel.Text = "Jeu à jour.";
                _progressBar.Value = 100;
                _playButton.Enabled = true;
                return;
            }

            await DownloadAndInstallAsync(httpClient, remoteVersion);
        }
        catch (Exception exception)
        {
            _statusLabel.Text = $"Erreur : {exception.Message}";

            // Si le jeu est déjà installé, on permet quand même de jouer hors-ligne du check de version.
            if (File.Exists(Path.Combine(_gameDirectory, ClientExeName)))
            {
                _playButton.Enabled = true;
            }
        }
    }

    private async Task DownloadAndInstallAsync(HttpClient httpClient, string remoteVersion)
    {
        _statusLabel.Text = "Téléchargement du jeu...";
        _playButton.Enabled = false;

        var tempZipPath = Path.Combine(Path.GetTempPath(), $"aetherion-client-{Guid.NewGuid():N}.zip");
        var tempExtractDir = Path.Combine(Path.GetTempPath(), $"aetherion-client-extract-{Guid.NewGuid():N}");

        try
        {
            using (var response = await httpClient.GetAsync(
                       $"{ServerBaseUrl}{GameZipAssetName}",
                       HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                await using var contentStream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None);

                var buffer = new byte[81920];
                long totalRead = 0;
                int bytesRead;
                while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    totalRead += bytesRead;

                    if (totalBytes > 0)
                    {
                        var percent = (int)(totalRead * 100 / totalBytes);
                        _progressBar.Value = Math.Clamp(percent, 0, 100);
                        _statusLabel.Text = $"Téléchargement... {percent}%";
                    }
                }
            }

            _statusLabel.Text = "Installation...";
            _progressBar.Value = 0;

            Directory.CreateDirectory(tempExtractDir);
            ZipFile.ExtractToDirectory(tempZipPath, tempExtractDir);

            Directory.CreateDirectory(_gameDirectory);
            SyncDirectory(tempExtractDir, _gameDirectory);

            await File.WriteAllTextAsync(_localGameVersionFilePath, remoteVersion);

            _statusLabel.Text = "Installation terminée.";
            _progressBar.Value = 100;
            _playButton.Enabled = true;
        }
        finally
        {
            if (File.Exists(tempZipPath))
            {
                File.Delete(tempZipPath);
            }

            if (Directory.Exists(tempExtractDir))
            {
                Directory.Delete(tempExtractDir, recursive: true);
            }
        }
    }

    /// <summary>
    /// Mirrors <paramref name="sourceDir"/> onto <paramref name="targetDir"/>: copies files that are new
    /// or whose content changed, and deletes anything in the target that the source no longer ships -
    /// so an update only touches what actually changed instead of wiping and reinstalling everything,
    /// which used to blow away e.g. locally-saved keybinds/settings on every single update.
    /// </summary>
    private static void SyncDirectory(string sourceDir, string targetDir)
    {
        foreach (var sourceFile in Directory.EnumerateFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourceDir, sourceFile);
            var targetFile = Path.Combine(targetDir, relativePath);

            if (FilesAreIdentical(sourceFile, targetFile))
            {
                continue;
            }

            var targetFileDir = Path.GetDirectoryName(targetFile);
            if (!string.IsNullOrEmpty(targetFileDir))
            {
                Directory.CreateDirectory(targetFileDir);
            }

            File.Copy(sourceFile, targetFile, overwrite: true);
        }

        foreach (var targetFile in Directory.EnumerateFiles(targetDir, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(targetDir, targetFile);
            var sourceFile = Path.Combine(sourceDir, relativePath);

            if (!File.Exists(sourceFile))
            {
                File.Delete(targetFile);
            }
        }

        // Clean up any directories left empty by the removals above.
        foreach (var directory in Directory.EnumerateDirectories(targetDir, "*", SearchOption.AllDirectories)
                     .OrderByDescending(dir => dir.Length))
        {
            if (!Directory.EnumerateFileSystemEntries(directory).Any())
            {
                Directory.Delete(directory);
            }
        }
    }

    private static bool FilesAreIdentical(string sourceFile, string targetFile)
    {
        if (!File.Exists(targetFile))
        {
            return false;
        }

        if (new FileInfo(sourceFile).Length != new FileInfo(targetFile).Length)
        {
            return false;
        }

        using var sourceStream = File.OpenRead(sourceFile);
        using var targetStream = File.OpenRead(targetFile);
        using var sha256 = System.Security.Cryptography.SHA256.Create();

        var sourceHash = sha256.ComputeHash(sourceStream);
        var targetHash = sha256.ComputeHash(targetStream);

        return sourceHash.AsSpan().SequenceEqual(targetHash);
    }

    private void LaunchGame()
    {
        var clientExePath = Path.Combine(_gameDirectory, ClientExeName);
        if (!File.Exists(clientExePath))
        {
            _statusLabel.Text = "Client introuvable, relance le launcher.";
            return;
        }

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = clientExePath,
            WorkingDirectory = _gameDirectory,
            UseShellExecute = true,
        });

        Close();
    }
}
