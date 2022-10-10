using Darkorbit.Game.Events;
using Darkorbit.Game.Objects;
using Darkorbit.Game.Ticks;
using Darkorbit.Managers;
using Darkorbit.Net;
using Darkorbit.Net.netty.commands;
using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Game
{
    class GameSession : Tick
    {
        public enum DisconnectionType
        {
            NORMAL,
            INACTIVITY,
            ADMIN,
            SOCKET_CLOSED,
            ERROR
        }

        public Player Player { get; set; }
        public GameClient Client { get; set; }

        public DateTime LastActiveTime = new DateTime();
        public bool InProcessOfDisconnection = false;
        public DateTime EstDisconnectionTime = new DateTime();
        public bool inDC = false;
        public DateTime TimeAfk { get; set; }

        public GameSession(Player player)
        {
            Player = player;
            Program.TickManager.AddTickPlayer(this);
        }

        public void Tick()
        {
            if (LastActiveTime.AddMinutes(43800) < DateTime.Now && !Player.AttackingOrUnderAttack(10))
                Disconnect(DisconnectionType.INACTIVITY);
            if (LastActiveTime.AddMinutes(52559942) < DateTime.Now && !Player.AttackingOrUnderAttack(10))
                Disconnect(DisconnectionType.ADMIN);
            if (EstDisconnectionTime < DateTime.Now && InProcessOfDisconnection)
                Disconnect(DisconnectionType.NORMAL);


            CheckAfk();
            CheckInGG();

        }
        
        private void CheckInGG()
        {

            if (inDC) { return; }

            if (Player.positionInitializacion.mapID == 51 || Player.positionInitializacion.mapID == 52 || Player.positionInitializacion.mapID == 53 || Player.positionInitializacion.mapID == 1111 || Player.positionInitializacion.mapID == 74 || Player.positionInitializacion.mapID == 76 || Player.positionInitializacion.mapID == 75)
            {
                if (!GameManager.playersInGalaxyGates.ContainsKey(Player.Id))
                {
                    GameManager.playersInGalaxyGates.TryAdd(Player.Id, true);
                }
            }
            else
            {
                if (GameManager.playersInGalaxyGates.ContainsKey(Player.Id))
                {
                    GameManager.playersInGalaxyGates.TryRemove(Player.Id, out var playersInGalaxyGates);
                }
            }

        }
        
        private void CheckAfk()
        {

            if (inDC) { return; }

            if (LastActiveTime.AddMinutes(5) < DateTime.Now && !Player.AttackingOrUnderAttack(10) && !Player.Storage.Jumping)
            {
                if (!GameManager.afkPlayers.ContainsKey(Player.Id))
                {
                    GameManager.afkPlayers.TryAdd(Player.Id, true);
                    TimeAfk = DateTime.Now;
                    //GameManager.SendPacketToAll($"0|A|STD| {Player.Name} is now AFK. ");
                }
            }
            else if (!Player.AttackingOrUnderAttack(10) && !Player.Storage.Jumping)
            {
                if (GameManager.afkPlayers.ContainsKey(Player.Id))
                {
                    GameManager.afkPlayers.TryRemove(Player.Id, out var afkPlayers);
                    TimeSpan afkTime = DateTime.Now - TimeAfk;
                    //GameManager.SendPacketToAll($"0|A|STD| {Player.Name} back from AFK. "+ afkTime.Minutes + " minutes, "+ afkTime.Seconds + " seconds afk.");
                }
            }

        }

        private void PrepareForDisconnect()
        {
            try
            {
                
                Player.TDMEnd = true;
                Player.LastCombatTime = DateTime.Now.AddSeconds(-999);
                Player.Group?.Leave(Player);
                Player.DisableAttack(Player.Settings.InGameSettings.selectedLaser);
                Duel.RemovePlayer(Player);
                Program.TickManager.RemoveTick(Player);
                Player.Spacemap.RemoveCharacter(Player);
                Player.Deselection();
                //PROBANDO PET DESCONECTAR
                if (Player.Pet != null && Player.Pet.Activated)
                {
                    Player.Pet.Deactivate();
                }
            }
            catch (Exception e)
            {
                Out.WriteLine("PrepareForDisconnect void exception: " + e, "GameSession.cs");
                Logger.Log("error_log", $"- [GameSession.cs] PrepareForDisconnect void exception: {e}");
            }
        }

        public void Disconnect(DisconnectionType dcType)
        {
            try
            {
                Player.Group?.UpdateTarget(Player, new List<command_i3O> { new GroupPlayerDisconnectedModule(true) });
                Player.UpdateCurrentCooldowns();
                Player.SaveSettings();

                /* Map Login / Datagrap for Player for Black Light Maps is Broken. This snures, the Player can login */

                //if (Player.Spacemap.Id == 306 || Player.Spacemap.Id == 307 || Player.Spacemap.Id == 308)
                //{
                //    Player.positionInitializacion.mapID = 16;
                //    Player.positionInitializacion.x = 0;
                //    Player.positionInitializacion.y = 0;
                //}
                QueryManager.SavePlayer.Information(Player);
                QueryManager.SavePlayer.Boosters(Player);
                QueryManager.SavePlayer.Ammunition(Player);
                QueryManager.SavePlayer.Items(Player);
                QueryManager.SavePlayer.saveEC(Player);
                QueryManager.SavePlayer.formations(Player);

                Player.Storage.InRangeAssets.Clear();
                Player.Storage.InRangeObjects.Clear();
                Player.InRangeCharacters.Clear();
                Player.TdmDestroyed = true;
                Player.TDMleft = false;
                Player.TDMright = false;
                EventManager.UltimateBattleArena.RemoveWaitingPlayer(Player);
                Player.TDMEnd = true;

                InProcessOfDisconnection = true;

                if(Player.Spacemap.Id == 81)
                {
                    Player.SetPosition(Player.GetBasePosition());
                }

                if (Player.Pet != null && Player.Pet.Activated)
                {
                    Player.Pet.Deactivate();
                }

                if (dcType == DisconnectionType.SOCKET_CLOSED)
                {
                    EventManager.UltimateBattleArena.RemoveWaitingPlayer(Player);
                    Player.TdmDestroyed = true;
                    Player.TDMEnd = true;
                    Player.TDMleft = false;
                    Player.TDMright = false;
                    EstDisconnectionTime = DateTime.Now.AddMinutes(1);
                    return;
                }

                foreach (var session in GameManager.GameSessions.Values)
                {
                    if (session != null)
                    {
                        var player = session.Player;
                        if (player.Storage.GroupInvites.ContainsKey(Player.Id))
                        {
                            player.Storage.GroupInvites.Remove(Player.Id);
                            player.SendCommand(GroupRemoveInvitationCommand.write(Player.Id, player.Id, GroupRemoveInvitationCommand.REVOKE));
                        }
                    }
                }
                PrepareForDisconnect();
                Player.TDMEnd = true;
                Player.TDMleft = false;
                Player.TDMright = false;
                Client.Close();
                EventManager.UltimateBattleArena.RemoveWaitingPlayer(Player);
                Player.TdmDestroyed = true;
                InProcessOfDisconnection = false;
                Program.TickManager.RemoveTick(this);

                GameManager.playersInGalaxyGates.TryRemove(Player.Id, out var playersInGalaxyGates);

                GameManager.afkPlayers.TryRemove(Player.Id, out var afkPlayers);

                GameManager.GameSessions.TryRemove(Player.Id, out var gameSession);

                Console.Title = $"Emulator | {GameManager.GameSessions.Count} users online";

                inDC = true;
            }
            catch (Exception e)
            {
                Out.WriteLine("Disconnect void exception: " + e, "GameSession.cs");
                Logger.Log("error_log", $"- [GameSession.cs] Disconnect void exception: {e}");
            }
        }
    }
}
