using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
  Bu sınıfın amacı “Şu CRUD operasyonlarını sunuyorum” diyen sözleşme.
  Hangi işlemleri sunacağımızı isimleriyle ve imzalarıyla tanımlıyor; içeride nasıl yapılacağını söylemiyor.
 */
namespace KutuphaneDataAccess.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        #region CRUD
        void Create(TEntity entity);
        IQueryable<TEntity> GetAll();  
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete (TEntity entity);
        #endregion
        Task<TEntity> GetByIdAsync(int id);
        void DeleteRange(List<TEntity> entities);
     
    }
}