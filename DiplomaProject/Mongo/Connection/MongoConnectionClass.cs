using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace DiplomaProject.Mongo.Connection
{
    internal class MongoConnectionClass
    {
        private readonly string _databaseName = Environment.GetEnvironmentVariable("DB_NAME") ?? "local";
        private readonly string _host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        private readonly int _port = Environment.GetEnvironmentVariable("DB_PORT") != null ? Convert.ToInt32(Environment.GetEnvironmentVariable("DB_PORT")) :  27017;

        public IMongoDatabase Database;

        public MongoConnectionClass()
        {
            MongoClientSettings mongoClientSettings = new MongoClientSettings
            {
                Server = new MongoServerAddress(_host, _port)
            };

            MongoClient client = new MongoClient(mongoClientSettings);

            Database = client.GetDatabase(_databaseName);
        }
    }
}
