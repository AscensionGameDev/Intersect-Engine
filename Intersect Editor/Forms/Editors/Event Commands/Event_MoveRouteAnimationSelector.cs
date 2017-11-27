using System;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Editor.Classes.Localization;

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
            cmbAnimation.Items.Add(Strings.general.none);
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            if (!newAction)
            {
                cmbAnimation.SelectedIndex =  Database.GameObjectListIndex(GameObjectType.Animation, action.AnimationIndex) + 1;
            }
            mNewAction = newAction;
            mRouteDesigner = moveRouteDesigner;
            mMyAction = action;
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpSetAnimation.Text = Strings.eventsetanimation.title;
            lblAnimation.Text = Strings.eventsetanimation.label;
            btnOkay.Text = Strings.eventsetanimation.okay;
            btnCancel.Text = Strings.eventsetanimation.cancel;
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            mMyAction.AnimationIndex =  Database.GameObjectIdFromList(GameObjectType.Animation, cmbAnimation.SelectedIndex - 1);
            mRouteDesigner.Controls.Remove(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mRouteDesigner.RemoveLastAction();
            mRouteDesigner.Controls.Remove(this);
        }
    }
}