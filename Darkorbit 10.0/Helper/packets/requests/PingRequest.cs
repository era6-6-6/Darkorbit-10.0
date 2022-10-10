using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Helper.packets.requests
{
    class PingRequest : IRequest
    {
        public const string Prefix = "ping";

        public void Read(string[] args = null)
        {

        }
    }
}
