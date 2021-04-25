using ContactManager.Entity.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactManager.DAL.Model
{
    public interface IContactRepository : IGenericRepository<Contact>
    {
        Contact GetContactByEmail(string Email);
        IEnumerable<Contact> GetActiveContacts();
        IEnumerable<Contact> GetInActiveContacts();
    }
}
