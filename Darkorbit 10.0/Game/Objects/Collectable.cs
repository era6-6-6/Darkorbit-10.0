using Darkorbit.Utils;
using System;
using System.Linq;
using Darkorbit.Game.Movements;
using Darkorbit.Net.netty.commands;
using Darkorbit.Game.Ticks;
using Darkorbit.Game.Objects.Collectables;
using Darkorbit.Net.netty;

namespace Darkorbit.Game.Objects
{
    internal abstract class Collectable : Object, Tick
    {
        public int CollectableId { get; set; }
        public string Hash { get; set; }
        public bool Respawnable { get; set; }
        public Character Character { get; set; }
        public Player ToPlayer { get; set; }
        public bool Disposed = false;

        public int Seconds => CollectableId == AssetTypeModule.BOXTYPE_PIRATE_BOOTY ? 5 : -1;

        public static bool BoxenEvent = false;


        public Collectable(int collectableId, Position position, Spacemap spacemap, bool respawnable, Player toPlayer) : base(Randoms.CreateRandomID(), position, spacemap)
        {
            Hash = Randoms.GenerateHash(10);
            CollectableId = collectableId;
            Respawnable = respawnable;
            ToPlayer = toPlayer;

            if (this is CargoBox)
            {
                Program.TickManager.AddTick(this);
                disposeTime = DateTime.Now;
            }

        }

        public DateTime collectTime = new DateTime();
        public DateTime disposeTime = new DateTime();
        public void Tick()
        {
            if (!Disposed)
            {
                if (this is CargoBox && disposeTime.AddMinutes(2) < DateTime.Now)
                    Dispose();

                if (Character != null && Character.Collecting)
                {
                    if (!Character.Moving)
                    {

                        if (this is GreenBooty)
                        {
                            if (this is GreenBooty && Character is Player player && player.bootyKeys.greenKeys > 0)
                            {
                                if (collectTime.AddSeconds(Seconds) < DateTime.Now)
                                {
                                    Reward(Character is Pet pet ? pet.Owner : Character as Player);
                                    Dispose();
                                }
                            } else
                            {
                                CancelCollection();
                            }
                        }
                        else if (this is RedBooty)
                        {
                            if (this is RedBooty && Character is Player player && player.bootyKeys.redKeys > 0)
                            {
                                if (collectTime.AddSeconds(Seconds) < DateTime.Now)
                                {
                                    Reward(Character is Pet pet ? pet.Owner : Character as Player);
                                    Dispose();
                                }
                            }
                            else
                            {
                                CancelCollection();
                            }
                        }
                        else if (this is BlueBooty)
                        {
                            if (this is BlueBooty && Character is Player player && player.bootyKeys.blueKeys > 0)
                            {
                                if (collectTime.AddSeconds(Seconds) < DateTime.Now)
                                {
                                    Reward(Character is Pet pet ? pet.Owner : Character as Player);
                                    Dispose();
                                }
                            }
                            else
                            {
                                CancelCollection();
                            }
                        }
                        else
                        {
                            if (collectTime.AddSeconds(Seconds) < DateTime.Now)
                            {
                                Reward(Character is Pet pet ? pet.Owner : Character as Player);
                                Dispose();
                            }
                        }
                    }
                    else CancelCollection();
                }
            }

            /*
            if (!Disposed)
                foreach (var character in Spacemap.Characters.Values)
                {
                    if (character.Position.X == Position.X && character.Position.Y == Position.Y && character is Pet)
                        Collect(character as Pet);
                }
                */
        }

        public void CancelCollection()
        {
            Character.Collecting = false;

            var packet = $"0|{ServerCommands.SET_ATTRIBUTE}|{ServerCommands.ASSEMBLE_COLLECTION_BEAM_CANCELLED}|{(Character is Pet ? 1 : 0)}|{Character.Id}";

            if (Character is Player player)
            {
                //player.SendPacket($"0|LM|ST|SLC");
                player.SendPacket(packet);
            }
            else if (Character is Pet pet)
                pet.SendPacketToInRangePlayers(packet);

            if (this is GreenBooty && Character is Player && (Character as Player).bootyKeys.greenKeys <= 0)
                (Character as Player).SendPacket("0|A|STM|You do not have a green booty key. Purchase them in the shop or activate the auto purchase feature in settings.");

            if (this is RedBooty && Character is Player && (Character as Player).bootyKeys.redKeys <= 0)
                (Character as Player).SendPacket("0|A|STM|You do not have a red booty key. Purchase them in the shop or activate the auto purchase feature in settings.");

            if (this is BlueBooty && Character is Player && (Character as Player).bootyKeys.blueKeys <= 0)
                (Character as Player).SendPacket("0|A|STM|You do not have a blue booty key. Purchase them in the shop or activate the auto purchase feature in settings.");

            Character = null;

            Program.TickManager.RemoveTick(this);
        }

        public void Collect(Character character)
        {
            if (Disposed)
            {
                return;
            }

            Character = character;
            Character.Collecting = true;
            Character.Moving = false;
            collectTime = DateTime.Now;

            string packet = $"0|{ServerCommands.SET_ATTRIBUTE}|{ServerCommands.ASSEMBLE_COLLECTION_BEAM_ACTIVE}|{(Character is Pet ? 1 : 0)}|{Character.Id}|{Seconds}";

            if (Character is Player player)
            {
                //player.SendPacket($"0|LM|ST|SLA|{Seconds}");
                player.SendPacket(packet);
            }
            else if (Character is Pet pet)
            {
                pet.SendPacketToInRangePlayers(packet);
            }

            Program.TickManager.AddTick(this);
        }

        public void Dispose()
        {
            Disposed = true;
            Character = null;
            Spacemap.Objects.TryRemove(Id, out Object collectable);
            Program.TickManager.RemoveTick(this);
            GameManager.SendCommandToMap(Spacemap.Id, DisposeBoxCommand.write(Hash, true));

            if (Respawnable)
            {
                Respawn();
            }
        }

        public async void Respawn()
        {
            if (this is BonusBox)
                await System.Threading.Tasks.Task.Delay(5000);

            Position = Position.Random(Spacemap, 0, 20800, 0, 12800);

           /* if(Spacemap.Id == 16)
            {
                Position = Position.Random(Spacemap, 0, 41500, 0, 25500);
            }*/

            Spacemap.Objects.TryAdd(Id, this);

            if (this is CargoBox)
            {
                Program.TickManager.AddTick(this);
            }
            if (this is AlienEgg)
            {
                BoxenEvent = true;
                Program.TickManager.AddTick(this);
            }
            foreach (GameSession gameSession in GameManager.GameSessions.Values.Where(x => x.Player.Storage.InRangeObjects.ContainsKey(Id)))
            {
                gameSession?.Player.Storage.InRangeObjects.TryRemove(Id, out Object obj);
            }

            Disposed = false;
        }

        public abstract void Reward(Player player);

        public abstract byte[] GetCollectableCreateCommand();
    }
}
