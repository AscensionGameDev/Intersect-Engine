using Intersect.Config.Guilds;
using Intersect.Enums;
using Intersect.GameObjects;
using System;
using System.Collections.Generic;

namespace Intersect.Client.Framework.Entities
{
    public interface IPlayer : IEntity
    {
        Guid Class { get; }
        long Experience { get; }
        long ExperienceToNextLevel { get; }
        int StatPoints { get; }
        bool IsInParty { get; }
        IReadOnlyList<IPartyMember> PartyMembers { get; }
        long CombatTimer { get; }
        Guid TargetIndex { get; }
        TargetTypes TargetType { get; }
        IReadOnlyList<IFriendInstance> Friends { get; }
        IReadOnlyList<IHotbarInstance> HotbarSlots { get; }
        IReadOnlyDictionary<Guid, long> ItemCooldowns { get; }
        IReadOnlyDictionary<Guid, long> SpellCooldowns { get; }
        IReadOnlyDictionary<Guid, QuestProgress> QuestProgress { get; }
        Guid[] HiddenQuests { get; }
        bool IsInGuild { get; }
        string GuildName { get; }
        GuildRank GuildRank { get; }

        void AddToHotbar(byte hotbarSlot, sbyte itemType, int itemSlot);
        void AutoTarget();
        void ClearTarget();
        int FindHotbarItem(IHotbarInstance hotbarInstance);
        int FindHotbarSpell(IHotbarInstance hotbarInstance);
        int FindItem(Guid itemId, int itemVal = 1);
        long GetItemCooldown(Guid id);
        bool GetRealLocation(ref int x, ref int y, ref Guid mapId);
        long GetSpellCooldown(Guid id);
        void HotbarSwap(byte index, byte swapIndex);
        bool IsBusy();
        bool IsEquipped(int slot);
        bool IsInMyParty(Guid id);
        bool IsInMyParty(IPlayer player); 
        long ItemCdRemainder(int slot);
        bool ItemOnCd(int slot);
        void StopBlocking();
        void SwapItems(int Label, int Color);
        void SwapSpells(int spell1, int spell2);
        bool TryAttack();
        bool TryBlock();
        void TryDepositItem(int index);
        void TryDropItem(int index);
        void TryForgetSpell(int index);
        bool TryPickupItem(Guid mapId, int tileIndex, Guid uniqueId = default, bool firstOnly = false);
        bool TryTarget();
        bool TryTarget(IEntity entity, bool force = false);
        void TryUseItem(int index);
        void TryUseSpell(Guid spellId);
        void TryUseSpell(int index);
    }
}