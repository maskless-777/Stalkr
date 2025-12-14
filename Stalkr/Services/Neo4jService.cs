using Neo4j.Driver;

namespace Stalkr.Services
{
    public class Neo4jService : IAsyncDisposable
    {
        private readonly IDriver _driver;

        public Neo4jService(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public IAsyncSession Session()
            => _driver.AsyncSession();

        public async ValueTask DisposeAsync()
            => await _driver.DisposeAsync();
    }
}
