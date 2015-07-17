using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using Intersect_Editor.Classes;
using Intersect_Editor.Classes.Maps;

namespace Intersect_Editor.Forms
{
    public partial class frmMapProperties : DockContent
    {
        public frmMapProperties()
        {
            InitializeComponent();
        }

        public void Init(MapStruct map)
        {
            if (map == null)
            {
                gridMapProperties.SelectedObject = null;
            }
            else
            {
                gridMapProperties.SelectedObject = new MapProperties(map);
            }
        }
    }
}
