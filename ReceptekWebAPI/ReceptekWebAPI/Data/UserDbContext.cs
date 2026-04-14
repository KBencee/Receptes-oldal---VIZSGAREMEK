using Microsoft.EntityFrameworkCore;
using ReceptekWebAPI.Entities;
using System.Linq;

namespace ReceptekWebAPI.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Recept> Receptek { get; set; } = null!;
        public DbSet<Cimke> Cimkek { get; set; } = null!;
        public DbSet<ReceptCimke> ReceptCimkek { get; set; } = null!;
        public DbSet<MentettRecept> MentettReceptek { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;

        public DbSet<ReceptKomment> ReceptKommentek { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Receptek)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.UserId, l.ReceptId });

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Recept)
                .WithMany()
                .HasForeignKey(l => l.ReceptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReceptKomment>()
                .HasOne(rk => rk.User)
                .WithMany()
                .HasForeignKey(rk => rk.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReceptKomment>()
                .HasOne(rk => rk.Recept)
                .WithMany(r => r.Kommentek)
                .HasForeignKey(rk => rk.ReceptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReceptKomment>()
                .Property(rk => rk.IrtaEkkor)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Recept>()
                .Property(r => r.FeltoltveEkkor)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<ReceptCimke>()
                .HasKey(rc => new { rc.ReceptId, rc.CimkeId });

            modelBuilder.Entity<ReceptCimke>()
                .HasOne(rc => rc.Recept)
                .WithMany(r => r.ReceptCimkek)
                .HasForeignKey(rc => rc.ReceptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReceptCimke>()
                .HasOne(rc => rc.Cimke)
                .WithMany(c => c.ReceptCimkek)
                .HasForeignKey(rc => rc.CimkeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MentettRecept>()
                .HasKey(mr => new { mr.UserId, mr.ReceptId });

            modelBuilder.Entity<MentettRecept>()
                .HasOne(mr => mr.User)
                .WithMany(u => u.MentettReceptek)
                .HasForeignKey(mr => mr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MentettRecept>()
                .HasOne(mr => mr.Recept)
                .WithMany(r => r.MentettReceptek)
                .HasForeignKey(mr => mr.ReceptId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
