using System;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventMoveRouteAnimationSelector : UserControl
    {

        private MoveRouteAction mMyAction;

        private bool mNewAction;

        private EventMoveRouteDesigner mRouteDesigner;

        public EventMoveRouteAnimationSelector(
            EventMoveRouteDesigner moveRouteDesigner,
            MoveRouteAction action,
            bool newAction = false
        )
        {
            InitializeComponent();
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.General.none);
            cmbAnimation.Items.AddRange(AnimationBase.Names);
            if (!newAction)
            {
                cmbAnimation.SelectedIndex = AnimationBase.ListIndex(action.AnimationId) + 1;
            }

            mNewAction = newAction;
            mRouteDesigner = moveRouteDesigner;
            mMyAction = action;
            InitLocalization();
        }

        private void InitLocalization()
        {
            grpSetAnimation.Text = Strings.EventSetAnimation.title;
            lblAnimation.Text = Strings.EventSetAnimation.label;
            btnOkay.Text = Strings.EventSetAnimation.okay;
            btnCancel.Text = Strings.EventSetAnimation.cancel;
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            mMyAction.AnimationId = AnimationBase.IdFromList(cmbAnimation.SelectedIndex - 1);
            mRouteDesigner.Controls.Remove(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mRouteDesigner.RemoveLastAction();
            mRouteDesigner.Controls.Remove(this);
        }

    }

}
