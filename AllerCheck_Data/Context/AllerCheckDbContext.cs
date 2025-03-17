using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllerCheck_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace AllerCheck_Data.Context
{   
    public partial class AllerCheckDbContext : DbContext
    {
        public AllerCheckDbContext(DbContextOptions<AllerCheckDbContext> options) : base(options)
        {
        }

        public virtual DbSet<BlackList> BlackLists { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Content> Contents { get; set; }
        public virtual DbSet<FavoriteList> FavoriteLists { get; set; }
        public virtual DbSet<Producer> Producers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ContentProduct> ContentProducts { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UyelikTipi> UyelikTipleri { get; set; }
        public virtual DbSet<FavoriteListDetail> FavoriteListDetails { get; set; }
        public virtual DbSet<RiskStatus> RiskStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("uye");
                entity.Property(e => e.UserId).HasColumnName("Id");
                entity.Property(e => e.UserName).HasColumnName("UserName");
                entity.Property(e => e.UserSurname).HasColumnName("UserSurname");
                entity.Property(e => e.MailAdress).HasColumnName("MailAdress");
                entity.Property(e => e.UserPassword).HasColumnName("UserPassword");
                entity.Property(e => e.CreatedDate).HasColumnName("UyelikTarihi");
                entity.Property(e => e.UyelikTipiId).HasColumnName("UyelikTipiId");
                entity.Property(e => e.CreatedBy).HasColumnName("CreatedBy");

                entity.HasOne(d => d.UyelikTipi)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UyelikTipiId)
                    .HasConstraintName("fk_uye_uyeliktipi");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("kategori");
                entity.Property(e => e.CategoryId).HasColumnName("kategoriid");
                entity.Property(e => e.CategoryName).HasColumnName("kategoriadi");
                entity.Property(e => e.TopCategoryId).HasColumnName("ustkategoriid");
                entity.HasOne(d => d.TopCategory)
                    .WithMany(p => p.InverseTopCategory)
                    .HasForeignKey(d => d.TopCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Content>(entity =>
            {
                entity.ToTable("icerik");
                entity.Property(e => e.ContentId).HasColumnName("icerikid");
                entity.Property(e => e.ContentName).HasColumnName("icerikadi");
                entity.Property(e => e.ContentInfo).HasColumnName("icerikbilgi");
                entity.Property(e => e.RiskStatusId).HasColumnName("riskdurumid");

                entity.HasOne(d => d.RiskStatus)
                    .WithMany(p => p.Contents)
                    .HasForeignKey(d => d.RiskStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_icerik_riskdurumu");
            });

            modelBuilder.Entity<ContentProduct>(entity =>
            {
                entity.ToTable("urunicerik");
                entity.Property(e => e.ContentProductId).HasColumnName("urunicerikid");
                entity.Property(e => e.ContentId).HasColumnName("icerikid");
                entity.Property(e => e.ProductId).HasColumnName("urunid");

                entity.HasOne(d => d.Content)
                    .WithMany(p => p.ContentProducts)
                    .HasForeignKey(d => d.ContentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ContentProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("iletisim");
                entity.Property(e => e.ContactId).HasColumnName("iletisimid");
                entity.Property(e => e.Message).HasColumnName("mesaj");
                entity.Property(e => e.UserId).HasColumnName("uyeid");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<BlackList>(entity =>
            {
                entity.ToTable("karaliste");
                entity.Property(e => e.BlackListId).HasColumnName("karalisteid");
                entity.Property(e => e.UserId).HasColumnName("uyeid");
                entity.Property(e => e.ContentId).HasColumnName("icerikid");

                entity.HasOne(d => d.Content)
                    .WithMany(p => p.BlackLists)
                    .HasForeignKey(d => d.ContentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BlackLists)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FavoriteList>(entity =>
            {
                entity.ToTable("favorilistesi");
                entity.Property(e => e.FavoriteListId).HasColumnName("favorilistesiid");
                entity.Property(e => e.ListName).HasColumnName("listeadi");
                entity.Property(e => e.UserId).HasColumnName("uyeid");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FavoriteLists)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FavoriteListDetail>(entity =>
            {
                entity.ToTable("FavoriListeDetay");
                entity.Property(e => e.FavoriteListDetailId).HasColumnName("FavoriListeDetayId");
                entity.Property(e => e.FavoriteListId).HasColumnName("FavoriListesiId");
                entity.Property(e => e.ProductId).HasColumnName("UrunId");

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
                entity.ToTable("uretici");
                entity.Property(e => e.ProducerId).HasColumnName("ureticiid");
                entity.Property(e => e.ProducerName).HasColumnName("ureticiadi");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("urun");
                entity.Property(e => e.ProductId).HasColumnName("urunid");
                entity.Property(e => e.ProductName).HasColumnName("urunadi");
                entity.Property(e => e.CategoryId).HasColumnName("kategoriid");
                entity.Property(e => e.ProducerId).HasColumnName("ureticiid");
                entity.Property(e => e.UserId).HasColumnName("uyeid");
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.CreatedDate).HasColumnName("createddate");
                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedby");
                entity.Property(e => e.ModifiedDate).HasColumnName("modifieddate");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .IsRequired(false);

                entity.HasOne(d => d.Producer)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProducerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .IsRequired(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .IsRequired(false);
            });

            modelBuilder.Entity<RiskStatus>(entity =>
            {
                entity.ToTable("riskdurumu");
                entity.Property(e => e.RiskStatusId).HasColumnName("riskdurumid");
                entity.Property(e => e.RiskStatusName).HasColumnName("riskseviyesi");
            });

            modelBuilder.Entity<UyelikTipi>(entity =>
            {
                entity.ToTable("uyeliktipi");
                entity.Property(e => e.UyelikTipiId).HasColumnName("uyeliktipiid");
                entity.Property(e => e.TipAdi).HasColumnName("uyeliktipiadi");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
