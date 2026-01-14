namespace Dash.Infrastructure.Options;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    public required string Provider { get; set; }
    public required string ConnectionString { get; set; }
}

