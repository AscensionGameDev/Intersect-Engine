using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Intersect_Editor.Classes.Core;

namespace Intersect_Editor.Forms
{
    public partial class frmOptions : Form
    {
        public frmOptions()
        {
            InitializeComponent();
            InitForm();
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

        private void frmOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            Preferences.SavePreference("SuppressTextureWarning", chkSuppressTilesetWarning.Checked.ToString());
            Preferences.SavePreference("ClientPath",txtGamePath.Text);
        }

        private void btnBrowseClient_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialogue = new OpenFileDialog();

            dialogue.Title = "Browse for Client...";
            dialogue.CheckFileExists = true;
            dialogue.CheckPathExists = true;
            dialogue.DefaultExt = "exe";
            dialogue.Filter = "(*.exe)|*.exe|All Files(*.*)|*.*";
            dialogue.RestoreDirectory = true;
            dialogue.ReadOnlyChecked = true;
            dialogue.ShowReadOnly = true;

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
