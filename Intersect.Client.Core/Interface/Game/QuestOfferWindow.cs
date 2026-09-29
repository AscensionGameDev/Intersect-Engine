using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Configuration;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.GameObjects;

namespace Intersect.Client.Interface.Game;


public partial class QuestOfferWindow
{
    private const int MaxRewardSlots = 8;

    private const int RewardSlotSize = 36;

    private const int RewardSlotSpacing = 6;

    private Button mAcceptButton;

    private Button mDeclineButton;

    private string mQuestOfferText = string.Empty;

    //Controls
    private WindowControl mQuestOfferWindow;

    private ScrollControl mQuestPromptArea;

    private RichLabel mQuestPromptLabel;

    private Label mQuestPromptTemplate;

    private Label mQuestTitle;

    private Label mRewardHeaderLabel;

    private readonly ImagePanel[] mRewardIcons = new ImagePanel[MaxRewardSlots];

    private readonly Label[] mRewardQuantityLabels = new Label[MaxRewardSlots];

    private readonly Guid[] mRewardItemIds = new Guid[MaxRewardSlots];

    private readonly int[] mRewardQuantities = new int[MaxRewardSlots];

    private static ItemDescriptor mCachedCurrencyItem;

    public QuestOfferWindow(Canvas gameCanvas)
    {
        mQuestOfferWindow = new WindowControl(gameCanvas, Strings.QuestOffer.Title, false, "QuestOfferWindow");
        mQuestOfferWindow.DisableResizing();
        mQuestOfferWindow.IsClosable = false;

        //Menu Header
        mQuestTitle = new Label(mQuestOfferWindow, "QuestTitle");

        mQuestPromptArea = new ScrollControl(mQuestOfferWindow, "QuestOfferArea");

        mQuestPromptTemplate = new Label(mQuestPromptArea, "QuestOfferTemplate");

        mQuestPromptLabel = new RichLabel(mQuestPromptArea);

        //Accept Button
        mAcceptButton = new Button(mQuestOfferWindow, "AcceptButton");
        mAcceptButton.SetText(Strings.QuestOffer.Accept);
        mAcceptButton.Clicked += _acceptButton_Clicked;

        //Decline Button
        mDeclineButton = new Button(mQuestOfferWindow, "DeclineButton");
        mDeclineButton.SetText(Strings.QuestOffer.Decline);
        mDeclineButton.Clicked += _declineButton_Clicked;

        mQuestOfferWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        Interface.InputBlockingComponents.Add(mQuestOfferWindow);

        // mQuestPromptArea/mAcceptButton/mDeclineButton get reparented into the WindowControl's InnerPanel by
        // LoadJsonUi, so the reward row needs to be attached there too to share the same coordinate space
        // (rather than mQuestOfferWindow's own space, which is offset by the titlebar height).
        var contentParent = mQuestPromptArea.Parent ?? mQuestOfferWindow;

        // Shrink the scrollable description area to make room for a reward preview row above the buttons.
        mQuestPromptArea.SetBounds(mQuestPromptArea.X, mQuestPromptArea.Y, mQuestPromptArea.Width, 258);

        mRewardHeaderLabel = new Label(contentParent, "QuestRewardHeader");
        mRewardHeaderLabel.SetBounds(mQuestPromptArea.X, mQuestPromptArea.Y + 262, mQuestPromptArea.Width, 16);
        mRewardHeaderLabel.SetText(Strings.QuestOffer.Rewards);
        mRewardHeaderLabel.SetTextColor(new Color(255, 230, 230, 230), Framework.Gwen.Control.ComponentState.Normal);
        mRewardHeaderLabel.IsHidden = true;

        for (var i = 0; i < MaxRewardSlots; i++)
        {
            var slotIndex = i;

            var icon = new ImagePanel(contentParent, $"QuestRewardIcon{i}");
            icon.SetBounds(
                mQuestPromptArea.X + i * (RewardSlotSize + RewardSlotSpacing),
                mQuestPromptArea.Y + 282,
                RewardSlotSize,
                RewardSlotSize
            );
            icon.IsHidden = true;
            icon.HoverEnter += (sender, arguments) => RewardIcon_HoverEnter(slotIndex);
            icon.HoverLeave += (sender, arguments) => Interface.GameUi.ItemDescriptionWindow?.Hide();
            mRewardIcons[i] = icon;

            var quantityLabel = new Label(contentParent, $"QuestRewardQuantity{i}");
            quantityLabel.SetBounds(
                mQuestPromptArea.X + i * (RewardSlotSize + RewardSlotSpacing),
                mQuestPromptArea.Y + 282 + RewardSlotSize - 14,
                RewardSlotSize,
                14
            );
            quantityLabel.TextAlign = Framework.Gwen.Pos.Right | Framework.Gwen.Pos.Bottom;
            quantityLabel.SetTextColor(new Color(255, 255, 255, 255), Framework.Gwen.Control.ComponentState.Normal);
            quantityLabel.IsHidden = true;
            mRewardQuantityLabels[i] = quantityLabel;
        }
    }

