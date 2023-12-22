using ManagerOrder.IRepo;
using ManagerOrder.Models.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerOrder.Repo
{
    public class GenericRepo<T>:IRepo<T> where T:class
    {
        protected ThangThuyDataContext db { get; set; }
        protected DbSet<T> table = null;

        public GenericRepo()
        {
            db = new ThangThuyDataContext();
            table = db.Set<T>();
        }

        public GenericRepo(ThangThuyDataContext db)
        {
            this.db = db;
            table = db.Set<T>();
        }

        public List<T> GetAll()
        {
            return table.ToList();
        }

        public T GetByID(long id)
        {
            return table.Find(id);
        }

        public int Create(T item)
        {
            table.Add(item);
            return db.SaveChanges();
        }

        public int Update(T item)
        {
            table.Attach(item);
            db.Entry(item).State = EntityState.Modified;
            return db.SaveChanges();
        }

        public int Delete(long id)
        {
            table.Remove(table.Find(id));
            return db.SaveChanges();
        }
    }
}
