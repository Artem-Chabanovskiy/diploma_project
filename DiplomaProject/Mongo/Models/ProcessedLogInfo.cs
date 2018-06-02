using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DiplomaProject.Mongo.Models
{
    internal class ProcessedLogInfo
    {
        public int ConnectionRequests;
        public int RequestVerb;
        public int ResponseStatus;
        public int ResponseBytes;
        public int NginxAccess;
        public int HttpReferer;
        public string Millis;
        public string Timestamp;
        public string RemoteAddres;

        public ProcessedLogInfo(int connectionRequests, int requestVerb, int responseStatus,
            int responseBytes, int nginxAccess, int httpReferer, string millis, string timestamp, string remoteAddres)
        {
            ConnectionRequests = connectionRequests;
            RequestVerb = requestVerb;
            ResponseStatus = responseStatus;
            ResponseBytes = responseBytes;
            NginxAccess = nginxAccess;
            HttpReferer = httpReferer;
            Millis = millis;
            Timestamp = timestamp;
            RemoteAddres = remoteAddres;
        }
    }
}
