using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Variables;
using Intersect.GameObjects.Events.Commands;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;


public partial class EventCommandInput : UserControl
{

    private readonly FrmEvent mEventEditor;

    bool mLoading = false;

    private InputVariableCommand mMyCommand;

    public EventCommandInput(InputVariableCommand refCommand, FrmEvent editor)
    {
        mLoading = true;
        InitializeComponent();
        mMyCommand = refCommand;
        mEventEditor = editor;

        txtText.Text = mMyCommand.Text;
        txtTitle.Text = mMyCommand.Title;
        nudMaxVal.Value = mMyCommand.Maximum;
        nudMinVal.Value = mMyCommand.Minimum;

        if (mMyCommand.VariableType == VariableType.PlayerVariable)
        {
            rdoPlayerVariables.Checked = true;
        }
        else if (mMyCommand.VariableType == VariableType.ServerVariable)
        {
            rdoGlobalVariables.Checked = true;
        }
        else if (mMyCommand.VariableType == VariableType.GuildVariable)
        {
            rdoGuildVariables.Checked = true;
        }
        else if (mMyCommand.VariableType == VariableType.UserVariable)
        {
            rdoUserVariables.Checked = true;
        }

        LoadVariableList();
        InitLocalization();
        mLoading = false;
    }

    private void InitLocalization()
    {
        grpInput.Text = Strings.EventInput.title;
        lblText.Text = Strings.EventInput.text;
        lblTitle.Text = Strings.EventInput.titlestr;
        lblCommands.Text = Strings.EventInput.commands;
        btnSave.Text = Strings.EventInput.okay;
        btnCancel.Text = Strings.EventInput.cancel;
        rdoPlayerVariables.Text = Strings.EventInput.playervariable;
        rdoGlobalVariables.Text = Strings.EventInput.globalvariable;
        rdoGuildVariables.Text = Strings.EventInput.guildvariable;
        rdoUserVariables.Text = Strings.GameObjectStrings.UserVariable;
    }

    private void LoadVariableList()
    {
        cmbVariable.Items.Clear();
        if (rdoPlayerVariables.Checked)
        {
            cmbVariable.Items.AddRange(PlayerVariableDescriptor.Names);
            cmbVariable.SelectedIndex = PlayerVariableDescriptor.ListIndex(mMyCommand.VariableId);

            if (cmbVariable.SelectedIndex != -1)
            {
                UpdateMinMaxValues(
                    PlayerVariableDescriptor.Get(PlayerVariableDescriptor.IdFromList(cmbVariable.SelectedIndex)).DataType
                );
            }
        }
        else if (rdoGlobalVariables.Checked)
        {
            cmbVariable.Items.AddRange(ServerVariableDescriptor.Names);
            cmbVariable.SelectedIndex = ServerVariableDescriptor.ListIndex(mMyCommand.VariableId);

            if (cmbVariable.SelectedIndex != -1)
            {
                UpdateMinMaxValues(
                    ServerVariableDescriptor.Get(ServerVariableDescriptor.IdFromList(cmbVariable.SelectedIndex)).DataType
                );
            }
        }
        else if (rdoGuildVariables.Checked)
        {
            cmbVariable.Items.AddRange(GuildVariableDescriptor.Names);
            cmbVariable.SelectedIndex = GuildVariableDescriptor.ListIndex(mMyCommand.VariableId);
            if (cmbVariable.SelectedIndex != -1)
            {
                UpdateMinMaxValues(
                    GuildVariableDescriptor.Get(GuildVariableDescriptor.IdFromList(cmbVariable.SelectedIndex)).DataType
                );
            }
        }
        else if (rdoUserVariables.Checked)
        {
            cmbVariable.Items.AddRange(UserVariableDescriptor.Names);
            cmbVariable.SelectedIndex = UserVariableDescriptor.ListIndex(mMyCommand.VariableId);
            if (cmbVariable.SelectedIndex != -1 && UserVariableDescriptor.TryGet(UserVariableDescriptor.IdFromList(cmbVariable.SelectedIndex), out var userVar))
            {
                UpdateMinMaxValues(
                   userVar.DataType
                );
            }
        }
    }

