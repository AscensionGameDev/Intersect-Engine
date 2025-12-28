using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Configuration;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Web;
using Newtonsoft.Json;

namespace Intersect.Editor.Forms;

public partial class FrmUploadToServer : DarkDialog
{
    private string? _selectedDirectory;
    private TokenResponse? _tokenResponse;

    public FrmUploadToServer()
    {
        InitializeComponent();
        Icon = Program.Icon;
        LoadSettings();
    }

    private void LoadSettings()
    {
        // Load saved settings
        var savedUrl = Preferences.LoadPreference("upload_serverUrl");
        if (!string.IsNullOrWhiteSpace(savedUrl))
        {
            txtServerUrl.Text = savedUrl;
        }
        else if (ClientConfiguration.Instance.UpdateUrl is { } updateUrl && !string.IsNullOrWhiteSpace(updateUrl))
        {
            txtServerUrl.Text = updateUrl;
        }

        var savedType = Preferences.LoadPreference("upload_type");
        if (savedType == "editor")
        {
            rbEditorAssets.Checked = true;
        }
        else
        {
            rbClientAssets.Checked = true;
        }

        // Try to load token from preferences
        var rawTokenResponse = Preferences.LoadPreference(nameof(TokenResponse));
        if (!string.IsNullOrWhiteSpace(rawTokenResponse))
        {
            try
            {
                _tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(rawTokenResponse);
            }
            catch
            {
                // Token deserialization failed, will need to authenticate
            }
        }
    }

    private void SaveSettings()
    {
        Preferences.SavePreference("upload_serverUrl", txtServerUrl.Text);
        Preferences.SavePreference("upload_type", rbEditorAssets.Checked ? "editor" : "client");
        if (!string.IsNullOrWhiteSpace(_selectedDirectory))
        {
            Preferences.SavePreference("upload_lastDirectory", _selectedDirectory);
        }
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
        using var folderDialog = new FolderBrowserDialog
        {
            Description = Strings.UploadToServer.SourceDirectoryPrompt,
            ShowNewFolderButton = false
        };

        var lastDir = Preferences.LoadPreference("upload_lastDirectory");
        if (!string.IsNullOrWhiteSpace(lastDir) && Directory.Exists(lastDir))
        {
            folderDialog.SelectedPath = lastDir;
        }

        if (folderDialog.ShowDialog() == DialogResult.OK)
        {
            _selectedDirectory = folderDialog.SelectedPath;
            txtDirectory.Text = _selectedDirectory;
            btnUpload.Enabled = true;
        }
    }

    private async void btnUpload_Click(object sender, EventArgs e)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(txtServerUrl.Text))
        {
            DarkMessageBox.ShowError(
                Strings.UploadToServer.InvalidUrl,
                Strings.UploadToServer.Title,
                DarkUI.Controls.DarkDialogButton.Ok,
                Icon
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(_selectedDirectory) || !Directory.Exists(_selectedDirectory))
        {
            DarkMessageBox.ShowError(
                Strings.UploadToServer.InvalidDirectory,
                Strings.UploadToServer.Title,
                DarkUI.Controls.DarkDialogButton.Ok,
                Icon
            );
            return;
        }

        SaveSettings();

        // Disable controls during upload
        btnUpload.Enabled = false;
        btnBrowse.Enabled = false;
        txtServerUrl.Enabled = false;
        rbClientAssets.Enabled = false;
        rbEditorAssets.Enabled = false;
        progressBar.Value = 0;
        lblStatus.Text = Strings.UploadToServer.Uploading.ToString(0);

        try
        {
            await PerformUpload();
        }
        catch (Exception ex)
        {
            lblStatus.Text = Strings.UploadToServer.Error.ToString(ex.Message);
            DarkMessageBox.ShowError(
                Strings.UploadToServer.Error.ToString(ex.Message),
                Strings.UploadToServer.Failed,
                DarkUI.Controls.DarkDialogButton.Ok,
                Icon
            );
        }
        finally
        {
            // Re-enable controls
            btnUpload.Enabled = true;
            btnBrowse.Enabled = true;
            txtServerUrl.Enabled = true;
            rbClientAssets.Enabled = true;
            rbEditorAssets.Enabled = true;
        }
    }

    private async Task PerformUpload()
    {
        var uploadType = rbEditorAssets.Checked ? "editor" : "client";
        var serverUrl = txtServerUrl.Text.TrimEnd('/');
        var endpoint = $"{serverUrl}/api/v1/editor/updates/{uploadType}";

        // Get all files in the directory
        var files = Directory.GetFiles(_selectedDirectory!, "*.*", SearchOption.AllDirectories);
        var totalFiles = files.Length;
        var uploadedFiles = 0;

        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromMinutes(30); // Long timeout for large uploads

        // Add authorization header if we have a token
        if (_tokenResponse != null)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenResponse.AccessToken);
        }

        // Upload files in batches
        const int batchSize = 10;
        for (var i = 0; i < files.Length; i += batchSize)
        {
            var batch = files.Skip(i).Take(batchSize).ToArray();

            using var content = new MultipartFormDataContent();

            foreach (var filePath in batch)
            {
                var relativePath = Path.GetRelativePath(_selectedDirectory!, filePath);
                var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                content.Add(fileContent, "files", relativePath.Replace('\\', '/'));
            }

            var response = await httpClient.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Upload failed with status {response.StatusCode}: {errorContent}");
            }

            uploadedFiles += batch.Length;
            var progress = (int)((uploadedFiles / (float)totalFiles) * 100);
            progressBar.Value = Math.Min(progress, 100);
            lblStatus.Text = Strings.UploadToServer.FilesUploaded.ToString(uploadedFiles, totalFiles);
            Application.DoEvents();
        }

        progressBar.Value = 100;
        lblStatus.Text = Strings.UploadToServer.Success;

        DarkMessageBox.ShowInformation(
            Strings.UploadToServer.Success,
            Strings.UploadToServer.Completed,
            DarkUI.Controls.DarkDialogButton.Ok,
            Icon
        );
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
        Close();
    }
}
