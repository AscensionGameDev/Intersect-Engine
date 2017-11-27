using System;
using System.Windows.Forms;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms
{
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();
        }

        private void lblWebsite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://ascensiongamedev.com");
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            InitLocalization();
        }

        private void InitLocalization()
        {
            Text = Strings.about.title;
            lblVersion.Text = Strings.about.version.ToString( Application.ProductVersion);
            lblWebsite.Text = Strings.about.site;
        }
    }
}