using Microsoft.EntityFrameworkCore;
using ErpApi.Models;

namespace ErpApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
=======
using ERPBackend.Models;

namespace ERPBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
/// cursor/develop-net-core-erp-backend-api-2f37
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
=======
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
/// erp
        {
        }

        public DbSet<User> Users { get; set; }
/// cursor/develop-net-core-erp-backend-api-2f37
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
=======
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
////// erp
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.Items)
                .WithOne(ii => ii.Invoice)
                .HasForeignKey(ii => ii.InvoiceId);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<InvoiceItem>()
                .Property(ii => ii.Price)
                .HasColumnType("decimal(18,2)");
=======

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
//// cursor/develop-net-core-erp-backend-api-2f37
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ShopName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.GST).IsRequired().HasMaxLength(15);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(15);
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.ItemName);
                entity.Property(e => e.ItemName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.StockQuantity).IsRequired();
                entity.Property(e => e.Case).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.GST).IsRequired().HasColumnType("decimal(5,2)");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Category).HasMaxLength(50);
            });

            // Customer configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.PhoneNumber);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(15);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.GST).HasMaxLength(15);
                entity.Property(e => e.Email).HasMaxLength(100);
=======
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.ShopName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
                entity.Property(e => e.GST).HasMaxLength(15);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(15);
            });

            // Customer configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.PhoneNumber).HasMaxLength(15);
                entity.Property(e => e.GST).HasMaxLength(15);
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ItemName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.StockQuantity).IsRequired();
                entity.Property(e => e.Case).HasMaxLength(50);
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(10,2)");
                entity.Property(e => e.GST).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.SKU).HasMaxLength(50);
                entity.HasIndex(e => e.SKU).IsUnique();
///// erp
            });

            // Invoice configuration
            modelBuilder.Entity<Invoice>(entity =>
            {
//// cursor/develop-net-core-erp-backend-api-2f37
                entity.HasIndex(e => e.InvoiceNumber).IsUnique();
                entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.SubTotal).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.GSTAmount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
=======
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.InvoiceDate).IsRequired();
                entity.Property(e => e.SubTotal).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalGST).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.HasIndex(e => e.InvoiceNumber).IsUnique();

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Invoices)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
//// erp
            });

            // InvoiceItem configuration
            modelBuilder.Entity<InvoiceItem>(entity =>
            {
////cursor/develop-net-core-erp-backend-api-2f37
                entity.Property(e => e.ItemName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Case).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.GST).IsRequired().HasColumnType("decimal(5,2)");
                entity.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");
=======
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ItemName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Case).HasMaxLength(50);
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(10,2)");
                entity.Property(e => e.GST).HasColumnType("decimal(5,2)");
                entity.Property(e => e.LineTotal).HasColumnType("decimal(10,2)");
///// erp

                entity.HasOne(e => e.Invoice)
                    .WithMany(i => i.InvoiceItems)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
//// cursor/develop-net-core-erp-backend-api-2f37
                    .WithMany()
=======
                    .WithMany(p => p.InvoiceItems)
//// erp
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

/// cursor/develop-net-core-erp-backend-api-2f37
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is User || e.Entity is Product || e.Entity is Customer || e.Entity is Invoice)
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var entity = entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    if (entity is User user)
                    {
                        user.CreatedAt = DateTime.UtcNow;
                        user.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (entity is Product product)
                    {
                        product.CreatedAt = DateTime.UtcNow;
                        product.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (entity is Customer customer)
                    {
                        customer.CreatedAt = DateTime.UtcNow;
                        customer.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (entity is Invoice invoice)
                    {
                        invoice.CreatedAt = DateTime.UtcNow;
                        invoice.UpdatedAt = DateTime.UtcNow;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entity is User user)
                    {
                        user.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (entity is Product product)
                    {
                        product.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (entity is Customer customer)
                    {
                        customer.UpdatedAt = DateTime.UtcNow;
                    }
                    else if (entity is Invoice invoice)
                    {
                        invoice.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }
=======
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is User || e.Entity is Customer || e.Entity is Product || e.Entity is Invoice)
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    if (entityEntry.Entity is User user)
                        user.CreatedAt = DateTime.UtcNow;
                    else if (entityEntry.Entity is Customer customer)
                        customer.CreatedAt = DateTime.UtcNow;
                    else if (entityEntry.Entity is Product product)
                        product.CreatedAt = DateTime.UtcNow;
                    else if (entityEntry.Entity is Invoice invoice)
                        invoice.CreatedAt = DateTime.UtcNow;
                }

                if (entityEntry.State == EntityState.Modified)
                {
                    if (entityEntry.Entity is User user)
                        user.UpdatedAt = DateTime.UtcNow;
                    else if (entityEntry.Entity is Customer customer)
                        customer.UpdatedAt = DateTime.UtcNow;
                    else if (entityEntry.Entity is Product product)
                        product.UpdatedAt = DateTime.UtcNow;
                    else if (entityEntry.Entity is Invoice invoice)
                        invoice.UpdatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
//// erp
        }
    }
}