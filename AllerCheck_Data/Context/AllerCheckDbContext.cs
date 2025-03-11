using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllerCheck_Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AllerCheck_Data.Context
{   
    public partial class AllerCheckDbContext : DbContext
    {
        public AllerCheckDbContext(DbContextOptions<AllerCheckDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Content> Contents { get; set; }
        public virtual DbSet<ContentProduct> ContentProducts { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<BlackList> BlackLists { get; set; }
        public virtual DbSet<FavoriteList> FavoriteLists { get; set; }
        public virtual DbSet<FavoriteListDetail> FavoriteListDetails { get; set; }
        public virtual DbSet<Producer> Producers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<RiskStatus> RiskStatuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=ALITUMER;Database=AllerCheckDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");
                entity.Property(e => e.AdminUsername).HasMaxLength(50);
                entity.Property(e => e.AdminMail).HasMaxLength(50);
                entity.Property(e => e.AdminPassword).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.Property(e => e.UserName).HasMaxLength(60);
                entity.Property(e => e.UserSurname).HasMaxLength(60);
                entity.Property(e => e.MailAdress).HasMaxLength(350);
                entity.Property(e => e.UserPassword).HasMaxLength(50);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");
                entity.Property(e => e.CategoryName).HasMaxLength(75);

                entity.HasOne(d => d.TopCategory)
                    .WithMany(p => p.InverseTopCategory)
                    .HasForeignKey(d => d.TopCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Content>(entity =>
            {
                entity.ToTable("Content");
                entity.Property(e => e.ContentName).HasMaxLength(100);
                entity.Property(e => e.ContentInfo).HasMaxLength(1500);

                entity.HasOne(d => d.RiskStatus)
                    .WithMany(p => p.Contents)
                    .HasForeignKey(d => d.RiskStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ContentProduct>(entity =>
            {
                entity.ToTable("ContentProduct");

                entity.HasOne(d => d.Content)
                    .WithMany(p => p.ContentProducts)
                    .HasForeignKey(d => d.ContentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ContentProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contact");
                entity.Property(e => e.Message).HasMaxLength(500);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<BlackList>(entity =>
            {
                entity.ToTable("BlackList");

                entity.HasOne(d => d.Content)
                    .WithMany(p => p.BlackLists)
                    .HasForeignKey(d => d.ContentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BlackLists)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FavoriteList>(entity =>
            {
                entity.ToTable("FavoriteList");
                entity.Property(e => e.ListName).HasMaxLength(100);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FavoriteLists)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FavoriteListDetail>(entity =>
            {
                entity.ToTable("FavoriteListDetail");

                entity.HasOne(d => d.FavoriteList)
                    .WithMany(p => p.FavoriteListDetails)
                    .HasForeignKey(d => d.FavoriteListId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.FavoriteListDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Producer>(entity =>
            {
                entity.ToTable("Producer");
                entity.Property(e => e.ProducerName).HasMaxLength(100);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");
                entity.Property(e => e.ProductName).HasMaxLength(150);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Producer)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProducerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<RiskStatus>(entity =>
            {
                entity.ToTable("RiskStatus");
                entity.Property(e => e.RiskStatusName).HasMaxLength(25);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
