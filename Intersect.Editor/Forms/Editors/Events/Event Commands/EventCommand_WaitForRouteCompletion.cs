using System;
using System.Linq;
using System.Windows.Forms;

using Intersect.Editor.Localization;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.GameObjects.Maps;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{

    public partial class EventCommandWaitForRouteCompletion : UserControl
    {

        private readonly EventBase mEditingEvent;

        private MapBase mCurrentMap;

        private WaitForRouteCommand mEditingCommand;

        private FrmEvent mEventEditor;

        public EventCommandWaitForRouteCompletion(
            WaitForRouteCommand refCommand,
            FrmEvent eventEditor,
            MapBase currentMap,
            EventBase currentEvent
        )
        {
            InitializeComponent();

            //Grab event editor reference
            mEventEditor = eventEditor;
            mEditingEvent = currentEvent;
            mEditingCommand = refCommand;
            mCurrentMap = currentMap;
            InitLocalization();
            cmbEntities.Items.Clear();
            if (!mEditingEvent.CommonEvent)
            {
                cmbEntities.Items.Add(Strings.EventWaitForRouteCompletion.player);
                if (mEditingCommand.TargetId == Guid.Empty)
                {
                    cmbEntities.SelectedIndex = -1;
                }

                foreach (var evt in mCurrentMap.LocalEvents)
                {
                    cmbEntities.Items.Add(
                        evt.Key == mEditingEvent.Id
                            ? Strings.EventWaitForRouteCompletion.This + " "
                            : "" + evt.Value.Name
                    );

                    if (mEditingCommand.TargetId == evt.Key)
                    {
                        cmbEntities.SelectedIndex = cmbEntities.Items.Count - 1;
                    }
                }
            }

            if (cmbEntities.SelectedIndex == -1 && cmbEntities.Items.Count > 0)
            {
                cmbEntities.SelectedIndex = 0;
            }

            mEditingCommand = refCommand;
            mEventEditor = eventEditor;
        }

        private void InitLocalization()
        {
            grpWaitRoute.Text = Strings.EventWaitForRouteCompletion.title;
            lblEntity.Text = Strings.EventWaitForRouteCompletion.label;
            btnSave.Text = Strings.EventWaitForRouteCompletion.okay;
            btnCancel.Text = Strings.EventWaitForRouteCompletion.cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!mEditingEvent.CommonEvent)
            {
                if (cmbEntities.SelectedIndex == 0)
                {
                    mEditingCommand.TargetId = Guid.Empty;
                }
                else
                {
                    mEditingCommand.TargetId = mCurrentMap.LocalEvents.Keys.ToList()[cmbEntities.SelectedIndex - 1];
                }
            }

            mEventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mEventEditor.CancelCommandEdit();
        }

    }

}
