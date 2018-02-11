using System.Windows.Forms;

namespace Intersect.Editor.Forms
{
    public partial class FrmLoadingContent : Form
    {
        public FrmLoadingContent()
        {
            InitializeComponent();
        }

        public void SetProgress(int percent)
        {
            lblProgress.Text = "Downloading: " + percent + "% Complete";
        }
    }
}