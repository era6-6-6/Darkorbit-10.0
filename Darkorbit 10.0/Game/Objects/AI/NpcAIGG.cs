using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Game.Movements;

namespace Darkorbit.Game.Objects.AI
{
    class NpcAIGG
    {
        public NpcGG NpcGG { get; set; }

        public NpcAIOption AIOption = NpcAIOption.FLY_TO_ENEMY;
        private static int ALIEN_DISTANCE_TO_USER = 300;

        public NpcAIGG(NpcGG npcgg) { NpcGG = npcgg; }

        public DateTime lastMovement = new DateTime();

        public void SwitchEnemy()
        {
            Random rand = new Random();
            int playerCount = 0;

            foreach (var players in NpcGG.Spacemap.Characters.Values)
            {
                if (players is Player p && (p.AlphaGate != null && p.AlphaGate.GetGateMapId() == NpcGG.gateId || p.BetaGates != null && p.BetaGates.GetGateMapId() == NpcGG.gateId || p.GammaGates != null && p.GammaGates.GetGateMapId() == NpcGG.gateId)) playerCount++;
            }

            int randChar = 0;
            if (playerCount > 0) randChar = rand.Next(0, playerCount);
            int count = 0;

            foreach (var players in NpcGG.Spacemap.Characters.Values)
            {
                if (players is Player p && (p.AlphaGate != null && p.AlphaGate.GetGateMapId() == NpcGG.gateId || p.BetaGates != null && p.BetaGates.GetGateMapId() == NpcGG.gateId || p.GammaGates != null && p.GammaGates.GetGateMapId() == NpcGG.gateId))
                {
                    if (count == randChar)
                    {
                        var player = players as Player;

                        NpcGG.Attacking = true;
                        NpcGG.Selected = player;
                        AIOption = NpcAIOption.FLY_TO_ENEMY;
                    }
                    count++;
                }
            }
        }

        public void TickAI()
        {
            if (lastMovement.AddSeconds(1) < DateTime.Now)
            {
                switch (AIOption)
                {
                    case NpcAIOption.SEARCH_FOR_ENEMIES:
                        NpcGG.destPosition = null;

                        if (NpcGG.MainAttacker is Player)
                        {
                            NpcGG.Attacking = true;
                            NpcGG.Selected = NpcGG.MainAttacker;
                            AIOption = NpcAIOption.FLY_TO_ENEMY;
                        }
                        else
                        {
                            SwitchEnemy();
                        }
                        break;
                    case NpcAIOption.FLY_TO_ENEMY:
                        if (NpcGG.Selected != null && NpcGG.Selected is Player/* && !(NpcGG.Selected as Player).Storage.IsInDemilitarizedZone && NpcGG.Position.DistanceTo((NpcGG.Selected as Player).Position) < NpcGG.RenderRange1*/)
                        {
                            var player = NpcGG.Selected as Player;

                            if (NpcGG.destPosition == null)
                            {
                                NpcGG.destPosition = Position.GetPosOnCircle(player.Position, ALIEN_DISTANCE_TO_USER);
                            }
                            else
                            {
                                if (!NpcGG.Moving) NpcGG.destPosition = Position.GetPosOnCircle(player.Position, ALIEN_DISTANCE_TO_USER);
                            }
                            Movement.Move(NpcGG, NpcGG.destPosition);
                            AIOption = NpcAIOption.FLY_TO_ENEMY;
                        }
                        else
                        {
                            NpcGG.Attacking = false;
                            NpcGG.Selected = null;
                            AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                        }
                        break;
                    case NpcAIOption.WAIT_PLAYER_MOVE:
                        if (NpcGG.Selected != null && NpcGG.Selected is Player/* && !(NpcGG.Selected as Player).Storage.IsInDemilitarizedZone*/)
                        {
                            var player = NpcGG.Selected as Player;

                            if (player.Moving)
                                AIOption = NpcAIOption.FLY_TO_ENEMY;
                        }
                        else
                        {
                            NpcGG.Attacking = false;
                            NpcGG.Selected = null;
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
