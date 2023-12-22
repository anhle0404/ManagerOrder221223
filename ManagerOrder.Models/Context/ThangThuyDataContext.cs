using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ManagerOrder.Models.Entities;

#nullable disable

namespace ManagerOrder.Models.Context
{
    public partial class ThangThuyDataContext : DbContext
    {
        public ThangThuyDataContext()
        {
        }

        public ThangThuyDataContext(DbContextOptions<ThangThuyDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<HistoryOrder> HistoryOrders { get; set; }
        public virtual DbSet<HistoryOrderDetail> HistoryOrderDetails { get; set; }
        public virtual DbSet<RegisterCustomer> RegisterCustomers { get; set; }
        public virtual DbSet<RegisterProduct> RegisterProducts { get; set; }
        public virtual DbSet<RegisterUser> RegisterUsers { get; set; }
        public virtual DbSet<Unit> Units { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=D:\\MyProject\\ManagerOrder151223\\ManagerOrder.Models\\Database\\ThangThuyData.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HistoryOrder>(entity =>
            {
                entity.ToTable("HistoryOrder");

                entity.HasIndex(e => e.OrderCode, "IDX_HistoryOrder_OrderCode");

                entity.HasIndex(e => e.CreatedDate, "Idx_HistoryOrder_CreatedDate");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.SalesStaffId).HasColumnName("SalesStaffID");

                entity.Property(e => e.ShipperId).HasColumnName("ShipperID");
            });

            modelBuilder.Entity<HistoryOrderDetail>(entity =>
            {
                entity.ToTable("HistoryOrderDetail");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.HistoryOrderId).HasColumnName("HistoryOrderID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");
            });

            modelBuilder.Entity<RegisterCustomer>(entity =>
            {
                entity.ToTable("RegisterCustomer");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<RegisterProduct>(entity =>
            {
                entity.ToTable("RegisterProduct");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<RegisterUser>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Unit");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
