using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Items;
using Intersect.Client.Framework.Maps;
using Intersect.Client.Framework.Spells;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Network.Packets.Server;
using System;
using System.Collections.Generic;

namespace Intersect.Client.Framework.Entities
{
    public interface IEntity
    {
        int AnimationFrame { get; set; }
        List<IAnimation> Animations { get; set; }
        long AnimationTimer { get; set; }
        Dictionary<SpriteAnimations, GameTexture> AnimatedTextures { get; set; }
        int AttackTime { get; set; }
        long AttackTimer { get; set; }
        bool Blocking { get; set; }
        long CastTime { get; set; }
        Color Color { get; set; }
        Guid CurrentMap { get; set; }
        IDash Dashing { get; set; }
        Queue<IDash> DashQueue { get; set; }
        long DashTimer { get; set; }
        byte Dir { get; set; }
        float elapsedtime { get; set; }
        Guid[] Equipment { get; set; }
        IAnimation[] EquipmentAnimations { get; set; }
        string Face { get; set; }
        ILabel FooterLabel { get; set; }
        Gender Gender { get; set; }
        ILabel HeaderLabel { get; set; }
        bool HideEntity { get; set; }
        bool HideName { get; set; }
        Guid Id { get; set; }
        IItem[] Inventory { get; set; }
        bool InView { get; set; }
        bool IsLocal { get; set; }
        bool IsMoving { get; set; }
        long LastActionTime { get; set; }
        IMapInstance LatestMap { get; set; }
        int Level { get; set; }
        IMapInstance MapInstance { get; }
        int[] MaxVital { get; set; }
        int MoveDir { get; set; }
        long MoveTimer { get; set; }
        int[] MyEquipment { get; set; }
        string MySprite { get; set; }
        string Name { get; set; }
        Color NameColor { get; set; }
        float OffsetX { get; set; }
        float OffsetY { get; set; }
        bool Passable { get; set; }
        HashSet<IEntity> RenderList { get; set; }
        Guid SpellCast { get; set; }
        ISpell[] Spells { get; set; }
        SpriteAnimations SpriteAnimation { get; set; }
        int SpriteFrame { get; set; }
        int SpriteFrames { get; }
        long SpriteFrameTimer { get; set; }
        int[] Stat { get; set; }
        List<IStatus> Status { get; }
        int Target { get; set; }
        GameTexture Texture { get; set; }
        string TransformedSprite { get; set; }
        int Type { get; set; }
        int[] Vital { get; set; }
        int WalkFrame { get; set; }
        FloatRect WorldPos { get; set; }
        byte X { get; set; }
        byte Y { get; set; }
        byte Z { get; set; }

        void AddAnimations(List<AnimationBase> anims);
        void AddChatBubble(string text);
        int CalculateAttackTime();
        bool CanBeAttacked();
        void ClearAnimations(List<IAnimation> anims);
        HashSet<IEntity> DetermineRenderOrder(HashSet<IEntity> renderList, IMapInstance map);
        void Dispose();
        void Draw();
        void DrawCastingBar();
        void DrawChatBubbles();
        void DrawEquipment(string filename, int alpha);
        void DrawHpBar();
        void DrawLabels(string label, int position, Color labelColor, Color textColor, Color borderColor = null, Color backgroundColor = null);
        void DrawName(Color textColor, Color borderColor = null, Color backgroundColor = null);
        void DrawTarget(int priority);
        Pointf GetCenterPos();
        EntityTypes GetEntityType();
        float GetLabelLocation(LabelType type);
        float GetMovementTime();
        int GetShieldSize();
        IStatus GetStatus(Guid guid);
        float GetTopPos(int overrideHeight = 0);
        bool IsDisposed();
        bool IsStealthed();
        int IsTileBlocked(int x, int y, int z, Guid mapId, ref IEntity blockedBy, bool ignoreAliveResources = true, bool ignoreDeadResources = true, bool ignoreNpcAvoids = true);
        void Load(EntityPacket packet);
        void LoadAnimationTextures(string tex);
        void ResetSpriteFrame();
        void SortStatuses();
        bool StatusActive(Guid guid);
        bool Update();
        void UpdateSpriteAnimation();
    }
}