using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.Editor.Maps;

using WeifenLuo.WinFormsUI.Docking;

namespace Intersect.Editor.Forms.DockingElements
{

    public partial class FrmMapProperties : DockContent
    {

        //Cross Thread Delegates
        public delegate void UpdateProperties();

        public UpdateProperties UpdatePropertiesDelegate;

        public FrmMapProperties()
        {
            InitializeComponent();
            UpdatePropertiesDelegate = Update;

            this.Icon = Properties.Resources.Icon;
        }

        public void Init(MapInstance map)
        {
            if (gridMapProperties.InvokeRequired)
            {
                gridMapProperties.Invoke((MethodInvoker) delegate { Init(map); });

                return;
            }

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
