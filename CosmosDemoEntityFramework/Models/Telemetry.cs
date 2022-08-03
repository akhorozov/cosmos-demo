namespace CosmosDemoEntityFramework;
using System.Text.Json.Serialization;

public class Telemetry
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string MonitorId { get; set; } = "";
    public ICollection<Reading>? Readings
    {
        get;
        set;
    }
    public string Notes { get; set; } = "";
    public MonitorStatus MonitorStatus { get; set; }
}