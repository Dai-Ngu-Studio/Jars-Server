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
        public virtual DbSet<AccountDevice> AccountDevices { get; set; } = null!;
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
            if (!optionsBuilder.IsConfigured)
            {
                string? DatabasePassword = Environment.GetEnvironmentVariable("SA_PASSWORD");
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseSqlServer("server=db,1433;database=JarsDatabase;uid=sa;pwd=" + DatabasePassword + ";");
                }
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

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                entity.Property(e => e.PhotoUrl).IsUnicode(false);
            });

            modelBuilder.Entity<AccountDevice>(entity =>
            {
                entity.HasKey(e => e.FcmToken)
                    .HasName("PK__AccountD__F325AEE3BC77EB52");

                entity.ToTable("AccountDevice");

                entity.Property(e => e.FcmToken)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.AccountId)
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasColumnName("AccountID");

                entity.Property(e => e.LastActiveDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountDevices)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__AccountDe__Accou__46E78A0C");
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bill");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasColumnName("AccountID");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.ContractId).HasColumnName("ContractID");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.LeftAmount).HasColumnType("money");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Bill__AccountID__38996AB5");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__Bill__CategoryID__398D8EEE");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.ContractId)
                    .HasConstraintName("FK__Bill__ContractID__3A81B327");
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
                    .HasConstraintName("FK__BillDetai__BillI__4222D4EF");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ParentCategoryId).HasColumnName("ParentCategoryID");

                entity.HasOne(d => d.ParentCategory)
                    .WithMany(p => p.InverseParentCategory)
                    .HasForeignKey(d => d.ParentCategoryId)
                    .HasConstraintName("FK__Category__Parent__2D27B809");
            });

            modelBuilder.Entity<CategoryWallet>(entity =>
            {
                entity.ToTable("CategoryWallet");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ParentCategoryId).HasColumnName("ParentCategoryID");

                entity.HasOne(d => d.ParentCategory)
                    .WithMany(p => p.InverseParentCategory)
                    .HasForeignKey(d => d.ParentCategoryId)
                    .HasConstraintName("FK__CategoryW__Paren__267ABA7A");
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

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.NoteId).HasColumnName("NoteID");

                entity.Property(e => e.ScheduleTypeId).HasColumnName("ScheduleTypeID");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Contract__Accoun__33D4B598");

                entity.HasOne(d => d.Note)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.NoteId)
                    .HasConstraintName("FK__Contract__NoteID__34C8D9D1");

                entity.HasOne(d => d.ScheduleType)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.ScheduleTypeId)
                    .HasConstraintName("FK__Contract__Schedu__35BCFE0A");
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.ToTable("Note");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ContractId).HasColumnName("ContractID");

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.ContractId)
                    .HasConstraintName("FK_ContractID_ID_Contract");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.TransactionId)
                    .HasConstraintName("FK_TransactionID_ID_Transaction");
            });

            modelBuilder.Entity<ScheduleType>(entity =>
            {
                entity.ToTable("ScheduleType");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
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
                    .HasConstraintName("FK__Transacti__BillI__3F466844");

                entity.HasOne(d => d.Note)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.NoteId)
                    .HasConstraintName("FK__Transacti__NoteI__3D5E1FD2");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.WalletId)
                    .HasConstraintName("FK__Transacti__Walle__3E52440B");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("Wallet");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasColumnName("AccountID");

                entity.Property(e => e.CategoryWalletId).HasColumnName("CategoryWalletID");

                entity.Property(e => e.Percentage).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.WalletAmount).HasColumnType("money");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Wallets)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Wallet__AccountI__29572725");

                entity.HasOne(d => d.CategoryWallet)
                    .WithMany(p => p.Wallets)
                    .HasForeignKey(d => d.CategoryWalletId)
                    .HasConstraintName("FK__Wallet__Category__2A4B4B5E");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}