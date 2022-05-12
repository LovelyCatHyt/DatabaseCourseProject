using Microsoft.EntityFrameworkCore;

namespace StudentManageSystem.DataBase
{
    public interface IDbVisitor
    {
        /// <summary>
        /// 初始化传入一个数据库上下文
        /// </summary>
        /// <param name="context"></param>
        public void Init(DbContext context);
    }
}