    private void UpdateMinMaxValues(VariableDataType type)
    {
        lblMaxVal.Show();
        lblMinVal.Show();
        nudMaxVal.Show();
        nudMinVal.Show();

        switch (type)
        {
            case VariableDataType.Integer:
            case VariableDataType.Number:
                lblMinVal.Text = Strings.EventInput.minval;
                lblMaxVal.Text = Strings.EventInput.maxval;

                break;
            case VariableDataType.String:
                lblMinVal.Text = Strings.EventInput.minlength;
                lblMaxVal.Text = Strings.EventInput.maxlength;

                break;
            case VariableDataType.Boolean:
                lblMaxVal.Hide();
                lblMinVal.Hide();
                nudMaxVal.Hide();
                nudMinVal.Hide();

                break;
        }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        mMyCommand.Text = txtText.Text;
        mMyCommand.Title = txtTitle.Text;
        mMyCommand.Maximum = (int) nudMaxVal.Value;
        mMyCommand.Minimum = (int) nudMinVal.Value;

        if (rdoPlayerVariables.Checked)
        {
            mMyCommand.VariableType = VariableType.PlayerVariable;
            mMyCommand.VariableId = PlayerVariableDescriptor.IdFromList(cmbVariable.SelectedIndex);
        }
        else if (rdoGlobalVariables.Checked)
        {
            mMyCommand.VariableType = VariableType.ServerVariable;
            mMyCommand.VariableId = ServerVariableDescriptor.IdFromList(cmbVariable.SelectedIndex);
        }
        else if (rdoGuildVariables.Checked)
        {
            mMyCommand.VariableType = VariableType.GuildVariable;
            mMyCommand.VariableId = GuildVariableDescriptor.IdFromList(cmbVariable.SelectedIndex);
        }
        else if (rdoUserVariables.Checked)
        {
            mMyCommand.VariableType = VariableType.UserVariable;
            mMyCommand.VariableId = UserVariableDescriptor.IdFromList(cmbVariable.SelectedIndex);
        }

        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }

    private void lblCommands_Click(object sender, EventArgs e)
    {
        BrowserUtils.Open("http://www.ascensiongamedev.com/community/topic/749-event-text-variables/");
    }

    private void rdoGlobalVariables_CheckedChanged(object sender, EventArgs e)
    {
        VariableRadioChanged();
    }

    private void rdoGuildVariables_CheckedChanged(object sender, EventArgs e)
    {
        VariableRadioChanged();
    }

    private void rdoPlayerVariables_CheckedChanged(object sender, EventArgs e)
    {
        VariableRadioChanged();
    }

    private void rdoUserVariables_CheckedChanged(object sender, EventArgs e)
    {
        VariableRadioChanged();
    }

    private void VariableRadioChanged()
    {
        LoadVariableList();
        if (!mLoading && cmbVariable.Items.Count > 0)
        {
            cmbVariable.SelectedIndex = 0;
        }
    }

    private void cmbVariable_SelectedIndexChanged(object sender, EventArgs e)
    {
        var idx = cmbVariable.SelectedIndex;
        if (rdoPlayerVariables.Checked && PlayerVariableDescriptor.TryGet(PlayerVariableDescriptor.IdFromList(idx), out var playerVar))
        {
            UpdateMinMaxValues(playerVar.DataType);
        }
        else if (rdoGlobalVariables.Checked && ServerVariableDescriptor.TryGet(ServerVariableDescriptor.IdFromList(idx), out var serverVar))
        {
            UpdateMinMaxValues(serverVar.DataType);
        }
        else if (rdoGuildVariables.Checked && GuildVariableDescriptor.TryGet(GuildVariableDescriptor.IdFromList(idx), out var guildVar))
        {
            UpdateMinMaxValues(guildVar.DataType);
        }
        else if (rdoUserVariables.Checked && UserVariableDescriptor.TryGet(UserVariableDescriptor.IdFromList(idx), out var userVar))
        {
            UpdateMinMaxValues(userVar.DataType);
        }
    }

}
