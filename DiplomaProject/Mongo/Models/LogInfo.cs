using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DiplomaProject.Mongo.Models
{   
    internal class LogInfo
    {
        [BsonId]                             public ObjectId Id;
        [BsonElement("from_nginx")]          public string FromNginx;
        [BsonElement("gl2_remote_ip")]       public string GlRemoteIp;
        [BsonElement("gl2_remote_port")]     public string GlRemotePort;
        [BsonElement("connection_requests")] public int ConnectionRequests;
        [BsonElement("source")]              public string Source;
        [BsonElement("gl2_source_input")]    public string SourceInput;
        [BsonElement("http_user_agent")]     public string HttpUserAgent;
        [BsonElement("remote_user")]         public string RemoteUser;
        [BsonElement("gl2_source_node")]     public string SourceNode;
        [BsonElement("timestamp")]           public string Timestamp;
        [BsonElement("request_verb")]        public string RequestVerb;
        [BsonElement("remote_addr")]         public string RemoteAdress;
        [BsonElement("response_status")]     public int ResponseStatus;
        [BsonElement("level")]               public int Level;
        [BsonElement("streams")]             public string[] Streams;
        [BsonElement("http_version")]        public string HttpVersion;
        [BsonElement("response_bytes")]      public int ResponseBytes;
        [BsonElement("message")]             public string Message;
        [BsonElement("nginx_access")]        public string NginxAccess;
        [BsonElement("cs")]                  public string Cs;
        [BsonElement("connection_id")]       public int ConnectionId;
        [BsonElement("http_referer")]        public string HttpReferer;
        [BsonElement("request_path")]        public string RequestPath;
        [BsonElement("millis")]              public float Millis;
        [BsonElement("facility")]            public int Facility;
    }
}
