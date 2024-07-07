using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.GameObjects;
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
            cmbVariable.Items.AddRange(PlayerVariableBase.Names);
            cmbVariable.SelectedIndex = PlayerVariableBase.ListIndex(mMyCommand.VariableId);

            if (cmbVariable.SelectedIndex != -1)
            {
                UpdateMinMaxValues(
                    PlayerVariableBase.Get(PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex)).Type
                );
            }
        }
        else if (rdoGlobalVariables.Checked)
        {
            cmbVariable.Items.AddRange(ServerVariableBase.Names);
            cmbVariable.SelectedIndex = ServerVariableBase.ListIndex(mMyCommand.VariableId);

            if (cmbVariable.SelectedIndex != -1)
            {
                UpdateMinMaxValues(
                    ServerVariableBase.Get(ServerVariableBase.IdFromList(cmbVariable.SelectedIndex)).Type
                );
            }
        }
        else if (rdoGuildVariables.Checked)
        {
            cmbVariable.Items.AddRange(GuildVariableBase.Names);
            cmbVariable.SelectedIndex = GuildVariableBase.ListIndex(mMyCommand.VariableId);
            if (cmbVariable.SelectedIndex != -1)
            {
                UpdateMinMaxValues(
                    GuildVariableBase.Get(GuildVariableBase.IdFromList(cmbVariable.SelectedIndex)).Type
                );
            }
        }
        else if (rdoUserVariables.Checked)
        {
            cmbVariable.Items.AddRange(UserVariableBase.Names);
            cmbVariable.SelectedIndex = UserVariableBase.ListIndex(mMyCommand.VariableId);
            if (cmbVariable.SelectedIndex != -1 && UserVariableBase.TryGet(UserVariableBase.IdFromList(cmbVariable.SelectedIndex), out var userVar))
            {
                UpdateMinMaxValues(
                   userVar.Type
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
            mMyCommand.VariableId = PlayerVariableBase.IdFromList(cmbVariable.SelectedIndex);
        }
        else if (rdoGlobalVariables.Checked)
        {
            mMyCommand.VariableType = VariableType.ServerVariable;
            mMyCommand.VariableId = ServerVariableBase.IdFromList(cmbVariable.SelectedIndex);
        }
        else if (rdoGuildVariables.Checked)
        {
            mMyCommand.VariableType = VariableType.GuildVariable;
            mMyCommand.VariableId = GuildVariableBase.IdFromList(cmbVariable.SelectedIndex);
        }
        else if (rdoUserVariables.Checked)
        {
            mMyCommand.VariableType = VariableType.UserVariable;
            mMyCommand.VariableId = UserVariableBase.IdFromList(cmbVariable.SelectedIndex);
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
        if (rdoPlayerVariables.Checked && PlayerVariableBase.TryGet(PlayerVariableBase.IdFromList(idx), out var playerVar))
        {
            UpdateMinMaxValues(playerVar.Type);
        }
        else if (rdoGlobalVariables.Checked && ServerVariableBase.TryGet(ServerVariableBase.IdFromList(idx), out var serverVar))
        {
            UpdateMinMaxValues(serverVar.Type);
        }
        else if (rdoGuildVariables.Checked && GuildVariableBase.TryGet(GuildVariableBase.IdFromList(idx), out var guildVar))
        {
            UpdateMinMaxValues(guildVar.Type);
        }
        else if (rdoUserVariables.Checked && UserVariableBase.TryGet(UserVariableBase.IdFromList(idx), out var userVar))
        {
            UpdateMinMaxValues(userVar.Type);
        }
    }

}
