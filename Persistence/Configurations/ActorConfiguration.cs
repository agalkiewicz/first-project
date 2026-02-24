using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.ToTable("Actors");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
               .ValueGeneratedOnAdd();

        builder.Property(a => a.FirstName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(a => a.LastName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(a => a.DateOfBirth)
               .IsRequired();

        builder.Property(a => a.Created)
               .IsRequired()
               .ValueGeneratedOnAdd();

        builder.Property(a => a.LastModified)
               .IsRequired()
               .ValueGeneratedOnUpdate();

        builder
            .HasMany(a => a.Movies)
            .WithMany(m => m.Actors)
            .UsingEntity<Dictionary<string, object>>(
                "MovieActors",
                right => right.HasOne<Movie>()
                    .WithMany()
                    .HasForeignKey("MovieId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<Actor>()
                    .WithMany()
                    .HasForeignKey("ActorId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("MovieActors");
                    join.HasKey("MovieId", "ActorId");
                });

        builder.HasIndex(a => new { a.FirstName, a.LastName });
    }
}
