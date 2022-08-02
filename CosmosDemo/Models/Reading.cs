namespace CosmosDemo;

public class Reading
{
    public DateTime ReadingTime { get; set; } = DateTime.UtcNow;
    public double WindSpeed { get; set; }
    public double Temperature { get; set; }
    public double Altitude { get; set; }
}
