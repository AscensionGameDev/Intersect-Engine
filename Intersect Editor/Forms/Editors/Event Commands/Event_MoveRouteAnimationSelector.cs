using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Localization;

namespace Intersect.Editor.Forms.Editors.Event_Commands
{
    public partial class EventMoveRouteAnimationSelector : UserControl
    {
        private MoveRouteAction mMyAction;
        private bool mNewAction;
        private EventMoveRouteDesigner mRouteDesigner;

        public EventMoveRouteAnimationSelector(EventMoveRouteDesigner moveRouteDesigner, MoveRouteAction action,
            bool newAction = false)
        {
            InitializeComponent();
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.Get("general", "none"));
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            if (!newAction)
            {
                cmbAnimation.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Animation, action.AnimationIndex);
            }
            mNewAction = newAction;
            mRouteDesigner = moveRouteDesigner;
            mMyAction = action;
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
            mMyAction.AnimationIndex =
                Database.GameObjectIdFromList(GameObjectType.Animation, cmbAnimation.SelectedIndex);
            mRouteDesigner.Controls.Remove(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mRouteDesigner.RemoveLastAction();
            mRouteDesigner.Controls.Remove(this);
        }
    }
}