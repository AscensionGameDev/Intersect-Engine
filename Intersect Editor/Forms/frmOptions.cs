using System;
using System.Windows.Forms;
using Intersect.Editor.Classes.Core;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms
{
    public partial class FrmOptions : Form
    {
        public FrmOptions()
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
            Text = Strings.options.title;
            btnTileHeader.Text = Strings.options.generaltab.ToString( Application.ProductVersion);
            chkSuppressTilesetWarning.Text = Strings.options.tilesetwarning;
            grpClientPath.Text = Strings.options.pathgroup;
            btnBrowseClient.Text = Strings.options.browsebtn;
        }

        private void frmOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            Preferences.SavePreference("SuppressTextureWarning", chkSuppressTilesetWarning.Checked.ToString());
            Preferences.SavePreference("ClientPath", txtGamePath.Text);
        }

        private void btnBrowseClient_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialogue = new OpenFileDialog()
            {
                Title = Strings.options.dialogueheader,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "exe",
                Filter = "(*.exe)|*.exe|" + Strings.options.dialogueallfiles + "(*.*)|*.*",
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