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

        protected DatabaseObject(int index) : this(Guid.NewGuid(), index)
        {
        }

        protected DatabaseObject(Guid guid) : this(guid, Lookup.NextIndex)
        {
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

        [JsonIgnore][NotMapped]
        public int Index { get; }

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