using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Domain.Common;

namespace PersonalSite.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    private static readonly MethodInfo ApplySoftDeleteMethod =
        typeof(ApplicationDbContext).GetMethod(nameof(ApplySoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Instance)!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        ConfigureUtcDateTimes(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;
            if (!clr.IsAbstract && typeof(AuditableEntity).IsAssignableFrom(clr))
                ApplySoftDeleteMethod.MakeGenericMethod(clr).Invoke(this, [modelBuilder]);
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utc = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Property(nameof(AuditableEntity.UpdatedAtUtc)).CurrentValue = utc;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private static void ConfigureUtcDateTimes(ModelBuilder modelBuilder)
    {
        var utc = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var utcNullable = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()))
        {
            if (property.ClrType == typeof(DateTime))
                property.SetValueConverter(utc);
            else if (property.ClrType == typeof(DateTime?))
                property.SetValueConverter(utcNullable);
        }
    }

    private void ApplySoftDeleteFilter<T>(ModelBuilder modelBuilder)
        where T : AuditableEntity =>
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
}
