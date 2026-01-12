using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Dash.Infrastructure.Conventions;

public class SnakeCaseNamingConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(
            IConventionModelBuilder modelBuilder,
            IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var entity in modelBuilder.Metadata.GetEntityTypes())
        {
            // Table names
            entity.SetTableName(ToSnakeCase(entity.GetTableName()));

            // Column names
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.GetColumnName()));
            }

            // Primary keys
            foreach (var key in entity.GetKeys())
            {
                key.SetName(ToSnakeCase(key.GetName()));
            }

            // Foreign keys
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(ToSnakeCase(foreignKey.GetConstraintName()));
            }

            // Indexes
            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()));
            }
        }
    }

    private static string ToSnakeCase(string? name)
    {
        if (string.IsNullOrEmpty(name))
            return name ?? string.Empty;

        return string.Concat(
            name.Select((c, i) => i > 0 && char.IsUpper(c)
                ? "_" + c.ToString()
                : c.ToString())
        ).ToLower();
    }
}
