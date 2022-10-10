
using Darkorbit.Game.Movements;
using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Objects.AI
{
    class FlagshipAI
    {
        public NPCFlagship NPCFlagship { get; set; }

        public NpcAIOption AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
        private static int ALIEN_DISTANCE_TO_USER = 300; //old 300

        public FlagshipAI(NPCFlagship npcflagship) { NPCFlagship = npcflagship; }

        public DateTime lastMovement = new DateTime();

        public void TickAI()
        {
            if(lastMovement.AddSeconds(1) < DateTime.Now)
            {
                switch (AIOption)
                {
                    case NpcAIOption.SEARCH_FOR_ENEMIES:
                        foreach (var players in NPCFlagship.InRangeCharacters.Values)
                        {
                            if (players is Player && players.FactionId != NPCFlagship.FactionId)
                            {
                                var player = players as Player;

                                if (player.Storage.IsInDemilitarizedZone || player.Invisible || NPCFlagship.Position.DistanceTo(player.Position) > NPCFlagship.RenderRange || player.FactionId == NPCFlagship.FactionId)
                                {
                                    NPCFlagship.Attacking = false;
                                    NPCFlagship.Selected = null;
                                    AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                                }
                                else
                                {
                                    if (NPCFlagship.Ship.Aggressive)
                                    {
                                        NPCFlagship.Attacking = true;
                                    }

                                    NPCFlagship.Selected = player;
                                    AIOption = NpcAIOption.FLY_TO_ENEMY;
                                }
                            }
                            //if (players is Npc && players.FactionId != NPCFlagship.FactionId)
                            //{
                            //    var player = players as Npc;

                            //    if (player.Storage.IsInDemilitarizedZone || player.Invisible || NPCFlagship.Position.DistanceTo(player.Position) > NPCFlagship.RenderRange)
                            //    {
                            //        NPCFlagship.Attacking = false;
                            //        NPCFlagship.Selected = null;
                            //        AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                            //    }
                            //    else
                            //    {
                            //        if (NPCFlagship.Ship.Aggressive)
                            //            NPCFlagship.Attacking = true;

                            //        NPCFlagship.Selected = player;
                            //        AIOption = NpcAIOption.FLY_TO_ENEMY;
                            //    }
                            //}
                            if (players is NPCFlagship && players.FactionId != NPCFlagship.FactionId)
                            {
                                var player = players as NPCFlagship;

                                if (player.Storage.IsInDemilitarizedZone || NPCFlagship.Position.DistanceTo(player.Position) > NPCFlagship.RenderRange || player.FactionId == NPCFlagship.FactionId)
                                {
                                    NPCFlagship.Attacking = false;
                                    NPCFlagship.Selected = null;
                                    AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                                }
                                else
                                {
                                    if (NPCFlagship.Ship.Aggressive)
                                    {
                                        NPCFlagship.Attacking = true;
                                        player.ReceiveAttack(NPCFlagship);
                                        if (player.Selected != null)
                                        {
                                            (player as NPCFlagship).FlagshipAI.AIOption = NpcAIOption.FLY_TO_ENEMY;
                                            Movement.Move(player, Position.GetPosOnCircle(player.Selected.Position, ALIEN_DISTANCE_TO_USER));
                                            //Console.WriteLine($"Attacking Attacker: {player.FlagshipAI.AIOption}");
                                        }
                                    }

                                    NPCFlagship.Selected = player;
                                    AIOption = NpcAIOption.FLY_TO_ENEMY;
                                }
                            }
                        }

                        if (!NPCFlagship.Moving && NPCFlagship.Selected == null)
                        {
                            int nextPosX = Randoms.random.Next(20000);
                            int nextPosY = Randoms.random.Next(12800);

                            Movement.Move(NPCFlagship, new Position(nextPosX, nextPosY));
                        }
                        break;
                    case NpcAIOption.FLY_TO_ENEMY:
                        if (NPCFlagship.Selected != null && NPCFlagship.Selected is Character && !(NPCFlagship.Selected as Character).Storage.IsInDemilitarizedZone && NPCFlagship.Position.DistanceTo((NPCFlagship.Selected as Character).Position) < NPCFlagship.RenderRange)
                        {
                            var player = NPCFlagship.Selected as Character;

                            if (NPCFlagship.Selected is Npc)
                            {
                                player = NPCFlagship.Selected as Npc;
                            }

                            if (NPCFlagship.Selected is Player)
                            {
                                player = NPCFlagship.Selected as Player;
                            }

                            if (NPCFlagship.Selected is NPCFlagship)
                            {
                                player = NPCFlagship.Selected as NPCFlagship;
                            }

                            Movement.Move(NPCFlagship, Position.GetPosOnCircle(player.Position, ALIEN_DISTANCE_TO_USER));
                            AIOption = NpcAIOption.WAIT_PLAYER_MOVE;
                        } 
                        else
                        {
                            NPCFlagship.Attacking = false;
                            NPCFlagship.Selected = null;
                            AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                        }
                        break;
                    case NpcAIOption.WAIT_PLAYER_MOVE:
                        if (NPCFlagship.Selected != null && NPCFlagship.Selected is Character && !(NPCFlagship.Selected as Character).Storage.IsInDemilitarizedZone)
                        {
                            var player = NPCFlagship.Selected as Character;

                            if (player.Moving)
                                AIOption = NpcAIOption.FLY_TO_ENEMY;
                        }
                        else
                        {
                            NPCFlagship.Attacking = false;
                            NPCFlagship.Selected = null;
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
