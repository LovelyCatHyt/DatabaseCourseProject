using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManageSystem.Infos
{
    public class Info : BaseInfo
    {
        public object? Sender { get; }

        public string Message { get; }

        public Info(string message)
        {
            Sender = null;
            Message = message;
        }

        public Info(object sender, string message)
        {
            Sender = sender;
            Message = message;
        }

        public override bool NoError() => true;

        public override string ToString() => Message;
    }
}
