using AspNetCoreDemo.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Repository.IRepository.Base
{
    /// <summary>
    /// 仓储基类调用通用CRUD
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseRepository<T> where T : class
    {
        public void BeginTran();

        public int SaveChange();

        #region 查询
        public IQueryable<T> Query(Expression<Func<T, bool>> whereExpression);

        public IQueryable<T> GetAllQuery();

        public List<T> GetAll();

        public T GetSingleById(int id);

        public Task<T> GetSingleByIdAsync(int id);

        public T GetSingleByExpression(Expression<Func<T, bool>> whereExpression);

        public Task<T> GetSingleByExpressionAsync(Expression<Func<T, bool>> whereExpression);

        public List<T> GetListByExpression(Expression<Func<T, bool>> whereExpression);

        public Task<List<T>> GetListByExpressionAsync(Expression<Func<T, bool>> whereExpression);

        public List<T> GetListByExpression(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true);

        public Task<List<T>> GetListByExpressionAsync(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true);

        public List<T> GetListAsNoTrackingByExpression(Expression<Func<T, bool>> whereExpression);

        public Task<(List<T>, int)> GetPageListByExpressionAsync(Expression<Func<T, bool>> whereExpression, PageDto pageDto);


        public List<TResult> Join<TInner, TKey, TResult>(Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector)
            where TInner : class, new()
            where TResult : class;

        public Task<List<TResult>> JoinAsync<TInner, TKey, TResult>(Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector)
            where TInner : class, new()
            where TResult : class;
        #endregion

        #region 新增
        public int Add(T model);

        public Task<int> AddAsync(T model);

        public int AddRange(List<T> Entity);

        public Task<int> AddRangeAsync(List<T> Entity);

        public void AddRangeNoSave(List<T> Entity);

        public Task<int> AddRangeOverWriteAsync(List<T> Entity, Expression<Func<T, bool>> delLambda);
        #endregion

        #region 修改
        public int Update(T model);

        public Task<int> UpdateAsync(T model);

        public int Update(List<T> list);

        public Task<int> UpdateAsync(List<T> list);

        public void Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> updateLambda);
        #endregion

        #region 删除
        public int Delete(T model);

        public Task<int> DeleteAsync(T model);

        public int Delete(Expression<Func<T, bool>> whereLambda);

        public void DeleteNoSave(Expression<Func<T, bool>> whereLambda);

        public Task<int> DeleteAsync(Expression<Func<T, bool>> whereLambda);
        #endregion
    }
}
