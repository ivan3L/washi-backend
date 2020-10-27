﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Washi.API.Domain.Models;
using Washi.API.Extensions;

namespace Washi.API.Domain.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CountryCurrency> CountryCurrencies { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //User Entity
            builder.Entity<User>().ToTable("Users");
            builder.Entity<User>().HasKey(p => p.Id);
            builder.Entity<User>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<User>().Property(p => p.Email).IsRequired().HasMaxLength(50);
            builder.Entity<User>().Property(p => p.Password).IsRequired().HasMaxLength(50);
            builder.Entity<User>().HasData
                (
                    new User { Id = 100, Email = "felipedota2@gmail.com", Password = "slark" },
                    new User { Id = 101, Email = "xavistian@gmail.com", Password = "tiaaaaaaaan" }
                );
            // PaymentMethod Entity
            builder.Entity<PaymentMethod>().ToTable("PaymentMethods");
            builder.Entity<PaymentMethod>().HasKey(p => p.Id);
            builder.Entity<PaymentMethod>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<PaymentMethod>().Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Entity<PaymentMethod>().HasData
                (
                    new PaymentMethod { Id = 67, Name = "TarjetaDeRegalo"},
                    new PaymentMethod { Id = 68, Name = "TarjetaConeyPark" }
                );

            // Service Entity
            builder.Entity<Service>().ToTable("Services");
            builder.Entity<Service>().HasKey(p => p.Id);
            builder.Entity<Service>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Service>().Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Entity<Service>().HasData
                (
                    new Service { Id = 100, Name = "LavadoalSeco" },
                    new Service { Id = 101, Name = "Planchado" }
                );
            ApplySnakeCaseNamingConvention(builder);

            //Currency Entity
            builder.Entity<Currency>().ToTable("Currencies");
            builder.Entity<Currency>().HasKey(c => c.Id);
            builder.Entity<Currency>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Currency>().Property(c => c.Name).IsRequired().HasMaxLength(50);
            builder.Entity<Currency>().Property(c => c.Symbol).IsRequired().HasMaxLength(5);
            builder.Entity<Currency>().HasData
                (
                    new Currency { Id=2, Name="Dólar Estadounidense", Symbol="$"},
                    new Currency { Id = 1, Name = "Sol", Symbol = "S/" },
                    new Currency { Id = 3, Name = "Euro", Symbol = "€" }
                );

            //CountryCurrency
            builder.Entity<CountryCurrency>().ToTable("CountryCurrencies");
            builder.Entity<CountryCurrency>().HasKey(cc => new { cc.CountryId, cc.CurrencyId });
            builder.Entity<CountryCurrency>().HasOne(cc => cc.Country).WithMany(c => c.CountryCurrencies).HasForeignKey(cc => cc.CountryId);
            builder.Entity<CountryCurrency>().HasOne(cc => cc.Currency).WithMany(c => c.CountryCurrencies).HasForeignKey(cc => cc.CurrencyId);
            builder.Entity<CountryCurrency>().HasData
                (
                    new CountryCurrency { Id=1, CountryId=1,CurrencyId=1},
                    new CountryCurrency { Id=2, CountryId=1,CurrencyId=2},
                    new CountryCurrency { Id=3, CountryId=1, CurrencyId=3},
                    new CountryCurrency { Id=4, CountryId=2,CurrencyId=2}
                );
            //Country Entity
            builder.Entity<Country>().ToTable("Countries");
            builder.Entity<Country>().HasKey(c => c.Id);
            builder.Entity<Country>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<Country>().Property(c => c.Name).IsRequired().HasMaxLength(50);
            builder.Entity<Country>().HasMany(c => c.Departments).WithOne(d => d.Country).HasForeignKey(d => d.CountryId);
            builder.Entity<Country>().HasData
                (
                    new Country { Id = 1, Name = "Perú" },
                    new Country { Id = 2, Name = "Estados Unidos" }
                );
        }

        private void ApplySnakeCaseNamingConvention(ModelBuilder builder)
        {
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName().ToSnakeCase());
                foreach (var property in entity.GetProperties())
                    property.SetColumnName(property.GetColumnName().ToSnakeCase());
                foreach (var key in entity.GetKeys())
                    key.SetName(key.GetName().ToSnakeCase());
                foreach (var foreignKey in entity.GetForeignKeys())
                    foreignKey.SetConstraintName(foreignKey.GetConstraintName().ToSnakeCase());
                foreach (var index in entity.GetIndexes())
                    index.SetName(index.GetName().ToSnakeCase());
            }
        }

    }
}
