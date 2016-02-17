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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_WaitForRouteCompletion : UserControl
    {
        private FrmEvent _eventEditor;
        private readonly EventStruct _editingEvent;
        private EventCommand _editingCommand;
        private MapStruct _currentMap;
        public EventCommand_WaitForRouteCompletion(EventCommand refCommand, FrmEvent eventEditor, MapStruct currentMap, EventStruct currentEvent)
        {
            InitializeComponent();

            //Grab event editor reference
            _eventEditor = eventEditor;
            _editingEvent = currentEvent;
            _editingCommand = refCommand;
            _currentMap = currentMap;

            cmbEntities.Items.Clear();
            cmbEntities.Items.Add("Player");
            cmbEntities.SelectedIndex = 0;

            if (!_editingEvent.CommonEvent)
            {
                for (int i = 0; i < currentMap.Events.Count; i++)
                {
                    if (i == _editingEvent.MyIndex)
                    {
                        cmbEntities.Items.Add((i + 1) + ". [THIS EVENT] " + currentMap.Events[i].MyName);

                        if (refCommand.Ints[0] == i)
                        {
                            cmbEntities.SelectedIndex = i + 1;
                        }
                    }
                    else
                    {
                        if (currentMap.Events[i].Deleted == 0)
                        {
                            cmbEntities.Items.Add((i + 1) + ". " + currentMap.Events[i].MyName);
                            if (refCommand.Ints[0] == i)
                            {
                                cmbEntities.SelectedIndex = i + 1;
                            }
                        }
                    }
                }
            }

            _editingCommand = refCommand;
            _eventEditor = eventEditor;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int slot = 0;
            if (!_editingEvent.CommonEvent)
            {
                for (int i = 0; i < _currentMap.Events.Count; i++)
                {
                    if (cmbEntities.SelectedIndex == 0)
                    {
                        _editingCommand.Ints[0] = -1;
                    }
                    else
                    {
                        if (_currentMap.Events[i].Deleted == 0)
                        {
                            slot++;
                            if (cmbEntities.SelectedIndex == slot)
                            {
                                _editingCommand.Ints[0] = i;
                                break;
                            }
                        }
                    }
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
