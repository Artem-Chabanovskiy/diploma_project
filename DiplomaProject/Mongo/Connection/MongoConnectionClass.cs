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
        private const string DatabaseName = "local";
        private const string Host = "localhost";
        private const int Port = 27017;

        public IMongoDatabase Database;

        public MongoConnectionClass()
        {
            MongoClientSettings mongoClientSettings = new MongoClientSettings
            {
                Server = new MongoServerAddress(Host, Port)
            };

            MongoClient client = new MongoClient(mongoClientSettings);

            Database = client.GetDatabase(DatabaseName);
        }
    }
}
