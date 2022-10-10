using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Darkorbit.Game.Objects.Players.Stations;

namespace Darkorbit.Game.Events
{
    class TDM_Team
    {
        public List<Player> players = new List<Player>();
        public string direction = "";
    }

    class TDM_Lobby
    {
        public List<Player> players = new List<Player>();
        public int mapId = 0;
        public List<TDM_Team> teams = new List<TDM_Team>();
        public bool running = false;
    }

    internal class TeamDeathmatchNew
    {
        public int UserId { get; set; }
        public static bool Active = false;
        public int roundTime = 0;
        public List<Player> waitingPlayers = new List<Player>();
        public string Mode = "";
        public List<TDM_Lobby> tdmLobbies = new List<TDM_Lobby>();
        public List<Position> leftPositions = new List<Position>();
        public List<Position> rightPositions = new List<Position>();
        private static Random rnd;
        private Task notify;

        public void Start(string mode)
        {
            rnd = new Random();

            leftPositions.Add(new Position(2775, 3050));
            leftPositions.Add(new Position(2825, 3250));
            leftPositions.Add(new Position(2875, 3450));
            leftPositions.Add(new Position(2925, 3650));
            leftPositions.Add(new Position(2975, 3850));
            leftPositions.Add(new Position(2975, 4050));
            leftPositions.Add(new Position(2925, 3650));
            leftPositions.Add(new Position(2875, 3450));
            leftPositions.Add(new Position(2825, 3250));
            leftPositions.Add(new Position(2775, 3050));

            rightPositions.Add(new Position(6775, 3050));
            rightPositions.Add(new Position(6825, 3250));
            rightPositions.Add(new Position(6875, 3450));
            rightPositions.Add(new Position(6925, 3650));
            rightPositions.Add(new Position(6975, 3850));
            rightPositions.Add(new Position(6975, 4050));
            rightPositions.Add(new Position(6925, 3650));
            rightPositions.Add(new Position(6875, 3450));
            rightPositions.Add(new Position(6825, 3250));
            rightPositions.Add(new Position(6775, 3050));

            Active = true;
            Mode = mode.Replace("tdm_", "");
            notify = Task.Factory.StartNew(() => TDMLoop());
            Loop();
        }

        public async void Loop()
        {
            while (Active)
            {
                Console.WriteLine("tdm running. players waiting: " + waitingPlayers.Count);
                switch (Mode)
                {
                    case "2v2":
                        if (waitingPlayers.Count >= 2)
                        {
                            List<Player> tmp = new List<Player>();
                            foreach (Player p in waitingPlayers)
                            {
                                if (tmp.Count < 2) tmp.Add(p);
                                else break;
                            }
                            CreateLobby(tmp, Mode);
                        }
                        break;
                    case "3v3":
                        if (waitingPlayers.Count >= 6)
                        {
                            List<Player> tmp = new List<Player>();
                            foreach (Player p in waitingPlayers)
                            {
                                if (tmp.Count < 6) tmp.Add(p);
                                else break;
                            }
                            CreateLobby(tmp, Mode);
                        }
                        break;
                    case "4v4":
                        if (waitingPlayers.Count >= 8)
                        {
                            List<Player> tmp = new List<Player>();
                            foreach (Player p in waitingPlayers)
                            {
                                if (tmp.Count < 8) tmp.Add(p);
                                else break;
                            }
                            CreateLobby(tmp, Mode);
                        }
                        break;
                }
                await Task.Delay(30 * 1000);
            }
        }

