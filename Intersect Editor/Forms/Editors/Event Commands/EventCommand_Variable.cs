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
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.Localization;


namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class EventCommand_Variable : UserControl
    {
        private EventCommand _myCommand;
        private readonly FrmEvent _eventEditor;
        private bool _loading = false;
        public EventCommand_Variable(EventCommand refCommand, FrmEvent editor)
        {
            InitializeComponent();
            _myCommand = refCommand;
            _eventEditor = editor;
            _loading = true;
            InitLocalization();
            if (_myCommand.Ints[0] == (int) SwitchVariableTypes.ServerVariable)
            {
                rdoGlobalVariable.Checked = true;
            }
            else
            {
                rdoPlayerVariable.Checked = true;
            }
            _loading = false;
            InitEditor();
        }

        private void InitLocalization()
        {
            grpSetVariable.Text = Strings.Get("eventsetvariable", "title");
            grpVariableSelection.Text = Strings.Get("eventsetvariable", "label");
            lblVariable.Text = Strings.Get("eventsetvariable", "label");
            rdoGlobalVariable.Text = Strings.Get("eventsetvariable", "global");
            rdoPlayerVariable.Text = Strings.Get("eventsetvariable", "player");
            optSet.Text = Strings.Get("eventsetvariable", "set");
            optAdd.Text = Strings.Get("eventsetvariable", "add");
            optSubtract.Text = Strings.Get("eventsetvariable", "subtract");
            optRandom.Text = Strings.Get("eventsetvariable", "random");
            lblRandomLow.Text = Strings.Get("eventsetvariable", "randomlow");
            lblRandomHigh.Text = Strings.Get("eventsetvariable", "randomhigh");
            btnSave.Text = Strings.Get("eventsetvariable", "okay");
            btnCancel.Text = Strings.Get("eventsetvariable", "cancel");
        }

        private void InitEditor()
        {
            cmbVariable.Items.Clear();
            int varCount = 0;
            if (rdoPlayerVariable.Checked)
            {
                cmbVariable.Items.AddRange(Database.GetGameObjectList(GameObject.PlayerVariable));
                cmbVariable.SelectedIndex = Database.GameObjectListIndex(GameObject.PlayerVariable, _myCommand.Ints[1]);
            }
            else
            {
                cmbVariable.Items.AddRange(Database.GetGameObjectList(GameObject.ServerVariable));
                cmbVariable.SelectedIndex = Database.GameObjectListIndex(GameObject.ServerVariable, _myCommand.Ints[1]);
            }
            switch (_myCommand.Ints[2])
            {
                case 0:
                    optSet.Checked = true;
                    txtSet.Text = _myCommand.Ints[3].ToString();
                    break;
                case 1:
                    optAdd.Checked = true;
                    txtAdd.Text = _myCommand.Ints[3].ToString();
                    break;
                case 2:
                    optSubtract.Checked = true;
                    txtSubtract.Text = _myCommand.Ints[3].ToString();
                    break;
                case 3:
                    optRandom.Checked = true;
                    txtRandomLow.Text = _myCommand.Ints[3].ToString();
                    txtRandomHigh.Text = _myCommand.Ints[4].ToString();
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
            if (rdoPlayerVariable.Checked)
            {
                _myCommand.Ints[0] = (int) SwitchVariableTypes.PlayerVariable;
                _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.PlayerVariable,
                        cmbVariable.SelectedIndex);
            }
            if (rdoGlobalVariable.Checked)
            {
                _myCommand.Ints[0] = (int) SwitchVariableTypes.ServerVariable;
                _myCommand.Ints[1] = Database.GameObjectIdFromList(GameObject.ServerVariable,
                        cmbVariable.SelectedIndex);
            }
            if (optSet.Checked)
            {
                _myCommand.Ints[2] = 0;
                if (int.TryParse(txtSet.Text, out n))
                {
                    _myCommand.Ints[3] = n;
                }
                else
                {
                    _myCommand.Ints[3] = 0;
                }
            }
            else if (optAdd.Checked)
            {
                _myCommand.Ints[2] = 1;
                if (int.TryParse(txtAdd.Text, out n))
                {
                    _myCommand.Ints[3] = n;
                }
                else
                {
                    _myCommand.Ints[3] = 0;
                }
            }
            else if (optSubtract.Checked)
            {
                _myCommand.Ints[2] = 2;
                if (int.TryParse(txtSubtract.Text, out n))
                {
                    _myCommand.Ints[3] = n;
                }
                else
                {
                    _myCommand.Ints[3] = 0;
                }
            }
            else if (optRandom.Checked)
            {
                _myCommand.Ints[2] = 3;
                if (int.TryParse(txtRandomLow.Text, out n))
                {
                    _myCommand.Ints[3] = n;
                }
                else
                {
                    _myCommand.Ints[3] = 0;
                }
                if (int.TryParse(txtRandomHigh.Text, out n))
                {
                    _myCommand.Ints[4] = n;
                }
                else
                {
                    _myCommand.Ints[4] = 0;
                }
                if (_myCommand.Ints[4] < _myCommand.Ints[3])
                {
                    n = _myCommand.Ints[3];
                    _myCommand.Ints[3] = _myCommand.Ints[4];
                    _myCommand.Ints[4] = n;
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

        private void rdoPlayerVariable_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!_loading && cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
            if (!_loading) optSet.Checked = true;
            if (!_loading) txtSet.Text = "0";
        }

        private void rdoGlobalVariable_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
            if (!_loading && cmbVariable.Items.Count > 0) cmbVariable.SelectedIndex = 0;
            if (!_loading) optSet.Checked = true;
            if (!_loading) txtSet.Text = "0";
        }
    }
}
