using System.IO.Compression;
using System.Net.Security;

namespace Intersect.Launcher;

public sealed class LauncherForm : Form
{
    private const string ServerBaseUrl = "https://github.com/Yorudan210/aetherion-releases/releases/latest/download/";
    private const string PatchNotesUrl = "http://159.69.156.73/patch-notes.txt";

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
    private static readonly Color ColorPanelLight = Color.FromArgb(28, 31, 46);
    private static readonly Color ColorBorder = Color.FromArgb(58, 52, 74);
    private static readonly Color ColorGold = Color.FromArgb(235, 196, 110);
    private static readonly Color ColorGoldDim = Color.FromArgb(150, 130, 90);
    private static readonly Color ColorText = Color.FromArgb(214, 214, 224);
    private static readonly Color ColorMuted = Color.FromArgb(140, 140, 158);
    private static readonly Color ColorGreen = Color.FromArgb(120, 200, 140);
    private static readonly Color ColorRed = Color.FromArgb(230, 100, 100);

    // --- Panneau gauche : identité, version, statut, actions -----------------
    private readonly Label _localVersionValue;
    private readonly Label _serverVersionValue;
    private readonly PictureBox _statusIcon;
    private readonly Label _statusLabel;
    private readonly SimpleProgressBar _progressBar;
    private readonly Button _playButton;
    private readonly Button _retryButton;
    private readonly PictureBox? _logoBox;

    // --- Panneau droit : nouveautés + journal ---------------------------------
    private readonly TextBox _patchNotesBox;
    private readonly TextBox _logBox;

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
        ClientSize = new Size(960, 600);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = ColorBackground;
        DoubleBuffered = true;

