using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiplomaProject.Exapmles;
using DiplomaProject.Helpers;
using Newtonsoft.Json.Linq;
using DiplomaProject.Mongo.Connection;
using DiplomaProject.Mongo.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceStack.Redis;

namespace DiplomaProject
{
    internal class Program
    {
        private static readonly MongoConnectionClass DbClass = new MongoConnectionClass();

        /*
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Invalid amount of parameters");
            }

            long result0;
            long result1;

            bool firstArgParse = long.TryParse(args[0], out result0);
            bool secondArgParse = long.TryParse(args[1], out result1);

            bool successParce = firstArgParse && secondArgParse;
            if (!successParce || (result0 > result1))
            {
                Console.WriteLine("Invalid arguments");
                return;
            }

            List<ProcessedLogInfo> resultList = DataHelper.GetAndProcessData(DbClass, result0, result1);

            if (!resultList.Any())
            {
                Console.WriteLine("No data on this dates");
                return;
            }

            Console.WriteLine(resultList.Count() % 2 == 0 ? "normal state of system" : "anomaly state");
        }
        */

        private static void Main(string[] args)
        {

            using (var redisConsumer = new RedisClient(Environment.GetEnvironmentVariable("redis") ?? "localhost"))
            using (IRedisSubscription subscription = redisConsumer.CreateSubscription())
            {
                subscription.OnSubscribe = channel =>
                {
                    Console.WriteLine($"Subscribed to '{channel}'");
                };
                subscription.OnUnSubscribe = channel =>
                {
                    Console.WriteLine($"UnSubscribed from '{channel}'");
                };
                subscription.OnMessage = (channel, msg) =>
                {
                    JToken obj = JToken.Parse(msg);

                    List<ProcessedLogInfo> resultList = 
                                    DataHelper.GetAndProcessData(DbClass, Convert.ToInt64(obj["date_low"]), Convert.ToInt64(obj["date_upper"]));

                    if (!resultList.Any())
                    {
                        Console.WriteLine("No data on this dates");
                        return;
                    }

                    Console.WriteLine(resultList.Count % 2 == 0 ? "normal state of system" : "anomaly state");
                };

                subscription.SubscribeToChannels("main");
            }
        }
    }
}
