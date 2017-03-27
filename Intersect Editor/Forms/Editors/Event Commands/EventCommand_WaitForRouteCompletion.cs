using System;
using System.Linq;
using System.Windows.Forms;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_WaitForRouteCompletion : UserControl
    {
        private readonly EventBase _editingEvent;
        private MapBase _currentMap;
        private EventCommand _editingCommand;
        private FrmEvent _eventEditor;

        public EventCommand_WaitForRouteCompletion(EventCommand refCommand, FrmEvent eventEditor, MapBase currentMap,
            EventBase currentEvent)
        {
            InitializeComponent();

            //Grab event editor reference
            _eventEditor = eventEditor;
            _editingEvent = currentEvent;
            _editingCommand = refCommand;
            _currentMap = currentMap;
            InitLocalization();
            cmbEntities.Items.Clear();
            if (!_editingEvent.CommonEvent)
            {
                cmbEntities.Items.Add(Strings.Get("eventwaitforroutecompletion", "player"));
                if (_editingCommand.Ints[0] == -1) cmbEntities.SelectedIndex = -1;
                foreach (var evt in _currentMap.Events)
                {
                    cmbEntities.Items.Add(evt.Key == _editingEvent.Index
                        ? Strings.Get("eventwaitforroutecompletion", "this") + " "
                        : "" + evt.Value.Name);
                    if (_editingCommand.Ints[0] == evt.Key) cmbEntities.SelectedIndex = cmbEntities.Items.Count - 1;
                }
            }
            if (cmbEntities.SelectedIndex == -1 && cmbEntities.Items.Count > 0)
            {
                cmbEntities.SelectedIndex = 0;
            }

            _editingCommand = refCommand;
            _eventEditor = eventEditor;
        }

        private void InitLocalization()
        {
            grpWaitRoute.Text = Strings.Get("eventwaitforroutecompletion", "title");
            lblEntity.Text = Strings.Get("eventwaitforroutecompletion", "label");
            btnSave.Text = Strings.Get("eventwaitforroutecompletion", "okay");
            btnCancel.Text = Strings.Get("eventwaitforroutecompletion", "cancel");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!_editingEvent.CommonEvent)
            {
                if (cmbEntities.SelectedIndex == 0)
                {
                    _editingCommand.Ints[0] = -1;
                }
                else
                {
                    _editingCommand.Ints[0] = _currentMap.Events.Keys.ToList()[cmbEntities.SelectedIndex - 1];
                }
            }
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }
    }
}