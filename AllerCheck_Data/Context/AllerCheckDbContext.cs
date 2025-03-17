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
                entity.Property(e => e.UserId).HasColumnName("uyeid");
                entity.Property(e => e.UserName).HasColumnName("uyeadi");
                entity.Property(e => e.UserSurname).HasColumnName("uyesoyadi");
                entity.Property(e => e.MailAdress).HasColumnName("mailadresi");
                entity.Property(e => e.UserPassword).HasColumnName("uyesifre");
                entity.Property(e => e.CreatedDate).HasColumnName("createddate");
                entity.Property(e => e.UyelikTipiId).HasColumnName("UyelikTipiId");
                entity.Property(e => e.CreatedBy).HasColumnName("CreatedBy");

                entity.HasOne(d => d.UyelikTipi)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UyelikTipiId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Uye_UyelikTipi");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("kategori");
                entity.Property(e => e.CategoryId).HasColumnName("KategoriId");
                entity.Property(e => e.CategoryName).HasColumnName("KategoriAdi");
                entity.Property(e => e.TopCategoryId).HasColumnName("UstKategoriId");
                entity.HasOne(d => d.TopCategory)
                    .WithMany(p => p.InverseTopCategory)
                    .HasForeignKey(d => d.TopCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Content>(entity =>
            {
                entity.ToTable("icerik");
                entity.Property(e => e.ContentId).HasColumnName("IcerikId");
                entity.Property(e => e.ContentName).HasColumnName("IcerikAdi");
                entity.Property(e => e.ContentInfo).HasColumnName("IcerikBilgi");
                entity.Property(e => e.RiskStatusId)
                    .HasColumnName("RiskDurumId");

                entity.HasOne(d => d.RiskStatus)
                    .WithMany(p => p.Contents)
                    .HasForeignKey(d => d.RiskStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Icerik_RiskDurumu");
            });

            modelBuilder.Entity<ContentProduct>(entity =>
            {
                entity.ToTable("urunicerik");
                entity.Property(e => e.ContentProductId).HasColumnName("UrunIcerikId");
                entity.Property(e => e.ContentId).HasColumnName("IcerikId");
                entity.Property(e => e.ProductId).HasColumnName("UrunId");

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
                entity.ToTable("Iletisim");
                entity.Property(e => e.ContactId).HasColumnName("IletisimId");
                entity.Property(e => e.Message).HasColumnName("Mesaj");
                entity.Property(e => e.UserId).HasColumnName("UyeId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<BlackList>(entity =>
            {
                entity.ToTable("karaliste");
                entity.Property(e => e.BlackListId).HasColumnName("KaraListeId");
                entity.Property(e => e.UserId).HasColumnName("UyeId");
                entity.Property(e => e.ContentId).HasColumnName("IcerikId");

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
                entity.Property(e => e.FavoriteListId).HasColumnName("FavoriListesiId");
                entity.Property(e => e.ListName).HasColumnName("ListeAdi");
                entity.Property(e => e.UserId).HasColumnName("UyeId");

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
                entity.Property(e => e.ProducerId).HasColumnName("UreticiId");
                entity.Property(e => e.ProducerName).HasColumnName("UreticiAdi");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("urun");
                entity.Property(e => e.ProductId).HasColumnName("UrunId");
                entity.Property(e => e.ProductName).HasColumnName("UrunAdi");
                entity.Property(e => e.CategoryId).HasColumnName("KategoriId");
                entity.Property(e => e.ProducerId).HasColumnName("UreticiId");
                entity.Property(e => e.UserId).HasColumnName("UyeId");
                entity.Property(e => e.CreatedDate).HasColumnName("UrunKayitTarihi");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

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
                entity.Property(e => e.RiskStatusId).HasColumnName("RiskDurumId");
                entity.Property(e => e.RiskStatusName).HasColumnName("RiskSeviyesi");
            });

            modelBuilder.Entity<UyelikTipi>(entity =>
            {
                entity.ToTable("uyeliktipi");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
