using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactManager.DAL.Model
{
    //public interface IUnitOfWork<out TContext> where TContext : DbContext, new()
    //{
    //    TContext Context { get; }
    //    void CreateTransaction();
    //    void Commit();
    //    void Rollback();
    //    void Save();
    //}

    public interface IUnitOfWork : IDisposable
    {
        IContactRepository Contacts { get; }
        int Save();
    }
}   
