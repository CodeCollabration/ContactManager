using ContactManager.Entity.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ContactManager.Entity.Model
{
    public class AppDbContext : DbContext
    {
        //public AppDbContext()
        //{

        //}
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {            
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ContactConfiguration());
        }

        public override int SaveChanges()
        {
            var entities = from e in ChangeTracker.Entries()
                           where e.State == EntityState.Added
                               || e.State == EntityState.Modified
                           select e.Entity;
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(
                    entity,
                    validationContext,
                    validateAllProperties: true);
            }
            return base.SaveChanges();
        }

        public DbSet<Contact> Contacts { get; set; }        
    }
}