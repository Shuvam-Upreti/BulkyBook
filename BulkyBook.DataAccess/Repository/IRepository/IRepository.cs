using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    //Generic repository of category
    public interface IRepository<T> where T : class
    {
        //T -Category

        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? inculdeProperties = null);//Index
        void Add(T entity);//Create
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true);//Edit Post
        void Remove(T entity);//Delete
        void RemoveRange(IEnumerable<T> entity);//Remove more than one entity


    }
}
