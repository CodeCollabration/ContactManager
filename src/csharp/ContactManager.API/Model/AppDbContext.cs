//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using ContactManager.Entity.Entity;

//namespace ContactManager.API.Model
//{
//    public class AppDbContext : DbContext
//    {
//        public AppDbContext(DbContextOptions<AppDbContext> options)
//            : base(options)
//        {

//        }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.ApplyConfiguration(new ContactConfiguration());
//        }

//        public DbSet<Contact> Contacts { get; set; }
//    }
//}