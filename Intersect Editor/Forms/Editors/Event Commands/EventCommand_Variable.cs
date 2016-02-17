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
    public partial class EventCommand_Variable : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        public EventCommand_Variable(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            cmbVariable.Items.Clear();
            for (int i = 0; i < Constants.VariableCount; i++)
            {
                cmbVariable.Items.Add("Variable #" + (i + 1));
            }
            cmbVariable.SelectedIndex = _myCommand.Ints[0];
            switch (_myCommand.Ints[1])
            {
                case 0:
                    optSet.Checked = true;
                    txtSet.Text = _myCommand.Ints[2].ToString();
                    break;
                case 1:
                    optAdd.Checked = true;
                    txtAdd.Text = _myCommand.Ints[2].ToString();
                    break;
                case 2:
                    optSubtract.Checked = true;
                    txtSubtract.Text = _myCommand.Ints[2].ToString();
                    break;
                case 3:
                    optRandom.Checked = true;
                    txtRandomLow.Text = _myCommand.Ints[2].ToString();
                    txtRandomHigh.Text = _myCommand.Ints[3].ToString();
                    break;
            }
            UpdateFormElements();
        }

        private void UpdateFormElements()
        {
            txtSet.Enabled = optSet.Checked;
            txtAdd.Enabled = optAdd.Checked;
            txtSubtract.Enabled = optSubtract.Checked;
            txtRandomLow.Enabled = optRandom.Checked;
            txtRandomHigh.Enabled = optRandom.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int n;
            _myCommand.Ints[0] = cmbVariable.SelectedIndex;
            if (optSet.Checked)
            {
                _myCommand.Ints[1] = 0;
                if (int.TryParse(txtSet.Text, out n))
                {
                    _myCommand.Ints[2] = n;
                }
                else
                {
                    _myCommand.Ints[2] = 0;
                }
            }
            else if (optAdd.Checked)
            {
                _myCommand.Ints[1] = 1;
                if (int.TryParse(txtAdd.Text, out n))
                {
                    _myCommand.Ints[2] = n;
                }
                else
                {
                    _myCommand.Ints[2] = 0;
                }
            }
            else if (optSubtract.Checked)
            {
                _myCommand.Ints[1] = 2;
                if (int.TryParse(txtSubtract.Text, out n))
                {
                    _myCommand.Ints[2] = n;
                }
                else
                {
                    _myCommand.Ints[2] = 0;
                }
            }
            else if (optRandom.Checked)
            {
                _myCommand.Ints[1] = 3;
                if (int.TryParse(txtRandomLow.Text, out n))
                {
                    _myCommand.Ints[2] = n;
                }
                else
                {
                    _myCommand.Ints[2] = 0;
                }
                if (int.TryParse(txtRandomHigh.Text, out n))
                {
                    _myCommand.Ints[3] = n;
                }
                else
                {
                    _myCommand.Ints[3] = 0;
                }
                if (_myCommand.Ints[3] < _myCommand.Ints[2])
                {
                    n = _myCommand.Ints[2];
                    _myCommand.Ints[2] = _myCommand.Ints[3];
                    _myCommand.Ints[3] = n;
                }
            }
            _eventEditor.FinishCommandEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _eventEditor.CancelCommandEdit();
        }

        private void optSet_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void optAdd_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void optSubtract_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }

        private void optRandom_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormElements();
        }
    }
}
