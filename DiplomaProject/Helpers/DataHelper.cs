using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiplomaProject.Mongo.Connection;
using DiplomaProject.Mongo.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace DiplomaProject.Helpers
{
    internal class DataHelper
    {
        public static List<ProcessedLogInfo> PreprocessLogClass(List<LogInfo> plainLogs)
        {
            List<ProcessedLogInfo> processedList = new List<ProcessedLogInfo>();
            LogInfo prevLog = new LogInfo();

            foreach (LogInfo plainLog in plainLogs)
            {
                //no formatting, already good
                int connectionId = plainLog.ConnectionId;
                int responseBytes = plainLog.ResponseBytes;
                int responseStatus = plainLog.ResponseStatus;
                int connectionRequests = plainLog.ConnectionRequests;

                //GET = 0, POST = 1
                int requestVerb = (plainLog.RequestVerb == "POST") ? 1 : 0;
                //true = 1, false = 0
                int nginxAccess = (plainLog.NginxAccess == "true") ? 1 : 0;
                //refferer prev = prev curr => 1 else 0
                int httpRefferer = (plainLog.HttpReferer == prevLog.HttpReferer) ? 1 : 0;
                //millis => to int (take part before comma and add as thouthands)
                int millis = Converters.ParseMillis(plainLog.Millis);
                //date to unix timestamp
                long timestamp = Converters.ParseDate(plainLog.Timestamp);

                ProcessedLogInfo processedLog = new ProcessedLogInfo(connectionId, connectionRequests, requestVerb, responseStatus,
                    responseBytes, nginxAccess, httpRefferer, millis, timestamp);

                prevLog = plainLog;
                processedList.Add(processedLog);
            }

            return processedList;
        }

        public static async void PreprocessJsonFile(string filePath, MongoConnectionClass db)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                string data = r.ReadToEnd();

                JToken jsonData = JToken.Parse(data);

                List<InsertOneModel<BsonDocument>> insertModels = jsonData.Select(token => token["_source"].ToBsonDocument())
                    .Select(neededData => new InsertOneModel<BsonDocument>(neededData)).ToList();

                await db.Database.GetCollection<BsonDocument>("DCLogs").BulkWriteAsync(insertModels);
            }
        }

        public static void FilterData(MongoConnectionClass db)
        {
            IMongoCollection<BsonDocument> collection = db.Database.GetCollection<BsonDocument>("logs");

            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = 
                builder.Eq("connection_id", BsonNull.Value) |
                builder.Eq("connection_requests", BsonNull.Value) |
                builder.Eq("cs", BsonNull.Value) |
                builder.Eq("http_referer", BsonNull.Value) |
                builder.Eq("http_user_agent", BsonNull.Value) |
                builder.Eq("http_version", BsonNull.Value) |
                builder.Eq("millis", BsonNull.Value) |
                builder.Eq("nginx_access", BsonNull.Value) |
                builder.Eq("remote_user", BsonNull.Value) |
                builder.Eq("response_bytes", BsonNull.Value) |
                builder.Eq("response_status", BsonNull.Value);

            DeleteResult result =  collection.DeleteMany(filter);

            Console.WriteLine("Deleted crashed logs. Total deleted files : " + result.DeletedCount);
        }

        public static async void FixTimeStampFormat(MongoConnectionClass db)
        {
            IMongoCollection<LogInfo> collection = db.Database.GetCollection<LogInfo>("logs");

            long amount = collection.Count(new BsonDocument());
            int pack = (int) Math.Round((double)amount / 100);

            for (int i = 0; i < 100 + 1; i++)
            {
                List<LogInfo> data = await collection.Find(new BsonDocument()).Skip(pack * i).Limit(pack)
                    .ToListAsync();

                foreach (LogInfo log in data)
                {
                   await collection.UpdateManyAsync(new BsonDocument("_id", log.Id),                       
                       new BsonDocument("$set", new BsonDocument("timestamp", Converters.ParseDate(log.Timestamp)))); 
                }

                Console.WriteLine("finished iteration : " + i);
            }
        }
    }
}
