using AspNetCoreDemo.Common.Extensions;
using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore;
using AspNetCoreDemo.Repository.IRepository.Base;
using AspNetCoreDemo.Repository.Uow;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Repository.Repository.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class, new()
    {
        private readonly IUnitofWork _unitofWork;
        public BaseContext _context;

        public BaseRepository(IUnitofWork unitOfWork)
        {
            _unitofWork = unitOfWork;
            _context = unitOfWork.GetDbContext();
        }
        public void BeginTran()
        {
            _unitofWork.BeginTran();
        }

        public int SaveChange()
        {
            _unitofWork.CommitTran();
            return _context.SaveChanges();
        }
        public Task<int> SaveChangeAsync()
        {
            _unitofWork.CommitTran();
            return _context.SaveChangesAsync();
        }

        #region 查询

        public IQueryable<T> Query(Expression<Func<T, bool>> whereExpression)
        {
            return _context.Set<T>().Where(whereExpression).AsNoTracking();
        }

        public IQueryable<T> GetAllQuery()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T GetSingleById(int id)
        {
            return _context.Set<T>().Find(id);
        }
        public async Task<T> GetSingleByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public T GetSingleByExpression(Expression<Func<T, bool>> whereExpression)
        {
            return _context.Set<T>().SingleOrDefault(whereExpression);
        }

        public async Task<T> GetSingleByExpressionAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(whereExpression);
        }

        public List<T> GetListByExpression(Expression<Func<T, bool>> whereExpression)
        {
            return _context.Set<T>().Where(whereExpression).ToList();
        }

        public async Task<List<T>> GetListByExpressionAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await _context.Set<T>().Where(whereExpression).ToListAsync();
        }

        public List<T> GetListByExpression(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true)
        {
            return isAsc ? _context.Set<T>().Where(whereExpression).OrderBy(orderByExpression).ToList()
                : _context.Set<T>().Where(whereExpression).OrderByDescending(orderByExpression).ToList();
        }

        public async Task<List<T>> GetListByExpressionAsync(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression, bool isAsc = true)
        {
            return await (isAsc ? _context.Set<T>().Where(whereExpression).OrderBy(orderByExpression).ToListAsync()
                : _context.Set<T>().Where(whereExpression).OrderByDescending(orderByExpression).ToListAsync());
        }

        public List<T> GetListAsNoTrackingByExpression(Expression<Func<T, bool>> whereExpression)
        {
            return _context.Set<T>().Where(whereExpression).AsNoTracking().ToList();
        }


        public async Task<(List<T>, int)> GetPageListByExpressionAsync(Expression<Func<T, bool>> whereExpression, PageDto dto)
        {
            return (await _context.Set<T>().AsNoTracking().Where(whereExpression).Page(dto.PageSize, dto.Page, out int totalCount).ToListAsync(), totalCount);
        }

        public List<TResult> Join<TInner, TKey, TResult>(Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector)
            where TInner : class, new()
            where TResult : class
        {
            return _context.Set<T>().Join(_context.Set<TInner>(), outerKeySelector, innerKeySelector, resultSelector).ToList();
        }

        public async Task<List<TResult>> JoinAsync<TInner, TKey, TResult>(Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector)
            where TInner : class, new()
            where TResult : class
        {
            return await _context.Set<T>().Join(_context.Set<TInner>(), outerKeySelector, innerKeySelector, resultSelector).ToListAsync();
        }
        #endregion

        #region 新增
        public int Add(T model)
        {
            _context.Set<T>().Add(model);
            return _context.SaveChanges();
        }

        public async Task<int> AddAsync(T model)
        {
            _context.Set<T>().Add(model);
            return await _context.SaveChangesAsync();
        }

        public int AddRange(List<T> Entity)
        {
            _context.AddRange(Entity);
            return _context.SaveChanges();
        }

        public async Task<int> AddRangeAsync(List<T> Entity)
        {
            _context.AddRange(Entity);
            return await _context.SaveChangesAsync();
        }

        public void AddRangeNoSave(List<T> Entity)
        {
            _context.AddRange(Entity);
        }

        public Task<int> AddRangeOverWriteAsync(List<T> Entity, Expression<Func<T, bool>> delLambda)
        {
            BeginTran();
            var list = _context.Set<T>().Where(delLambda).ToList();
            _context.RemoveRange(list);
            _context.AddRange(Entity);
            return SaveChangeAsync();
        }
        #endregion

        #region 修改
        public int Update(T model)
        {
            _context.Entry<T>(model).State = EntityState.Modified;
            return _context.SaveChanges();
        }

        public async Task<int> UpdateAsync(T model)
        {
            _context.Entry<T>(model).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public int Update(List<T> list)
        {
            if (list != null)
            {
                foreach (var items in list)
                {
                    var EntityModel = _context.Entry(items);
                    _context.Set<T>().Attach(items);
                    EntityModel.State = EntityState.Modified;
                }
            }

            return _context.SaveChanges();
        }

        public async Task<int> UpdateAsync(List<T> list)
        {
            if (list != null)
            {
                foreach (var items in list)
                {
                    var EntityModel = _context.Entry(items);
                    _context.Set<T>().Attach(items);
                    EntityModel.State = EntityState.Modified;
                }
            }

            return await _context.SaveChangesAsync();
        }

        public void Update(Expression<Func<T, bool>> whereLambda, Expression<Func<T, T>> updateLambda)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 删除
        public int Delete(T model)
        {
            _context.Set<T>().Attach(model);
            _context.Set<T>().Remove(model);
            return _context.SaveChanges();
        }

        public async Task<int> DeleteAsync(T model)
        {
            _context.Set<T>().Attach(model);
            _context.Set<T>().Remove(model);
            return await _context.SaveChangesAsync();
        }

        public int Delete(Expression<Func<T, bool>> whereLambda)
        {
            var list = _context.Set<T>().Where(whereLambda).ToList();
            _context.RemoveRange(list);
            return _context.SaveChanges();
        }

        public void DeleteNoSave(Expression<Func<T, bool>> whereLambda)
        {
            var list = _context.Set<T>().Where(whereLambda).ToList();
            _context.RemoveRange(list);
        }

        public async Task<int> DeleteAsync(Expression<Func<T, bool>> whereLambda)
        {
            var list = _context.Set<T>().Where(whereLambda).ToList();
            _context.RemoveRange(list);
            return await _context.SaveChangesAsync();
        }
        #endregion
    }
}
