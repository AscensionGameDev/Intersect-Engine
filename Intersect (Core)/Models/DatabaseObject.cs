using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Intersect.Collections;
using Intersect.Enums;

using Newtonsoft.Json;

namespace Intersect.Models
{

    public abstract class DatabaseObject<TObject> : IDatabaseObject where TObject : DatabaseObject<TObject>
    {

        public const string Deleted = "ERR_DELETED";

        private string mBackup;

        protected DatabaseObject() : this(Guid.Empty)
        {
        }

        [JsonConstructor]
        protected DatabaseObject(Guid guid)
        {
            Id = guid;
            TimeCreated = DateTime.Now.ToBinary();
        }

        public static KeyValuePair<Guid, string>[] ItemPairs => Lookup.OrderBy(p => p.Value?.TimeCreated)
            .Select(pair => new KeyValuePair<Guid, string>(pair.Key, pair.Value?.Name ?? Deleted))
            .ToArray();

        public static string[] Names => Lookup.OrderBy(p => p.Value?.TimeCreated)
            .Select(pair => pair.Value?.Name ?? Deleted)
            .ToArray();

        public static DatabaseObjectLookup Lookup => LookupUtils.GetLookup(typeof(TObject));

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; protected set; }

        public long TimeCreated { get; set; }

        [JsonIgnore]
        [NotMapped]
        public GameObjectType Type => LookupUtils.GetGameObjectType(typeof(TObject));

        [JsonIgnore]
        [NotMapped]
        public string DatabaseTable => Type.GetTable();

        [JsonProperty(Order = -4)]
        [Column(Order = 0)]
        public string Name { get; set; }

        public virtual void Load(string json, bool keepCreationTime = false)
        {
            var oldTime = TimeCreated;
            JsonConvert.PopulateObject(
                json, this, new JsonSerializerSettings
                {
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );

            if (keepCreationTime)
            {
                TimeCreated = oldTime;
            }
        }

        // TODO: Can we remove this comment?
        // public virtual void Load(string json);

        public void MakeBackup()
        {
            mBackup = JsonData;
        }

        public void DeleteBackup()
        {
            mBackup = null;
        }

        public void RestoreBackup()
        {
            if (mBackup != null)
            {
                Load(mBackup);
            }
        }

        [JsonIgnore]
        [NotMapped]

        // TODO: Should eventually be formatting.none
        public virtual string JsonData => JsonConvert.SerializeObject(this, Formatting.Indented);

        public virtual void Delete()
        {
            Lookup.Delete(this as TObject);
        }

        public static Guid IdFromList(int listIndex)
        {
            if (listIndex < 0 || listIndex > Lookup.KeyList.Count)
            {
                return Guid.Empty;
            }

            return Lookup.KeyList.OrderBy(pairs => Lookup[pairs]?.TimeCreated).ToArray()[listIndex];
        }

        public static TObject FromList(int listIndex)
        {
            if (listIndex < 0 || listIndex > Lookup.ValueList.Count)
            {
                return null;
            }

            return (TObject) Lookup.ValueList.OrderBy(databaseObject => databaseObject?.TimeCreated).ToArray()[
                listIndex];
        }

        public int ListIndex()
        {
            return ListIndex(Id);
        }

        public static int ListIndex(Guid id)
        {
            return Lookup.KeyList.OrderBy(pairs => Lookup[pairs]?.TimeCreated).ToList().IndexOf(id);
        }

        public static TObject Get(Guid id)
        {
            return Lookup.Get<TObject>(id);
        }

        public static string GetName(Guid id)
        {
            return Lookup.Get(id)?.Name ?? Deleted;
        }

        public static string[] GetNameList()
        {
            return Lookup.Select(pair => pair.Value?.Name ?? Deleted).ToArray();
        }

    }

}
