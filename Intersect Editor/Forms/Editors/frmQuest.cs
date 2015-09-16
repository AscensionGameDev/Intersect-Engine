using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Intersect_Editor.Forms.Editors
{
    public partial class frmQuest : Form
    {
        public frmQuest()
        {
            InitializeComponent();
        }

        private void frmQuest_Load(object sender, EventArgs e)
        {
            lstQuests.SelectedIndex = 0;
            cmbClass.Items.Clear();
        }
    }
}
