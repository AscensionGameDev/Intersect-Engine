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
            Text = Strings.VariableEditor.title;
            grpTypes.Text = Strings.VariableEditor.type;
            grpList.Text = Strings.VariableEditor.list;
            rdoPlayerVariables.Text = Strings.VariableEditor.playervariables;
            rdoGlobalVariables.Text = Strings.VariableEditor.globalvariables;
            grpEditor.Text = Strings.VariableEditor.editor;
            lblName.Text = Strings.VariableEditor.name;
            grpValue.Text = Strings.VariableEditor.value;
            cmbBooleanValue.Items.Clear();
            cmbBooleanValue.Items.Add(Strings.VariableEditor.False);
            cmbBooleanValue.Items.Add(Strings.VariableEditor.True);
            cmbVariableType.Items.Clear();
            foreach (var itm in Strings.VariableEditor.types)
            {
                cmbVariableType.Items.Add(itm.Value);
            }
            btnNew.Text = Strings.VariableEditor.New;
            btnDelete.Text = Strings.VariableEditor.delete;
            btnUndo.Text = Strings.VariableEditor.undo;
            btnSave.Text = Strings.VariableEditor.save;
            btnCancel.Text = Strings.VariableEditor.cancel;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.PlayerVariable)
            {
                InitEditor();
                if (mEditorItem != null && !PlayerVariableBase.Lookup.Values.Contains(mEditorItem))
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
            if (rdoPlayerVariables.Checked)
            {
                PacketSender.SendCreateObject(GameObjectType.PlayerVariable);
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
                    DarkMessageBox.ShowWarning(Strings.VariableEditor.deleteprompt,
                        Strings.VariableEditor.deletecaption, DarkDialogButton.YesNo,
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
                if (rdoPlayerVariables.Checked)
                {
                    obj = PlayerVariableBase.Get(PlayerVariableBase.IdFromList(lstObjects.SelectedIndex));
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
            cmbBooleanValue.Hide();
            nudVariableValue.Hide();
            if (rdoPlayerVariables.Checked)
            {
                lstObjects.Items.AddRange(PlayerVariableBase.Names);
                lblId.Text = Strings.VariableEditor.textidpv;
            }
            else if (rdoGlobalVariables.Checked)
            {
                for (int i = 0; i < ServerVariableBase.Lookup.Count; i++)
                {
                    var var = ServerVariableBase.Get(ServerVariableBase.IdFromList(i));
                    lstObjects.Items.Add(var.Name + "  =  " + var.Value.StringRepresentation(var.Type));
                }
                lblId.Text = Strings.VariableEditor.textidgv;
            }
            UpdateEditor();
        }

        private void rdoPlayerVariables_CheckedChanged(object sender, EventArgs e)
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
                grpValue.Hide();
                if (rdoPlayerVariables.Checked)
                {
                    lblObject.Text = Strings.VariableEditor.playervariable;
                    txtObjectName.Text = ((PlayerVariableBase)mEditorItem).Name;
                    txtId.Text = ((PlayerVariableBase)mEditorItem).TextId;
                    cmbVariableType.SelectedIndex = (int) (((PlayerVariableBase) mEditorItem).Type - 1);
                }
                else if (rdoGlobalVariables.Checked)
                {
                    lblObject.Text = Strings.VariableEditor.globalvariable;
                    txtObjectName.Text = ((ServerVariableBase)mEditorItem).Name;
                    txtId.Text = ((ServerVariableBase)mEditorItem).TextId;
                    cmbVariableType.SelectedIndex = (int)(((ServerVariableBase)mEditorItem).Type - 1);
                    grpValue.Show();
                }

                InitValueGroup();
            }
            else
            {
                grpEditor.Hide();
            }
        }

        private void UpdateSelection()
        {
            if (lstObjects.SelectedIndex > -1)
            {
                grpEditor.Show();
                grpValue.Hide();
                if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.Get(PlayerVariableBase.IdFromList(lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] = obj.Name;
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get(ServerVariableBase.IdFromList(lstObjects.SelectedIndex));
                    lstObjects.Items[lstObjects.SelectedIndex] = obj.Name + "  =  " + obj.Value.StringRepresentation(obj.Type);
                }
            }
        }

        private void txtObjectName_TextChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.Get(PlayerVariableBase.IdFromList(lstObjects.SelectedIndex));
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
                if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.Get(PlayerVariableBase.IdFromList(lstObjects.SelectedIndex));
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
                        obj.Value.Integer = (long) nudVariableValue.Value;
                        UpdateSelection();
                    }
                }
            }
        }

        private void cmbVariableType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoPlayerVariables.Checked)
                {
                    var obj = PlayerVariableBase.Get(PlayerVariableBase.IdFromList(lstObjects.SelectedIndex));
                    obj.Type = (VariableDataTypes) (cmbVariableType.SelectedIndex + 1);
                }
                else if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get(ServerVariableBase.IdFromList(lstObjects.SelectedIndex));
                    obj.Type = (VariableDataTypes)(cmbVariableType.SelectedIndex + 1);
                }
                InitValueGroup();
                UpdateSelection();
            }
        }

        private void InitValueGroup()
        {
            if (rdoPlayerVariables.Checked)
            {
                grpValue.Hide();
            }
            else
            {
                if (lstObjects.SelectedIndex > -1)
                {
                    var obj = ServerVariableBase.Get(ServerVariableBase.IdFromList(lstObjects.SelectedIndex));
                    cmbBooleanValue.Hide();
                    nudVariableValue.Hide();
                    switch (obj.Type)
                    {
                        case VariableDataTypes.Boolean:
                            cmbBooleanValue.Show();
                            cmbBooleanValue.SelectedIndex = Convert.ToInt32(obj.Value.Boolean);
                            break;

                        case VariableDataTypes.Integer:
                            nudVariableValue.Show();
                            nudVariableValue.Value = obj.Value.Integer;
                            break;

                        case VariableDataTypes.Number:
                            break;

                        case VariableDataTypes.String:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private void cmbBooleanValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoGlobalVariables.Checked)
                {
                    var obj = ServerVariableBase.Get(ServerVariableBase.IdFromList(lstObjects.SelectedIndex));
                    if (obj != null)
                    {
                        obj.Value.Boolean = Convert.ToBoolean(cmbBooleanValue.SelectedIndex);
                        UpdateSelection();
                    }
                }
            }
        }
    }
}