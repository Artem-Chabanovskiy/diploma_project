using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using DiplomaProject.Mongo.Connection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DiplomaProject
{
    internal class Program
    {
        private static MongoConnectionClass _dbClass;

        private static void Main(string[] args)
        {
        }

        private static async void PreprocessJsonFile(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                var data = r.ReadToEnd();

                var jsonData = JToken.Parse(data);

                var insertModels = new List<InsertOneModel<BsonDocument>>();

                foreach (var token in jsonData)
                {
                    var neededData = token["_source"].ToBsonDocument();
                    insertModels.Add(new InsertOneModel<BsonDocument>(neededData));
                }

                await _dbClass.Database.GetCollection<BsonDocument>("DCLogs").BulkWriteAsync(insertModels);
            }
        }
    }
}
