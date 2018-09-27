using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Migration.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects.Events;
using Intersect.Migration.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects.Maps;
using Intersect.Migration.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects
{
    public abstract class DatabaseObject
    {
        public const string DATABASE_TABLE = "";
        public const GameObject OBJECT_TYPE = GameObject.Animation;
        private int mId = -1;
        private byte[] mBackup;

        protected DatabaseObject(int id)
        {
            mId = id;
        }

        public virtual int GetId()
        {
            return mId;
        }

        public abstract void Load(byte[] packet);

        public virtual void MakeBackup()
        {
            mBackup = GetData();
        }

        public virtual void RestoreBackup()
        {
            if (mBackup != null)
                Load(mBackup);
        }

        public virtual void DeleteBackup()
        {
            mBackup = null;
        }

        public abstract void Delete();
        public abstract byte[] GetData();
        public abstract GameObject GetGameObjectType();
        public abstract string GetTable();

        //Game Object Handling
        public static string[] GetGameObjectList(GameObject type)
        {
            var items = new List<string>();
            switch (type)
            {
                case GameObject.Animation:
                    foreach (var obj in AnimationBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Class:
                    foreach (var obj in ClassBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Item:
                    foreach (var obj in ItemBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Npc:
                    foreach (var obj in NpcBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Projectile:
                    foreach (var obj in ProjectileBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Quest:
                    foreach (var obj in QuestBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Resource:
                    foreach (var obj in ResourceBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Shop:
                    foreach (var obj in ShopBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Spell:
                    foreach (var obj in SpellBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Map:
                    foreach (var obj in MapBase.GetObjects())
                        items.Add(obj.Value.MyName);
                    break;
                case GameObject.CommonEvent:
                    foreach (var obj in EventBase.GetObjects())
                        items.Add(obj.Value.MyName);
                    break;
                case GameObject.PlayerSwitch:
                    foreach (var obj in PlayerSwitchBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.PlayerVariable:
                    foreach (var obj in PlayerVariableBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.ServerSwitch:
                    foreach (var obj in ServerSwitchBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.ServerVariable:
                    foreach (var obj in ServerVariableBase.GetObjects())
                        items.Add(obj.Value.Name);
                    break;
                case GameObject.Tileset:
                    foreach (var obj in TilesetBase.GetObjects())
                        items.Add(obj.Value.Value);
                    break;
                case GameObject.Time:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return items.ToArray();
        }

        public static int GameObjectIdFromList(GameObject type, int listIndex)
        {
            if (listIndex < 0) return -1;
            switch (type)
            {
                case GameObject.Animation:
                    if (listIndex >= AnimationBase.ObjectCount()) return -1;
                    return AnimationBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Class:
                    if (listIndex >= ClassBase.ObjectCount()) return -1;
                    return ClassBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Item:
                    if (listIndex >= ItemBase.ObjectCount()) return -1;
                    return ItemBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Npc:
                    if (listIndex >= NpcBase.ObjectCount()) return -1;
                    return NpcBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Projectile:
                    if (listIndex >= ProjectileBase.ObjectCount()) return -1;
                    return ProjectileBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Quest:
                    if (listIndex >= QuestBase.ObjectCount()) return -1;
                    return QuestBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Resource:
                    if (listIndex >= ResourceBase.ObjectCount()) return -1;
                    return ResourceBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Shop:
                    if (listIndex >= ShopBase.ObjectCount()) return -1;
                    return ShopBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Spell:
                    if (listIndex >= SpellBase.ObjectCount()) return -1;
                    return SpellBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Map:
                    if (listIndex >= MapBase.ObjectCount()) return -1;
                    return MapBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.CommonEvent:
                    if (listIndex >= EventBase.ObjectCount()) return -1;
                    return EventBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.PlayerSwitch:
                    if (listIndex >= PlayerSwitchBase.ObjectCount()) return -1;
                    return PlayerSwitchBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.PlayerVariable:
                    if (listIndex >= PlayerVariableBase.ObjectCount()) return -1;
                    return PlayerVariableBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.ServerSwitch:
                    if (listIndex >= ServerSwitchBase.ObjectCount()) return -1;
                    return ServerSwitchBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.ServerVariable:
                    if (listIndex >= ServerVariableBase.ObjectCount()) return -1;
                    return ServerVariableBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Tileset:
                    if (listIndex >= TilesetBase.ObjectCount()) return -1;
                    return TilesetBase.GetObjects().Keys.ToList()[listIndex];
                case GameObject.Time:
                    return -1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static int GameObjectListIndex(GameObject type, int id)
        {
            switch (type)
            {
                case GameObject.Animation:
                    return AnimationBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Class:
                    return ClassBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Item:
                    return ItemBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Npc:
                    return NpcBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Projectile:
                    return ProjectileBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Quest:
                    return QuestBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Resource:
                    return ResourceBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Shop:
                    return ShopBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Spell:
                    return SpellBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Map:
                    return MapBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.CommonEvent:
                    return EventBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.PlayerSwitch:
                    return PlayerSwitchBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.PlayerVariable:
                    return PlayerVariableBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.ServerSwitch:
                    return ServerSwitchBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.ServerVariable:
                    return ServerVariableBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Tileset:
                    return TilesetBase.GetObjects().Keys.ToList().IndexOf(id);
                case GameObject.Time:
                    return -1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}