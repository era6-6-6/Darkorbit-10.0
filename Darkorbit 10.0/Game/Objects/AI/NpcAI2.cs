
using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Darkorbit.Game.Movements;

namespace Darkorbit.Game.Objects.AI
{
    class NpcAi2
    {
        public Npcx2 Npcx2 { get; set; }

        public NpcAIOption AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
        private static int ALIEN_DISTANCE_TO_USER = 300;

        public NpcAi2(Npcx2 npcx2) { Npcx2 = npcx2; }

        public DateTime lastMovement = new DateTime();


        public void TickAI()
        {
            if (lastMovement.AddSeconds(1) < DateTime.Now)
            {
                switch (AIOption)
                {
                    case NpcAIOption.SEARCH_FOR_ENEMIES:
                        foreach (var players in Npcx2.InRangeCharacters.Values)
                        {
                            if (players is Player)
                            {
                                var player = players as Player;

                                if (player.Storage.IsInDemilitarizedZone || player.Invisible || player.LastAttackTime(15) || Npcx2.Position.DistanceTo(player.Position) > Npcx2.RenderRange)
                                {
                                    Npcx2.Attacking = false;
                                    Npcx2.Selected = null;
                                    AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                                }
                                else
                                {
                                    if (Npcx2.Ship.Aggressive)
                                        Npcx2.Attacking = true;

                                    Npcx2.Selected = player;
                                    AIOption = NpcAIOption.FLY_TO_ENEMY;
                                }
                            }
                        }

                        if (!Npcx2.Moving && Npcx2.Selected == null)
                        {
                            int nextPosX = Randoms.random.Next(41800);
                            int nextPosY = Randoms.random.Next(26000);
                            Movement.Move(Npcx2, new Position(nextPosX, nextPosY));
                        }

                        break;
                    case NpcAIOption.FLY_TO_ENEMY:
                        if (Npcx2.Selected != null && Npcx2.Selected is Player && !(Npcx2.Selected as Player).Storage.IsInDemilitarizedZone && Npcx2.Position.DistanceTo((Npcx2.Selected as Player).Position) < Npcx2.RenderRange)
                        {
                            var player = Npcx2.Selected as Player;

                            Movement.Move(Npcx2, Position.GetPosOnCircle(player.Position, ALIEN_DISTANCE_TO_USER));
                            AIOption = NpcAIOption.WAIT_PLAYER_MOVE;
                        }
                        else
                        {
                            Npcx2.Attacking = false;
                            Npcx2.Selected = null;
                            AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                        }
                        break;
                    case NpcAIOption.WAIT_PLAYER_MOVE:
                        if (Npcx2.Selected != null && Npcx2.Selected is Player && !(Npcx2.Selected as Player).Storage.IsInDemilitarizedZone)
                        {
                            var player = Npcx2.Selected as Player;

                            if (player.Moving)
                                AIOption = NpcAIOption.FLY_TO_ENEMY;
                        }
                        else
                        {
                            Npcx2.Attacking = false;
                            Npcx2.Selected = null;
                            AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                        }
                        break;
                }

                lastMovement = DateTime.Now;
            }
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}