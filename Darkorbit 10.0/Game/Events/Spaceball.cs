using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Darkorbit.Game.Events
{
    internal class Spaceball
    {
        public Objects.Spaceball Character { get; set; }

        public bool Active = false;
        //public int Type = Ship.SPACEBALL_SUMMER;
        public int Type = Ship.SPACEBALL_SOCCER;
        public List<Portal> Portals = new List<Portal>();
        public int Limit = 20;
        public bool stopCount = false;


        public async void Start()
        {
            VoteManager.MaxVoteSP += 2;

            if (Active)
            {
                return;
            }

            Active = true;
            stopCount = false;

            for (int i = 10; i > 0; i--)
            {
                string packet = $"0|A|STD|MAP[4-4]SpaceBall Event {i} seconds...";
                GameManager.SendPacketToAll(packet);
                await Task.Delay(1000);

                if (stopCount)
                {
                    return;
                }

            }
            GameManager.SendPacketToAll($"0|n|KSMSG|SPACEBALL EVENT STARTED");
            Character = new Objects.Spaceball(Randoms.CreateRandomID(), Type);

            //Portals.Add(new Portal(Character.Spacemap, new Movements.Position(11111111 , 100000000), null, 0, 61, 0, false, false, false));
           // Portals.Add(new Portal(Character.Spacemap, Character.EICPosition, null, 0, 61, 0, true, false, false));
            //Portals.Add(new Portal(Character.Spacemap, new Movements.Position(1111111 , 100000000), null, 0, 61, 0, false, false, false));

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
            mySqlClient.ExecuteNonQuery($"UPDATE event SET active = '0'  WHERE eventoname = 'Spaceball'");
            GameManager.SendPacketToAll("0|A|STD|Spaceball event ended");
            Active = false;
            stopCount = true;
            Limit = 20;

            foreach (GameSession gameSession in GameManager.GameSessions.Values)
            {
                Player player = gameSession.Player;
                player.SettingsManager.SendMenuBarsCommand();
            }
            VoteManager.VotosSpaceball += 2;
            foreach (Portal portal in Portals)
            {
                portal.Remove();
            }

            Character.Spacemap.RemoveCharacter(Character);
            Program.TickManager.RemoveTick(Character);
        }
    }
}
