using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SpatialiteSharp;

namespace TestGdal
{
    internal class Spatialite2GeoJson
    {
        private string _connectionString = "";
        private string _geoColumnName = "geo";

        public void SetDateBase(string filePath)
        {
            _connectionString = $"Data Source = {filePath}";
        }

        public Spatialite2GeoJson()
        {
        }

        public Spatialite2GeoJson(string filePath)
        {
            this.SetDateBase(filePath);
        }

        public string GetGeoJson(string tableName)
        {
            if (_connectionString == "")
                throw new Exception("Please set up the file path of your database by * SetDateBase *");

            var features = new List<Feature>();

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                //open spatialite by spatialitesharp
                SpatialiteSharp.SpatialiteLoader.Load(conn);
                using (var command = conn.CreateCommand())
                {
                    command.CommandText =
                        $"SELECT ASGEOJSON({_geoColumnName}) as geometry,GeometryType({_geoColumnName}) as geomtype, *  from {tableName}; ";
                    var reader = command.ExecuteReader();

                    var propertyNames =
                        Enumerable.Range(0, reader.FieldCount)
                            .Select(reader.GetName)
                            .Where(x => x != "geo" && x != "geometry" && x != "geomtype")
                            .ToList();

                    if (!reader.HasRows) return JsonConvert.SerializeObject(new FeatureCollection(features));

                    var jsonSerializer =  new JsonSerializerSettings{ContractResolver = new CamelCasePropertyNamesContractResolver()};
                  
                    while (reader.Read())
                    {
                        //get geometry type
                        var type = Convert.ToString(reader["geomtype"]);

                        var propertyDict = propertyNames.ToDictionary(property => property,
                            property => reader[property]);

                        var geoJsonText = Convert.ToString(reader["geometry"]);

                        switch (type)
                        {
                            case "POINT":
                                var point = JsonConvert.DeserializeObject<Point>(geoJsonText, jsonSerializer);
                                features.Add(new Feature(point, propertyDict));
                                break;
                            case "LINESTRING":
                                var linestring = JsonConvert.DeserializeObject<LineString>(geoJsonText, jsonSerializer);
                                features.Add(new Feature(linestring, propertyDict));
                                break;
                            case "POLYGON":
                                var polygon = JsonConvert.DeserializeObject<Polygon>(geoJsonText, jsonSerializer);
                                features.Add(new Feature(polygon, propertyDict));
                                break;
                            case "MULTIPOINT":
                                var multipoint = JsonConvert.DeserializeObject<MultiPoint>(geoJsonText, jsonSerializer);
                                features.Add(new Feature(multipoint, propertyDict));
                                break;
                            case "MULTILINESTRING":
                                var mutlilinestring = JsonConvert.DeserializeObject<MultiLineString>(geoJsonText, jsonSerializer);
                                features.Add(new Feature(mutlilinestring, propertyDict));
                                break;
                            case "MULTIPOLYGON":
                                var multipolygon = JsonConvert.DeserializeObject<MultiPolygon>(geoJsonText,jsonSerializer);
                                features.Add(new Feature(multipolygon, propertyDict));
                                break;
                        }
                    }

                    return JsonConvert.SerializeObject(new FeatureCollection(features));
                }
            }
        }

        public class SpatialiteLoader
        {
            private static readonly object Lock = new object();
            private static bool _haveSetPath;

            /// <summary>
            /// Loads mod_spatialite.dll on the given connection
            /// 
            /// </summary>
            public static void Load(SQLiteConnection conn)
            {
                lock (SpatialiteLoader.Lock)
                {
                    if (!SpatialiteLoader._haveSetPath)
                    {
                        var dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        var spatialitePath = Path.Combine(dllPath, Environment.Is64BitProcess ? "x64" : "x86", "spatialite") + ";";
                        var paths = Environment.GetEnvironmentVariable("PATH");

                        Environment.SetEnvironmentVariable("PATH", spatialitePath + paths);
                        SpatialiteLoader._haveSetPath = true;
                    }
                }
                conn.LoadExtension("mod_spatialite.dll");
            }
        }
    }
}