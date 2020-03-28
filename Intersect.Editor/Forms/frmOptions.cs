using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;

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
            Text = Strings.Options.title;
            btnTileHeader.Text = Strings.Options.generaltab.ToString(Application.ProductVersion);
            chkSuppressTilesetWarning.Text = Strings.Options.tilesetwarning;
            grpClientPath.Text = Strings.Options.pathgroup;
            btnBrowseClient.Text = Strings.Options.browsebtn;
        }

        private void frmOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            Preferences.SavePreference("SuppressTextureWarning", chkSuppressTilesetWarning.Checked.ToString());
            Preferences.SavePreference("ClientPath", txtGamePath.Text);
        }

        private void btnBrowseClient_Click(object sender, EventArgs e)
        {
            var dialogue = new OpenFileDialog()
            {
                Title = Strings.Options.dialogueheader,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "exe",
                Filter = "(*.exe)|*.exe|" + Strings.Options.dialogueallfiles + "(*.*)|*.*",
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
