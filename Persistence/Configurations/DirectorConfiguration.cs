using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DirectorConfiguration : IEntityTypeConfiguration<Director>
{
    public void Configure(EntityTypeBuilder<Director> builder)
    {
        builder.ToTable("Directors");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
               .ValueGeneratedOnAdd();

        builder.Property(d => d.FirstName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(d => d.LastName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(d => d.DateOfBirth)
               .IsRequired();

        builder.Property(d => d.Created)
               .IsRequired()
               .ValueGeneratedOnAdd();

        builder.Property(d => d.LastModified)
               .IsRequired()
               .ValueGeneratedOnUpdate();

        builder
            .HasMany(d => d.Movies)
            .WithOne(m => m.Director)
            .HasForeignKey(m => m.DirectorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(d => new { d.FirstName, d.LastName });
    }
}
