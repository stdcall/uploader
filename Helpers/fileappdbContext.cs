using Microsoft.EntityFrameworkCore;
using helloworld.Entities;
namespace helloworld.Helpers
{
    public partial class fileappdbContext : DbContext
    {
        public fileappdbContext()
        {
        }

        public fileappdbContext(DbContextOptions<fileappdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Upload> Uploads { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pgcrypto");

            modelBuilder.Entity<Upload>(entity =>
            {
                entity.ToTable("uploads");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Blob).HasColumnName("blob");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.Owner).HasColumnName("owner");

                entity.Property(e => e.Sha256).HasColumnName("sha256");

                entity.HasOne(d => d.OwnerNavigation)
                    .WithMany(p => p.Uploads)
                    .HasForeignKey(d => d.Owner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("uploads_owner_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.Email)
                    .HasName("users_email_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(255);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password");

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasColumnName("salt");
            });
        }
    }
}
