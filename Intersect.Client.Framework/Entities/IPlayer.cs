using System;
using System.Collections.Generic;

using Intersect.Config.Guilds;
using Intersect.Enums;
using Intersect.GameObjects;

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
        bool IsBusy { get; }

        bool TryGetRealLocation(ref int x, ref int y, ref Guid mapId);
        bool TryTarget();
        bool TryTarget(IEntity entity, bool force = false);
        void AutoTarget();
        void ClearTarget();
        void AddToHotbar(int hotbarSlot, sbyte itemType, int itemSlot);
        int FindHotbarItem(IHotbarInstance hotbarInstance);
        int FindHotbarSpell(IHotbarInstance hotbarInstance);
        void HotbarSwap(int index, int swapIndex);
        int FindItem(Guid itemId, int itemVal = 1);
        void SwapItems(int item1, int item2);
        long GetItemCooldown(Guid id);
        long GetItemRemainingCooldown(int slot);
        bool IsItemOnCooldown(int slot);
        void TryDropItem(int index);
        void TryUseItem(int index);
        void SwapSpells(int spell1, int spell2);
        long GetSpellCooldown(Guid id);
        long GetSpellRemainingCooldown(int slot);
        bool IsSpellOnCooldown(int slot);
        void TryForgetSpell(int index);
        void TryUseSpell(Guid spellId);
        void TryUseSpell(int index);
        bool IsEquipped(int slot);
        bool IsInMyParty(Guid id);
        bool IsInMyParty(IPlayer player);
        bool TryAttack();
        bool TryBlock();




        
        
    }
}
