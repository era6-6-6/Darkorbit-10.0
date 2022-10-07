using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Api
{
    public class Api
    {
        //everything should be called from there instead of make everything static for no reason
        public GameManager GameManager = new();
    }
}