        // Barre de titre custom (fenêtre sans bordure système) --------------
        var titleBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 34,
            BackColor = ColorPanel,
        };
        titleBar.MouseDown += TitleBar_MouseDown;
        titleBar.MouseMove += TitleBar_MouseMove;
        titleBar.MouseUp += (_, _) => _dragging = false;

        var titleText = new Label
        {
            Text = "LAUNCHER AETHERION",
            ForeColor = ColorGoldDim,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Left,
            Width = 300,
            Padding = new Padding(14, 0, 0, 0),
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
            Width = 42,
            Cursor = Cursors.Hand,
        };
        closeButton.MouseEnter += (_, _) => closeButton.ForeColor = ColorRed;
        closeButton.MouseLeave += (_, _) => closeButton.ForeColor = ColorText;
        closeButton.Click += (_, _) => Close();

        titleBar.Controls.Add(titleText);
        titleBar.Controls.Add(closeButton);

        // ===================== PANNEAU GAUCHE =====================
        var leftPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 340,
            BackColor = ColorBackground,
            Padding = new Padding(20, 16, 20, 16),
        };

        var logoPath = Path.Combine(_baseDirectory, LogoFileName);
        Control logoOrTitle;
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
                    Location = new Point(20, 16),
                    Size = new Size(300, 150),
                    BackColor = Color.Transparent,
                };
                logoOrTitle = _logoBox;
            }
            catch
            {
                _logoBox = null;
                logoOrTitle = BuildFallbackTitle();
            }
        }
        else
        {
            logoOrTitle = BuildFallbackTitle();
        }

        var versionBox = BuildInfoBox("INFORMATIONS DE VERSION", out var versionBoxBody);
        versionBox.Location = new Point(20, 182);
        versionBox.Size = new Size(300, 86);

        var localRow = BuildInfoRow("Version locale :", out _localVersionValue);
        var serverRow = BuildInfoRow("Version serveur :", out _serverVersionValue);
        serverRow.Top = localRow.Bottom + 4;
        versionBoxBody.Controls.Add(localRow);
        versionBoxBody.Controls.Add(serverRow);

        var statusBox = BuildInfoBox("ÉTAT DE LA MISE À JOUR", out var statusBoxBody);
        statusBox.Location = new Point(20, 282);
        statusBox.Size = new Size(300, 110);

        _statusIcon = new PictureBox
        {
            Location = new Point(0, 2),
            Size = new Size(20, 20),
            SizeMode = PictureBoxSizeMode.CenterImage,
        };
        _statusLabel = new Label
        {
            Text = "Initialisation...",
            ForeColor = ColorText,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft,
            Location = new Point(26, 0),
            Size = new Size(254, 22),
        };
        _progressBar = new SimpleProgressBar
        {
            Location = new Point(0, 30),
            Size = new Size(280, 8),
        };
        var logLink = new Label
        {
            Text = "Voir le journal ⟶",
            ForeColor = ColorMuted,
            Font = new Font("Segoe UI", 8),
            AutoSize = true,
            Location = new Point(0, 48),
            Cursor = Cursors.Default,
        };
        statusBoxBody.Controls.Add(_statusIcon);
        statusBoxBody.Controls.Add(_statusLabel);
        statusBoxBody.Controls.Add(_progressBar);
        statusBoxBody.Controls.Add(logLink);

        _playButton = new Button
        {
            Text = "▶  LANCER LE JEU",
            Location = new Point(20, 410),
            Size = new Size(300, 42),
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

        _retryButton = new Button
        {
            Text = "RÉESSAYER",
            Location = new Point(20, 410),
            Size = new Size(300, 42),
            Visible = false,
            BackColor = ColorPanelLight,
            ForeColor = ColorText,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Cursor = Cursors.Hand,
        };
        _retryButton.FlatAppearance.BorderSize = 1;
        _retryButton.FlatAppearance.BorderColor = ColorBorder;
        _retryButton.FlatAppearance.MouseOverBackColor = ColorBorder;
        _retryButton.Click += async (_, _) => await RunStartupSequenceAsync();

        var quitButton = new Button
        {
            Text = "QUITTER",
            Location = new Point(20, 460),
            Size = new Size(300, 34),
            BackColor = ColorBackground,
            ForeColor = ColorMuted,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9),
            Cursor = Cursors.Hand,
        };
        quitButton.FlatAppearance.BorderSize = 1;
        quitButton.FlatAppearance.BorderColor = ColorBorder;
        quitButton.FlatAppearance.MouseOverBackColor = ColorPanelLight;
        quitButton.Click += (_, _) => Close();

        leftPanel.Controls.Add(logoOrTitle);
        leftPanel.Controls.Add(versionBox);
        leftPanel.Controls.Add(statusBox);
        leftPanel.Controls.Add(_playButton);
        leftPanel.Controls.Add(_retryButton);
        leftPanel.Controls.Add(quitButton);

        // ===================== PANNEAU DROIT =====================
        var rightPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = ColorPanel,
            Padding = new Padding(20, 16, 20, 16),
        };

        var newsHeader = BuildSectionHeader("DERNIÈRES NOUVELLES", "Actualités et notes de mise à jour");
        _patchNotesBox = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            BorderStyle = BorderStyle.None,
            BackColor = ColorPanelLight,
            ForeColor = ColorText,
            Font = new Font("Segoe UI", 9.5f),
            Text = "Chargement des nouveautés...",
            Dock = DockStyle.Fill,
        };
        var newsBoxWrap = new Panel
        {
            Location = new Point(0, 56),
            Size = new Size(560, 300),
            BackColor = ColorPanelLight,
            Padding = new Padding(14),
        };
        newsBoxWrap.Controls.Add(_patchNotesBox);

        var logHeader = BuildSectionHeader("JOURNAL DU LAUNCHER", "État de la vérification et de l'installation");
        logHeader.Top = 372;
        _logBox = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            BorderStyle = BorderStyle.None,
            BackColor = ColorPanelLight,
            ForeColor = ColorMuted,
            Font = new Font("Consolas", 9f),
            Dock = DockStyle.Fill,
        };
        var logBoxWrap = new Panel
        {
            Location = new Point(0, 428),
            Size = new Size(560, 120),
            BackColor = ColorPanelLight,
            Padding = new Padding(14),
        };
        logBoxWrap.Controls.Add(_logBox);

        var rightInner = new Panel { Dock = DockStyle.Fill };
        rightInner.Controls.Add(newsHeader);
        rightInner.Controls.Add(newsBoxWrap);
        rightInner.Controls.Add(logHeader);
        rightInner.Controls.Add(logBoxWrap);
        rightInner.Resize += (_, _) =>
        {
            newsBoxWrap.Width = rightInner.Width;
            logHeader.Width = rightInner.Width;
            logBoxWrap.Width = rightInner.Width;
        };
        rightPanel.Controls.Add(rightInner);

        Controls.Add(rightPanel);
        Controls.Add(leftPanel);
        Controls.Add(titleBar);

        Load += async (_, _) => await RunStartupSequenceAsync();
    }

    private Control BuildFallbackTitle()
    {
        return new Label
        {
            Text = "AETHERION",
            Font = new Font("Georgia", 24, FontStyle.Bold),
            ForeColor = ColorGold,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(20, 50),
            Size = new Size(300, 80),
        };
    }

    private Panel BuildInfoBox(string title, out Panel body)
    {
        var box = new Panel
        {
            BackColor = ColorPanel,
            Padding = new Padding(14, 10, 14, 10),
        };
        using (var pen = new Pen(ColorBorder))
        {
            // La bordure est dessinée via OnPaint du panel parent (voir boucle ci-dessous) ;
            // on se contente ici d'un leger panel interieur pour positionner les enfants.
        }
        var header = new Label
        {
            Text = title,
            ForeColor = ColorGold,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            AutoSize = false,
            Dock = DockStyle.Top,
            Height = 20,
        };
        body = new Panel
        {
            Dock = DockStyle.Fill,
        };
        box.Controls.Add(body);
        box.Controls.Add(header);
        box.Paint += (_, e) =>
        {
            using var pen = new Pen(ColorBorder);
            e.Graphics.DrawRectangle(pen, 0, 0, box.Width - 1, box.Height - 1);
        };
        return box;
    }

    private Panel BuildInfoRow(string label, out Label valueLabel)
    {
        var row = new Panel { Size = new Size(272, 20) };
        var labelControl = new Label
        {
            Text = label,
            ForeColor = ColorMuted,
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = false,
            Dock = DockStyle.Left,
            Width = 140,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        valueLabel = new Label
        {
            Text = "—",
            ForeColor = ColorText,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
        };
        row.Controls.Add(valueLabel);
        row.Controls.Add(labelControl);
        return row;
    }

    private Panel BuildSectionHeader(string title, string subtitle)
    {
        var header = new Panel { Location = new Point(0, 0), Size = new Size(560, 48) };
        var titleLabel = new Label
        {
            Text = title,
            ForeColor = ColorGold,
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            AutoSize = false,
            Dock = DockStyle.Top,
            Height = 26,
        };
        var subtitleLabel = new Label
        {
            Text = subtitle,
            ForeColor = ColorMuted,
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = false,
            Dock = DockStyle.Top,
            Height = 18,
        };
        header.Controls.Add(subtitleLabel);
        header.Controls.Add(titleLabel);
        return header;
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

    /// <summary>
    /// Ajoute une ligne au journal (panneau droit) et met aussi a jour le statut court affiche dans le
    /// panneau gauche, pour que les deux zones restent synchronisees sans dupliquer les appels.
    /// </summary>
    private void Log(string message, Color? statusColor = null)
    {
        _statusLabel.Text = message;
        _statusLabel.ForeColor = statusColor ?? ColorText;

        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        _logBox.AppendText($"[{timestamp}] {message}{Environment.NewLine}");
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

    /// <summary>
    /// Réessaie une opération réseau transitoire jusqu'à <paramref name="maxAttempts"/> fois, avec une
    /// courte pause entre chaque essai - utile contre les coupures/latences ponctuelles plutôt que de
    /// faire échouer tout le launcher sur un simple accroc réseau.
    /// </summary>
    private static async Task<T> RetryAsync<T>(Func<Task<T>> operation, int maxAttempts = 3)
    {
        Exception? lastException = null;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return await operation();
            }
            catch (Exception exception)
            {
                lastException = exception;
                if (attempt < maxAttempts)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1.5 * attempt));
                }
            }
        }

        throw lastException!;
    }

    private void ShowFailureState(string message)
    {
        Log(message, ColorRed);
        _progressBar.Value = 0;
        _retryButton.Visible = true;
        _playButton.Visible = !_retryButton.Visible || File.Exists(Path.Combine(_gameDirectory, ClientExeName));
    }

    private async Task RunStartupSequenceAsync()
    {
        _retryButton.Visible = false;
        _playButton.Visible = true;

        _ = LoadPatchNotesAsync();

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
    /// Récupère les notes de version depuis le site (texte brut) pour les afficher dans le panneau
    /// "Dernières nouvelles" - echec silencieux (juste un message dans la zone) puisque ce n'est pas
    /// bloquant pour jouer.
    /// </summary>
    private async Task LoadPatchNotesAsync()
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var text = await RetryAsync(() => httpClient.GetStringAsync(PatchNotesUrl), maxAttempts: 2);
            if (IsHandleCreated)
            {
                Invoke(() => _patchNotesBox.Text = text.Replace("\n", "\r\n"));
            }
        }
        catch
        {
            if (IsHandleCreated)
            {
                Invoke(() => _patchNotesBox.Text = "Impossible de récupérer les nouveautés pour le moment.");
            }
        }
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
            Log("Vérification du launcher...");

            var remoteVersion = (await httpClient.GetStringAsync($"{ServerBaseUrl}{LauncherVersionAssetName}")).Trim();
            var localVersion = File.Exists(_localLauncherVersionFilePath)
                ? (await File.ReadAllTextAsync(_localLauncherVersionFilePath)).Trim()
                : string.Empty;

            if (string.IsNullOrEmpty(remoteVersion) || string.Equals(remoteVersion, localVersion, StringComparison.Ordinal))
            {
                return false;
            }

            Log("Mise à jour du launcher...");

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
            Log("Vérification des mises à jour...");

            var remoteVersion = (await RetryAsync(() => httpClient.GetStringAsync($"{ServerBaseUrl}{GameVersionAssetName}"))).Trim();
            var localVersion = File.Exists(_localGameVersionFilePath)
                ? (await File.ReadAllTextAsync(_localGameVersionFilePath)).Trim()
                : string.Empty;

            _serverVersionValue.Text = $"v{remoteVersion}";
            _localVersionValue.Text = string.IsNullOrEmpty(localVersion) ? "—" : $"v{localVersion}";

            var clientExists = File.Exists(Path.Combine(_gameDirectory, ClientExeName));

            if (clientExists && string.Equals(remoteVersion, localVersion, StringComparison.Ordinal))
            {
                Log("Le jeu est à jour.", ColorGreen);
                _progressBar.Value = 100;
                _playButton.Enabled = true;
                return;
            }

            await DownloadAndInstallAsync(httpClient, remoteVersion);
        }
        catch (Exception exception)
        {
            var hasLocalClient = File.Exists(Path.Combine(_gameDirectory, ClientExeName));
            ShowFailureState(hasLocalClient
                ? $"Impossible de vérifier les mises à jour ({exception.Message}). Tu peux jouer avec la version déjà installée."
                : $"Erreur : {exception.Message}");

            // Si le jeu est déjà installé, on permet quand même de jouer hors-ligne du check de version.
            if (hasLocalClient)
            {
                _playButton.Enabled = true;
                _playButton.Visible = true;
            }
        }
    }

    private async Task DownloadAndInstallAsync(HttpClient httpClient, string remoteVersion)
    {
        Log("Téléchargement du jeu...");
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
                var lastLoggedPercent = -1;
                while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    totalRead += bytesRead;

                    if (totalBytes > 0)
                    {
                        var percent = (int)(totalRead * 100 / totalBytes);
                        _progressBar.Value = Math.Clamp(percent, 0, 100);
                        _statusLabel.Text = $"Téléchargement... {percent}%";

                        // On n'ecrit dans le journal que tous les 10% pour ne pas le noyer de lignes.
                        if (percent / 10 != lastLoggedPercent / 10)
                        {
                            lastLoggedPercent = percent;
                            _logBox.AppendText($"[{DateTime.Now:HH:mm:ss}] Téléchargement... {percent}%{Environment.NewLine}");
                        }
                    }
                }
            }

            Log("Vérification du fichier téléchargé...");

            // Un telechargement coupe/corrompu produit un zip illisible - mieux vaut le detecter ici et
            // le signaler clairement que de planter plus tard avec une erreur obscure pendant l'extraction.
            try
            {
                using var verifyArchive = ZipFile.OpenRead(tempZipPath);
                _ = verifyArchive.Entries.Count;
            }
            catch (InvalidDataException)
            {
                throw new InvalidDataException("Le fichier téléchargé est corrompu (coupure réseau probable). Réessaie.");
            }

            Log("Installation...");
            _progressBar.Value = 0;

            Directory.CreateDirectory(tempExtractDir);
            ZipFile.ExtractToDirectory(tempZipPath, tempExtractDir);

            Directory.CreateDirectory(_gameDirectory);
            SyncDirectory(tempExtractDir, _gameDirectory);

            await File.WriteAllTextAsync(_localGameVersionFilePath, remoteVersion);
            _localVersionValue.Text = $"v{remoteVersion}";

            Log("Installation terminée.", ColorGreen);
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
            Log("Client introuvable, relance le launcher.", ColorRed);
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
