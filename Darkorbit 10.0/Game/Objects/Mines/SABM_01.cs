
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Objects.Mines
{
    class SABM_01 : Mine
    {
        public SABM_01(Player player, Spacemap spacemap, Position position, int mineTypeId) : base(player, spacemap, position, mineTypeId) { }

        public override void Action(Player player)
        {
            var damage = Maths.GetPercentage(player.CurrentShieldPoints, 50);
            //damage += Maths.GetPercentage(damage, player.GetSkillPercentage("Detonation"));

            AttackManager.Damage(Player, player as Player, DamageType.MINE, damage, false, false, true, false);
        }
    }
}
