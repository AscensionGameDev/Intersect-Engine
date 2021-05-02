using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;

namespace Intersect.Editor.Forms
{

    public partial class FrmAbout : Form
    {

        public FrmAbout()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.Icon;
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
            Text = Strings.About.title;
            lblVersion.Text = Strings.About.version.ToString(Application.ProductVersion);
            lblWebsite.Text = Strings.About.site;
        }

    }

}
