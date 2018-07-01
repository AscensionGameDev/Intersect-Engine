using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Intersect.Collections;
using Intersect.Enums;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Intersect.Models
{
    public abstract class DatabaseObject<TObject> : IDatabaseObject where TObject : DatabaseObject<TObject>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; protected set; }

        private string mBackup;

        protected DatabaseObject() : this(Guid.Empty, -1)
        {
        }

        protected DatabaseObject(int index) : this(Guid.NewGuid(), index)
        {
        }

        protected DatabaseObject(Guid guid) : this(guid, Lookup.NextIndex)
        {
        }


        public static string[] Names => Lookup.Select(pair => pair.Value?.Name ?? "ERR_DELETED").ToArray();

        public static int IdFromList(int listIndex) => listIndex < 0 ? -1 : (Lookup.ValueList?[listIndex]?.Index ?? -1);

        public static TObject FromList(int listIndex) => listIndex < 0 ? null : listIndex > Lookup.ValueList.Count ? null : (TObject)Lookup.ValueList?[listIndex];

        public static int ListIndex(int id)
        {
            var index = Lookup.IndexList?.IndexOf(id);
            if (!index.HasValue) throw new ArgumentNullException();
            return index.Value;
        }
        public int ListIndex()
        {
            return ListIndex(Id);
        }
        public static int ListIndex(Guid id)
        {
            var index = Lookup.Keys.ToList().IndexOf(id);
            return index;
        }

        [JsonConstructor]
        protected DatabaseObject(Guid guid, int index)
        {
            Id = guid;
            Index = index;
        }

        [NotNull] public static DatabaseObjectLookup Lookup => LookupUtils.GetLookup(typeof(TObject));

        [JsonIgnore][NotMapped]
        public GameObjectType Type => LookupUtils.GetGameObjectType(typeof(TObject));

        [JsonIgnore][NotMapped]
        public string DatabaseTable => Type.GetTable();

        [JsonIgnore]//[NotMapped]
        public int Index { get; protected set; }

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
        public virtual string JsonData => JsonConvert.SerializeObject(this,Formatting.Indented);

        public virtual void Delete() => Lookup.Delete((TObject) this);

        public static string GetName(int index) => Lookup.Get(index)?.Name ?? "ERR_DELETED";

        public static string[] GetNameList() =>
            Lookup.Select(pair => pair.Value?.Name ?? "ERR_DELETED").ToArray();
    }
}