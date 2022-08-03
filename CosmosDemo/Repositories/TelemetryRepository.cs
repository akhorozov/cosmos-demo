using Azure.Cosmos;

namespace CosmosDemo;
public class TelemetryRepository
{
    private readonly IConfiguration _config;
    private readonly ILogger<TelemetryRepository> _logger;
    private readonly IHostEnvironment _enviroment;
    private readonly CosmosClient _client;
    private const string DBNAME = "TelemetryDb";
    private const string CONTAINERNAME = "Telemetry";

    public TelemetryRepository(IConfiguration config, ILogger<TelemetryRepository> logger, IHostEnvironment enviroment)
    {
        _config = config;
        _logger = logger;
        _enviroment = enviroment;
        var connString = _config.GetConnectionString("Cosmos");
        _client = new CosmosClient(connString);
    }

    internal async Task<Telemetry> Create(Telemetry item)
    {
        var container = await GetContainer();
        var result = await container.CreateItemAsync(item, new PartitionKey(item.MonitorId));
        return result.Value;
    }

    internal async Task<List<Telemetry>> GetAll(string monitorId)
    {
        var container = await GetContainer();
        var sql = @"SELECT VALUE c
              FROM c
              WHERE c.MonitorId = @monitorId";

        var query = new QueryDefinition(sql)
            .WithParameter("@monitorId", monitorId);

        var results = new List<Telemetry>();

        var iterator = container
            .GetItemQueryIterator<Telemetry>(query);

        await foreach (Telemetry result in iterator)
        {
            results.Add(result);
        }

        return results;

    }

    internal async Task<Telemetry?> GetById(string id)
    {
        var sql = $"SELECT * FROM c WHERE c.id = @id";
        var query = new QueryDefinition(sql).WithParameter("@id", id);

        var container = await GetContainer();
        var iterator = container.GetItemQueryIterator<Telemetry>(query);

        var enumerator = iterator.GetAsyncEnumerator();
        if (await enumerator.MoveNextAsync())
        {
            return enumerator.Current;
        }
        return null;
    }

    internal async Task<Telemetry> Update(Telemetry item)
    {
        var container = await GetContainer();
        var result = await container.ReplaceItemAsync(item, item.Id);
        return result.Value;
    }

    internal async Task<Telemetry> Delete(Telemetry item)
    {
        var container = await GetContainer();
        var result = await container.DeleteItemAsync<Telemetry>(item.Id, new PartitionKey(item.MonitorId));
        return result.Value;
    }
    async Task InitializeDatabaseAsync()
    {
        if (_enviroment.IsDevelopment())
        {
            var result = await _client.CreateDatabaseIfNotExistsAsync(DBNAME);
            await result.Database.CreateContainerIfNotExistsAsync(
                CONTAINERNAME, "/MonitorId");
        }
    }

    async Task<CosmosContainer> GetContainer()
    {
        await InitializeDatabaseAsync();
        return _client.GetContainer(DBNAME, CONTAINERNAME);
    }

}