using AspNetCoreDemo.Repository.IRepository.Base;
using AspNetCoreDemo.Service.IService.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Service.Service.Base
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected IBaseRepository<T> _baseRepository;

        #region 查询
        public T GetSingleById(int id)
        {
            return _baseRepository.GetSingleById(id);
        }

        public T GetSingleByExpression(Expression<Func<T, bool>> whereExpression)
        {
            return _baseRepository.GetSingleByExpression(whereExpression);
        }

        public List<T> GetListByExpression(Expression<Func<T, bool>> whereExpression)
        {
            return _baseRepository.GetListByExpression(whereExpression);
        }

        public List<T> GetListByExpression(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true)
        {
            return _baseRepository.GetListByExpression(whereExpression, orderByExpression);
        }

        public List<TResult> Join<TInner, TKey, TResult>(Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector)
            where TInner : class, new()
            where TResult : class
        {
            return _baseRepository.Join(outerKeySelector, innerKeySelector, resultSelector);
        }
        #endregion

        #region 新增
        public int Add(T model)
        {
           return  _baseRepository.Add(model);
        }

        public int AddRange(List<T> Entity)
        {
           return  _baseRepository.AddRange(Entity);
        }

        public Task<int> AddRangeOverwriteAsync(List<T> Entity, Expression<Func<T, bool>> delLambda)
        {
            return _baseRepository.AddRangeOverWriteAsync(Entity, delLambda);
        }
        #endregion

        #region 修改
        public int Update(T model)
        {
            return _baseRepository.Update(model);
        }

        public int Update(List<T> list)
        {
            return _baseRepository.Update(list);
        }

        public int Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> updateLambda)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 删除
        public int Delete(T model)
        {
            return _baseRepository.Delete(model);
        }

        public int Delete(Expression<Func<T, bool>> whereLambda)
        {
            throw new NotImplementedException();
        }

        public async Task<int> DeleteAsync(Expression<Func<T, bool>> whereLambda)
        {
            return await _baseRepository.DeleteAsync(whereLambda);
        }
        #endregion
    }
}
