using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManageSystem.Infos
{
    /// <summary>
    /// info 基类
    /// </summary>
    public abstract class BaseInfo
    {
        public abstract bool NoError();
        public abstract override string ToString();
        public static implicit operator bool(BaseInfo i) => i.NoError();
        public static implicit operator string(BaseInfo i) => i.ToString();
    }
}
