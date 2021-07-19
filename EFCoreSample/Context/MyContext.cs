using EFCoreSample.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFCoreSample.Context
{
    public class MyContext : DbContext
    {
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string connectionString = "Host=localhost;Port=15432;Database=EFCoreSample;Username=@YOU_USER_NAME;Password=@YOUR_PASSWORD";
            optionsBuilder
                .UseNpgsql(connectionString)
                .EnableSensitiveDataLogging()
                .UseLazyLoadingProxies() //LazyLoading
                .LogTo(Console.Write, LogLevel.Information);

            base.OnConfiguring(optionsBuilder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    const string connectionString = "Server=localhost;Database=EFCoreSample;Trusted_Connection=True";
        //    optionsBuilder
        //        .UseSqlServer(connectionString);
        //    .EnableSensitiveDataLogging()
        //    .UseLazyLoadingProxies() //LazyLoading
        //    .LogTo(Console.Write, LogLevel.Information);

        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //filtros globais
            modelBuilder.Entity<Departamento>().HasQueryFilter(p => !p.Excluido);

            //propriedade sombra
            modelBuilder
                .Entity<Departamento>()
                .Property<DateTime>("UltimaAtualizacao");

            //índice
            modelBuilder
                .Entity<Departamento>()
                .HasIndex(d => new { d.Descricao, d.Excluido })
                .HasDatabaseName("idx_meu_indice_composto_departamento")
                .HasFilter("Descricao IS NOT NULL")
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            foreach (var entry in this.ChangeTracker.Entries()
                .Where(e => e.Properties.Any(p => p.Metadata.Name == "UltimaAtualizacao")))
            {
                entry.Property("UltimaAtualizacao").CurrentValue = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }

        public void SeedData()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();

            var departamentos = new List<Departamento>();

            for (int i = 0; i < 200; i++)
            {
                departamentos.Add(new Departamento($"Departamento {i}"));
            }

            Departamentos.AddRange(departamentos);
            SaveChanges();

            var funcionarios = new List<Funcionario>();
            var random = new Random();

            for (int i = 0; i < 2000; i++)
            {
                funcionarios.Add(new Funcionario
                {
                    DataAdmissao = DateTime.UtcNow,
                    Nome = $"Funcionário {i}",
                    DepartamentoId = departamentos[random.Next(0, 199)].Id
                });
            }

            Funcionarios.AddRange(funcionarios);
            SaveChanges();
        }
    }
}
