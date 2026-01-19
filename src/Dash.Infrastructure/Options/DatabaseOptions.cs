using Dash.Infrastructure.Enums;

namespace Dash.Infrastructure.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";
    public required DatabaseProvider Provider { get; set; }
    public required string ConnectionString { get; set; }
}

