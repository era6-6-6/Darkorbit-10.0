using Darkorbit.Game.Objects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Data
{
    internal class DataDemaner
    {
        public List<Npc> Minions { get; private set; }

        public ConcurrentDictionary<int, Player> Players { get; private set; }


        public void SetMinions(List<Npc> minions)
        {
            lock(Minions)
            {
                Minions = minions;
            }
        
        }
        public void SetPlayers(ConcurrentDictionary<int, Player> players)
        {
            lock (Players)
            {
                Players = players;
            }
        }


        public DataDemaner()
        {
            Minions = new List<Npc>();
            Players = new ConcurrentDictionary<int, Player>();
        }
    }
}
