using System.Windows.Forms;
using Intersect.Localization;
using Intersect_Editor.Classes.Maps;
using WeifenLuo.WinFormsUI.Docking;

namespace Intersect_Editor.Forms
{
    public partial class frmMapProperties : DockContent
    {
        public frmMapProperties()
        {
            InitializeComponent();
        }

        public void Init(MapInstance map)
        {
            gridMapProperties.SelectedObject = new MapProperties(map);
            InitLocalization();
        }

        private void InitLocalization()
        {
            Text = Strings.Get("mapproperties", "title");
        }

        public void Update()
        {
            gridMapProperties.Refresh();
        }

        public GridItem Selection()
        {
            return gridMapProperties.SelectedGridItem;
        }
    }
}