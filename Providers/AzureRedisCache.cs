namespace Starship.Azure.Providers {
    /*public class AzureRedisCache {
        public AzureRedisCache(string name, string password) {
            Name = name;
            Password = password;
        }

        static AzureRedisCache() {
            Connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        }

        public RedisValue Increment(string key) {
            return GetDatabase().StringIncrement(key);
        }

        public RedisValue Get(string key) {
            return GetDatabase().StringGet(key);
        }

        private IDatabase GetDatabase() {
            var connectionstring = string.Format("{0}.redis.cache.windows.net,ssl=true,password={1}", Name, Password);
            var connection = Connections.GetOrAdd(connectionstring, add => ConnectionMultiplexer.Connect(connectionstring));
            return connection.GetDatabase();
        }

        private string Password { get; set; }

        private string Name { get; set; }

        private static ConcurrentDictionary<string, ConnectionMultiplexer> Connections { get; set; }
    }*/
}