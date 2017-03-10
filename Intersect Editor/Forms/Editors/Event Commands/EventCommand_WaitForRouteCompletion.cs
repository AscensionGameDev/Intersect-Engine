/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Linq;
using System.Windows.Forms;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps;
using Intersect_Library.Localization;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_WaitForRouteCompletion : UserControl
    {
        private FrmEvent _eventEditor;
        private readonly EventBase _editingEvent;
        private EventCommand _editingCommand;
        private MapBase _currentMap;
        public EventCommand_WaitForRouteCompletion(EventCommand refCommand, FrmEvent eventEditor, MapBase currentMap, EventBase currentEvent)
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
                    cmbEntities.Items.Add(evt.Key == _editingEvent.MyIndex ? Strings.Get("eventwaitforroutecompletion", "this")+ " " : "" + evt.Value.MyName);
                    if (_editingCommand.Ints[0] == evt.Key) cmbEntities.SelectedIndex = cmbEntities.Items.Count-1;
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
                    _editingCommand.Ints[0] = _currentMap.Events.Keys.ToList()[cmbEntities.SelectedIndex-1];
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
