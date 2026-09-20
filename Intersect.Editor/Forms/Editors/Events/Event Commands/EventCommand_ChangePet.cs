using Intersect.Editor.Localization;
using Intersect.Framework.Core.GameObjects.Events.Commands;
using Intersect.Framework.Core.GameObjects.Pets;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;


public partial class EventCommandChangePet : UserControl
{

    private readonly FrmEvent mEventEditor;

    private ChangePetCommand mMyCommand;

    public EventCommandChangePet(ChangePetCommand refCommand, FrmEvent editor)
    {
        InitializeComponent();
        mMyCommand = refCommand;
        mEventEditor = editor;
        InitLocalization();

        // First entry dismisses the current pet, the rest are the pets defined in the Pet Editor.
        cmbPet.Items.Clear();
        cmbPet.Items.Add(Strings.EventChangePet.none);
        cmbPet.Items.AddRange(PetDescriptor.Names);
        cmbPet.SelectedIndex = mMyCommand.PetId == Guid.Empty ? 0 : PetDescriptor.ListIndex(mMyCommand.PetId) + 1;
        if (cmbPet.SelectedIndex < 0)
        {
            cmbPet.SelectedIndex = 0;
        }
    }

    private void InitLocalization()
    {
        grpPet.Text = Strings.EventChangePet.title;
        lblPet.Text = Strings.EventChangePet.label;
        btnSave.Text = Strings.EventChangePet.okay;
        btnCancel.Text = Strings.EventChangePet.cancel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        mMyCommand.PetId = cmbPet.SelectedIndex > 0 ? PetDescriptor.IdFromList(cmbPet.SelectedIndex - 1) : Guid.Empty;

        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }

}
