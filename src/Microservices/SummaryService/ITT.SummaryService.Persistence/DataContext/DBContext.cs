using ITT.SummaryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.SummaryService.Persistence.DataContext
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }

        public DBContext()
        {

        }


        public class DBContextFactory : IDesignTimeDbContextFactory<DBContext>
        {
            DBContext IDesignTimeDbContextFactory<DBContext>.CreateDbContext(string[] args)
            {
                var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

                var optionsBuilder = new DbContextOptionsBuilder<DBContext>();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("ConnString")!);

                return new DBContext(optionsBuilder.Options);
            }
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureModels(builder);
        }

        protected void ConfigureModels(ModelBuilder modelBuilder)
        {
        }


        public DbSet<TextSummary>? TextSummaries { get; set; }



    }

}
