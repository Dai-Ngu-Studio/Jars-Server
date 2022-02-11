using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JARS_DAL.Models
{
    public partial class JarsDatabaseContext : DbContext
    {
        public JarsDatabaseContext()
        {
        }

        public JarsDatabaseContext(DbContextOptions<JarsDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<AccountType> AccountTypes { get; set; } = null!;
        public virtual DbSet<Bill> Bills { get; set; } = null!;
        public virtual DbSet<BillDetail> BillDetails { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<CategoryWallet> CategoryWallets { get; set; } = null!;
        public virtual DbSet<Contract> Contracts { get; set; } = null!;
        public virtual DbSet<Note> Notes { get; set; } = null!;
        public virtual DbSet<ScheduleType> ScheduleTypes { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<Wallet> Wallets { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string DatabasePassword = Environment.GetEnvironmentVariable("SA_PASSWORD");
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("server=0.0.0.0,1433;database=JarsDatabase;uid=sa;pwd=" + DatabasePassword + ";");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.Id)
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.AccountTypeId).HasColumnName("AccountTypeID");

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                entity.Property(e => e.PhotoUrl).IsUnicode(false);

                entity.HasOne(d => d.AccountType)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.AccountTypeId)
                    .HasConstraintName("FK__Account__Account__267ABA7A");
            });

            modelBuilder.Entity<AccountType>(entity =>
            {
                entity.ToTable("AccountType");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bill");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.ContractId).HasColumnName("ContractID");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.LeftAmount).HasColumnType("money");

                entity.Property(e => e.RecurringTransactionId).HasColumnName("RecurringTransactionID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__Bill__CategoryID__3B75D760");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.ContractId)
                    .HasConstraintName("FK__Bill__ContractID__3C69FB99");
            });

            modelBuilder.Entity<BillDetail>(entity =>
            {
                entity.ToTable("BillDetail");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BillId).HasColumnName("BillID");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.HasOne(d => d.Bill)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.BillId)
                    .HasConstraintName("FK__BillDetai__BillI__440B1D61");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ParentCategoryId).HasColumnName("ParentCategoryID");
            });

            modelBuilder.Entity<CategoryWallet>(entity =>
            {
                entity.ToTable("CategoryWallet");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ParentCategoryId).HasColumnName("ParentCategoryID");

                entity.Property(e => e.WalletId).HasColumnName("WalletID");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.CategoryWallets)
                    .HasForeignKey(d => d.WalletId)
                    .HasConstraintName("FK__CategoryW__Walle__2C3393D0");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("Contract");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasColumnName("AccountID");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.NoteId).HasColumnName("NoteID");

                entity.Property(e => e.ScheduleTypeId).HasColumnName("ScheduleTypeID");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Contract__Accoun__36B12243");

                entity.HasOne(d => d.Note)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.NoteId)
                    .HasConstraintName("FK__Contract__NoteID__37A5467C");

                entity.HasOne(d => d.ScheduleType)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.ScheduleTypeId)
                    .HasConstraintName("FK__Contract__Schedu__38996AB5");
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.ToTable("Note");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Image).IsUnicode(false);
            });

            modelBuilder.Entity<ScheduleType>(entity =>
            {
                entity.ToTable("ScheduleType");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.BillId).HasColumnName("BillID");

                entity.Property(e => e.NoteId).HasColumnName("NoteID");

                entity.Property(e => e.TransactionDate).HasColumnType("datetime");

                entity.Property(e => e.WalletId).HasColumnName("WalletID");

                entity.HasOne(d => d.Bill)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.BillId)
                    .HasConstraintName("FK__Transacti__BillI__412EB0B6");

                entity.HasOne(d => d.Note)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.NoteId)
                    .HasConstraintName("FK__Transacti__NoteI__3F466844");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.WalletId)
                    .HasConstraintName("FK__Transacti__Walle__403A8C7D");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("Wallet");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasColumnName("AccountID");

                entity.Property(e => e.Percentage).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.WalletAmount).HasColumnType("money");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Wallets)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Wallet__AccountI__29572725");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
