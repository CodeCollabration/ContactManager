using ContactManager.Entity.Entity;
using ContactManager.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactManager.DAL.Model
{
    public class ContactRepository : GenericRepository<Contact>, IContactRepository
    {       
        public ContactRepository(AppDbContext context) : base(context)
        {
        }

        public Contact GetContactByEmail(string Email)
        {
            return  _context.Contacts.Where(c => c.Email.Equals(Email)).FirstOrDefault();
        }

        public IEnumerable<Contact> GetActiveContacts()
        {
            return _context.Contacts.Where(c => c.IsActive);
        }

        public IEnumerable<Contact> GetInActiveContacts()
        {
            return _context.Contacts.Where(c => !c.IsActive);
        }
    }
}
