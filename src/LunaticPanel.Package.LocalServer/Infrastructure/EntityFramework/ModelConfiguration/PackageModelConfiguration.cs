using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models;
using LuncaticPanel.Package.Server.Domain.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Globalization;

namespace LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.ModelConfiguration;

internal sealed class PackageModelConfiguration : IEntityTypeConfiguration<PackageModel>
{
    public void Configure(EntityTypeBuilder<PackageModel> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired().HasMaxLength(DomainValidationExt.PKG_ID_MAX_LENGTH);
        builder.Property(p => p.Updated).HasConversion(p => p.ToString(), p => DateTime.Parse(p, CultureInfo.InvariantCulture));
        builder.Property(p => p.Created).HasConversion(p => p.ToString(), p => DateTime.Parse(p, CultureInfo.InvariantCulture));
        builder.Property(p => p.Created)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(p => p.Updated)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsConcurrencyToken();
    }
}
