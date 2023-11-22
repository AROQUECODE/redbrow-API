using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace aroque.code.API.Models;

public partial class RedbrowContext : DbContext
{
    public RedbrowContext()
    {
    }

    public RedbrowContext(DbContextOptions<RedbrowContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TokenHistory> TokenHistories { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TokenHistory>(entity =>
        {
            entity.HasKey(e => e.IdToken).HasName("PK__tokenHis__FEFE350D5B84DE12");

            entity.ToTable("tokenHistory");

            entity.Property(e => e.IdToken).HasColumnName("idToken");
            entity.Property(e => e.Activo).HasComputedColumnSql("(case when [FechaExpiracion]<getdate() then CONVERT([bit],(0)) else CONVERT([bit],(1)) end)", false);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaExpiracion).HasColumnType("datetime");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TokenHistories)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__tokenHist__idUsu__3B75D760");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuario__645723A62DC2490F");

            entity.ToTable("usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Clave)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("clave");
            entity.Property(e => e.Correo)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.CreadoPor)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("creadoPor");
            entity.Property(e => e.Edad).HasColumnName("edad");
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
