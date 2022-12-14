using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Objects.Mines
{
    class DDM_01 : Mine
    {
        public DDM_01(Player player, Spacemap spacemap, Position position, int mineTypeId) : base(player, spacemap, position, mineTypeId) { }

        public override void Action(Player player)
        {
            var damage = Maths.GetPercentage(player.MaxHitPoints, 20);
            //damage += Maths.GetPercentage(damage, player.GetSkillPercentage("Detonation"));

            if (Lance)
                damage += Maths.GetPercentage(damage, 50);

            AttackManager.Damage(Player, player as Player, DamageType.MINE, damage, true, true, false, false);
        }
    }
}
