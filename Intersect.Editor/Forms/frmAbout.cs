using Intersect.Editor.Core;
using Intersect.Editor.Localization;
using Intersect.Utilities;

namespace Intersect.Editor.Forms;


public partial class FrmAbout : Form
{

    public FrmAbout()
    {
        InitializeComponent();
        Icon = Program.Icon;
    }

    private void lblWebsite_Click(object sender, EventArgs e)
    {
        BrowserUtils.Open("https://ascensiongamedev.com");
    }

    private void frmAbout_Load(object sender, EventArgs e)
    {
        InitLocalization();
    }

    private void InitLocalization()
    {
        Text = Strings.About.title;
        lblVersion.Text = Strings.About.version.ToString(Application.ProductVersion);
        lblVersion.Location = new System.Drawing.Point(
            (lblVersion.Parent?.ClientRectangle.Right - (lblVersion.Parent?.Padding.Right + lblVersion.Width + 4)) ?? 0,
            (lblVersion.Parent?.ClientRectangle.Bottom - (lblVersion.Parent?.Padding.Bottom + lblVersion.Height + 4)) ?? 0
        );
        lblWebsite.Text = Strings.About.site;
    }

}
