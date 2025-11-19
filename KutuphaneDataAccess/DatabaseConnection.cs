using Katmanli.DataAccess.Entities;
using KutuphaneCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KutuphaneDataAccess
{
    public class DatabaseConnection: DbContext
    {


        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UploadImage> UploadImages { get; set; }
        public DbSet<BorrowRequest> BorrowRequests { get; set; }

        public DatabaseConnection(DbContextOptions<DatabaseConnection> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           // optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BackendUdemy;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }


        //
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UploadImage>(e =>
            {
                e.ToTable("UploadImages");      // <— KRİTİK
                e.HasIndex(x => x.FileKey).IsUnique();
                e.Property(x => x.FileKey).HasMaxLength(64).IsRequired();
                e.Property(x => x.Base64Data).HasColumnType("nvarchar(max)").IsRequired();
                e.Property(x => x.ResimYolu).HasColumnType("varchar(512)");
            });

            modelBuilder.Entity<Book>(e =>
            {
                e.Property(x => x.TotalCopies).HasDefaultValue(1);
                // İstersen başka Book config’lerin varsa burada tut
            });

            // YENİ: BorrowRequest – ilişki, tarih tipleri ve indeksler
            modelBuilder.Entity<BorrowRequest>(e =>
            {
                e.HasOne(x => x.Book)
                 .WithMany(x => x.BorrowRequests)
                 .HasForeignKey(x => x.BookId)
                 .OnDelete(DeleteBehavior.Restrict);

                // EF Core 8: DateOnly -> 'date'
                e.Property(x => x.StartDate).HasColumnType("date");
                e.Property(x => x.EndDate).HasColumnType("date");

                e.HasIndex(x => new { x.BookId, x.Status, x.StartDate, x.EndDate });
                e.HasIndex(x => new { x.UserId, x.Status });
            });
        }

    }

}
