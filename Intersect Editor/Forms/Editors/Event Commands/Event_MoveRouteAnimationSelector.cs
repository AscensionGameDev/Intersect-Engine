
using System;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect;
using Intersect.GameObjects.Events;
using Intersect.Localization;

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
            cmbAnimation.Items.Add(Strings.Get("general","none"));
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObject.Animation));
            if (!newAction)
            {
                cmbAnimation.SelectedIndex = Database.GameObjectListIndex(GameObject.Animation,action.AnimationIndex);
            }
            _newAction = newAction;
            _routeDesigner = moveRouteDesigner;
            _myAction = action;
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpSetAnimation.Text = Strings.Get("eventsetanimation", "title");
            lblAnimation.Text = Strings.Get("eventsetanimation", "label");
            btnOkay.Text = Strings.Get("eventsetanimation", "okay");
            btnCancel.Text = Strings.Get("eventsetanimation", "cancel");
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            _myAction.AnimationIndex = Database.GameObjectIdFromList(GameObject.Animation,cmbAnimation.SelectedIndex);
            _routeDesigner.Controls.Remove(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _routeDesigner.RemoveLastAction();
            _routeDesigner.Controls.Remove(this);
        }
    }
}
