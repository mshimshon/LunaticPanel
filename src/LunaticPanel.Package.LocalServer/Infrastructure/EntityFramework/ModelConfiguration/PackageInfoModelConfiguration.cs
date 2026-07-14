using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models;
using LuncaticPanel.Package.Server.Domain.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.ModelConfiguration;

internal class PackageInfoModelConfiguration : IEntityTypeConfiguration<PackageInfoModel>
{
    public void Configure(EntityTypeBuilder<PackageInfoModel> builder)
    {
        builder.HasKey(p => new { p.PackageId, p.Version });
        builder.HasOne(x => x.Package).WithMany(p => p.Versions).HasForeignKey(x => x.PackageId);
        builder.Property(p => p.Copyright).HasMaxLength(DomainValidationExt.PKG_COPYRIGHT_MAX_LENGTH);
        builder.Property(p => p.PanelVersion).IsRequired();
        builder.Property(p => p.DotnetVersion).IsRequired();
        builder.Property(p => p.Author).IsRequired().HasMaxLength(DomainValidationExt.PKG_AUTHOR_MAX_LENGTH);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(DomainValidationExt.PKG_DESC_MAX_LENGTH);
        builder.Property(p => p.PluginEntryFile).IsRequired();
        builder.Property(p => p.Title).IsRequired().HasMaxLength(DomainValidationExt.PKG_TITLE_MAX_LENGTH);
        builder.Property(p => p.Version).IsRequired();
        builder.Property(p => p.Created)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(p => p.Updated)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsConcurrencyToken();
    }
}
