using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class Event_MoveRouteAnimationSelector : UserControl
    {
        private MoveRouteAction _myAction;
        private bool _newAction = false;
        private Event_MoveRouteDesigner _routeDesigner;

        public Event_MoveRouteAnimationSelector(Event_MoveRouteDesigner moveRouteDesigner, MoveRouteAction action,
            bool newAction = false)
        {
            InitializeComponent();
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.Get("general", "none"));
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            if (!newAction)
            {
                cmbAnimation.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Animation, action.AnimationIndex);
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
            _myAction.AnimationIndex = Database.GameObjectIdFromList(GameObjectType.Animation, cmbAnimation.SelectedIndex);
            _routeDesigner.Controls.Remove(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _routeDesigner.RemoveLastAction();
            _routeDesigner.Controls.Remove(this);
        }
    }
}