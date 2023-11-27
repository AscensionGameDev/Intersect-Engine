using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Intersect.Editor.Forms.DockingElements;
using Intersect.Editor.Localization;
using Intersect.Enums;
using Intersect.Extensions;
using Intersect.GameObjects;
using Intersect.GameObjects.Annotations;

namespace Intersect.Editor.Forms;

public partial class FrmVariableSelector : Form
{
    private Guid mSelectedVariableId { get; set; }

    private VariableType mSelectedVariableType { get; set; }

    private bool mResult { get; set; }

    private bool mPopulating { get; set; }

    private VariableDataType mFilterType { get; set; }

    private VariableSelection mSelection { get; set; }

    public FrmVariableSelector(VariableType variableType, Guid variableId, VariableDataType filterType)
    {
        PreInit();

        mSelectedVariableId = variableId;
        mSelectedVariableType = variableType;
        mFilterType = filterType;

        PostInit();
    }

    public FrmVariableSelector()
    {
        PreInit();
        PostInit();
    }

    private void PreInit()
    {
        InitializeComponent();
    }

    private void PostInit()
    {
        mPopulating = true;
        Icon = Program.Icon;

        InitLocalization();

        PopulateForm();
    }

    private void InitLocalization()
    {
        Text = Strings.VariableSelector.Title;

        grpSelection.Text = Strings.VariableSelector.LabelGroup;
        grpVariableType.Text = Strings.VariableSelector.LabelVariableType;
        grpVariable.Text = Strings.VariableSelector.LabelVariableValue;

        btnOk.Text = Strings.General.Okay;
        btnCancel.Text = Strings.General.Cancel;

        cmbVariableType.Items.Clear();

        cmbVariableType.Items.AddRange(Strings.VariableSelector.VariableTypes.Values.ToArray());
    }

    private void PopulateForm()
    {
        cmbVariableType.SelectedIndex = (int)mSelectedVariableType;

        ReloadVariablesOf(mSelectedVariableType);

        cmbVariables.SelectedIndex = mSelectedVariableType.GetRelatedTable().ListIndex(mSelectedVariableId, mFilterType);

        mPopulating = false;
    }

    private void ReloadVariablesOf(VariableType type)
    {
        cmbVariables.Items.Clear();

        cmbVariables.Items.AddRange(type.GetRelatedTable().Names(mFilterType));
    }

    private void cmbVariableType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (mPopulating)
        {
            return;
        }

        mSelectedVariableType = (VariableType)cmbVariableType.SelectedIndex;
        ReloadVariablesOf(mSelectedVariableType);

        // Force reset the variable selection
        if (cmbVariables.Items.Count > 0)
        {
            cmbVariables.SelectedIndex = 0;
            mSelectedVariableId = mSelectedVariableType.GetRelatedTable().IdFromList(0, mFilterType);
        }
        else
        {
            cmbVariables.SelectedIndex = -1;
            mSelectedVariableId = Guid.Empty;
        }
    }

    public bool GetResult()
    {
        return mResult;
    }

    public VariableSelection GetSelection()
    {
        return mSelection;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        mResult = true;
        mSelection = new VariableSelection(mSelectedVariableType, mSelectedVariableId);
        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void cmbVariables_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (mPopulating)
        {
            return;
        }

        mSelectedVariableId = mSelectedVariableType.GetRelatedTable().IdFromList(cmbVariables.SelectedIndex, mFilterType);
    }
}

public class VariableSelection
{
    public VariableSelection(VariableType variableType, Guid variableId)
    {
        VariableType = variableType;
        VariableId = variableId;
    }

    public VariableType VariableType { get; set; }

    public Guid VariableId { get; set; }
}