    private void RewardIcon_HoverEnter(int slotIndex)
    {
        if (mRewardItemIds[slotIndex] == Guid.Empty)
        {
            return;
        }

        if (!ItemDescriptor.TryGet(mRewardItemIds[slotIndex], out var itemDescriptor))
        {
            return;
        }

        Interface.GameUi.ItemDescriptionWindow?.Show(itemDescriptor, mRewardQuantities[slotIndex]);
    }

    private static bool TryGetCurrencyItem(out ItemDescriptor currencyItem)
    {
        if (mCachedCurrencyItem == null)
        {
            foreach (var obj in ItemDescriptor.Lookup.Values)
            {
                if (obj is ItemDescriptor { ItemType: ItemType.Currency } item)
                {
                    mCachedCurrencyItem = item;
                    break;
                }
            }
        }

        currencyItem = mCachedCurrencyItem;
        return currencyItem != null;
    }

    private void SetRewardSlot(int slotIndex, Guid itemId, int quantity)
    {
        if (!ItemDescriptor.TryGet(itemId, out var itemDescriptor))
        {
            HideRewardSlot(slotIndex);
            return;
        }

        var texture = Globals.ContentManager?.GetTexture(TextureType.Item, itemDescriptor.Icon);

        mRewardIcons[slotIndex].Texture = texture;
        mRewardIcons[slotIndex].RenderColor = itemDescriptor.Color;
        mRewardIcons[slotIndex].IsHidden = false;

        mRewardItemIds[slotIndex] = itemId;
        mRewardQuantities[slotIndex] = quantity;

        mRewardQuantityLabels[slotIndex].Text = quantity > 1 ? $"x{quantity}" : string.Empty;
        mRewardQuantityLabels[slotIndex].IsHidden = false;
    }

    private void HideRewardSlot(int slotIndex)
    {
        mRewardIcons[slotIndex].IsHidden = true;
        mRewardQuantityLabels[slotIndex].IsHidden = true;
        mRewardItemIds[slotIndex] = Guid.Empty;
        mRewardQuantities[slotIndex] = 0;
    }

    private void UpdateRewardPreview(QuestDescriptor quest)
    {
        var slotIndex = 0;

        if (quest.RewardCurrency > 0 && TryGetCurrencyItem(out var currencyItem))
        {
            var amount = quest.RewardCurrency > int.MaxValue ? int.MaxValue : (int)quest.RewardCurrency;
            SetRewardSlot(slotIndex++, currencyItem.Id, amount);
        }

        if (quest.RewardItems != null)
        {
            foreach (var rewardItem in quest.RewardItems)
            {
                if (slotIndex >= MaxRewardSlots)
                {
                    break;
                }

                SetRewardSlot(slotIndex++, rewardItem.ItemId, rewardItem.Quantity);
            }
        }

        for (var i = slotIndex; i < MaxRewardSlots; i++)
        {
            HideRewardSlot(i);
        }

        mRewardHeaderLabel.IsHidden = slotIndex == 0;
    }

    private void _declineButton_Clicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.QuestOffers.Count > 0)
        {
            PacketSender.SendDeclineQuest(Globals.QuestOffers[0]);
            Globals.QuestOffers.RemoveAt(0);
        }
    }

    private void _acceptButton_Clicked(Base sender, MouseButtonState arguments)
    {
        if (Globals.QuestOffers.Count > 0)
        {
            PacketSender.SendAcceptQuest(Globals.QuestOffers[0]);
            Globals.QuestOffers.RemoveAt(0);
        }
    }

    public void Update(QuestDescriptor quest)
    {
        if (quest == null)
        {
            Hide();
        }
        else
        {
            Show();
            var preferEnglish = string.Equals(ClientConfiguration.Instance.Language, "en", StringComparison.OrdinalIgnoreCase);
            var startDescription = quest.GetLocalizedStartDescription(preferEnglish);
            mQuestTitle.Text = quest.GetLocalizedName(preferEnglish);
            if (mQuestOfferText != startDescription)
            {
                mQuestPromptLabel.ClearText();
                mQuestPromptLabel.Width = mQuestPromptArea.Width - mQuestPromptArea.VerticalScrollBar.Width;
                mQuestPromptLabel.AddText(startDescription, mQuestPromptTemplate);

                mQuestPromptLabel.SizeToChildren(false, true);
                mQuestOfferText = startDescription;
            }

            UpdateRewardPreview(quest);
        }
    }

    public void Show()
    {
        mQuestOfferWindow.IsHidden = false;
    }

    public void Close()
    {
        mQuestOfferWindow.Close();
    }

    public bool IsVisible()
    {
        return !mQuestOfferWindow.IsHidden;
    }

    public void Hide()
    {
        mQuestOfferWindow.IsHidden = true;
    }

}
