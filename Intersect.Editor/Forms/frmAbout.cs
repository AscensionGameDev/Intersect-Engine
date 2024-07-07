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
        BrowserUtils.Open("http://ascensiongamedev.com");
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
