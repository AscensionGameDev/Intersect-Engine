using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Intersect.DevHub;

public sealed class HubForm : Form
{
    private const string TestHost = "159.69.156.73";
    private const int TestPort = 5401;
    private const string PanelUrl = "http://159.69.156.73/admin/";

    private static readonly Color ColorBackground = Color.FromArgb(16, 18, 28);
    private static readonly Color ColorPanel = Color.FromArgb(28, 31, 46);
    private static readonly Color ColorBorder = Color.FromArgb(58, 52, 74);
    private static readonly Color ColorGold = Color.FromArgb(235, 196, 110);
    private static readonly Color ColorText = Color.FromArgb(214, 214, 224);
    private static readonly Color ColorMuted = Color.FromArgb(140, 140, 158);
    private static readonly Color ColorGreen = Color.FromArgb(120, 200, 140);
    private static readonly Color ColorRed = Color.FromArgb(230, 100, 100);

    private readonly string _root;
    private readonly string _clientExe;
    private readonly string _editorExe;
    private readonly string _clientConfig;
    private readonly string _editorConfig;
    private readonly string _resSyncScript;

    private readonly Label _targetLabel;
    private readonly Label _clientLabel;
    private readonly Label _editorLabel;
    private readonly Label _resourcesLabel;
    private readonly TextBox _logBox;
    private readonly System.Windows.Forms.Timer _timer;

    private Process? _consoleProcess;
    private Process? _checkProcess;

    public HubForm()
    {
        _root = FindRoot();
        _clientExe = Path.Combine(_root, "Intersect.Client", "bin", "Debug", "net8.0", "Intersect Client.exe");
        _editorExe = Path.Combine(_root, "Intersect.Editor", "bin", "Debug", "net8.0-windows", "win-x64", "Intersect Editor.exe");
        _clientConfig = Path.Combine(_root, "Intersect.Client", "bin", "Debug", "net8.0", "resources", "config.json");
        _editorConfig = Path.Combine(_root, "Intersect.Editor", "bin", "Debug", "net8.0-windows", "win-x64", "resources", "config.json");
        _resSyncScript = Path.Combine(_root, "ResSync.ps1");

        Text = "Aetherion Dev Hub";
        ClientSize = new Size(560, 690);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = ColorBackground;
        ForeColor = ColorText;
        Font = new Font("Segoe UI", 10);
        if (Environment.ProcessPath is { } processPath)
        {
            Icon = Icon.ExtractAssociatedIcon(processPath);
        }

        AddLabel("AETHERION", 24, 16, 300, 36, 20, true, ColorGold);
        AddLabel("Outils de développement", 26, 54, 300, 22, 10, false, ColorMuted);

        AddLabel("Cible du client :", 24, 92, 120, 22, 10, false, ColorMuted);
        _targetLabel = AddLabel("-", 150, 92, 390, 22, 10, true, ColorText);
        AddLabel("Client :", 24, 118, 120, 22, 10, false, ColorMuted);
        _clientLabel = AddLabel("-", 150, 118, 390, 22, 10, true, ColorText);
        AddLabel("Éditeur :", 24, 144, 120, 22, 10, false, ColorMuted);
        _editorLabel = AddLabel("-", 150, 144, 390, 22, 10, true, ColorText);
        AddLabel("Ressources :", 24, 170, 120, 22, 10, false, ColorMuted);
        _resourcesLabel = AddLabel("-", 150, 170, 390, 22, 10, true, ColorMuted);

        AddButton("Lancer Client + Éditeur", 24, 210, 512, true, (_, _) =>
        {
            StartApp("Client", _clientExe);
            StartApp("Éditeur", _editorExe);
        });
        AddButton("Lancer le Client", 24, 264, 250, false, (_, _) => StartApp("Client", _clientExe));
        AddButton("Lancer l'Éditeur", 286, 264, 250, false, (_, _) => StartApp("Éditeur", _editorExe));
        AddButton("Tout arrêter", 24, 318, 250, false, (_, _) => StopApps());
        AddButton("Panel web du serveur", 286, 318, 250, false, (_, _) =>
        {
            Process.Start(new ProcessStartInfo(PanelUrl) { UseShellExecute = true });
            Log("Panel web ouvert.");
        });
        AddButton("Compiler tout (ordre sûr)", 24, 372, 250, false, (_, _) => BuildAll());
        AddButton("Mettre à jour (git pull)", 286, 372, 250, false, (_, _) =>
        {
            Log("git pull + sous-modules lancés dans une console.");
            _consoleProcess = RunConsole("git pull; git submodule update --init --recursive");
        });
        AddButton("Cibler le serveur de test (Client + Éditeur)", 24, 426, 512, true, (_, _) => SetTestTarget());
        AddButton("Récupérer les ressources", 24, 480, 250, true, (_, _) =>
        {
            Log("Récupération des ressources lancée dans une console.");
            _consoleProcess = RunConsole($"& '{_resSyncScript}' Pull");
        });
        AddButton("Envoyer mes ressources", 286, 480, 250, false, (_, _) =>
        {
            Log("Envoi des ressources lancé dans une console.");
            _consoleProcess = RunConsole($"& '{_resSyncScript}' Push");
        });
        AddButton("Ouvrir le dossier du projet", 24, 534, 512, false, (_, _) =>
            Process.Start(new ProcessStartInfo("explorer.exe", $"\"{_root}\"") { UseShellExecute = true }));

        _logBox = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            BorderStyle = BorderStyle.None,
            BackColor = ColorPanel,
            ForeColor = ColorMuted,
            Font = new Font("Consolas", 9),
            Location = new Point(24, 592),
            Size = new Size(512, 82),
        };
        Controls.Add(_logBox);

