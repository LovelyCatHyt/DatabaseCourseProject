using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.Infos;

namespace StudentManageSystem.DataBase
{
    public abstract class DbValidator<TEntity, TId, TName>
        where TEntity : class
        where TId : IComparable
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _entities;

        protected DbValidator(DbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        /// <summary>
        /// 验证一个字符串是否由纯中文字符组成
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool NameValidate(string? name)
        {
            // 判空
            if (string.IsNullOrWhiteSpace(name)) return false;
            // 正则   
            return Regex.IsMatch(name, @"^[一-\uffff]+$");
        }

        /// <summary>
        /// 获取实体的 Id
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract TId IdOfEntity(TEntity entity);

        /// <summary>
        /// 获取实体的描述名称
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract TName NameOfEntity(TEntity entity);

        /// <summary>
        /// 验证单个数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract InfoGroup ValidateData(TEntity data);

        /// <summary>
        /// 验证单个数据能否插入到数据库中
        /// </summary>
        /// <param name="data"></param>
        /// <param name="idDuplicatedErrorMsg"></param>
        /// <returns></returns>
        protected virtual InfoGroup ValidateNewData(TEntity data, string idDuplicatedErrorMsg)
        {
            var info = new InfoGroup();
            var id = IdOfEntity(data);
            if (_entities.Any(e => IdOfEntity(e).CompareTo(id) == 0))
            {
                info.AddOrConcat(new Error(string.Format(idDuplicatedErrorMsg, NameOfEntity(data), id)));
            }

            return info;
        }

        /// <summary>
        /// 验证一系列数据
        /// </summary>
        /// <param name="dataEnumerable"></param>
        /// <returns></returns>
        public virtual InfoGroup ValidateRange(IEnumerable<TEntity> dataEnumerable) =>
            dataEnumerable.Aggregate(new InfoGroup(), (i, d) =>
            {
                i.AddOrConcat(ValidateData(d));
                return i;
            });

        /// <summary>
        /// 验证一系列新数据
        /// </summary>
        /// <param name="dataEnumerable"></param>
        /// <returns></returns>
        public abstract InfoGroup ValidateRangeNew(IEnumerable<TEntity> dataEnumerable);

        /// <summary>
        /// 验证一系列新数据
        /// </summary>
        /// <param name="dataEnumerable"></param>
        /// <param name="idDuplicatedErrorMsg">主键冲突时的错误信息</param>
        /// <returns></returns>
        protected virtual InfoGroup ValidateRangeNew(IEnumerable<TEntity> dataEnumerable, string idDuplicatedErrorMsg)
        {
            var info = new InfoGroup();
            var set = new HashSet<TId>();
            foreach (var entity in dataEnumerable)
            {
                var id = IdOfEntity(entity);
                if (set.Contains(id))
                {
                    info.AddOrConcat(new Error(string.Format(idDuplicatedErrorMsg, NameOfEntity(entity), id)));
                }
                else
                {
                    set.Add(id);
                }
                info.AddOrConcat(ValidateNewData(entity, idDuplicatedErrorMsg));
            }

            return info;
        }

    }
}
