using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using DiplomaProject.Helpers;
using Newtonsoft.Json.Linq;
using DiplomaProject.Mongo.Connection;
using DiplomaProject.Mongo.Models;
using Newtonsoft.Json;
using ServiceStack.Redis;

namespace DiplomaProject
{
    internal class Program
    {
        private static readonly MongoConnectionClass DbClass = new MongoConnectionClass();
        private const string FilePath = @"C:\Users\Artem Chabanovskiy\Envs\recurrent\data.json";
        private const string UrlPath = @"http://127.0.0.1:5000/train/data.json";

        private static void Main(string[] args)
        {
            string mode = Environment.GetEnvironmentVariable("APP_ENV") ?? "manual";

            if (mode == "manual")
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Invalid amount of parameters");
                    return;
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

                string json = JsonConvert.SerializeObject(resultList);
                File.WriteAllText(FilePath, json);
                Thread.Sleep(5 * 1000);

                string result = new WebClient().DownloadString(UrlPath);
                Console.WriteLine(result);

                JToken jObj = JToken.Parse(result);
                Console.WriteLine(jObj);
            }
            else
            {
                using (RedisClient redisConsumer = new RedisClient(Environment.GetEnvironmentVariable("redis") ?? "localhost"))
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

                        string json = JsonConvert.SerializeObject(resultList);
                        File.WriteAllText(FilePath, json);
                        Thread.Sleep(5 * 1000);

                        string result = new WebClient().DownloadString(UrlPath);
                        Console.WriteLine(result);

                        JToken jObj = JToken.Parse(result);
                        Console.WriteLine(jObj);
                    };

                    subscription.SubscribeToChannels("main");
                }
            }
        }
    }
}
