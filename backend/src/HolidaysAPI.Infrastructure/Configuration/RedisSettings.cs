namespace HolidaysAPI.Infrastructure.Configuration;

public class RedisSettings
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; set; } = string.Empty;
    
    public int DefaultExpirationMinutes { get; set; } = 1440;
    
    public bool AbortOnConnectFail { get; set; } = false;
    
    public int ConnectTimeout { get; set; } = 5000;
    
    public int SyncTimeout { get; set; } = 5000;
    
    public int ConnectRetry { get; set; } = 3;
    
    public bool Enabled { get; set; } = true;
}

