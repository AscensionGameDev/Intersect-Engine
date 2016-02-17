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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_ConditionalBranch : UserControl
    {
        private EventCommand _myCommand;
        private EventPage _currentPage;
        private readonly FrmEvent _eventEditor;
        public EventCommand_ConditionalBranch(EventCommand refCommand, EventPage refPage, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            _currentPage = refPage;
            cmbConditionType.SelectedIndex = _myCommand.Ints[0];
            UpdateFormElements();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Player Switch
                    cmbSwitch.SelectedIndex = _myCommand.Ints[1];
                    cmbSwitchVal.SelectedIndex = _myCommand.Ints[2];
                    break;
                case 1: //Player Variable
                    cmbVariable.SelectedIndex = _myCommand.Ints[1];
                    cmbVariableMod.SelectedIndex = _myCommand.Ints[2];
                    txtVariableVal.Text = _myCommand.Ints[3].ToString();
                    break;
                case 2: //Has Item
                    cmbItem.SelectedIndex = _myCommand.Ints[1];
                    scrlItemQuantity.Value = _myCommand.Ints[2];
                    lblItemQuantity.Text = @"Has at least: " + scrlItemQuantity.Value;
                    break;
                case 3: //Class Is
                    cmbClass.SelectedIndex = _myCommand.Ints[1];
                    break;
                case 4: //Knows spell
                    cmbSpell.SelectedIndex = _myCommand.Ints[1];
                    break;
                case 5: //Level is...
                    cmbLevelComparator.SelectedIndex = _myCommand.Ints[1];
                    scrlLevel.Value = _myCommand.Ints[2];
                    lblLevel.Text = @"Level: " + scrlLevel.Value;
                    break;
                case 6: //Self Switch
                    cmbSelfSwitch.SelectedIndex = _myCommand.Ints[1];
                    cmbSelfSwitchVal.SelectedIndex = _myCommand.Ints[2];
                    break;
            }
        }

        private void UpdateFormElements()
        {
            grpSwitch.Hide();
            grpPlayerVariable.Hide();
            grpHasItem.Hide();
            grpSpell.Hide();
            grpClass.Hide();
            grpLevel.Hide();
            grpSelfSwitch.Hide();
            switch (cmbConditionType.SelectedIndex)
            {
                case 0: //Player Switch
                    grpSwitch.Show();
                    cmbSwitch.Items.Clear();
                    for (int i = 0; i < Constants.SwitchCount; i++)
                    {
                        cmbSwitch.Items.Add("Switch #" + (i + 1));
                    }
                    cmbSwitch.SelectedIndex = 0;
                    cmbSwitchVal.SelectedIndex = 0;
                    break;
                case 1: //Player Variables
                    grpPlayerVariable.Show();
                    cmbVariable.Items.Clear();
                    for (int i = 0; i < Constants.VariableCount; i++)
                    {
                        cmbVariable.Items.Add("Variable #" + (i + 1));
                    }
                    cmbVariable.SelectedIndex = 0;
                    cmbVariableMod.SelectedIndex = 0;
                    txtVariableVal.Text = @"0";
                    break;
                case 2: //Has Item
                    grpHasItem.Show();
                    cmbItem.Items.Clear();
                    for (int i = 0; i < Globals.GameItems.Count(); i++)
                    {
                        cmbItem.Items.Add((i + 1) + ". " + Globals.GameItems[i].Name);
                    }
                    cmbItem.SelectedIndex = 0;
                    scrlItemQuantity.Value = 1;
                    break;
                case 3: //Class is
                    grpClass.Show();
                    cmbClass.Items.Clear();
                    for (int i = 0; i < Globals.GameClasses.Count(); i++)
                    {
                        cmbClass.Items.Add((i + 1) + ". " + Globals.GameClasses[i].Name);
                    }
                    cmbClass.SelectedIndex = 0;
                    break;
                case 4: //Knows spell
                    grpSpell.Show();
                    cmbSpell.Items.Clear();
                    for (int i = 0; i < Globals.GameSpells.Count(); i++)
                    {
                        cmbSpell.Items.Add((i + 1) + ". " + Globals.GameSpells[i].Name);
                    }
                    cmbSpell.SelectedIndex = 0;
                    break;
                case 5: //Level is...
                    grpLevel.Show();
                    cmbLevelComparator.SelectedIndex = 0;
                    scrlLevel.Value = 0;
                    lblLevel.Text = @"Level: " + scrlLevel.Value;
                    break;
                case 6: //Self Switch
                    grpSelfSwitch.Show();
                    cmbSelfSwitch.SelectedIndex = 0;
                    cmbSelfSwitchVal.SelectedIndex = 0;
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int n;
            if (_currentPage.Conditions.IndexOf(_myCommand) == -1)
            {
                if (_myCommand.Ints[4] == 0)
                // command.Ints[4 & 5] are reserved for referencing which command list the true/false braches follow
                {
                    for (var i = 0; i < 2; i++)
                    {
                        _currentPage.CommandLists.Add(new CommandList());
                        _myCommand.Ints[4 + i] = _currentPage.CommandLists.Count - 1;
                    }
                }
            }

            _myCommand.Ints[0] = cmbConditionType.SelectedIndex;
            switch (_myCommand.Ints[0])
            {
                case 0: //Player Switch
                    _myCommand.Ints[1] = cmbSwitch.SelectedIndex;
                    _myCommand.Ints[2] = cmbSwitchVal.SelectedIndex;
                    break;
                case 1: //Player Variable
                    _myCommand.Ints[1] = cmbVariable.SelectedIndex;
                    _myCommand.Ints[2] = cmbVariableMod.SelectedIndex;
                    if (int.TryParse(txtVariableVal.Text, out n))
                    {
                        _myCommand.Ints[3] = n;
                    }
                    else
                    {
                        _myCommand.Ints[3] = 0;
                    }
                    break;
                case 2: //Has Item
                    _myCommand.Ints[1] = cmbItem.SelectedIndex;
                    _myCommand.Ints[2] = scrlItemQuantity.Value;
                    break;
                case 3: //Class Is
                    _myCommand.Ints[1] = cmbClass.SelectedIndex;
                    break;
                case 4: //Knows spell
                    _myCommand.Ints[1] = cmbSpell.SelectedIndex;
                    break;
                case 5: //Level is
                    _myCommand.Ints[1] = cmbLevelComparator.SelectedIndex;
                    _myCommand.Ints[2] = scrlLevel.Value;
                    break;
                case 6: //Self Switch
                    _myCommand.Ints[1] = cmbSelfSwitch.SelectedIndex;
                    _myCommand.Ints[2] = cmbSelfSwitchVal.SelectedIndex;
                    break;
            }
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_currentPage.Conditions.IndexOf(_myCommand) > -1)
            {
                _currentPage.Conditions.Remove(_myCommand);
            }
            _eventEditor.CancelCommandEdit();
        }

        private void cmbConditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void scrlItemQuantity_Scroll(object sender, ScrollEventArgs e)
        {
            lblItemQuantity.Text = @"Has at least: " + scrlItemQuantity.Value;
        }

        private void scrlLevel_Scroll(object sender, ScrollEventArgs e)
        {
            lblLevel.Text = @"Level: " + scrlLevel.Value;
        }
    }
}
