using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiplomaProject.Mongo.Models;
using Newtonsoft.Json.Linq;

namespace DiplomaProject.Helpers
{
    internal static class Converters
    {
        public static int ParseMillis(string millis)
        {
            string[] splitted = millis.Split(new[] { "." }, StringSplitOptions.None);

            return Convert.ToInt32(splitted.Last()) + (Convert.ToInt32(splitted.First()) * 1000);
        }

        public static long ParseDate(string date)
        {
            //var t = Convert.ToDateTime(date);
            return (long)Convert.ToDateTime(date).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static int[] JSonArray2IntArray(JArray jsonArray)
        {
            int[] intArray = new int[jsonArray.Count()];
            for (int i = 0; i < intArray.Length; ++i)
            {
                intArray[i] = Convert.ToInt32(jsonArray[i]);
            }
            return intArray;
        }

        public static int RequestVerbConverter(string reqVerb)
        {
            switch (reqVerb.ToUpper())
            {
                case "GET":
                    return 0;
                case "POST":
                    return 1;
                case "PUT":
                    return 2;
                case "DELETE":
                    return 3;
                case "HEAD":
                    return 4;
                case "PATCH":
                    return 5;
                case "OPTIONS":
                    return 6;
                case "TRACE":
                    return 7;
                case "CONNECT":
                    return 8;
                default:
                    return 9;
            }
        }
    }
}
