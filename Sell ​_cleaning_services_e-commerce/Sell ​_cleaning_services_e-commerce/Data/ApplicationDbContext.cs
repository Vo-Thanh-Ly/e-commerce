using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sell__cleaning_services_e_commerce.Models;
using System.Data;

namespace Sell_​_cleaning_services_e_commerce.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Comment> Comments { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }

        public virtual DbSet<InvoiceStatus> InvoiceStatuses { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<PaymentLog> PaymentLogs { get; set; }

        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<ProductImage> ProductImages { get; set; }

        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);
            modelBuilder.UseCollation("Vietnamese_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0B12B32348");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFCA9B5E0D6A");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Product).WithMany(p => p.Comments)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Comments__Produc__440B1D61");
            });

            //modelBuilder.Entity<Invoice>(entity =>
            //{
            //    entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAB5A27AD698");

            //    entity.Property(e => e.InvoiceDate).HasDefaultValueSql("(getdate())");

            //    entity.HasOne(d => d.Payment).WithMany(p => p.Invoices).HasConstraintName("FK__Invoices__Paymen__5CD6CB2B");

            //    entity.HasOne(d => d.Status).WithMany(p => p.Invoices)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK__Invoices__Status__49C3F6B7");
            //});

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.InvoiceId)
                      .HasName("PK__Invoices__D796AAB5A27AD698");

                entity.Property(e => e.InvoiceDate)
                      .HasDefaultValueSql("(getdate())");

                // Cấu hình mối quan hệ một-một
                entity.HasOne(d => d.Payment)
                      .WithOne(p => p.Invoice)
                      .HasForeignKey<Payment>(d => d.InvoiceId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK__Invoices__Payment__5CD6CB2B");

                entity.HasOne(d => d.Status)
                      .WithMany(p => p.Invoices)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK__Invoices__Status__49C3F6B7");
            });


            modelBuilder.Entity<InvoiceDetail>(entity =>
            {
                entity.HasKey(e => e.InvoiceDetailId).HasName("PK__InvoiceD__1F157811D0C87BC2");

                entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InvoiceDe__Invoi__4CA06362");

                entity.HasOne(d => d.Product).WithMany(p => p.InvoiceDetails)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InvoiceDe__Produ__4D94879B");
            });

            modelBuilder.Entity<InvoiceStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId).HasName("PK__InvoiceS__C8EE2063B74CDE81");
            });


            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId)
                      .HasName("PK__Payments__9B556A38F2A55A04");

                entity.Property(e => e.PaymentDate)
                      .HasDefaultValueSql("(getdate())");



                entity.HasOne(d => d.PaymentMethod)
                      .WithMany(p => p.Payments)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK__Payments__PaymentMethod__5812160E");
            });


            modelBuilder.Entity<PaymentLog>(entity =>
            {
                entity.HasKey(e => e.LogId).HasName("PK__PaymentL__5E5486488C848839");

                entity.Property(e => e.LogDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Payment).WithMany(p => p.PaymentLogs)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PaymentLo__Payme__5BE2A6F2");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1D3FEB9E358");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CDBF58A32B");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsDiscontinued).HasDefaultValueSql("((0))");
                entity.Property(e => e.LastUpdatedDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Category).WithMany(p => p.Products)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Products__Catego__3C69FB99");
                entity.HasMany(p => p.ProductImages)
                   .WithOne(pi => pi.Product)
                   .HasForeignKey(pi => pi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.ImageId).HasName("PK__ProductI__7516F70CF1FBF9B2");

                entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIm__Produ__3F466844");
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(e => e.CartId).HasName("PK__Shopping__51BCD7B7A342901A");

                entity.Property(e => e.DateAdded).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Product).WithMany(p => p.ShoppingCarts)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ShoppingC__Produ__5165187F");
            });


            // Đổi tên các bảng trong ASP.NET Identity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User"); // Đổi tên bảng IdentityUser
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role"); // Đổi tên bảng IdentityRole
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRole"); // Đổi tên bảng IdentityUserRole
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaim"); // Đổi tên bảng IdentityUserClaim
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogin"); // Đổi tên bảng IdentityUserLogin
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaim"); // Đổi tên bảng IdentityRoleClaim
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserToken"); // Đổi tên bảng IdentityUserToken
            });

            // Cấu hình mối quan hệ giữa Comment và User
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId);

            // Cấu hình mối quan hệ giữa Invoice và User
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.User)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.CustomerId);

            // Cấu hình mối quan hệ giữa ShoppingCart và User
            modelBuilder.Entity<ShoppingCart>()
                .HasOne(sc => sc.User)
                .WithMany(u => u.ShoppingCarts)
                .HasForeignKey(sc => sc.UserId);


        }

    }
}
