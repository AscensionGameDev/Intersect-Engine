using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intersect.Editor.Forms.Controls
{
    public partial class GameObjectList : TreeView
    {
        public GameObjectList()
        {
            InitializeComponent();

            this.ImageList = this.imageList;
        }
    }
}