        public async void CreateLobby(List<Player> players, string mode)
        {
            TDM_Lobby tdmLobby = new TDM_Lobby();
            tdmLobby.players = players;

            tdmLobbies.Add(tdmLobby);

            OptionsBase optionsBase = new OptionsBase();
            optionsBase.RangeDisabled = true;
            optionsBase.DeathLocationRepair = false;
            optionsBase.LogoutBlocked = true;
            optionsBase.CloakBlocked = true;

            var spacemap = new Spacemap(81, "TDM I", -1, new List<NpcsBase>(), new List<PortalBase>(), new List<StationBase>(), optionsBase, true);
            int tdmId = int.Parse("77" + new Random().Next(1000000, 9999999));
            while (true)
            {
                Spacemap s = GameManager.GetSpacemap(tdmId);
                if (s != null) tdmId = int.Parse("77" + new Random().Next(1000000, 9999999));
                else break;
            }
            Console.WriteLine("tdm: " + tdmId);
            GameManager.Spacemaps.TryAdd(tdmId, spacemap);

            var teamCount = mode.Count(f => f == 'v');

            if (teamCount >= 1)
            {
                //left team
                var poi1 = new POI("uba_poi1", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(2500, 4500), new Position(2250, 4500), new Position(2250, 2000), new Position(2500, 2000) }, true, false);
                var poi2 = new POI("uba_poi2", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(3500, 2250), new Position(2250, 2250), new Position(2250, 2000), new Position(3500, 2000) }, true, false);
                var poi3 = new POI("uba_poi3", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(3500, 4500), new Position(3250, 4500), new Position(3250, 2000), new Position(3500, 2000) }, true, false);
                var poi4 = new POI("uba_poi4", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(3500, 4250), new Position(2250, 4250), new Position(2250, 4500), new Position(3500, 4500) }, true, false);
                spacemap.POIs.TryAdd("uba_poi1", poi1);
                spacemap.POIs.TryAdd("uba_poi2", poi2);
                spacemap.POIs.TryAdd("uba_poi3", poi3);
                spacemap.POIs.TryAdd("uba_poi4", poi4);

                //right team
                poi1 = new POI("uba_poi5", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(6500, 4500), new Position(6250, 4500), new Position(6250, 2000), new Position(6500, 2000) }, true, false);
                poi2 = new POI("uba_poi6", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(7500, 2250), new Position(6250, 2250), new Position(6250, 2000), new Position(7500, 2000) }, true, false);
                poi3 = new POI("uba_poi7", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(7500, 4500), new Position(7250, 4500), new Position(7250, 2000), new Position(7500, 2000) }, true, false);
                poi4 = new POI("uba_poi8", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(7500, 4250), new Position(6250, 4250), new Position(6250, 4500), new Position(7500, 4500) }, true, false);
                spacemap.POIs.TryAdd("uba_poi5", poi1);
                spacemap.POIs.TryAdd("uba_poi6", poi2);
                spacemap.POIs.TryAdd("uba_poi7", poi3);
                spacemap.POIs.TryAdd("uba_poi8", poi4);
            }
            if (teamCount == 2)
            {
                //top team
                var poi1 = new POI("uba_poi9", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(6200, 500), new Position(6200, 250), new Position(3700, 250), new Position(3700, 500) }, true, false);
                var poi2 = new POI("uba_poi10", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(6200, 1500), new Position(5950, 1500), new Position(5950, 250), new Position(6200, 250) }, true, false);
                var poi3 = new POI("uba_poi11", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(6200, 1500), new Position(6200, 1250), new Position(3700, 1250), new Position(3700, 1500) }, true, false);
                var poi4 = new POI("uba_poi12", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(3950, 1500), new Position(3700, 1500), new Position(3700, 250), new Position(3950, 250) }, true, false);
                spacemap.POIs.TryAdd("uba_poi9", poi1);
                spacemap.POIs.TryAdd("uba_poi10", poi2);
                spacemap.POIs.TryAdd("uba_poi11", poi3);
                spacemap.POIs.TryAdd("uba_poi12", poi4);
            }
            if (teamCount == 3)
            {
                //bottom team
                var poi1 = new POI("uba_poi13", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(6200, 5250), new Position(6200, 5000), new Position(3700, 5000), new Position(3700, 5250) }, true, false);
                var poi2 = new POI("uba_poi14", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(6200, 6250), new Position(5950, 6250), new Position(5950, 5000), new Position(6200, 5000) }, true, false);
                var poi3 = new POI("uba_poi15", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(6200, 6250), new Position(6200, 6000), new Position(3700, 6000), new Position(3700, 6250) }, true, false);
                var poi4 = new POI("uba_poi16", POITypes.NO_ACCESS, POIDesigns.SIMPLE, POIShapes.RECTANGLE, new List<Position> { new Position(3950, 6250), new Position(3700, 6250), new Position(3700, 5000), new Position(3950, 5000) }, true, false);
                spacemap.POIs.TryAdd("uba_poi13", poi1);
                spacemap.POIs.TryAdd("uba_poi14", poi2);
                spacemap.POIs.TryAdd("uba_poi15", poi3);
                spacemap.POIs.TryAdd("uba_poi16", poi4);
            }

            int maxPlayersPerTeam = tdmLobby.players.Count / (teamCount + 1);
            for (int i = 0; i < teamCount + 1; i++)
            {
                TDM_Team tmp = new TDM_Team();
                switch (i)
                {
                    case 0:
                        tmp.direction = "left";
                        break;
                    case 1:
                        tmp.direction = "right";
                        break;
                    case 2:
                        tmp.direction = "top";
                        break;
                    case 3:
                        tmp.direction = "bottom";
                        break;
                }
                foreach (Player p in players)
                {
                    if (!p.TDMTeamMatched && tmp.players.Count < maxPlayersPerTeam)
                    {
                        p.TDMTeamMatched = true;
                        tmp.players.Add(p);
                    }
                }
                tdmLobby.teams.Add(tmp);
            }

            tdmLobby.mapId = tdmId;

            foreach (Player p in players)
            {
                waitingPlayers.Remove(p);
                p.tdmLobby = tdmLobby;
                p.SendPacket("0|A|STD|TDM lobby created. Teleporting in a few moments...");
            }

            await Task.Delay(7 * 1000);

            foreach (TDM_Team t in tdmLobby.teams)
            {
                string direction = t.direction;
                foreach (Player p in t.players)
                {
                    switch (direction)
                    {
                        case "left":
                            p.AddVisualModifier(VisualModifierCommand.RED_GLOW, 0, "", 0, true);
                            p.TDMleft = true;
                            p.CurrentHitPoints = p.MaxHitPoints;
                            p.CurrentShieldConfig1 = p.GetMaxShieldPoints(1);
                            p.CurrentShieldConfig2 = p.GetMaxShieldPoints(2);
                            p.CpuManager.DisableCloak();
                            p.ResetCooldown();
                            p.Jump(tdmId, leftPositions[rnd.Next(0, leftPositions.Count - 1)]);
                            break;
                        case "right":
                            p.TDMright = true;
                            p.CurrentHitPoints = p.MaxHitPoints;
                            p.CurrentShieldConfig1 = p.GetMaxShieldPoints(1);
                            p.CurrentShieldConfig2 = p.GetMaxShieldPoints(2);
                            p.ResetCooldown();
                            p.CpuManager.DisableCloak();
                            p.AddVisualModifier(VisualModifierCommand.GREEN_GLOW, 0, "", 0, true);
                            p.Jump(tdmId, rightPositions[rnd.Next(0, rightPositions.Count - 1)]);
                            break;
                        case "top":
                            p.Jump(tdmId, leftPositions[rnd.Next(0, leftPositions.Count - 1)]);
                            break;
                        case "bottom":
                            p.Jump(tdmId, leftPositions[rnd.Next(0, leftPositions.Count - 1)]);
                            break;
                    }
                }
            }

            CheckIfPlayersAreOnMap(tdmLobby);
        }

        public async void CheckIfPlayersAreOnMap(TDM_Lobby l)
        {
            int playerCount = l.players.Count;
            int curPlayerCount = 0;

            while (curPlayerCount < playerCount)
            {
                foreach (Player p in l.players)
                {
                    if (p.Spacemap.Id == 81) curPlayerCount++;
                }

                await Task.Delay(1 * 1000);
            }

            await Task.Delay(5 * 1000);

            for (int i = 15; i >= 0; i--)
            {
                UbaCountdown2 elm1 = new UbaCountdown2(i, 1);
                UbaCountdown1 elm2 = new UbaCountdown1(elm1, 1000);
                if (i > 0)
                {
                    foreach (Player p in l.players)
                    {
                        p.SendCommand(elm2.write());
                    }
                }
                await Task.Delay(1 * 1000);
                if (i < 1)
                {
                    for (int j = 1; j <= 16; j++)
                    {
                        foreach (Player p in l.players)
                        {
                            p.SendCommand(MapRemovePOICommand.write("uba_poi" + j));
                        }

                        POI tmp1 = null;
                        GameManager.GetSpacemap(l.mapId).POIs.TryRemove("uba_poi" + j, out tmp1);
                    }
                    UbaCountdown init1 = new UbaCountdown(2);
                    UbaCountdown1 init2 = new UbaCountdown1(init1, 3000);
                    foreach (Player p in l.players)
                    {
                        p.SendCommand(init2.write());
                    }

                    l.running = true;

                    while (l.running)
                    {
                        roundTime--;
                        int teamsAlive = 0;

                        foreach (TDM_Team t in l.teams)
                        {
                            int alivePlayers = 0;

                            for (int j = t.players.Count - 1; j >= 0; j--)
                            {
                                if (!t.players[j].TdmDestroyed) alivePlayers++;
                            }

                            if (alivePlayers > 0)
                            {
                                teamsAlive++;
                            }
                        }

                        if (teamsAlive <= 1)
                        {
                            await Task.Delay(5 * 1000);

                            foreach (TDM_Team t in l.teams)
                            {
                                bool teamWon = false;

                                foreach (Player p2 in t.players)
                                {
                                    if (!p2.TdmDestroyed) teamWon = true;
                                }

                                ShowResult(t, teamWon);
                            }
                            DeleteTdmLobby(l);

                            break;
                        }
                        await Task.Delay(1 * 1000);
                    }
                }
            }
        }

        public List<Ubaf3kModule> GetRewards(int[] minMax, Player p)
        {
            int uridium = Randoms.random.Next(minMax[0], minMax[1]);
            int honor = Randoms.random.Next(minMax[2], minMax[3]);
            int experience = Randoms.random.Next(minMax[4], minMax[5]);
            int credits = Randoms.random.Next(minMax[6], minMax[7]);
            int ec = Randoms.random.Next(minMax[8], minMax[9]);
            int rsb75 = Randoms.random.Next(minMax[10], minMax[11]);
            int ucb100 = Randoms.random.Next(minMax[12], minMax[13]);

            List<Ubaf3kModule> tmp = new List<Ubaf3kModule>();
            tmp.Add(new Ubaf3kModule("URIDIUM", uridium));
            tmp.Add(new Ubaf3kModule("HONOR", honor));
            tmp.Add(new Ubaf3kModule("EXPERIENCE", experience));
            tmp.Add(new Ubaf3kModule("CREDITS", credits));
            tmp.Add(new Ubaf3kModule("EVENT COINS", ec));
            tmp.Add(new Ubaf3kModule("RSB", rsb75));
            tmp.Add(new Ubaf3kModule("UCB-100", ucb100));

            {
                if (uridium > 0) p.ChangeData(DataType.URIDIUM, uridium);
                if (honor > 0) p.ChangeData(DataType.HONOR, honor);
                if (experience > 0) p.ChangeData(DataType.EXPERIENCE, experience);
                if (credits > 0) p.ChangeData(DataType.CREDITS, credits);
                if (ec > 0) p.ChangeData(DataType.EC, ec);
                if (rsb75 > 0) p.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.RSB_75, rsb75);
                if (ucb100 > 0) p.AmmunitionManager.AddAmmo(Objects.Players.Managers.AmmunitionManager.UCB_100, ucb100);

            }

            return tmp;
        }

        public async void ShowResult(TDM_Team l, bool state)
        {
            if (state)
            {
                foreach (Player p in l.players)
                {
                    {
                        ShowWin(p);
                        p.RemoveVisualModifier(VisualModifierCommand.RED_GLOW);
                        p.RemoveVisualModifier(VisualModifierCommand.GREEN_GLOW);
                        p.SendCommand(UbaWindowInitializationCommand.write(new Uba43GModule(GetRewards(new int[] { 5000, 5000, 6000, 6000, 250000, 250000, 1000000, 1000000, 1, 5, 1000, 1500, 1500, 2800 }, p), (short)0, 1, 0, "Others", 0, 0), 6));
                    }
                }
            }
            else if (!state)
            {
                foreach (Player p in l.players)
                {
                    p.RemoveVisualModifier(VisualModifierCommand.RED_GLOW);
                    p.RemoveVisualModifier(VisualModifierCommand.GREEN_GLOW);
                    ShowLose(p);
                    p.SendCommand(UbaWindowInitializationCommand.write(new Uba43GModule(GetRewards(new int[] { 2500, 2500, 1000, 1000, 100000, 100000, 500000, 500000, 0, 2, 250, 500, 500, 1000 }, p), (short)1, 0, 1, "Others", 0, 0), 6));
                }

            }

            await Task.Delay(5000);

            foreach (Player p in l.players)
            {
                if(p.Spacemap.Id == 81)
                {
                    p.SetPosition(p.GetBasePosition());
                    p.Jump(p.GetBaseMapId(true), p.Position);
                }
                p.tdmLobby = null;
                p.TDMTeamMatched = false;
                p.TdmDestroyed = false;
                p.Storage.waitTDM = false;
                p.TDMright = false;
                p.TDMleft = false;
            }
        }

        public async void ShowWin(Player p)
        {
            await Task.Delay(3000);
            UbaCountdown init1 = new UbaCountdown(0);
            UbaCountdown1 init2 = new UbaCountdown1(init1, 3000);
            p.SendCommand(init2.write());
        }

        public async void ShowLose(Player p)
        {
            await Task.Delay(3000);
            p.RemoveVisualModifier(VisualModifierCommand.RED_GLOW);
            p.RemoveVisualModifier(VisualModifierCommand.GREEN_GLOW);
            UbaCountdown init1 = new UbaCountdown(1);
            UbaCountdown1 init2 = new UbaCountdown1(init1, 3000);
            p.SendCommand(init2.write());
        }
        public void TDMLoop()
        {
            try
            {
                while (Active)

                {
                    GameManager.SendPacketToAll($"0|A|STD|TDM Event Active! Type /register into the chat, to join the TDM. Type /check to see how many Players are registered ");
                    Thread.Sleep(70000);
                }
            }

            catch (Exception ex)
            {
                Logger.Log("error_log", $"- [Invasion.cs] Main void exception: {ex}");
            }

        }

        public void DeleteTdmLobby(TDM_Lobby l)
        {
            if (l != null)
            {
                foreach (Player p in l.players)
                {
                    p.tdmLobby = null;
                    p.TDMTeamMatched = false;
                    p.TdmDestroyed = false;
                }
                tdmLobbies.Remove(l);
                Spacemap tmp1 = null;
                GameManager.Spacemaps.TryRemove(l.mapId, out tmp1);
            }
        }
    }
}