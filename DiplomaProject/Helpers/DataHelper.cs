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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiplomaProject.Helpers
{
    internal class DataHelper
    {
        private const int Limit = 700000;

        public static List<ProcessedLogInfo> GetAndProcessData(MongoConnectionClass db, long min = 0, long max = long.MaxValue)
        {
            FilterDefinitionBuilder<LogInfo> builder = Builders<LogInfo>.Filter;
            FilterDefinition<LogInfo> filter = builder.Gte("unix_timestamp", min)
                                                & builder.Lte("unix_timestamp", max);

            IMongoCollection<LogInfo> logCollection = db.Database.GetCollection<LogInfo>("Copy_of_logs");

            try
            {
                //List<LogInfo> logList = logCollection.Find(new BsonDocument()).Limit(limit).ToList();
                List<LogInfo> logList = logCollection.Find(filter).ToList();
                return PreprocessLogClass(logList);
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("Can't load such big size of logs, try smaller time window");
                throw;
            }
        }

        public static List<ProcessedLogInfo> GetTrainData(MongoConnectionClass db, int iter)
        {
            IMongoCollection<LogInfo> logCollection = db.Database.GetCollection<LogInfo>("Copy_of_logs");

            try
            {
                //List<LogInfo> logList = logCollection.Find(new BsonDocument()).Limit(limit).ToList();
                List<LogInfo> logList = logCollection.Find(new BsonDocument()).Skip(iter * Limit).Limit(Limit).ToList();
                return PreprocessLogClass(logList);
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("Can't load such big size of logs, try smaller time window");
                throw;
            }
        }

        public static void GetDataAndTransformToJson(MongoConnectionClass db)
        {
            IMongoCollection<LogInfo> logCollection = db.Database.GetCollection<LogInfo>("Copy_of_logs");
            List<LogInfo> logList = logCollection.Find(new BsonDocument()).Limit(300).ToList();
            PreprocessLogClass(logList);
        }

        public static List<ProcessedLogInfo> PreprocessLogClass(List<LogInfo> plainLogs)
        {
            List<ProcessedLogInfo> processedList = new List<ProcessedLogInfo>();
            
            LogInfo prevLog = new LogInfo();

            foreach (LogInfo plainLog in plainLogs)
            {
                //no formatting, already good
                //int connectionId = plainLog.ConnectionId;
                int responseBytes = plainLog.ResponseBytes;
                int responseStatus = plainLog.ResponseStatus;
                int connectionRequests = plainLog.ConnectionRequests;
                string timestamp = plainLog.Timestamp;
                string remoteAdress = plainLog.RemoteAdress;
                string millis = plainLog.Millis;

                //GET = 0, POST = 1, PUT = 3 ... 
                int requestVerb = Converters.RequestVerbConverter(plainLog.RequestVerb);
                //true = 1, false = 0
                int nginxAccess = (plainLog.NginxAccess) ? 1 : 0;
                //refferer prev = prev curr => 1 else 0
                int httpRefferer = (plainLog.HttpReferer == prevLog.HttpReferer) ? 1 : 0;
                

                ProcessedLogInfo processedLog = new ProcessedLogInfo(connectionRequests, requestVerb, responseStatus,
                    responseBytes, nginxAccess, httpRefferer, millis, timestamp, remoteAdress);

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
            IMongoCollection<BsonDocument> collection = db.Database.GetCollection<BsonDocument>("Copy_of_logs");

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

        [Obsolete]
        public static void FixTimeStampFormat(MongoConnectionClass db)
        {
            IMongoCollection<LogInfo> collection = db.Database.GetCollection<LogInfo>("Copy_of_logs");

            long amount = collection.Count(new BsonDocument());
            int pack = (int) Math.Round((double)amount / 100);

            for (int i = 0; i < 100 + 1; i++)
            {
                List<LogInfo> data =  collection.Find(new BsonDocument()).Skip(pack * i).Limit(pack)
                    .ToList();

                foreach (LogInfo log in data)
                {
                   long result = Converters.ParseDate(log.Timestamp);

                    collection.UpdateOne(new BsonDocument("_id", log.Id),                       
                       new BsonDocument("$set", new BsonDocument("unix_timestamp", result)), new UpdateOptions { IsUpsert = true }); 
                }

                Console.WriteLine("finished iteration : " + i);
            }
        }
    }
}
