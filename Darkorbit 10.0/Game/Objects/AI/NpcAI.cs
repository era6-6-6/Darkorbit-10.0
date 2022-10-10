
using Darkorbit.Game;
using Darkorbit.Game.Movements;
using Darkorbit.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game.Objects.AI
{
    class NpcAI
    {
        public Npc Npc { get; set; }

        public NpcAIOption AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
        private static int ALIEN_DISTANCE_TO_USER = 300;

        public NpcAI(Npc npc) { Npc = npc; }

        public DateTime lastMovement = new DateTime();

        public void SwitchEnemy()
        {
            Random rand = new Random();
            int playerCount = 0;

            foreach (var players in Npc.InRangeCharacters.Values)
            {
                if (players is Player) playerCount++;
            }

            int randChar = 0;
            if (playerCount > 0) randChar = rand.Next(0, playerCount);
            int count = 0;

            foreach (var players in Npc.InRangeCharacters.Values)
            {
                if (players is Player)
                {
                    if (count == randChar)
                    {
                        var player = players as Player;

                        if (player.Storage.IsInDemilitarizedZone || player.Invisible || Npc.Position.DistanceTo(player.Position) > Npc.RenderRange1)
                        {
                            Npc.Attacking = false;
                            Npc.Selected = null;
                            AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                        }
                        else
                        {
                            if (Npc.Ship.Aggressive || Npc.aggressive)
                            {
                                Npc.Attacking = true;
                                Npc.Selected = player;
                                AIOption = NpcAIOption.FLY_TO_ENEMY;
                            }
                            else
                            {
                                AIOption = NpcAIOption.WAIT_PLAYER_MOVE;
                            }
                        }
                    }
                    count++;
                }
            }
        }

        public void TickAI()
        {
            if (Npc.Id == 80) return;
            if (lastMovement.AddSeconds(1) < DateTime.Now)
            {
                if (Npc.Spacemap.Id == 62 || Npc.Spacemap.Id == 63)
                {
                    if (Npc.Selected == null && Npc.MainAttacker == null)
                    {
                        Random rand = new Random();
                        int playerCount = 0;

                        foreach (var players in Npc.InRangeCharacters.Values)
                        {
                            if (players is Player) playerCount++;
                        }

                        int randChar = 0;
                        if (playerCount > 0) randChar = rand.Next(0, playerCount);
                        int count = 0;

                        foreach (var p in Npc.Spacemap.Characters.Values)
                        {
                            if (p is Player p1)
                            {
                                if (count == randChar)
                                {
                                    Npc.Attacking = true;
                                    Npc.Selected = p1;
                                    if (!Npc.Moving) Movement.Move(Npc, Position.GetPosOnCircle(p1.Position, ALIEN_DISTANCE_TO_USER));
                                }
                                count++;
                            }
                        }


                    } else
                    {
                        if (Npc.MainAttacker != null)
                        {
                            if (!Npc.Moving) Movement.Move(Npc, Position.GetPosOnCircle(Npc.MainAttacker.Position, ALIEN_DISTANCE_TO_USER));
                        } else if(Npc.Selected != null)
                        {
                            if (!Npc.Moving) Movement.Move(Npc, Position.GetPosOnCircle(Npc.Selected.Position, ALIEN_DISTANCE_TO_USER));
                        }
                    }
                }
                
                else
                {
                    if (AIOption == NpcAIOption.FLY_TO_ENEMY && Npc.Selected != null && Npc.MainAttacker == null)
                    {
                        if (Npc.switchEnemy <= 0)
                        {
                            Npc.switchEnemy = Npc.switchEnemyDefault;
                            AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                            Npc.Selected = null;
                        }
                        else
                        {
                            Npc.switchEnemy--;
                        }
                    }
                    else if (AIOption == NpcAIOption.CUBIKON_POSITION_MOVE && Npc.MainAttacker != null)
                    {
                        if (Npc.switchEnemy <= 0)
                        {
                            Npc.switchEnemy = Npc.switchEnemyDefault;
                        }
                        else
                        {
                            Npc.switchEnemy--;
                        }
                    }

                    switch (AIOption)
                    {
                        case NpcAIOption.SEARCH_FOR_ENEMIES:
                            Npc.destPosition = null;

                            if (Npc.MainAttacker is Player)
                            {
                                if (Npc.MainAttacker.Storage.IsInDemilitarizedZone || Npc.MainAttacker.Invisible || Npc.Position.DistanceTo(Npc.MainAttacker.Position) > Npc.RenderRange1)
                                {
                                    Npc.Attacking = false;
                                    Npc.Selected = null;
                                    AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                                }
                                else
                                {
                                    if (Npc.Ship.Aggressive || Npc.aggressive)
                                        Npc.Attacking = true;
                                    Npc.Selected = Npc.MainAttacker;
                                    AIOption = NpcAIOption.FLY_TO_ENEMY;
                                }
                            }
                            else
                            {
                                SwitchEnemy();
                            }
                            if (!Npc.Moving && Npc.Selected == null)
                            {
                                int nextPosX = Randoms.random.Next(20000);
                                int nextPosY = Randoms.random.Next(12800);
                                Movement.Move(Npc, new Position(nextPosX, nextPosY));
                            }
                            break;
                        case NpcAIOption.FLY_TO_ENEMY:
                            if (Npc.Selected != null && Npc.Selected is Player && !(Npc.Selected as Player).Storage.IsInDemilitarizedZone && Npc.Position.DistanceTo((Npc.Selected as Player).Position) < Npc.RenderRange1)
                            {
                                var player = Npc.Selected as Player;

                                if (Npc.destPosition == null)
                                {
                                    Npc.destPosition = Position.GetPosOnCircle(player.Position, ALIEN_DISTANCE_TO_USER);
                                }
                                else
                                {
                                    if (!Npc.Moving) Npc.destPosition = Position.GetPosOnCircle(player.Position, ALIEN_DISTANCE_TO_USER);
                                }
                                Movement.Move(Npc, Npc.destPosition);
                                AIOption = NpcAIOption.FLY_TO_ENEMY;
                            }
                            else
                            {
                                Npc.Attacking = false;
                                Npc.Selected = null;
                                AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                            }
                            break;
                        case NpcAIOption.WAIT_PLAYER_MOVE:
                            if (Npc.Selected != null && Npc.Selected is Player && !(Npc.Selected as Player).Storage.IsInDemilitarizedZone)
                            {
                                var player = Npc.Selected as Player;

                                if (player.Moving)
                                    AIOption = NpcAIOption.FLY_TO_ENEMY;
                            }
                            else
                            {
                                Npc.Attacking = false;
                                Npc.Selected = null;
                                AIOption = NpcAIOption.SEARCH_FOR_ENEMIES;
                            }
                            break;
                        case NpcAIOption.RANDOM_POSITION_MOVE:
                            if (!Npc.Moving) Movement.Move(Npc, new Position(Randoms.random.Next(0, 20600), Randoms.random.Next(0, 12600)));

                            break;
                        case NpcAIOption.CUBIKON_POSITION_MOVE:
                            int x1 = Npc.InitialPosition.X - 200;
                            int x2 = Npc.InitialPosition.X + 200;
                            int y1 = Npc.InitialPosition.Y - 200;
                            int y2 = Npc.InitialPosition.Y + 200;

                            if (Npc.switchEnemy == 0)
                            {
                                var attackers = Npc.Attackers.ToArray();
                                Random rand = new Random();
                                int rand1 = rand.Next(attackers.Length);
                                for (var i = 0; i < attackers.Length; i++)
                                {
                                    if (i == rand1)
                                    {
                                        Npc.cubiMainAttacker = attackers[i].Value.Player;
                                    }

                                }
                            }

                            Position pos = new Position(Randoms.random.Next(x1, x2), Randoms.random.Next(y1, y2));
                            if (!Npc.Moving) Movement.Move(Npc, pos);

                            break;
                        case NpcAIOption.PROTI_POSITION_MOVE:
                            if (lastMovement.AddSeconds(1) < DateTime.Now)
                                if (Npc.mother.MainAttacker is Player)
                            {
                                if (Npc.mother.MainAttacker.Storage.IsInDemilitarizedZone || Npc.mother.MainAttacker.Invisible || Npc.Position.DistanceTo(Npc.mother.MainAttacker.Position) > Npc.RenderRange1)
                                {
                                    Npc.Attacking = true;
                                    Npc.Selected = null;
                                }
                                else
                                {
                                    if (Npc.Ship.Aggressive || Npc.aggressive)
                                        Npc.Attacking = true;

                                    if (Npc.mother.cubiMainAttacker != null) Npc.Selected = Npc.mother.cubiMainAttacker;
                                    else Npc.Selected = Npc.mother.MainAttacker;
                                }
                            }

                            int x1P = Npc.mother.Position.X - 1000;
                            int x2P = Npc.mother.Position.X + 1000;
                            int y1P = Npc.mother.Position.Y - 1000;
                            int y2P = Npc.mother.Position.Y + 1000;

                            if (!Npc.Moving) Movement.Move(Npc, new Position(Randoms.random.Next(x1P, x2P), Randoms.random.Next(y1P, y2P)));

                            break;

                    }
                }

                lastMovement = DateTime.Now;
            }
        }

        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
