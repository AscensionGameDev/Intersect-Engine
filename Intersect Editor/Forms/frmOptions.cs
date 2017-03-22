using System;
using System.Windows.Forms;
using Intersect_Editor.Classes.Core;
using Intersect.Localization;

namespace Intersect_Editor.Forms
{
    public partial class frmOptions : Form
    {
        public frmOptions()
        {
            InitializeComponent();
            InitForm();
            InitLocalization();
        }

        private void InitForm()
        {
            var suppressTilesetWarning = Preferences.LoadPreference("SuppressTextureWarning");
            if (suppressTilesetWarning == "")
            {
                chkSuppressTilesetWarning.Checked = false;
            }
            else
            {
                chkSuppressTilesetWarning.Checked = Convert.ToBoolean(suppressTilesetWarning);
            }
            txtGamePath.Text = Preferences.LoadPreference("ClientPath");
        }

        private void InitLocalization()
        {
            Text = Strings.Get("options", "title");
            btnTileHeader.Text = Strings.Get("options", "generalbtn", Application.ProductVersion);
            chkSuppressTilesetWarning.Text = Strings.Get("options", "tilesetwarning");
            grpClientPath.Text = Strings.Get("options", "pathgroup");
            btnBrowseClient.Text = Strings.Get("options", "browsebtn");
        }

        private void frmOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            Preferences.SavePreference("SuppressTextureWarning", chkSuppressTilesetWarning.Checked.ToString());
            Preferences.SavePreference("ClientPath",txtGamePath.Text);
        }

        private void btnBrowseClient_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialogue = new OpenFileDialog()
            {
                Title = Strings.Get("options", "dialogueheader"),
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "exe",
                Filter = "(*.exe)|*.exe|" + Strings.Get("options", "dialogueallfiles") + "(*.*)|*.*",
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            if (dialogue.ShowDialog() == DialogResult.OK)
            {
                txtGamePath.Text = dialogue.FileName;
            }
        }

        private void chkSuppressTilesetWarning_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
