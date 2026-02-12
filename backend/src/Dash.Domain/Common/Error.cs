namespace Dash.Domain.Common;

public record Error(string Code, string Description)
{
    // No Error Helper
    public static readonly Error None = new(string.Empty, string.Empty);

    // Null Error Helper
    public static readonly Error NullValue = new("Error.NullValue", "The specified value is null.");
}

