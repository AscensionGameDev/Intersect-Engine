using WeifenLuo.WinFormsUI.Docking;
using Intersect_Editor.Classes;
using Intersect_Editor.Classes.Maps;
using System.Windows.Forms;

namespace Intersect_Editor.Forms
{
    public partial class frmMapProperties : DockContent
    {
        public frmMapProperties()
        {
            InitializeComponent();
        }

        public void Init(int mapNum)
        {
            if (mapNum == -1 || Globals.GameMaps[mapNum] == null)
            {
                gridMapProperties.SelectedObject = null;
            }
            else
            {
                gridMapProperties.SelectedObject = new MapProperties(Globals.GameMaps[mapNum]);
            }
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
