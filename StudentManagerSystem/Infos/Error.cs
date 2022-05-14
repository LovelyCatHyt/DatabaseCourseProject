using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManageSystem.Infos
{
    public class Error : BaseInfo
    {
        public object? Sender { get; }

        public string Message { get; }

        public Error(string message)
        {
            Sender = null;
            Message = message;
        }

        public Error(object sender, string message)
        {
            Sender = sender;
            Message = message;
        }

        public override bool NoError() => false;

        public override string ToString() => Message;
    }
}
