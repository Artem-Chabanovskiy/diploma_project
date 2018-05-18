﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiplomaProject.Mongo.Models;

namespace DiplomaProject.Helpers
{
    internal static class Converters
    {
        public static int ParseMillis(string millis)
        {
            string[] splitted = millis.Split(new[] { "," }, StringSplitOptions.None);

            return Convert.ToInt32(splitted.Last()) + (Convert.ToInt32(splitted.First()) * 1000);
        }

        public static long ParseDate(string date)
        {
            return (long)Convert.ToDateTime(date).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static List<Dictionary<int, long>> ConvertLogListToDictionaryList(List<ProcessedLogInfo> logList)
        {
            return logList.Select(ConvertLogToKeyDictionary).ToList();
        }

        public static Dictionary<int, long> ConvertLogToKeyDictionary(ProcessedLogInfo logInfo)
        {
            return new Dictionary<int, long>
            {
                {1, logInfo.ConnectionId },
                {2, logInfo.ConnectionRequests },
                {3, logInfo.RequestVerb },
                {4, logInfo.ResponseStatus },
                {5, logInfo.ResponseBytes },
                {6, logInfo.NginxAccess },
                {7, logInfo.HttpReferer },
                {8, logInfo.Millis },
                {9, logInfo.Timestamp }
            };
        }
    }
}