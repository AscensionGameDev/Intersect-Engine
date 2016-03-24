using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class Event_MoveRouteAnimationSelector : UserControl
    {
        private Event_MoveRouteDesigner _routeDesigner;
        private MoveRouteAction _myAction;
        private bool _newAction = false;
        public Event_MoveRouteAnimationSelector(Event_MoveRouteDesigner moveRouteDesigner,MoveRouteAction action, bool newAction = false)
        {
            InitializeComponent();
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add("None");
            for (int i = 0; i < Globals.GameAnimations.Length; i++)
            {
                cmbAnimation.Items.Add((i + 1) + ". " + Globals.GameAnimations[i].Name);
            }
            if (newAction)
            {
                cmbAnimation.SelectedIndex = action.AnimationIndex + 1;
            }
            _newAction = newAction;
            _routeDesigner = moveRouteDesigner;
            _myAction = action;
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            _myAction.AnimationIndex = cmbAnimation.SelectedIndex - 1;
            _routeDesigner.Controls.Remove(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _routeDesigner.RemoveLastAction();
            _routeDesigner.Controls.Remove(this);
        }
    }
}
