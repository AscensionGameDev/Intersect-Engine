using System.Windows.Forms;
using Intersect.Editor.Classes.Maps;
using Intersect.Editor.Classes.Localization;
using WeifenLuo.WinFormsUI.Docking;

namespace Intersect.Editor.Forms
{
    public partial class FrmMapProperties : DockContent
    {
        public FrmMapProperties()
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
            Text = Strings.MapProperties.title;
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