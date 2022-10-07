using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Data
{
    public class DataDemaner
    {
        public List<Npc> Minions { get; private set; }
        
        public ConcurrentDictionary<int, Player> Players { get; private set; }


        public DataDemaner()
        {
            Minions = new List<Npc>();
            Players = new ConcurrentDictionary<int, Player>();
        }
    }
}
