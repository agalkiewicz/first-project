using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Created)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(c => c.LastModified)
            .IsRequired()
            .ValueGeneratedOnUpdate();

        builder.HasIndex(c => c.Name)
            .IsUnique();
    }
}
