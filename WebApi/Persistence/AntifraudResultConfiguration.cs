using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApi.Persistence;

public class AntifraudResultConfiguration : IEntityTypeConfiguration<AntifraudResult>
{
    public void Configure(EntityTypeBuilder<AntifraudResult> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Embedding)
            .HasColumnType("vector(14)");
        builder.HasIndex(x => x.Embedding);
        
        builder.Property(x => x.Label)
            .HasMaxLength(20);
    }
}