        _timer = new System.Windows.Forms.Timer { Interval = 2000 };
        _timer.Tick += (_, _) => OnTick();
        _timer.Start();

        Load += (_, _) =>
        {
            Log($"Projet : {_root}");
            RefreshStatus();
            StartResourceCheck();
        };
        FormClosed += (_, _) => _timer.Stop();
    }

    private static string FindRoot()
    {
        foreach (var start in new[] { AppContext.BaseDirectory, Environment.CurrentDirectory })
        {
            var directory = new DirectoryInfo(start);
            while (directory != null)
            {
                if (Directory.Exists(Path.Combine(directory.FullName, "Intersect.Client")) &&
                    File.Exists(Path.Combine(directory.FullName, "ResSync.ps1")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }
        }

        MessageBox.Show(
            "Place « Aetherion Dev Hub.exe » dans le dossier du projet (celui qui contient Intersect.Client et ResSync.ps1).",
            "Aetherion Dev Hub",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
        );
        Environment.Exit(1);
        return string.Empty;
    }

    private Label AddLabel(string text, int x, int y, int width, int height, float size, bool bold, Color color)
    {
        var label = new Label
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(width, height),
            Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
            ForeColor = color,
        };
        Controls.Add(label);
        return label;
    }

    private void AddButton(string text, int x, int y, int width, bool primary, EventHandler onClick)
    {
        var button = new Button
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(width, 44),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            BackColor = primary ? ColorGold : ColorPanel,
            ForeColor = primary ? Color.FromArgb(24, 20, 14) : ColorText,
        };
        if (primary)
        {
            button.FlatAppearance.BorderSize = 0;
        }
        else
        {
            button.FlatAppearance.BorderColor = ColorBorder;
        }

        button.Click += onClick;
        Controls.Add(button);
    }

    private void Log(string message)
    {
        _logBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    }

    private void StartApp(string name, string exe)
    {
        if (!File.Exists(exe))
        {
            Log($"{name} introuvable : compile d'abord (bouton Compiler tout).");
            return;
        }

        Process.Start(new ProcessStartInfo(exe) { WorkingDirectory = Path.GetDirectoryName(exe)!, UseShellExecute = true });
        Log($"{name} lancé.");
    }

    private void StopApps()
    {
        foreach (var name in new[] { "Intersect Client", "Intersect Editor", "Intersect Server" })
        {
            foreach (var process in Process.GetProcessesByName(name))
            {
                try { process.Kill(); } catch { /* déjà terminé */ }
            }
        }

        Log("Client / Éditeur / Serveur local arrêtés.");
    }

    private Process? RunConsole(string script)
    {
        var full = $"Set-Location '{_root}'; {script}; Write-Host ''; Read-Host 'Terminé - appuie sur Entrée pour fermer'";
        var encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(full));
        return Process.Start(new ProcessStartInfo("powershell.exe", $"-NoProfile -ExecutionPolicy Bypass -EncodedCommand {encoded}")
        {
            UseShellExecute = true,
            WorkingDirectory = _root,
        });
    }

    private void BuildAll()
    {
        StopApps();
        Log("Compilation lancée dans une console (Network, Server, Client, Editor).");
        _consoleProcess = RunConsole(
            "$ok = $true; foreach ($p in 'Intersect.Network/Intersect.Network.csproj','Intersect.Server/Intersect.Server.csproj'," +
            "'Intersect.Client/Intersect.Client.csproj','Intersect.Editor/Intersect.Editor.csproj') { if ($ok) { " +
            "Write-Host \"=== $p\" -ForegroundColor Cyan; dotnet build $p -c Debug; " +
            "if ($LASTEXITCODE -ne 0) { $ok = $false; Write-Host \"ECHEC sur $p\" -ForegroundColor Red } } }; " +
            "if ($ok) { Write-Host 'Tout est compilé.' -ForegroundColor Green }"
        );
    }

    private void SetTestTarget()
    {
        foreach (var (name, path) in new[] { ("Client", _clientConfig), ("Éditeur", _editorConfig) })
        {
            if (!File.Exists(path))
            {
                Log($"{name} : config.json introuvable (lance-le une fois, puis réessaie).");
                continue;
            }

            var text = File.ReadAllText(path);
            text = Regex.Replace(text, "\"Host\"\\s*:\\s*\"[^\"]*\"", $"\"Host\": \"{TestHost}\"");
            text = Regex.Replace(text, "\"Port\"\\s*:\\s*\\d+", $"\"Port\": {TestPort}");
            File.WriteAllText(path, text, new UTF8Encoding(false));
            Log($"{name} → {TestHost}:{TestPort}");
        }

        RefreshStatus();
    }

    private void OnTick()
    {
        RefreshStatus();

        if (_consoleProcess is { HasExited: true })
        {
            _consoleProcess = null;
            StartResourceCheck();
        }
    }

    private void RefreshStatus()
    {
        SetRunningState(_clientLabel, "Intersect Client");
        SetRunningState(_editorLabel, "Intersect Editor");

        var target = "config.json introuvable (compile / récupère d'abord)";
        if (File.Exists(_clientConfig))
        {
            try
            {
                var text = File.ReadAllText(_clientConfig);
                var host = Regex.Match(text, "\"Host\"\\s*:\\s*\"([^\"]*)\"").Groups[1].Value;
                var port = Regex.Match(text, "\"Port\"\\s*:\\s*(\\d+)").Groups[1].Value;
                target = $"{host}:{port}";
            }
            catch
            {
                target = "config.json illisible";
            }
        }

        _targetLabel.Text = target;
    }

    private static void SetRunningState(Label label, string processName)
    {
        var running = Process.GetProcessesByName(processName).Length > 0;
        label.Text = running ? "en cours" : "arrêté";
        label.ForeColor = running ? ColorGreen : ColorMuted;
    }

    private void StartResourceCheck()
    {
        if (!File.Exists(_resSyncScript) || _checkProcess is { HasExited: false })
        {
            return;
        }

        _resourcesLabel.Text = "vérification...";
        _resourcesLabel.ForeColor = ColorMuted;

        var process = new Process
        {
            StartInfo = new ProcessStartInfo("powershell.exe", $"-NoProfile -ExecutionPolicy Bypass -File \"{_resSyncScript}\" Status")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
            },
            EnableRaisingEvents = true,
        };

        var output = new StringBuilder();
        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                lock (output) { output.AppendLine(e.Data); }
            }
        };
        process.Exited += (_, _) =>
        {
            process.WaitForExit();
            string text;
            lock (output) { text = output.ToString(); }
            if (IsHandleCreated)
            {
                BeginInvoke(() => ShowResourceResult(text));
            }
        };

        _checkProcess = process;
        process.Start();
        process.BeginOutputReadLine();
    }

    private void ShowResourceResult(string output)
    {
        var line = output.Split('\n').Select(l => l.Trim()).LastOrDefault(l => l.StartsWith("STATUS:"));
        if (line == null)
        {
            _resourcesLabel.Text = "vérification impossible";
            _resourcesLabel.ForeColor = ColorRed;
            return;
        }

        if (line.StartsWith("STATUS:UPTODATE"))
        {
            _resourcesLabel.Text = "à jour";
            _resourcesLabel.ForeColor = ColorGreen;
        }
        else if (line.StartsWith("STATUS:REMOTE_NEWER"))
        {
            _resourcesLabel.Text = "NOUVELLE VERSION disponible : clique sur Récupérer";
            _resourcesLabel.ForeColor = ColorGold;
        }
        else if (line.StartsWith("STATUS:LOCAL_CHANGES"))
        {
            var count = line.Split(' ').ElementAtOrDefault(1) ?? "?";
            _resourcesLabel.Text = $"{count} fichier(s) modifié(s) ici : à envoyer";
            _resourcesLabel.ForeColor = ColorGold;
        }
        else if (line.StartsWith("STATUS:EMPTY"))
        {
            _resourcesLabel.Text = "serveur vide : envoie depuis le PC qui a les ressources";
            _resourcesLabel.ForeColor = ColorGold;
        }
        else
        {
            _resourcesLabel.Text = line.Replace("STATUS:ERROR ", "erreur : ");
            _resourcesLabel.ForeColor = ColorRed;
        }
    }
}
