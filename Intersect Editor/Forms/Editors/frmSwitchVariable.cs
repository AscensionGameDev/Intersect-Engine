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


namespace Intersect_Editor.Forms.Editors
{
    public partial class frmSwitchVariable : Form
    {
        private string[] _playerSwitchBackup = new string[Options.MaxPlayerSwitches];
        private string[] _playerVariableBackup = new string[Options.MaxPlayerVariables];
        private string[] _serverSwitchBackup = new string[Options.MaxServerSwitches];
        private string[] _serverVariableBackup = new string[Options.MaxServerVariables];
        private int[] _serverVariableValBackup = new int[Options.MaxServerVariables];
        private bool[] _serverSwitchValBackup = new bool[Options.MaxServerSwitches];
        public frmSwitchVariable()
        {
            InitializeComponent();
            for (int i = 0; i < Options.MaxPlayerSwitches; i++)
            {
                _playerSwitchBackup[i] = Globals.PlayerSwitches[i];
            }
            for (int i = 0; i < Options.MaxPlayerVariables; i++)
            {
                _playerVariableBackup[i] = Globals.PlayerVariables[i];
            }
            for (int i = 0; i < Options.MaxServerSwitches; i++)
            {
                _serverSwitchBackup[i] = Globals.ServerSwitches[i];
                _serverSwitchValBackup[i] = Globals.ServerSwitchValues[i];
            }
            for (int i = 0; i < Options.MaxServerVariables; i++)
            {
                _serverVariableBackup[i] = Globals.ServerVariables[i];
                _serverVariableValBackup[i] = Globals.ServerVariableValues[i];
            }
        }

        public void InitEditor()
        {
            lstObjects.Items.Clear();
            grpEditor.Hide();
            if (rdoPlayerSwitch.Checked)
            {
                for (int i = 0; i < Options.MaxPlayerSwitches; i++)
                {
                    lstObjects.Items.Add((i + 1) + ". " + Globals.PlayerSwitches[i]);
                }
            }
            else if (rdoPlayerVariables.Checked)
            {
                for (int i = 0; i < Options.MaxPlayerVariables; i++)
                {
                    lstObjects.Items.Add((i + 1) + ". " + Globals.PlayerVariables[i]);
                }
            }
            else if (rdoGlobalSwitches.Checked)
            {
                for (int i = 0; i < Options.MaxServerSwitches; i++)
                {
                    lstObjects.Items.Add((i + 1) + ". " + Globals.ServerSwitches[i] + "  =  " + Globals.ServerSwitchValues[i].ToString());
                }
            }
            else if (rdoGlobalVariables.Checked)
            {
                for (int i = 0; i < Options.MaxServerVariables; i++)
                {
                    lstObjects.Items.Add((i + 1) + ". " + Globals.ServerVariables[i] + "  =  " + Globals.ServerVariableValues[i].ToString());
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Figure out what has changed and create massive packet of changes for server.
            ByteBuffer packetBuffer = new ByteBuffer();
            ByteBuffer dataBuffer = new ByteBuffer();
            int changeCount = 0;
            for (int i = 0; i < Options.MaxPlayerSwitches; i++)
            {
                if (Globals.PlayerSwitches[i] != _playerSwitchBackup[i])
                {
                    dataBuffer.WriteInteger((int) SwitchVariableTypes.PlayerSwitch);
                    dataBuffer.WriteInteger(i);
                    dataBuffer.WriteString(Globals.PlayerSwitches[i]);
                    changeCount++;
                }
            }
            for (int i = 0; i < Options.MaxPlayerVariables; i++)
            {
                if (Globals.PlayerVariables[i] != _playerVariableBackup[i])
                {
                    dataBuffer.WriteInteger((int)SwitchVariableTypes.PlayerVariable);
                    dataBuffer.WriteInteger(i);
                    dataBuffer.WriteString(Globals.PlayerVariables[i]);
                    changeCount++;
                }
            }
            for (int i = 0; i < Options.MaxServerSwitches; i++)
            {
                if (Globals.ServerSwitches[i] != _serverSwitchBackup[i] ||
                    Globals.ServerSwitchValues[i] != _serverSwitchValBackup[i])
                {
                    dataBuffer.WriteInteger((int)SwitchVariableTypes.ServerSwitch);
                    dataBuffer.WriteInteger(i);
                    dataBuffer.WriteString(Globals.ServerSwitches[i]);
                    dataBuffer.WriteInteger(Convert.ToInt32(Globals.ServerSwitchValues[i]));
                    changeCount++;
                }
            }
            for (int i = 0; i < Options.MaxServerVariables; i++)
            {
                if (Globals.ServerVariables[i] != _serverVariableBackup[i] ||
                    Globals.ServerVariableValues[i] != _serverVariableValBackup[i])
                {
                    dataBuffer.WriteInteger((int)SwitchVariableTypes.ServerVariable);
                    dataBuffer.WriteInteger(i);
                    dataBuffer.WriteString(Globals.ServerVariables[i]);
                    dataBuffer.WriteInteger(Globals.ServerVariableValues[i]);
                    changeCount++;
                }
            }
            packetBuffer.WriteLong((int)ClientPackets.SaveSwitchVariable);
            packetBuffer.WriteInteger(changeCount);
            packetBuffer.WriteBytes(dataBuffer.ToArray());
            dataBuffer.Dispose();
            Network.SendPacket(packetBuffer.ToArray());
            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Options.MaxPlayerSwitches; i++)
            {
                Globals.PlayerSwitches[i] = _playerSwitchBackup[i];
            }
            for (int i = 0; i < Options.MaxPlayerVariables; i++)
            {
                Globals.PlayerVariables[i] = _playerVariableBackup[i];
            }
            for (int i = 0; i < Options.MaxServerSwitches; i++)
            {
                Globals.ServerSwitches[i] = _serverSwitchBackup[i];
                Globals.ServerSwitchValues[i] = _serverSwitchValBackup[i];
            }
            for (int i = 0; i < Options.MaxServerVariables; i++)
            {
                Globals.ServerVariables[i] = _serverVariableBackup[i];
                Globals.ServerVariableValues[i] = _serverVariableValBackup[i];
            }
            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void frmSwitchVariable_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void rdoPlayerSwitch_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
        }

        private void rdoPlayerVariables_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
        }

