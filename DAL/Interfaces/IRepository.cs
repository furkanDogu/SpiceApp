using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.DataAccessLayer.Interfaces
{
    public interface IRepository<T> where T : class
    {
        bool Insert(T entity);
        bool DeleteById(int id);
        T FetchById(int id);
    }
}
