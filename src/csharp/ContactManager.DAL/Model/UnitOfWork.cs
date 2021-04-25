using ContactManager.DAL.Model;
using ContactManager.Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactManager.DAL.Model
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IContactRepository Contacts { get; }        

        public UnitOfWork(AppDbContext appDbContext,
            IContactRepository contactRepository)
        {
            this._context = appDbContext;
            this.Contacts = contactRepository;
        }
        public int Save()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
