using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceApp.DataAccessLayer.Interfaces
{
    public interface IUpdatableRepository<T> : IRepository<T> where T : class
    {
        bool Update(T entity);
    }
}
