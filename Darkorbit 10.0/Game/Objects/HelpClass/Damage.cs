using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Objects.HelpClass
{
    public class Damage
    {
        public int _Damage { get; set; }
        public DateTime LastAttack { get; set; }

        public Damage(int dmg , DateTime lastAttack)
        {
            _Damage = dmg;
            LastAttack = lastAttack;
        }
    }

}
