using AxiomSysDL.Models.SupplyChain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AxiomSysDL.Contexts
{

    public class SupplyChainContext : DbContext
    {
        public SupplyChainContext(
            DbContextOptions<SupplyChainContext> options)
            : base(options)
        {
        }

        public DbSet<SaDeliveryTerm> SaDeliveryTerms =>
            Set<SaDeliveryTerm>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entity = modelBuilder.Entity<SaDeliveryTerm>();

            entity.ToTable("SA_DELIVERY_TERMS");

            entity.HasKey(x => x.Code)
                .HasName("SA_DELIVERY_TERMS_PK");

            entity.Property(x => x.Code)
                .HasColumnName("CODE")
                .HasColumnType("NUMBER(3)")
                .ValueGeneratedNever();

            entity.Property(x => x.SName)
                .HasColumnName("S_NAME")
                .HasColumnType("VARCHAR2(60)")
                .IsRequired();

            entity.Property(x => x.BName)
                .HasColumnName("B_NAME")
                .HasColumnType("VARCHAR2(60)")
                .IsRequired();

            entity.Property(x => x.Days)
                .HasColumnName("DAYS")
                .HasColumnType("NUMBER(4)");

            entity.Property(x => x.ActiveFlag)
    .HasColumnName("ACTIVE_FLAG")
    .HasColumnType("NUMBER(1)")
    .HasConversion<bool?>(
        value => value.HasValue ? value.Value == 1 : null,
        value => value.HasValue ? (value.Value ? 1 : 0) : null
    );

            entity.Property(x => x.EntryUser)
                .HasColumnName("ENTRY_USER")
                .HasColumnType("NUMBER(10)")
                .IsRequired();

            entity.Property(x => x.EntryDate)
                .HasColumnName("ENTRY_DATE")
                .HasColumnType("DATE")
                .IsRequired();

            entity.Property(x => x.ChangeUser)
                .HasColumnName("CHANGE_USER")
                .HasColumnType("NUMBER(10)");

            entity.Property(x => x.ChangeDate)
                .HasColumnName("CHANGE_DATE")
                .HasColumnType("DATE");
        }
    }
}

