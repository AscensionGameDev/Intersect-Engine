using Intersect.Editor.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Intersect.Editor.Forms.Editors
{
	public partial class FormRandomPlacer : Form
	{
		public FormRandomPlacer()
		{
			InitializeComponent();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Hide();
			Globals.CurrentEditor = -1;
			Dispose();
		}

		private void btnGenerate_Click(object sender, EventArgs e)
		{
			Globals.MapEditorWindow.Mimus_TileRandomPlacer((int)nudMin.Value, (int)nudMax.Value, rbBlocked.Checked);
		}

		private void nudMin_ValueChanged(object sender, EventArgs e)
		{
			nudMin.Value = Math.Max(nudMin.Value, 0);
			nudMax.Value = Math.Max(nudMin.Value, nudMax.Value);
		}

		private void nudMax_ValueChanged(object sender, EventArgs e)
		{
			nudMax.Value = Math.Max(nudMin.Value, nudMax.Value);
		}

		private void FormRandomPlacer_Load(object sender, EventArgs e)
		{
			nudMin.Text = Localization.Strings.MimusRandomPlacer.min;
			nudMax.Text = Localization.Strings.MimusRandomPlacer.max;
			rbNone.Text = Localization.Strings.MimusRandomPlacer.attributeNone;
			rbBlocked.Text = Localization.Strings.MimusRandomPlacer.attributeBlocked;
			btnCancel.Text = Localization.Strings.MimusRandomPlacer.cancel;
			btnGenerate.Text = Localization.Strings.MimusRandomPlacer.generate;
		}
	}
}
