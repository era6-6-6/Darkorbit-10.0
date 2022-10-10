
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game
{
    class Clan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public int FactionId { get; set; }

        public Dictionary<int, Diplomacy> DiplomaciesSender = new Dictionary<int, Diplomacy>();
        public Dictionary<int, Diplomacy> DiplomaciesTo = new Dictionary<int, Diplomacy>();

        public Clan(int id, string name, string tag, int factionId)
        {
            Id = id;
            Name = name;
            Tag = tag;
            FactionId = factionId;
        }

        public short GetRelation(Clan clan)
        {
            if (clan == this && clan.Id != 0)
                return (short)Game.Diplomacy.ALLIED;
            if (DiplomaciesSender.ContainsKey(clan.Id))
                return (short)DiplomaciesSender[clan.Id];
            if (DiplomaciesTo.ContainsKey(clan.Id))
                return (short)DiplomaciesTo[clan.Id];
            return (short)Game.Diplomacy.NONE;
        }
    }
}
