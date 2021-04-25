using ContactManager.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.DAL.Model
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll(DataTablesHelper dataTables);
        Task<T> GetById(object id);
        Task<int> Insert(T obj);
        Task<int> Update(T obj);
        void Delete(T obj);
        int Count();
    }
}
