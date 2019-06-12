using System;
using System.IO;
using System.IO.Compression;

using Intersect.GameObjects.Maps;
using Intersect.Network;
using Intersect.Server.Database.GameData;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;

namespace Intersect.Server.Classes.Database.GameData.Migrations
{
    public static class Beta6Migration
    {
        private static readonly Ceras mCeras = new Ceras(false);

        public static void Run(GameContext context)
        {
            RemoveByteBufferUsageFromMaps(context);

            //Switch/Variable Changes :D

        }

        private static void RemoveByteBufferUsageFromMaps(GameContext context)
        {
            var connection = context.Database.GetDbConnection();
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "Select Id, Attributes, TileData from Maps";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader["Id"];

                    var tileData = (byte[])reader["TileData"];
                    var newTileData = ReencodeTileData(tileData);


                    var attributeData = (byte[])reader["Attributes"];
                    var newAttributeData = mCeras.Compress(JsonConvert.DeserializeObject<MapAttribute[,]>(System.Text.Encoding.UTF8.GetString(Decompress(attributeData)), new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Auto, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, ObjectCreationHandling = ObjectCreationHandling.Replace}));

                    using (var updateCmd = connection.CreateCommand())
                    {
                        updateCmd.CommandText = "UPDATE Maps SET Attributes = @Attributes, TileData = @TileData WHERE Id = @Id;";

                        if (context.Database.ProviderName.Contains("Sqlite"))
                        {
                            updateCmd.Parameters.Add(new SqliteParameter("@Id", id));
                            updateCmd.Parameters.Add(new SqliteParameter("@Attributes", newAttributeData));
                            updateCmd.Parameters.Add(new SqliteParameter("@TileData", newTileData));
                        }
                        else
                        {
                            updateCmd.Parameters.Add(new MySqlParameter("@Id", id));
                            updateCmd.Parameters.Add(new MySqlParameter("@Attributes", newAttributeData));
                            updateCmd.Parameters.Add(new MySqlParameter("@TileData", newTileData));
                        }
                        updateCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static byte[] ReencodeTileData(byte[] tileData)
        {
            var data = Decompress(tileData);
            var readPos = 0;
            var Layers = new TileArray[Options.LayerCount];
            for (var i = 0; i < Options.LayerCount; i++)
            {
                Layers[i].Tiles = new Tile[Options.MapWidth, Options.MapHeight];
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y].TilesetId = new Guid(new Byte[16] {data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++], data[readPos++]});
                        Layers[i].Tiles[x, y].X = BitConverter.ToInt32(data, readPos);
                        readPos += 4;
                        Layers[i].Tiles[x, y].Y = BitConverter.ToInt32(data, readPos);
                        readPos += 4;
                        Layers[i].Tiles[x, y].Autotile = data[readPos++];
                    }
                }
            }
            return mCeras.Compress(Layers); ;
        }

        private static byte[] Decompress(byte[] data)
        {
            var len = BitConverter.ToInt32(data, 0);
            var compressedData = new byte[data.Length - 4];
            for (int i = 4; i < data.Length; i++)
                compressedData[i - 4] = data[i];
            var decompessed = new byte[len];
            using (MemoryStream ms = new MemoryStream(compressedData))
            {
                using (DeflateStream decompressionStream = new DeflateStream(ms, CompressionMode.Decompress))
                {
                    decompressionStream.Read(decompessed, 0, decompessed.Length);
                }
                return decompessed;
            }
            return null;
        }
    }
}
