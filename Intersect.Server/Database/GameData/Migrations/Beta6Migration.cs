using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Events.Commands;
using Intersect.GameObjects.Maps;
using Intersect.Network;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.Server.Database.GameData.Migrations
{

    public static class Beta6Migration
    {

        private static Ceras mCeras;

        public static void Run(GameContext context)
        {
            var nameTypeDict = new Dictionary<string, Type>();
            nameTypeDict.Add("Intersect.GameObjects.Maps.TileArray[]", typeof(LegacyTileArray[]));
            mCeras = new Ceras(nameTypeDict);

            RemoveByteBufferUsageFromMaps(context);

            //Fix SetSwitch/SetVariable Event Commands
            FixVariableCommandsAndConditions(context, "Events", "Pages");

            //Convert Conditions (Events/EventCommands/Items/Quests/etc/etc/etc)
            FixVariableCommandsAndConditions(context, "Npcs", "AttackOnSightConditions");
            FixVariableCommandsAndConditions(context, "Npcs", "PlayerCanAttackConditions");
            FixVariableCommandsAndConditions(context, "Npcs", "PlayerFriendConditions");
            FixVariableCommandsAndConditions(context, "Spells", "CastRequirements");
            FixVariableCommandsAndConditions(context, "Resources", "HarvestingRequirements");
            FixVariableCommandsAndConditions(context, "Quests", "Requirements");
            FixVariableCommandsAndConditions(context, "Items", "UsageRequirements");
        }

        private static void FixVariableCommandsAndConditions(GameContext context, string table, string column)
        {
            var connection = context.Database.GetDbConnection();
            connection.Open();
            var updates = new List<KeyValuePair<object, string>>();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "Select Id, " + column + " FROM " + table + ";";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader["Id"];

                    var json = reader[column].ToString();

                    var fixedJson = FixJson(json);

                    updates.Add(new KeyValuePair<object, string>(id, fixedJson));
                }
            }

            connection.Close();
            connection.Open();

            if (updates.Count > 0)
            {
                var trans = connection.BeginTransaction();

                using (var updateCmd = connection.CreateCommand())
                {
                    var i = 0;
                    var currentCount = 0;
                    updateCmd.CommandText = "";
                    updateCmd.Transaction = trans;
                    foreach (var update in updates)
                    {
                        updateCmd.CommandText +=
                            "UPDATE " + table + " SET " + column + " = @json" + i + " WHERE Id = @Id" + i + ";";

                        if (context.Database.ProviderName.Contains("Sqlite"))
                        {
                            updateCmd.Parameters.Add(new SqliteParameter("@Id" + i, update.Key));
                            updateCmd.Parameters.Add(new SqliteParameter("@json" + i, update.Value));
                        }
                        else
                        {
                            updateCmd.Parameters.Add(new MySqlParameter("@Id" + i, update.Key));
                            updateCmd.Parameters.Add(new MySqlParameter("@json" + i, update.Value));
                        }

                        i++;
                        currentCount++;

                        if (currentCount > 256)
                        {
                            updateCmd.ExecuteNonQuery();
                            updateCmd.CommandText = "";
                            updateCmd.Parameters.Clear();
                            currentCount = 0;
                        }
                    }

                    updateCmd.ExecuteNonQuery();
                    updateCmd.Parameters.Clear();
                }

                trans.Commit();
            }

            connection.Close();
        }

        private static string FixJson(string json)
        {
            var obj = JArray.Parse(json);
            ParseJson(obj);
            var newJson = obj.ToString(Formatting.None);

            return newJson;
        }

        private static void ParseJson(JToken obj)
        {
            foreach (var child in obj)
            {
                if (child.Type == JTokenType.Object)
                {
                    //Stuff
                    var childObj = (JObject) child;

                    var type = child["$type"];
                    if (type != null)
                    {
                        switch (type.ToString())
                        {
                            case "Intersect.GameObjects.Events.Commands.SetSwitchCommand, Intersect Core":
                                var newNode = ConvertSetSwitchCommand(childObj);
                                childObj.RemoveAll();
                                foreach (var node in newNode)
                                {
                                    childObj.Add(node.Key, node.Value);
                                }

                                break;

                            case "Intersect.GameObjects.Events.Commands.SetVariableCommand, Intersect Core":
                                var newNode1 = ConvertSetVariableCommand(childObj);
                                childObj.RemoveAll();
                                foreach (var node in newNode1)
                                {
                                    childObj.Add(node.Key, node.Value);
                                }

                                break;
                            case "Intersect.GameObjects.Events.PlayerVariableCondition, Intersect Core":
                                var newNode2 = ConvertPlayerVariableCondition(childObj);
                                childObj.RemoveAll();
                                foreach (var node in newNode2)
                                {
                                    childObj.Add(node.Key, node.Value);
                                }

                                break;
                            case "Intersect.GameObjects.Events.PlayerSwitchCondition, Intersect Core":
                                var newNode3 = ConvertPlayerSwitchCondition(childObj);
                                childObj.RemoveAll();
                                foreach (var node in newNode3)
                                {
                                    childObj.Add(node.Key, node.Value);
                                }

                                break;
                            case "Intersect.GameObjects.Events.ServerVariableCondition, Intersect Core":
                                var newNode4 = ConvertGlobalVariableCondition(childObj);
                                childObj.RemoveAll();
                                foreach (var node in newNode4)
                                {
                                    childObj.Add(node.Key, node.Value);
                                }

                                break;
                            case "Intersect.GameObjects.Events.ServerSwitchCondition, Intersect Core":
                                var newNode5 = ConvertGlobalSwitchCondition(childObj);
                                childObj.RemoveAll();
                                foreach (var node in newNode5)
                                {
                                    childObj.Add(node.Key, node.Value);
                                }

                                break;
                            default:
                                break;
                        }
                    }
                }
                else if (child.Type == JTokenType.String)
                {
                    var property = child.Parent as JProperty;
                    if (property != null && property.Name == "Lists")
                    {
                        try
                        {
                            var newJson = FixJson(property.Value?.ToString());
                            property.Value = newJson;
                        }
                        catch
                        {
                            // Not important to abort or log
                        }
                    }
                }

                ParseJson(child);
            }
        }

        //OLD
        //public class SetSwitchCommand : EventCommand
        //{
        //    public override EventCommandType Type { get; } = EventCommandType.SetSwitch;
        //    public SwitchTypes SwitchType { get; set; } = SwitchTypes.PlayerSwitch;
        //    public Guid SwitchId { get; set; }
        //    public bool Value { get; set; }
        //    public bool SyncParty { get; set; }
        //}

        private static JObject ConvertSetSwitchCommand(JObject obj)
        {
            var cmd = new SetVariableCommand();

            if (obj.ContainsKey("SwitchId"))
            {
                cmd.VariableId = Guid.Parse(obj["SwitchId"].ToString());
            }

            if (obj.ContainsKey("SwitchType") && int.Parse(obj["SwitchType"].ToString()) == 1)
            {
                cmd.VariableType = VariableTypes.ServerVariable;
            }
            else
            {
                cmd.VariableType = VariableTypes.PlayerVariable;
            }

            if (obj.ContainsKey("SyncParty") && bool.Parse(obj["SyncParty"].ToString()))
            {
                cmd.SyncParty = true;
            }
            else
            {
                cmd.SyncParty = false;
            }

            var mod = new BooleanVariableMod();
            cmd.Modification = mod;

            if (obj.ContainsKey("Value") && bool.Parse(obj["Value"].ToString()))
            {
                mod.Value = true;
            }
            else
            {
                mod.Value = false;
            }

            var newJson = JsonConvert.SerializeObject(
                cmd, typeof(EventCommand),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None, TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );

            return JObject.Parse(newJson);
        }

        //OLD
        //public class SetVariableCommand : EventCommand
        //{
        //    public VariableMods ModType { get; set; } = VariableMods.Set;
        //    public int Value { get; set; }
        //    public int HighValue { get; set; }
        //    public Guid DuplicateVariableId { get; set; }
        //}

        private static JObject ConvertSetVariableCommand(JObject obj)
        {
            var cmd = new SetVariableCommand();

            if (obj.ContainsKey("VariableId"))
            {
                cmd.VariableId = Guid.Parse(obj["VariableId"].ToString());
            }

            if (obj.ContainsKey("VariableType") && int.Parse(obj["VariableType"].ToString()) == 1)
            {
                cmd.VariableType = VariableTypes.ServerVariable;
            }
            else
            {
                cmd.VariableType = VariableTypes.PlayerVariable;
            }

            if (obj.ContainsKey("SyncParty") && bool.Parse(obj["SyncParty"].ToString()))
            {
                cmd.SyncParty = true;
            }
            else
            {
                cmd.SyncParty = false;
            }

            var mod = new IntegerVariableMod();
            cmd.Modification = mod;

            if (obj.ContainsKey("Value"))
            {
                mod.Value = long.Parse(obj["Value"].ToString());
            }

            if (obj.ContainsKey("DupVariableId"))
            {
                mod.DuplicateVariableId = Guid.Parse(obj["DupVariableId"].ToString());
            }

            if (obj.ContainsKey("HighValue"))
            {
                mod.HighValue = long.Parse(obj["HighValue"].ToString());
            }

            if (obj.ContainsKey("ModType"))
            {
                mod.ModType = (VariableMods) int.Parse(obj["ModType"].ToString());
            }

            var newJson = JsonConvert.SerializeObject(
                cmd, typeof(EventCommand),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None, TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );

            return JObject.Parse(newJson);
        }

        //OLD
        //public class PlayerSwitchCondition : Condition
        //{
        //    public override ConditionTypes Type { get; } = ConditionTypes.PlayerSwitch;
        //    public Guid SwitchId { get; set; }
        //    public bool Value { get; set; }
        //}

        private static JObject ConvertPlayerSwitchCondition(JObject obj)
        {
            var cmd = new VariableIsCondition();

            if (obj.ContainsKey("SwitchId"))
            {
                cmd.VariableId = Guid.Parse(obj["SwitchId"].ToString());
            }

            cmd.VariableType = VariableTypes.PlayerVariable;

            var comp = new BooleanVariableComparison();
            cmd.Comparison = comp;

            comp.ComparingEqual = true;

            if (obj.ContainsKey("Value"))
            {
                comp.Value = bool.Parse(obj["Value"].ToString());
            }

            var newJson = JsonConvert.SerializeObject(
                cmd, typeof(Condition),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None, TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );

            return JObject.Parse(newJson);
        }

        //OLD
        //public class ServerSwitchCondition : Condition
        //{
        //    public override ConditionTypes Type { get; } = ConditionTypes.ServerSwitch;
        //    public Guid SwitchId { get; set; }
        //    public bool Value { get; set; }
        //}

        private static JObject ConvertGlobalSwitchCondition(JObject obj)
        {
            var cmd = new VariableIsCondition();

            if (obj.ContainsKey("SwitchId"))
            {
                cmd.VariableId = Guid.Parse(obj["SwitchId"].ToString());
            }

            cmd.VariableType = VariableTypes.ServerVariable;

            var comp = new BooleanVariableComparison();
            cmd.Comparison = comp;

            comp.ComparingEqual = true;

            if (obj.ContainsKey("Value"))
            {
                comp.Value = bool.Parse(obj["Value"].ToString());
            }

            var newJson = JsonConvert.SerializeObject(
                cmd, typeof(Condition),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None, TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );

            return JObject.Parse(newJson);
        }

        //OLD
        //public class PlayerVariableCondition : Condition
        //{
        //    public override ConditionTypes Type { get; } = ConditionTypes.PlayerVariable;
        //    public Guid VariableId { get; set; }
        //    public VariableComparators Comparator { get; set; } = VariableComparators.Equal;
        //    public VariableCompareTypes CompareType { get; set; } = VariableCompareTypes.StaticValue;
        //    public long Value { get; set; }
        //    public Guid CompareVariableId { get; set; }
        //}

        private static JObject ConvertPlayerVariableCondition(JObject obj)
        {
            var cmd = new VariableIsCondition();

            if (obj.ContainsKey("VariableId"))
            {
                cmd.VariableId = Guid.Parse(obj["VariableId"].ToString());
            }

            cmd.VariableType = VariableTypes.PlayerVariable;

            var comp = new IntegerVariableComparison();
            cmd.Comparison = comp;

            if (obj.ContainsKey("Value"))
            {
                comp.Value = long.Parse(obj["Value"].ToString());
            }

            if (obj.ContainsKey("CompareVariableId"))
            {
                comp.CompareVariableId = Guid.Parse(obj["CompareVariableId"].ToString());
            }

            if (obj.ContainsKey("Comparator"))
            {
                comp.Comparator = (VariableComparators) int.Parse(obj["Comparator"].ToString());
            }

            if (!obj.ContainsKey("CompareType"))
            {
                comp.CompareVariableId = Guid.Empty;
            }
            else
            {
                if (int.Parse(obj["CompareType"].ToString()) == 1)
                {
                    comp.CompareVariableType = VariableTypes.PlayerVariable;
                }
                else
                {
                    comp.CompareVariableType = VariableTypes.ServerVariable;
                }
            }

            var newJson = JsonConvert.SerializeObject(
                cmd, typeof(Condition),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None, TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );

            return JObject.Parse(newJson);
        }

        //OLD
        //public class ServerVariableCondition : Condition
        //{
        //    public override ConditionTypes Type { get; } = ConditionTypes.ServerVariable;
        //    public Guid VariableId { get; set; }
        //    public VariableComparators Comparator { get; set; } = VariableComparators.Equal;
        //    public VariableCompareTypes CompareType { get; set; } = VariableCompareTypes.StaticValue;
        //    public long Value { get; set; }
        //    public Guid CompareVariableId { get; set; }
        //}

        private static JObject ConvertGlobalVariableCondition(JObject obj)
        {
            var cmd = new VariableIsCondition();

            if (obj.ContainsKey("VariableId"))
            {
                cmd.VariableId = Guid.Parse(obj["VariableId"].ToString());
            }

            cmd.VariableType = VariableTypes.ServerVariable;

            var comp = new IntegerVariableComparison();
            cmd.Comparison = comp;

            if (obj.ContainsKey("Value"))
            {
                comp.Value = long.Parse(obj["Value"].ToString());
            }

            if (obj.ContainsKey("CompareVariableId"))
            {
                comp.CompareVariableId = Guid.Parse(obj["CompareVariableId"].ToString());
            }

            if (obj.ContainsKey("Comparator"))
            {
                comp.Comparator = (VariableComparators) int.Parse(obj["Comparator"].ToString());
            }

            if (!obj.ContainsKey("CompareType"))
            {
                comp.CompareVariableId = Guid.Empty;
            }
            else
            {
                if (int.Parse(obj["CompareType"].ToString()) == 1)
                {
                    comp.CompareVariableType = VariableTypes.PlayerVariable;
                }
                else
                {
                    comp.CompareVariableType = VariableTypes.ServerVariable;
                }
            }

            var newJson = JsonConvert.SerializeObject(
                cmd, typeof(Condition),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None, TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );

            return JObject.Parse(newJson);
        }

        private static void RemoveByteBufferUsageFromMaps(GameContext context)
        {
            var connection = context.Database.GetDbConnection();
            connection.Open();
            var updates = new List<Tuple<object, byte[], byte[]>>();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "Select Id, Attributes, TileData from Maps;";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader["Id"];

                    var tileData = (byte[]) reader["TileData"];
                    var newTileData = ReencodeTileData(tileData);

                    var attributeData = (byte[]) reader["Attributes"];
                    var newAttributeData = mCeras.Compress(
                        JsonConvert.DeserializeObject<MapAttribute[,]>(
                            System.Text.Encoding.UTF8.GetString(Decompress(attributeData)),
                            new JsonSerializerSettings()
                            {
                                TypeNameHandling = TypeNameHandling.Auto,
                                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                                ObjectCreationHandling = ObjectCreationHandling.Replace
                            }
                        )
                    );

                    updates.Add(new Tuple<object, byte[], byte[]>(id, newAttributeData, newTileData));
                }
            }

            connection.Close();
            connection.Open();
            if (updates.Count > 0)
            {
                var trans = connection.BeginTransaction();

                using (var updateCmd = connection.CreateCommand())
                {
                    updateCmd.CommandText = "";
                    updateCmd.Transaction = trans;
                    var i = 0;
                    var currentCount = 0;
                    foreach (var update in updates)
                    {
                        updateCmd.CommandText += "UPDATE Maps SET Attributes = @Attributes" +
                                                 i +
                                                 ", TileData = @TileData" +
                                                 i +
                                                 " WHERE Id = @Id" +
                                                 i +
                                                 ";";

                        if (context.Database.ProviderName.Contains("Sqlite"))
                        {
                            updateCmd.Parameters.Add(new SqliteParameter("@Id" + i, (object) update.Item1));
                            updateCmd.Parameters.Add(new SqliteParameter("@Attributes" + i, (byte[]) update.Item2));
                            updateCmd.Parameters.Add(new SqliteParameter("@TileData" + i, (byte[]) update.Item3));
                        }
                        else
                        {
                            updateCmd.Parameters.Add(new MySqlParameter("@Id" + i, (object) update.Item1));
                            updateCmd.Parameters.Add(new MySqlParameter("@Attributes" + i, (byte[]) update.Item2));
                            updateCmd.Parameters.Add(new MySqlParameter("@TileData" + i, (byte[]) update.Item3));
                        }

                        i++;
                        currentCount++;

                        if (currentCount > 256)
                        {
                            updateCmd.ExecuteNonQuery();
                            updateCmd.CommandText = "";
                            updateCmd.Parameters.Clear();
                            currentCount = 0;
                        }
                    }

                    updateCmd.ExecuteNonQuery();
                    updateCmd.Parameters.Clear();
                }

                trans.Commit();
            }

            connection.Close();
        }

        private static byte[] ReencodeTileData(byte[] tileData)
        {
            var data = Decompress(tileData);
            var readPos = 0;
            var Layers = new LegacyTileArray[5];
            for (var i = 0; i < 5; i++)
            {
                Layers[i].Tiles = new LegacyTile[Options.MapWidth, Options.MapHeight];
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y].TilesetId = new Guid(
                            new Byte[16]
                            {
                                data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++],
                                data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++],
                                data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++],
                                data[readPos++]
                            }
                        );

                        Layers[i].Tiles[x, y].X = BitConverter.ToInt32(data, readPos);
                        readPos += 4;
                        Layers[i].Tiles[x, y].Y = BitConverter.ToInt32(data, readPos);
                        readPos += 4;
                        Layers[i].Tiles[x, y].Autotile = data[readPos++];
                    }
                }
            }

            return mCeras.Compress(Layers);
        }

        private static byte[] Decompress(byte[] data)
        {
            var len = BitConverter.ToInt32(data, 0);
            var compressedData = new byte[data.Length - 4];
            for (var i = 4; i < data.Length; i++)
            {
                compressedData[i - 4] = data[i];
            }

            var decompessed = new byte[len];
            using (var ms = new MemoryStream(compressedData))
            {
                using (var decompressionStream = new DeflateStream(ms, CompressionMode.Decompress))
                {
                    decompressionStream.Read(decompessed, 0, decompessed.Length);
                }

                return decompessed;
            }

            return null;
        }

        private struct LegacyTileArray
        {

            public LegacyTile[,] Tiles;

        }

        private struct LegacyTile
        {

            public Guid TilesetId;

            public int X;

            public int Y;

            public byte Autotile;

            [JsonIgnore] public object TilesetTex;

        }

    }

}
