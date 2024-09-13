using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Service.IService.Base
{
    public interface IBaseService<T> where T : class
    {

        #region 查询
        T GetSingleById(int id);

        T GetSingleByExpression(Expression<Func<T, bool>> whereExpression);

        List<T> GetListByExpression(Expression<Func<T, bool>> whereExpression);

        List<T> GetListByExpression(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true);

        List<TResult> Join<TInner, TKey, TResult>(Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector)
            where TInner : class, new()
            where TResult : class;
        #endregion

        #region 新增
        int Add(T model);

        int AddRange(List<T> Entity);

        public Task<int> AddRangeOverwriteAsync(List<T> Entity, Expression<Func<T, bool>> delLambda);
        #endregion

        #region 修改
        int Update(T model);

        int Update(List<T> list);

        int Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> updateLambda);
        #endregion

        #region 删除
        int Delete(T model);

        int Delete(Expression<Func<T, bool>> whereLambda);

        Task<int> DeleteAsync(Expression<Func<T, bool>> whereLambda);
        #endregion
    }
}
