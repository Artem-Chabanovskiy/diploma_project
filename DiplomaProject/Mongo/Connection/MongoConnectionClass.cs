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
        private const string DatabaseName = "DCLogs";
        private const string Host = "localhost";
        private const int Port = 27017;

        public IMongoDatabase Database;

        public MongoConnectionClass()
        {
            var mongoClientSettings = new MongoClientSettings
            {
                Server = new MongoServerAddress(Host, Port)
            };

            var client = new MongoClient(mongoClientSettings);

            Database = client.GetDatabase(DatabaseName);
        }
    }
}