        private void rdoGlobalSwitches_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
        }

        private void rdoGlobalVariables_CheckedChanged(object sender, EventArgs e)
        {
            InitEditor();
        }

        private void lstObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            grpEditor.Hide();
            if (lstObjects.SelectedIndex > -1)
            {
                grpEditor.Show();
                lblValue.Hide();
                cmbSwitchValue.Hide();
                txtVariableVal.Hide();
                if (rdoPlayerSwitch.Checked)
                {
                    lblObject.Text = "Player Switch #" + (lstObjects.SelectedIndex + 1);
                    txtObjectName.Text = Globals.PlayerSwitches[lstObjects.SelectedIndex];
                }
                else if (rdoPlayerVariables.Checked)
                {
                    lblObject.Text = "Player Variable #" + (lstObjects.SelectedIndex + 1);
                    txtObjectName.Text = Globals.PlayerVariables[lstObjects.SelectedIndex];
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    lblObject.Text = "Server Switch #" + (lstObjects.SelectedIndex + 1);
                    txtObjectName.Text = Globals.ServerSwitches[lstObjects.SelectedIndex];
                    cmbSwitchValue.Show();
                    cmbSwitchValue.SelectedIndex =
                        cmbSwitchValue.Items.IndexOf(Globals.ServerSwitchValues[lstObjects.SelectedIndex].ToString());
                }
                else if (rdoGlobalVariables.Checked)
                {
                    lblObject.Text = "Server Variable #" + (lstObjects.SelectedIndex + 1);
                    txtObjectName.Text = Globals.ServerVariables[lstObjects.SelectedIndex];
                    txtVariableVal.Show();
                    txtVariableVal.Text = Globals.ServerVariableValues[lstObjects.SelectedIndex].ToString();
                }
            }
        }

        private void cmbSwitchValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoGlobalSwitches.Checked)
                {
                    Globals.ServerSwitchValues[lstObjects.SelectedIndex] = Convert.ToBoolean(cmbSwitchValue.SelectedIndex);
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
                cmbSwitchValue.Hide();
                txtVariableVal.Hide();
                if (rdoPlayerSwitch.Checked)
                {
                    lstObjects.Items[lstObjects.SelectedIndex] = (lstObjects.SelectedIndex + 1) + ". " + Globals.PlayerSwitches[lstObjects.SelectedIndex];
                }
                else if (rdoPlayerVariables.Checked)
                {
                    lstObjects.Items[lstObjects.SelectedIndex] = (lstObjects.SelectedIndex + 1) + ". " + Globals.PlayerVariables[lstObjects.SelectedIndex];
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    lstObjects.Items[lstObjects.SelectedIndex] = (lstObjects.SelectedIndex + 1) + ". " + Globals.ServerSwitches[lstObjects.SelectedIndex] + "  =  " + Globals.ServerSwitchValues[lstObjects.SelectedIndex];
                }
                else if (rdoGlobalVariables.Checked)
                {
                    lstObjects.Items[lstObjects.SelectedIndex] = (lstObjects.SelectedIndex + 1) + ". " + Globals.ServerVariables[lstObjects.SelectedIndex] + "  =  " + Globals.ServerVariableValues[lstObjects.SelectedIndex];
                }
            }
        }

        private void txtVariableVal_TextChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                if (rdoGlobalVariables.Checked)
                {
                    int readInt = 0;
                    if (int.TryParse(txtVariableVal.Text, out readInt))
                    {
                        Globals.ServerVariableValues[lstObjects.SelectedIndex] = readInt;
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
                    Globals.PlayerSwitches[lstObjects.SelectedIndex] = txtObjectName.Text;
                }
                else if (rdoPlayerVariables.Checked)
                {
                    Globals.PlayerVariables[lstObjects.SelectedIndex] = txtObjectName.Text;
                }
                else if (rdoGlobalSwitches.Checked)
                {
                    Globals.ServerSwitches[lstObjects.SelectedIndex] = txtObjectName.Text;
                }
                else if (rdoGlobalVariables.Checked)
                {
                    Globals.ServerVariables[lstObjects.SelectedIndex] = txtObjectName.Text;
                }
                UpdateSelection();
            }
        }
    }
}
