using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect;
using Intersect.GameObjects;
using Intersect.Localization;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms.Editors
{
    public partial class frmSwitchVariable : Form
    {
        private List<DatabaseObject> _changed = new List<DatabaseObject>();
        private DatabaseObject _editorItem = null;

        public frmSwitchVariable()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
            InitLocalization();
        }

        private void InitLocalization()
        {
            Text = Strings.Get("switchvariableeditor", "title");
            grpTypes.Text = Strings.Get("switchvariableeditor", "type");
            grpList.Text = Strings.Get("switchvariableeditor", "list");
            rdoPlayerSwitch.Text = Strings.Get("switchvariableeditor", "playerswitches");
            rdoPlayerVariables.Text = Strings.Get("switchvariableeditor", "playervariables");
            rdoGlobalSwitches.Text = Strings.Get("switchvariableeditor", "globalswitches");
            rdoGlobalVariables.Text = Strings.Get("switchvariableeditor", "globalvariables");
            grpEditor.Text = Strings.Get("switchvariableeditor", "editor");
            lblName.Text = Strings.Get("switchvariableeditor", "name");
            lblValue.Text = Strings.Get("switchvariableeditor", "value");
            cmbSwitchValue.Items.Clear();
            cmbSwitchValue.Items.Add(Strings.Get("switchvariableeditor", "false"));
            cmbSwitchValue.Items.Add(Strings.Get("switchvariableeditor", "true"));
            btnNew.Text = Strings.Get("switchvariableeditor", "new");
            btnDelete.Text = Strings.Get("switchvariableeditor", "delete");
            btnUndo.Text = Strings.Get("switchvariableeditor", "undo");
            btnSave.Text = Strings.Get("switchvariableeditor", "save");
            btnCancel.Text = Strings.Get("switchvariableeditor", "cancel");
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.PlayerSwitch)
            {
                InitEditor();
                if (_editorItem != null && !PlayerSwitchBase.Lookup.Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
            else if (type == GameObject.PlayerVariable)
            {
                InitEditor();
                if (_editorItem != null && !PlayerVariableBase.Lookup.Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
            else if (type == GameObject.ServerSwitch)
            {
                InitEditor();
                if (_editorItem != null && !ServerSwitchBase.Lookup.Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
            else if (type == GameObject.ServerVariable)
            {
                InitEditor();
                if (_editorItem != null && !ServerVariableBase.Lookup.Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (rdoPlayerSwitch.Checked)
            {
                PacketSender.SendCreateObject(GameObject.PlayerSwitch);
            }
            else if (rdoPlayerVariables.Checked)
            {
                PacketSender.SendCreateObject(GameObject.PlayerVariable);
            }
            else if (rdoGlobalSwitches.Checked)
            {
                PacketSender.SendCreateObject(GameObject.ServerSwitch);
            }
            else if (rdoGlobalVariables.Checked)
            {
                PacketSender.SendCreateObject(GameObject.ServerVariable);
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                _editorItem.RestoreBackup();
                UpdateEditor();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null)
            {
                if (
                    DarkMessageBox.ShowWarning(Strings.Get("switchvariableeditor", "deleteprompt"),
                        Strings.Get("switchvariableeditor", "deletecaption"), DarkDialogButton.YesNo,
                        Properties.Resources.Icon) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (var item in _changed)
            {
                item.RestoreBackup();
                item.DeleteBackup();
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Send Changed items
            foreach (var item in _changed)
            {
                PacketSender.SendSaveObject(item);
                item.DeleteBackup();
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void lstObjects_Click(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                DatabaseObject obj = null;
                if (rdoPlayerSwitch.Checked)
                {
                    obj =
                        PlayerSwitchBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.PlayerSwitch,
                            lstObjects.SelectedIndex));
                }
                else if (rdoPlayerVariables.Checked)
                {
                    obj =
                        PlayerVariableBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.PlayerVariable,
                            lstObjects.SelectedIndex));
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    obj =
                        ServerSwitchBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.ServerSwitch,
                            lstObjects.SelectedIndex));
                }
                else if (rdoGlobalVariables.Checked)
                {
                    obj =
                        ServerVariableBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.ServerVariable,
                            lstObjects.SelectedIndex));
                }
                if (obj != null)
                {
                    _editorItem = obj;
                    if (!_changed.Contains(obj))
                    {
                        _changed.Add(obj);
                        obj.MakeBackup();
                    }
                }
            }
            UpdateEditor();
        }

        public void InitEditor()
        {
            lstObjects.Items.Clear();
            grpEditor.Hide();
            cmbSwitchValue.Hide();
            txtVariableVal.Hide();
            if (rdoPlayerSwitch.Checked)
            {
                lstObjects.Items.AddRange(Database.GetGameObjectList(GameObject.PlayerSwitch));
            }
            else if (rdoPlayerVariables.Checked)
            {
                lstObjects.Items.AddRange(Database.GetGameObjectList(GameObject.PlayerVariable));
            }
            else if (rdoGlobalSwitches.Checked)
            {
                for (int i = 0; i < ServerSwitchBase.Lookup.Count; i++)
                {
                    var swtch = ServerSwitchBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.ServerSwitch, i));
                    lstObjects.Items.Add(swtch.Name + "  =  " + swtch.Value);
                }
            }
            else if (rdoGlobalVariables.Checked)
            {
                for (int i = 0; i < ServerVariableBase.Lookup.Count; i++)
                {
                    var var = ServerVariableBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.ServerVariable, i));
                    lstObjects.Items.Add(var.Name + "  =  " + var.Value);
                }
            }
            UpdateEditor();
        }

        private void frmSwitchVariable_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void rdoPlayerSwitch_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem = null;
            InitEditor();
        }

        private void rdoPlayerVariables_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem = null;
            InitEditor();
        }

        private void rdoGlobalSwitches_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem = null;
            InitEditor();
        }

        private void rdoGlobalVariables_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem = null;
            InitEditor();
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                grpEditor.Show();
                lblValue.Hide();
                if (rdoPlayerSwitch.Checked)
                {
                    lblObject.Text = Strings.Get("switchvariableeditor", "playerswitch");
                    txtObjectName.Text = ((PlayerSwitchBase) _editorItem).Name;
                }
                else if (rdoPlayerVariables.Checked)
                {
                    lblObject.Text = Strings.Get("switchvariableeditor", "playervariable");
                    txtObjectName.Text = ((PlayerVariableBase) _editorItem).Name;
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    lblObject.Text = Strings.Get("switchvariableeditor", "globalswitch");
                    txtObjectName.Text = ((ServerSwitchBase) _editorItem).Name;
                    cmbSwitchValue.Show();
                    cmbSwitchValue.SelectedIndex =
                        cmbSwitchValue.Items.IndexOf(((ServerSwitchBase) _editorItem).Value.ToString());
                }
                else if (rdoGlobalVariables.Checked)
                {
                    lblObject.Text = Strings.Get("switchvariableeditor", "globalvariable");
                    txtObjectName.Text = ((ServerVariableBase) _editorItem).Name;
                    txtVariableVal.Show();
                    txtVariableVal.Text = ((ServerVariableBase) _editorItem).Value.ToString();
                }
            }
            else
            {
                grpEditor.Hide();
            }
        }

        private void cmbSwitchValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoGlobalSwitches.Checked)
                {
                    var obj =
                        ServerSwitchBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.ServerSwitch,
                            lstObjects.SelectedIndex));
                    obj.Value = Convert.ToBoolean(cmbSwitchValue.SelectedIndex);
                    UpdateSelection();
                }
            }
        }

        private void UpdateSelection()
        {
            if (lstObjects.SelectedIndex > -1)
            {
                grpEditor.Show();
                lblValue.Hide();
                if (rdoPlayerSwitch.Checked)
                {
                    var obj =
                        PlayerSwitchBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.PlayerSwitch,
                            lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] = obj.Name;
                }
                else if (rdoPlayerVariables.Checked)
                {
                    var obj =
                        PlayerVariableBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.PlayerVariable,
                            lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] = obj.Name;
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    var obj =
                        ServerSwitchBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.ServerSwitch,
                            lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] = obj.Name + "  =  " + obj.Value;
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj =
                        ServerVariableBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.ServerVariable,
                            lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] = obj.Name + "  =  " + obj.Value;
                }
            }
        }

        private void txtVariableVal_TextChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoGlobalVariables.Checked)
                {
                    if (int.TryParse(txtVariableVal.Text, out int readInt))
                    {
                        var obj =
                            ServerVariableBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.ServerVariable,
                                lstObjects.SelectedIndex));
                        obj.Value = readInt;
                        UpdateSelection();
                    }
                }
            }
        }

        private void txtObjectName_TextChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoPlayerSwitch.Checked)
                {
                    var obj =
                        PlayerSwitchBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.PlayerSwitch,
                            lstObjects.SelectedIndex));
                    obj.Name = txtObjectName.Text;
                }
                else if (rdoPlayerVariables.Checked)
                {
                    var obj =
                        PlayerVariableBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.PlayerVariable,
                            lstObjects.SelectedIndex));
                    obj.Name = txtObjectName.Text;
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    var obj =
                        ServerSwitchBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.ServerSwitch,
                            lstObjects.SelectedIndex));
                    obj.Name = txtObjectName.Text;
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj =
                        ServerVariableBase.Lookup.Get(Database.GameObjectIdFromList(GameObject.ServerVariable,
                            lstObjects.SelectedIndex));
                    obj.Name = txtObjectName.Text;
                }
                UpdateSelection();
            }
        }
    }
}