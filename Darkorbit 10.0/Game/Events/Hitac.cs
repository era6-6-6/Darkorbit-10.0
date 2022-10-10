using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
{
    internal class Hitac
    {
        public Objects.Hitac Character { get; set; }

        public bool Active = false;
        //public int Type = Ship.SPACEBALL_SUMMER;
        public int Type = Ship.SHIP116;
        public List<Portal> Portals = new List<Portal>();
        public int Limit = 3;
        public bool stopCount = false;


        public async void Start()
        {
            VoteManager.MaxVotesHT += 2;

            if (Active)
            {
                return;
            }

            Active = true;
            stopCount = false;

            for (int i = 60; i > 0; i--)
            {
                string packet = $"0|A|STD|MAP[4-4]Hitac 2.0 Event {i} seconds...";
                GameManager.SendPacketToAll(packet);
                await Task.Delay(1000);

                if (stopCount)
                {
                    return;
                }

            }

            GameManager.SendPacketToAll($"0|n|KSMSG|Hitac 2.0 EVENT STARTED");
            Character = new Objects.Hitac(Randoms.CreateRandomID(), Type);

            Portals.Add(new Portal(Character.Spacemap, Character.MMOPosition, null, 0, 62, 0, true, false, false));
            Portals.Add(new Portal(Character.Spacemap, Character.EICPosition, null, 0, 61, 0, true, false, false));
            Portals.Add(new Portal(Character.Spacemap, Character.VRUPosition, null, 0, 61, 0, true, false, false));

            foreach (GameSession gameSession in GameManager.GameSessions.Values)
            {
                Player player = gameSession.Player;
                player.SettingsManager.SendMenuBarsCommand();

                foreach (Portal portal in Portals)
                {
                    GameManager.SendCommandToMap(Character.Spacemap.Id, portal.GetAssetCreateCommand());
                }
            }

            Character.Spacemap.AddCharacter(Character);

            Program.TickManager.AddTick(Character);
        }

        public bool Status()
        {
            return Active;
        }

        public void Stop()
        {

            if (!Active)
            {
                return;
            }

            using (var mySqlClient = SqlDatabaseManager.GetClient())
                mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '0'  WHERE eventoname = 'Hitac'");
            GameManager.SendPacketToAll("0|A|STD|Hitac 2.0 event ended!");
            Active = false;
            stopCount = true;
            Limit = 3;

            foreach (GameSession gameSession in GameManager.GameSessions.Values)
            {
                Player player = gameSession.Player;
                player.SettingsManager.SendMenuBarsCommand();
            }
            VoteManager.VotosHitac += 1;
            foreach (Portal portal in Portals)
            {
                portal.Remove();
            }

            Character.Spacemap.RemoveCharacter(Character);
            Program.TickManager.RemoveTick(Character);
        }
    }
}
