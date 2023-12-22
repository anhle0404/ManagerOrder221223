using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerOrder.IRepo
{
    public interface IRepo<T> where T:class
    {
        List<T> GetAll();
        T GetByID(long id);
        int Create(T item);
        int Update(T item);
        int Delete(long id);
    }
}
