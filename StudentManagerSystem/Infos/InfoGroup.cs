using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManageSystem.Infos
{
    public class InfoGroup : BaseInfo
    {
        private readonly List<BaseInfo> _infoList;

        public InfoGroup()
        {
            _infoList = new List<BaseInfo>();
        }

        /// <summary>
        /// 添加或连接信息. 参数为 <see cref="InfoGroup"/> 类型时将信息列表连接起来, 否则添加为子信息
        /// </summary>
        /// <param name="info">需添加或连接的信息</param>
        public void AddOrConcat(BaseInfo info)
        {
            if (info is InfoGroup group)
            {
                _infoList.AddRange(group._infoList);
            }
            else
            {
                _infoList.Add(info);
            }
        }

        public void Remove(BaseInfo info) => _infoList.Remove(info);

        public void Clear() => _infoList.Clear();

        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            for (var index = 0; index < _infoList.Count; index++)
            {
                var i = _infoList[index];
                strBuilder.AppendLine($"[{index}] {i}");
            }

            return strBuilder.ToString();
        }

        public override bool NoError() => !_infoList.Any() || _infoList.All(i => i);
    }
}
