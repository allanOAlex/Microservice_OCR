using ITT.DocumentUploadService.Domain.Entities;
using ITT.SummaryService.Shared.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITT.DocumentUploadService.Persistence.DataContext
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

                var connection = configuration.GetConnectionString("ConnString");
                var optionsBuilder = new DbContextOptionsBuilder<DBContext>();
                optionsBuilder.UseSqlServer(connection);

                return new DBContext(optionsBuilder.Options);
            }

            //public DBContext createdbcontext(string[] args)
            //{
            //    var optionsbuilder = new DbContextOptionsBuilder<DBContext>();
            //    optionsbuilder.UseSqlServer(AppConstants.Conn);

            //    return new DBContext(optionsbuilder.Options);
            //}

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


        public DbSet<FileUpload>? FileUploads { get; set; }



    }
}
