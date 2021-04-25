using Microsoft.EntityFrameworkCore;
using ContactManager.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ContactManager.DAL.Model
{
    public class GenericRepository<T> : IGenericRepository<T>, IDisposable where T : class, new()
    {
        private DbSet<T> _entities;
        public AppDbContext _context;
        private bool _isDisposed;

        public GenericRepository(AppDbContext context)
        {
            _isDisposed = false;
            _context = context;
        }

        public virtual IQueryable<T> Table
        {
            get { return Entities; }
        }
        protected virtual DbSet<T> Entities
        {
            get { return _entities ?? (_entities = _context.Set<T>()); }
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public async Task<int> Delete(int id)
        {
            var t = await _context.Set<T>().FindAsync(id);
            _context.Set<T>().Remove(t);
            return 1;
        }

        public async Task<IEnumerable<T>> GetAll(DataTablesHelper dataTables)
        {
            int skip = dataTables.Skip;

            if (dataTables.Filters.Count > 0)
            {
                var deleg = PredicateBuilder.GetExpression<T>(dataTables.Filters);
                //data = Entities.Where(deleg).AsQueryable<T>();

                if (!string.IsNullOrEmpty(dataTables.SortColumn) && !string.IsNullOrEmpty(dataTables.SortDirection))
                {
                    if (dataTables.SortDirection.ToLower().Equals("asc"))
                    {
                        return Entities.Where(deleg).AsQueryable().OrderBy(dataTables.SortColumn).Skip(skip).Take(dataTables.Pagesize);
                    }
                    else
                    {
                        return Entities.Where(deleg).AsQueryable().OrderByDescending(dataTables.SortColumn).Skip(skip).Take(dataTables.Pagesize);
                    }
                }
            }
            if (!string.IsNullOrEmpty(dataTables.SortColumn) && !string.IsNullOrEmpty(dataTables.SortDirection))
            {
                if (dataTables.SortDirection.ToLower().Equals("asc"))
                {
                    return Entities.OrderBy(dataTables.SortColumn).Skip(skip).Take(dataTables.Pagesize);
                }
                else
                {
                    return Entities.OrderByDescending(dataTables.SortColumn).Skip(skip).Take(dataTables.Pagesize);
                }
            }
            return await Entities.ToListAsync();
        }

        public async Task<T> GetById(object id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<int> Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");
                Entities.Add(entity);
                //if (_context == null || _isDisposed)
                //    _context = new AppDbContext();
                //Context.SaveChanges(); commented out call to SaveChanges as Context save changes will be 
                //called with Unit of work
            }
            catch (Exception dbEx)
            {
                throw dbEx;
            }


            await _context.Set<T>().AddAsync(entity);
            return 1;
        }

        public async Task<int> Update(T entity)
        {
            try
            {
                var e = _context.Set<T>().Update(entity);                
                SetEntryModified(entity);
                return 1;
            }
            catch (Exception ex)
            {
                SetEntryUnchanged(entity);
                return 0;
            }
        }

        public virtual void SetEntryModified(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
        public virtual void SetEntryUnchanged(T entity)
        {
            _context.Entry(entity).State = EntityState.Unchanged;
        }

        public void Dispose()
        {
            if (_context != null)
                _context.Dispose();
            _isDisposed = true;
        }
    }
}
