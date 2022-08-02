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

    internal async Task<Telemetry> Create(Telemetry model)
    {
        var container = await GetContainer();
        var result = await container.CreateItemAsync(model, new PartitionKey(model.MonitorId));
        return result.Value;
    }

    internal async Task<List<Telemetry>> GetAll()
    {
        throw new NotImplementedException();
    }

    internal async Task<Telemetry> GetById()
    {
        throw new NotImplementedException();
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
