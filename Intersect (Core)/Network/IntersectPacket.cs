using System;
using System.Collections.Generic;

using Intersect.Collections;
using Intersect.Network.Packets;
using Intersect.Network.Packets.Client;
using Intersect.Network.Packets.Server;
using MessagePack;

namespace Intersect.Network
{
    [Union(0, typeof(ConnectionPacket))]
    [Union(1, typeof(AbstractTimedPacket))]
    [Union(2, typeof(EditorPacket))]
    [Union(3, typeof(EntityPacket))]
    [Union(4, typeof(SlotQuantityPacket))]
    [Union(5, typeof(SlotSwapPacket))]
    [Union(6, typeof(AbandonQuestPacket))]
    [Union(7, typeof(AcceptTradePacket))]
    [Union(8, typeof(ActivateEventPacket))]
    [Union(9, typeof(AdminActionPacket))]
    [Union(10, typeof(BlockPacket))]
    [Union(11, typeof(BumpPacket))]
    [Union(12, typeof(CloseBagPacket))]
    [Union(13, typeof(CloseBankPacket))]
    [Union(14, typeof(CloseCraftingPacket))]
    [Union(15, typeof(CloseShopPacket))]
    [Union(16, typeof(CraftItemPacket))]
    [Union(17, typeof(CreateAccountPacket))]
    [Union(18, typeof(CreateCharacterPacket))]
    [Union(19, typeof(DeclineTradePacket))]
    [Union(20, typeof(DeleteCharacterPacket))]
    [Union(21, typeof(DirectionPacket))]
    [Union(22, typeof(EnterGamePacket))]
    [Union(23, typeof(EventInputVariablePacket))]
    [Union(24, typeof(EventResponsePacket))]
    [Union(25, typeof(ForgetSpellPacket))]
    [Union(26, typeof(FriendRequestResponsePacket))]
    [Union(27, typeof(HotbarUpdatePacket))]
    [Union(28, typeof(LoginPacket))]
    [Union(29, typeof(LogoutPacket))]
    [Union(30, typeof(NeedMapPacket))]
    [Union(31, typeof(NewCharacterPacket))]
    [Union(32, typeof(OpenAdminWindowPacket))]
    [Union(33, typeof(PartyInviteResponsePacket))]
    [Union(34, typeof(PartyKickPacket))]
    [Union(35, typeof(PartyLeavePacket))]
    [Union(36, typeof(PickupItemPacket))]
    [Union(37, typeof(QuestResponsePacket))]
    [Union(38, typeof(RequestFriendsPacket))]
    [Union(39, typeof(RequestPasswordResetPacket))]
    [Union(40, typeof(ResetPasswordPacket))]
    [Union(41, typeof(SelectCharacterPacket))]
    [Union(42, typeof(UnequipItemPacket))]
    [Union(43, typeof(UpdateFriendsPacket))]
    [Union(44, typeof(UpgradeStatPacket))]
    [Union(45, typeof(UseItemPacket))]
    [Union(46, typeof(UseSpellPacket))]
    [Union(47, typeof(LoginPacket))]
    [Union(48, typeof(TradeRequestResponsePacket))]
    [Union(49, typeof(Packets.Client.ChatMsgPacket))]
    [Union(50, typeof(Packets.Server.ChatMsgPacket))]
    [Union(51, typeof(Packets.Client.TradeRequestPacket))]
    [Union(52, typeof(Packets.Client.PartyInvitePacket))]
    [Union(53, typeof(Packets.Server.PartyInvitePacket))]
    [Union(54, typeof(Packets.Server.TradeRequestPacket))]
    [Union(55, typeof(Packets.Client.PingPacket))]
    [Union(56, typeof(ActionMsgPacket))]
    [Union(57, typeof(AdminPanelPacket))]
    [Union(58, typeof(AnnouncementPacket))]
    [Union(59, typeof(BagPacket))]
    [Union(60, typeof(BankPacket))]
    [Union(61, typeof(CancelCastPacket))]
    [Union(62, typeof(CharacterCreationPacket))]
    [Union(63, typeof(CharacterPacket))]
    [Union(64, typeof(CharactersPacket))]
    [Union(65, typeof(ChatBubblePacket))]
    [Union(66, typeof(ConfigPacket))]
    [Union(67, typeof(CraftingTablePacket))]
    [Union(68, typeof(EnteringGamePacket))]
    [Union(69, typeof(EnterMapPacket))]
    [Union(70, typeof(EntityDashPacket))]
    [Union(71, typeof(EntityDiePacket))]
    [Union(72, typeof(EntityDirectionPacket))]
    [Union(73, typeof(EntityLeftPacket))]
    [Union(74, typeof(EntityMovePacket))]
    [Union(75, typeof(EntityStatsPacket))]
    [Union(76, typeof(EntityVitalsPacket))]
    [Union(77, typeof(EntityZDimensionPacket))]
    [Union(78, typeof(EquipmentPacket))]
    [Union(79, typeof(ErrorMessagePacket))]
    [Union(80, typeof(EventDialogPacket))]
    [Union(81, typeof(ExperiencePacket))]
    [Union(82, typeof(FriendRequestPacket))]
    [Union(83, typeof(FriendsPacket))]
    [Union(84, typeof(GameDataPacket))]
    [Union(85, typeof(GameObjectPacket))]
    [Union(86, typeof(HidePicturePacket))]
    [Union(87, typeof(HoldPlayerPacket))]
    [Union(88, typeof(HotbarPacket))]
    [Union(89, typeof(InputVariablePacket))]
    [Union(90, typeof(InventoryPacket))]
    [Union(91, typeof(InventoryUpdatePacket))]
    [Union(92, typeof(ItemCooldownPacket))]
    [Union(93, typeof(LabelPacket))]
    [Union(94, typeof(MapAreaPacket))]
    [Union(95, typeof(MapEntitiesPacket))]
    [Union(96, typeof(MapEntityStatusPacket))]
    [Union(97, typeof(MapEntityVitalsPacket))]
    [Union(98, typeof(MapGridPacket))]
    [Union(99, typeof(MapItemsPacket))]
    [Union(100, typeof(MapItemUpdatePacket))]
    [Union(101, typeof(MapListPacket))]
    [Union(102, typeof(MapPacket))]
    [Union(103, typeof(MoveRoutePacket))]
    [Union(104, typeof(NpcAggressionPacket))]
    [Union(105, typeof(OpenEditorPacket))]
    [Union(106, typeof(PartyMemberPacket))]
    [Union(107, typeof(PartyPacket))]
    [Union(108, typeof(PartyUpdatePacket))]
    [Union(109, typeof(PasswordResetResultPacket))]
    [Union(110, typeof(PlayAnimationPacket))]
    [Union(111, typeof(PlayerDeathPacket))]
    [Union(112, typeof(PlayMusicPacket))]
    [Union(113, typeof(PlaySoundPacket))]
    [Union(114, typeof(ProjectileDeadPacket))]
    [Union(115, typeof(QuestOfferPacket))]
    [Union(116, typeof(QuestProgressPacket))]
    [Union(117, typeof(ShopPacket))]
    [Union(118, typeof(ShowPicturePacket))]
    [Union(119, typeof(SpellCastPacket))]
    [Union(120, typeof(SpellCooldownPacket))]
    [Union(121, typeof(SpellPacket))]
    [Union(122, typeof(SpellsPacket))]
    [Union(123, typeof(SpellUpdatePacket))]
    [Union(124, typeof(StatPointsPacket))]
    [Union(125, typeof(StatusPacket))]
    [Union(126, typeof(StopMusicPacket))]
    [Union(127, typeof(StopSoundsPacket))]
    [Union(128, typeof(TargetOverridePacket))]
    [Union(129, typeof(TimeDataPacket))]
    [Union(130, typeof(TimePacket))]
    [Union(131, typeof(TradePacket))]
    [MessagePackObject]
    public abstract class IntersectPacket : IPacket
    {
        [IgnoreMember]
        private byte[] mCachedData = null;

        [IgnoreMember]
        private byte[] mCachedCompresedData = null;

        /// <inheritdoc />
        public virtual void Dispose()
        {
        }

        /// <inheritdoc />
        [IgnoreMember]
        public virtual byte[] Data
        {
            get
            {
                if (mCachedData == null)
                    mCachedData = MessagePacker.Instance.Serialize(this) ?? throw new Exception("Failed to serialize packet.");

                return mCachedData;
            }
        }

        public virtual void ClearCachedData()
        {
            mCachedData = null;
        }

        [IgnoreMember]
        public virtual bool IsValid => true;
        [IgnoreMember]
        public virtual long ReceiveTime { get; set; }
        [IgnoreMember]
        public virtual long ProcessTime { get; set; }

        /// <inheritdoc />
        public virtual Dictionary<string, SanitizedValue<object>> Sanitize()
        {
            return null;
        }

    }

}
