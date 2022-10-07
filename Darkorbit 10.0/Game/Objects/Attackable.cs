using Darkorbit.Game.Ticks;
using Darkorbit.Api;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit_10._0.Game.Objects
{
    public abstract class Attackable : Tick
    {
        public int ID { get; }

        protected Api.Api Api { get; set; }
        public abstract string Username { get; set; }
        public abstract Clan Clan { get; set; }
        public abstract Position Position { get; set; }
        public abstract Spacemap Spacemap { get; set; }

        public abstract int FactionId { get; set; }
        public abstract int CurrentHP { get; set; }
        public abstract int MaxHP { get; set; }
        public abstract int CurrentShield { get; set; }
        public abstract int MaxShield { get; set; }
        public abstract int CurrentNanoHull { get; set; }
        public abstract int MaxNanoHull { get; set; }
        public abstract int ShieldAbsorption { get; set; }
        public abstract int ShieldPenetration { get; set; }
        public DateTime LastCombatTime { get; set; }
        public virtual int AttackRange => 670;
        public virtual int RenderRange => 2000;
        public virtual int RenderRange1 => 1000;
        public virtual int RenderRange2 => 300_000;

        public bool Invisible { get; set; }
        public bool Invincible { get; set; }

        public bool Destroyed { get; set; } = false;
        public bool SpaceballDestroyed { get; set; } = false;
        public bool TdmDestroyed { get; set; } = false;
        public bool TDMLeft { get; set; } = false;
        public bool TDMRight { get; set; } = false;
        public bool CR { get; set; } = false;
        public bool CubiPremium { get; set; } = false;

        public Character MainAttacker { get; set; }
        public ConcurrentDictionary<int, Attacker> Attackers = new();
        
        public ConcurrentDictionary<int, Character> InRangeCharacters = new();
        public ConcurrentDictionary<int, VisualModifierCommand> VisualModifiers = new();
        public ConcurrentDictionary<Attackable, Damage> DamageReceived = new();

        public Attackable? Selected { get; set; }

        public static readonly List<int> BLACKLIGHT = new()
        {
            90
        };
        public static readonly List<int> BLACK = new()
        {
            45
        };
        public static readonly List<int> BLACKI = new()
        {
            41
        };

        public static bool x2EventActive { get; set; } = false;

        protected Attackable(int id , Api api)
        {
            ID = id;
            Invisible = false;
            Invincible = false;
            if(Clan == null)
        }






        public void Tick()
        {
            throw new NotImplementedException();
        }
    }
   
}
