using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Intersect.Collections;
using Intersect.Enums;
using Intersect.GameObjects;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Intersect.Models
{
    public abstract class DatabaseObject<TObject> : IDatabaseObject where TObject : DatabaseObject<TObject>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; protected set; }

        public long TimeCreated { get; set; }

        private string mBackup;

        protected DatabaseObject() : this(Guid.Empty)
        {
        }

        public static string[] Names => Lookup.OrderBy(p => p.Value?.TimeCreated).Select(pair => pair.Value?.Name ?? "ERR_DELETED").ToArray();

        public static Guid IdFromList(int listIndex) => listIndex < 0 ? Guid.Empty : listIndex > Lookup.KeyList.Count ? Guid.Empty : Lookup.KeyList.OrderBy(pairs => Lookup[pairs].TimeCreated).ToArray()[listIndex];

        public static TObject FromList(int listIndex) => listIndex < 0 ? null : listIndex > Lookup.ValueList.Count ? null : (TObject)Lookup.ValueList?.OrderBy(pairs => pairs.TimeCreated).ToArray()[listIndex];

        public int ListIndex()
        {
            return ListIndex(Id);
        }
        public static int ListIndex(Guid id)
        {
            var index = Lookup.KeyList.OrderBy(pairs => Lookup[pairs].TimeCreated).ToList().IndexOf(id);
            return index;
        }

        public static TObject Get(Guid id)
        {
            return Lookup.Get<TObject>(id);
        }

        [JsonConstructor]
        protected DatabaseObject(Guid guid)
        {
            Id = guid;
            TimeCreated = DateTime.Now.ToBinary();
        }

        [NotNull] public static DatabaseObjectLookup Lookup => LookupUtils.GetLookup(typeof(TObject));

        [JsonIgnore][NotMapped]
        public GameObjectType Type => LookupUtils.GetGameObjectType(typeof(TObject));

        [JsonIgnore][NotMapped]
        public string DatabaseTable => Type.GetTable();

        [JsonProperty(Order = -4)]
        [Column(Order = 0)]
        public string Name { get; set; }

        public virtual void Load(string json) => JsonConvert.PopulateObject(json, this, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
       // public virtual void Load(string json);


        public void MakeBackup() => mBackup = JsonData;
        public void DeleteBackup() => mBackup = null;

        public void RestoreBackup()
        {
            if (mBackup != null)
            {
                Load(mBackup);
            }
        }

        [JsonIgnore][NotMapped]
        public virtual string JsonData => JsonConvert.SerializeObject(this,Formatting.Indented); //Should eventually be formatting.none

        public virtual void Delete() => Lookup.Delete((TObject) this);

        public static string GetName(Guid id) => Lookup.Get(id)?.Name ?? "ERR_DELETED";

        public static string[] GetNameList() =>
            Lookup.Select(pair => pair.Value?.Name ?? "ERR_DELETED").ToArray();
    }
}