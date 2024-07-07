using Intersect.Editor.Content;
using Intersect.Editor.Localization;
using Intersect.GameObjects.Events.Commands;
using Intersect.Utilities;
using Graphics = System.Drawing.Graphics;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands;


public partial class EventCommandChangeSprite : UserControl
{

    private readonly FrmEvent mEventEditor;

    private ChangeSpriteCommand mMyCommand;

    public EventCommandChangeSprite(ChangeSpriteCommand refCommand, FrmEvent editor)
    {
        InitializeComponent();
        mMyCommand = refCommand;
        mEventEditor = editor;
        InitLocalization();
        cmbSprite.Items.Clear();
        cmbSprite.Items.Add(Strings.General.None);
        cmbSprite.Items.AddRange(
            GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Entity));
        cmbSprite.SelectedIndex = Math.Max(0, cmbSprite.Items.IndexOf(TextUtils.NullToNone(mMyCommand.Sprite)));
        UpdatePreview();
    }

    private void InitLocalization()
    {
        grpChangeSprite.Text = Strings.EventChangeSprite.title;
        lblSprite.Text = Strings.EventChangeSprite.label;
        btnSave.Text = Strings.EventChangeSprite.okay;
        btnCancel.Text = Strings.EventChangeSprite.cancel;
    }

    private void UpdatePreview()
    {
        var destBitmap = new Bitmap(pnlPreview.Width, pnlPreview.Height);
        var g = Graphics.FromImage(destBitmap);
        g.Clear(System.Drawing.Color.Black);
        if (File.Exists("resources/entities/" + cmbSprite.Text))
        {
            var sourceBitmap = new Bitmap("resources/entities/" + cmbSprite.Text);
            g.DrawImage(
                sourceBitmap,
                new Rectangle(
                    pnlPreview.Width / 2 - sourceBitmap.Width / (Options.Instance.Sprites.NormalFrames * 2), pnlPreview.Height / 2 - sourceBitmap.Height / (Options.Instance.Sprites.Directions * 2),
                    sourceBitmap.Width / Options.Instance.Sprites.NormalFrames, sourceBitmap.Height / Options.Instance.Sprites.Directions
                ), new Rectangle(0, 0, sourceBitmap.Width / Options.Instance.Sprites.NormalFrames, sourceBitmap.Height / Options.Instance.Sprites.Directions), GraphicsUnit.Pixel
            );
        }

        g.Dispose();
        pnlPreview.BackgroundImage = destBitmap;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        mMyCommand.Sprite = cmbSprite.Text;
        mEventEditor.FinishCommandEdit();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        mEventEditor.CancelCommandEdit();
    }

    private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdatePreview();
    }

}
