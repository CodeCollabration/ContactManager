using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ContactManager.Entity.Entity
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> modelBuilder)
        {
            modelBuilder.Property(contact => contact.FirstName)
                 .IsRequired()
                 .IsUnicode(false)
                 .HasMaxLength(100);
            modelBuilder.Property(contact => contact.LastName)
                .IsUnicode(false)
                .HasMaxLength(100);
            modelBuilder.Property(contact => contact.Email)
                .IsUnicode(false)
                .HasMaxLength(255);
        }
    }
}
