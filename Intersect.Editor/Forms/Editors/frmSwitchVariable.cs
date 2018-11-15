using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Models;

namespace Intersect.Editor.Forms.Editors
{
    public partial class FrmSwitchVariable : EditorForm
    {
        private List<IDatabaseObject> mChanged = new List<IDatabaseObject>();
        private IDatabaseObject mEditorItem;

        public FrmSwitchVariable()
        {
            ApplyHooks();
            InitializeComponent();
            InitLocalization();
            nudVariableValue.Minimum = long.MinValue;
            nudVariableValue.Maximum = long.MaxValue;
        }

        private void InitLocalization()
        {
            Text = Strings.SwitchVariableEditor.title;
            grpTypes.Text = Strings.SwitchVariableEditor.type;
            grpList.Text = Strings.SwitchVariableEditor.list;
            rdoPlayerSwitch.Text = Strings.SwitchVariableEditor.playerswitches;
            rdoPlayerVariables.Text = Strings.SwitchVariableEditor.playervariables;
            rdoGlobalSwitches.Text = Strings.SwitchVariableEditor.globalswitches;
            rdoGlobalVariables.Text = Strings.SwitchVariableEditor.globalvariables;
            grpEditor.Text = Strings.SwitchVariableEditor.editor;
            lblName.Text = Strings.SwitchVariableEditor.name;
            lblValue.Text = Strings.SwitchVariableEditor.value;
            cmbSwitchValue.Items.Clear();
            cmbSwitchValue.Items.Add(Strings.SwitchVariableEditor.False);
            cmbSwitchValue.Items.Add(Strings.SwitchVariableEditor.True);
            btnNew.Text = Strings.SwitchVariableEditor.New;
            btnDelete.Text = Strings.SwitchVariableEditor.delete;
            btnUndo.Text = Strings.SwitchVariableEditor.undo;
            btnSave.Text = Strings.SwitchVariableEditor.save;
            btnCancel.Text = Strings.SwitchVariableEditor.cancel;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.PlayerSwitch)
            {
                InitEditor();
                if (mEditorItem != null && !PlayerSwitchBase.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
            }
            else if (type == GameObjectType.PlayerVariable)
            {
                InitEditor();
                if (mEditorItem != null && !PlayerVariableBase.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
            }
            else if (type == GameObjectType.ServerSwitch)
            {
                InitEditor();
                if (mEditorItem != null && !ServerSwitchBase.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
            }
            else if (type == GameObjectType.ServerVariable)
            {
                InitEditor();
                if (mEditorItem != null && !ServerVariableBase.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (rdoPlayerSwitch.Checked)
            {
                PacketSender.SendCreateObject(GameObjectType.PlayerSwitch);
            }
            else if (rdoPlayerVariables.Checked)
            {
                PacketSender.SendCreateObject(GameObjectType.PlayerVariable);
            }
            else if (rdoGlobalSwitches.Checked)
            {
                PacketSender.SendCreateObject(GameObjectType.ServerSwitch);
            }
            else if (rdoGlobalVariables.Checked)
            {
                PacketSender.SendCreateObject(GameObjectType.ServerVariable);
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                mEditorItem.RestoreBackup();
                UpdateEditor();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null)
            {
                if (
                    DarkMessageBox.ShowWarning(Strings.SwitchVariableEditor.deleteprompt,
                        Strings.SwitchVariableEditor.deletecaption, DarkDialogButton.YesNo,
                        Properties.Resources.Icon) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(mEditorItem);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (var item in mChanged)
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
            foreach (var item in mChanged)
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
                IDatabaseObject obj = null;
                if (rdoPlayerSwitch.Checked)
                {
                    obj = PlayerSwitchBase.Get(PlayerSwitchBase.IdFromList(lstObjects.SelectedIndex));
                }
                else if (rdoPlayerVariables.Checked)
                {
                    obj = PlayerVariableBase.Get(PlayerVariableBase.IdFromList(lstObjects.SelectedIndex));
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    obj = ServerSwitchBase.Get(ServerSwitchBase.IdFromList(lstObjects.SelectedIndex));
                }
                else if (rdoGlobalVariables.Checked)
                {
                    obj = ServerVariableBase.Get(ServerVariableBase.IdFromList(lstObjects.SelectedIndex));
                }
                if (obj != null)
                {
                    mEditorItem = obj;
                    if (!mChanged.Contains(obj))
                    {
                        mChanged.Add(obj);
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
            nudVariableValue.Hide();
            if (rdoPlayerSwitch.Checked)
            {
                lstObjects.Items.AddRange(PlayerSwitchBase.Names);
                lblId.Text = Strings.SwitchVariableEditor.textidps;
            }
            else if (rdoPlayerVariables.Checked)
            {
                lstObjects.Items.AddRange(PlayerVariableBase.Names);
                lblId.Text = Strings.SwitchVariableEditor.textidpv;
            }
            else if (rdoGlobalSwitches.Checked)
            {
                for (int i = 0; i < ServerSwitchBase.Lookup.Count; i++)
                {
                    var swtch = ServerSwitchBase.Get(ServerSwitchBase.IdFromList(i));
                    lstObjects.Items.Add(swtch.Name + "  =  " + swtch.Value);
                    lblId.Text = Strings.SwitchVariableEditor.textidgs;
                }
            }
            else if (rdoGlobalVariables.Checked)
            {
                for (int i = 0; i < ServerVariableBase.Lookup.Count; i++)
                {
                    var var = ServerVariableBase.Get(ServerVariableBase.IdFromList(i));
                    lstObjects.Items.Add(var.Name + "  =  " + var.Value);
                    lblId.Text = Strings.SwitchVariableEditor.textidgv;
                }
            }
            UpdateEditor();
        }

        private void frmSwitchVariable_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void rdoPlayerSwitch_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem = null;
            InitEditor();
        }

        private void rdoPlayerVariables_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem = null;
            InitEditor();
        }

        private void rdoGlobalSwitches_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem = null;
            InitEditor();
        }

        private void rdoGlobalVariables_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem = null;
            InitEditor();
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                grpEditor.Show();
                lblValue.Hide();
                if (rdoPlayerSwitch.Checked)
                {
                    lblObject.Text = Strings.SwitchVariableEditor.playerswitch;
                    txtObjectName.Text = ((PlayerSwitchBase)mEditorItem).Name;
                    txtId.Text = ((PlayerSwitchBase) mEditorItem).TextId;
                }
                else if (rdoPlayerVariables.Checked)
                {
                    lblObject.Text = Strings.SwitchVariableEditor.playervariable;
                    txtObjectName.Text = ((PlayerVariableBase)mEditorItem).Name;
                    txtId.Text = ((PlayerVariableBase)mEditorItem).TextId;
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    lblObject.Text = Strings.SwitchVariableEditor.globalswitch;
                    txtObjectName.Text = ((ServerSwitchBase)mEditorItem).Name;
                    txtId.Text = ((ServerSwitchBase)mEditorItem).TextId;
                    cmbSwitchValue.Show();
                    cmbSwitchValue.SelectedIndex =
                        cmbSwitchValue.Items.IndexOf(((ServerSwitchBase)mEditorItem).Value.ToString());
                    lblValue.Show();
                }
                else if (rdoGlobalVariables.Checked)
                {
                    lblObject.Text = Strings.SwitchVariableEditor.globalvariable;
                    txtObjectName.Text = ((ServerVariableBase)mEditorItem).Name;
                    txtId.Text = ((ServerVariableBase)mEditorItem).TextId;
                    nudVariableValue.Show();
                    nudVariableValue.Value = ((ServerVariableBase) mEditorItem).Value;
                    lblValue.Show();
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
                    var obj = ServerSwitchBase.Get(ServerSwitchBase.IdFromList(lstObjects.SelectedIndex));
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
                    var obj = PlayerSwitchBase.Get(PlayerSwitchBase.IdFromList(lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] = obj.Name;
                }
                else if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.Get(PlayerVariableBase.IdFromList(lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] = obj.Name;
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    var obj = ServerSwitchBase.Get(ServerSwitchBase.IdFromList(lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] = obj.Name + "  =  " + obj.Value;
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get(ServerVariableBase.IdFromList(lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] = obj.Name + "  =  " + obj.Value;
                }
            }
        }

        private void txtObjectName_TextChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoPlayerSwitch.Checked)
                {
                    var obj = PlayerSwitchBase.Get(PlayerSwitchBase.IdFromList(lstObjects.SelectedIndex));
                    obj.Name = txtObjectName.Text;
                }
                else if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.Get(PlayerVariableBase.IdFromList(lstObjects.SelectedIndex));
                    obj.Name = txtObjectName.Text;
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    var obj =ServerSwitchBase.Get(ServerSwitchBase.IdFromList(lstObjects.SelectedIndex));
                    obj.Name = txtObjectName.Text;
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj =ServerVariableBase.Get(ServerVariableBase.IdFromList(lstObjects.SelectedIndex));
                    obj.Name = txtObjectName.Text;
                }
                UpdateSelection();
            }
        }

        private void txtId_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar == ' ';
        }

        private void txtId_TextChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoPlayerSwitch.Checked)
                {
                    var obj = PlayerSwitchBase.Get(PlayerSwitchBase.IdFromList(lstObjects.SelectedIndex));
                    obj.TextId = txtId.Text;
                }
                else if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.Get(PlayerVariableBase.IdFromList(lstObjects.SelectedIndex));
                    obj.TextId = txtId.Text;
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    var obj = ServerSwitchBase.Get(ServerSwitchBase.IdFromList(lstObjects.SelectedIndex));
                    obj.TextId = txtId.Text;
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get(ServerVariableBase.IdFromList(lstObjects.SelectedIndex));
                    obj.TextId = txtId.Text;
                }
            }
        }

        private void nudVariableValue_ValueChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get(ServerVariableBase.IdFromList(lstObjects.SelectedIndex));
                    if (obj != null)
                    {
                        obj.Value = (long) nudVariableValue.Value;
                        UpdateSelection();
                    }
                }
            }
        }
    }
}