
using System;
using System.Collections.Generic;
using System.Linq;

namespace Darkorbit.Game.Objects.Players.Managers
{
    public class BoosterBase
    {
        public short Type { get; set; }
        public int Seconds { get; set; }

        public BoosterBase(short type, int seconds)
        {
            Type = type;
            Seconds = seconds;
        }
    }

    internal class BoosterManager : AbstractManager
    {
        public Dictionary<short, List<BoosterBase>> Boosters = new Dictionary<short, List<BoosterBase>>();

        public BoosterManager(Player player) : base(player) { }

        private DateTime boosterTime = new DateTime();
        public void Tick()
        {



            if (boosterTime.AddSeconds(5) < DateTime.Now)
            {
                for (short i = 0; i < Boosters.ToList().Count; i++)
                {
                    List<BoosterBase> boosters = Boosters.ToList()[i].Value;

                    for (short k = 0; k < boosters.Count; k++)
                    {
                        boosters[k].Seconds -= 5;

                        if (boosters[k].Seconds <= 0)
                        {
                            Remove((BoosterType)boosters[k].Type);
                        }
                    }
                }
                boosterTime = DateTime.Now;
            }
        }

        public void Add(BoosterType boosterType, int hours)
        {
            Player.SendPacket($"0|A|STM|{boosterType.ToString()} received");
            hours = (int)TimeSpan.FromSeconds(hours).TotalHours;

            int seconds = (int)TimeSpan.FromHours(hours).TotalSeconds;
            short boostedAttributeType = GetBoosterType((short)boosterType);

            Console.WriteLine($"Player {Player.Name} purchased / added Booster: {boosterType.ToString()} for {seconds}");

            if (boostedAttributeType != -1)
            {
                if (!Boosters.ContainsKey(boostedAttributeType))
                {
                    Boosters[boostedAttributeType] = new List<BoosterBase>();
                }

                if (Boosters[boostedAttributeType].Where(x => x.Type == (short)boosterType).Count() <= 0)
                {
                    Boosters[boostedAttributeType].Add(new BoosterBase((short)boosterType, seconds));
                }
                else
                {
                    Boosters[boostedAttributeType].Where(x => x.Type == (short)boosterType).FirstOrDefault().Seconds += seconds;
                }

                Update();
                QueryManager.SavePlayer.Boosters(Player);
            }

        }

        public void Remove(BoosterType boosterType)
        {

            short boostedAttributeType = GetBoosterType((short)boosterType);

            if (boostedAttributeType != -1)
            {
                if (Boosters.ContainsKey(boostedAttributeType))
                {
                    Boosters[boostedAttributeType].Remove(Boosters[boostedAttributeType].Where(x => x.Type == (short)boosterType).FirstOrDefault());
                }

                if (Boosters[boostedAttributeType].Count == 0)
                {
                    Boosters.Remove(boostedAttributeType);
                }

                Update();
                QueryManager.SavePlayer.Boosters(Player);
            }
        }

        public bool checkIfIssetBoosters()
        {
            bool haveBooster = false;

            if (Boosters.ContainsKey((short)BoostedAttributeType.DAMAGE) && Boosters[(short)BoostedAttributeType.DAMAGE].Count >= 1 || Boosters.ContainsKey((short)BoostedAttributeType.SHIELD) && Boosters[(short)BoostedAttributeType.SHIELD].Count >= 1 || Boosters.ContainsKey((short)BoostedAttributeType.MAXHP) && Boosters[(short)BoostedAttributeType.MAXHP].Count >= 1
                || Boosters.ContainsKey((short)BoostedAttributeType.REPAIR) && Boosters[(short)BoostedAttributeType.REPAIR].Count >= 1 || Boosters.ContainsKey((short)BoostedAttributeType.HONOUR) && Boosters[(short)BoostedAttributeType.HONOUR].Count >= 1 || Boosters.ContainsKey((short)BoostedAttributeType.EP) && Boosters[(short)BoostedAttributeType.EP].Count >= 1)
            {
                haveBooster = true;
            }

            return haveBooster;
        }

        public void Update()
        {
            List<BoosterUpdateModule> boostedAttributes = new List<BoosterUpdateModule>();

            if (Boosters.ContainsKey((short)BoostedAttributeType.DAMAGE) && Boosters[(short)BoostedAttributeType.DAMAGE].Count >= 1)
            {
                boostedAttributes.Add(new BoosterUpdateModule(new BoostedAttributeTypeModule(BoostedAttributeTypeModule.DAMAGE), GetPercentage(BoostedAttributeType.DAMAGE), Boosters[(short)BoostedAttributeType.DAMAGE].Select(x => new BoosterTypeModule(x.Type)).ToList()));
            }

            if (Boosters.ContainsKey((short)BoostedAttributeType.SHIELD) && Boosters[(short)BoostedAttributeType.SHIELD].Count >= 1)
            {
                boostedAttributes.Add(new BoosterUpdateModule(new BoostedAttributeTypeModule(BoostedAttributeTypeModule.SHIELD), GetPercentage(BoostedAttributeType.SHIELD), Boosters[(short)BoostedAttributeType.SHIELD].Select(x => new BoosterTypeModule(x.Type)).ToList()));
            }

            if (Boosters.ContainsKey((short)BoostedAttributeType.MAXHP) && Boosters[(short)BoostedAttributeType.MAXHP].Count >= 1)
            {
                boostedAttributes.Add(new BoosterUpdateModule(new BoostedAttributeTypeModule(BoostedAttributeTypeModule.MAXHP), GetPercentage(BoostedAttributeType.MAXHP), Boosters[(short)BoostedAttributeType.MAXHP].Select(x => new BoosterTypeModule(x.Type)).ToList()));
            }

            if (Boosters.ContainsKey((short)BoostedAttributeType.REPAIR) && Boosters[(short)BoostedAttributeType.REPAIR].Count >= 1)
            {
                boostedAttributes.Add(new BoosterUpdateModule(new BoostedAttributeTypeModule(BoostedAttributeTypeModule.REPAIR), GetPercentage(BoostedAttributeType.REPAIR), Boosters[(short)BoostedAttributeType.REPAIR].Select(x => new BoosterTypeModule(x.Type)).ToList()));
            }

            if (Boosters.ContainsKey((short)BoostedAttributeType.HONOUR) && Boosters[(short)BoostedAttributeType.HONOUR].Count >= 1)
            {
                boostedAttributes.Add(new BoosterUpdateModule(new BoostedAttributeTypeModule(BoostedAttributeTypeModule.HONOUR), GetPercentage(BoostedAttributeType.HONOUR), Boosters[(short)BoostedAttributeType.HONOUR].Select(x => new BoosterTypeModule(x.Type)).ToList()));
            }

            if (Boosters.ContainsKey((short)BoostedAttributeType.EP) && Boosters[(short)BoostedAttributeType.EP].Count >= 1)
            {
                boostedAttributes.Add(new BoosterUpdateModule(new BoostedAttributeTypeModule(BoostedAttributeTypeModule.EP), GetPercentage(BoostedAttributeType.EP), Boosters[(short)BoostedAttributeType.EP].Select(x => new BoosterTypeModule(x.Type)).ToList()));
            }

            //Testing
            //boostedAttributes.Add(new BoosterUpdateModule(new BoostedAttributeTypeModule(BoostedAttributeTypeModule.DAMAGE_PVE), GetPercentage(BoostedAttributeType.DAMAGE_PVE), Boosters[(short)BoostedAttributeType.DAMAGE_PVE].Select(x => new BoosterTypeModule(x.Type)).ToList()));
            //End testing

            Player.SendCommand(AttributeBoosterUpdateCommand.write(boostedAttributes));
            Player.SendCommand(AttributeHitpointUpdateCommand.write(Player.CurrentHitPoints, Player.MaxHitPoints, Player.CurrentNanoHull, Player.MaxNanoHull));
            Player.SendCommand(AttributeShieldUpdateCommand.write(Player.CurrentShieldPoints, Player.MaxShieldPoints));

            Player.SettingsManager.SendMenuBarsCommand();
        }

        public int GetPercentage(BoostedAttributeType boostedAttributeType)
        {
            int percentage = 0;

            if (Boosters.ContainsKey((short)boostedAttributeType))
            {
                foreach (BoosterBase booster in Boosters[(short)boostedAttributeType])
                {
                    percentage += GetBoosterPercentage(booster.Type);
                }
            }

            return percentage;
        }

        private short GetBoosterType(short boosterType)
        {
            short boostedAttributeType = -1;

            switch (boosterType)
            {
                case BoosterTypeModule.DMG_B01:
                case BoosterTypeModule.DMG_B02:
                case BoosterTypeModule.DMG_B03:
                case BoosterTypeModule.DMG_B04:
                    boostedAttributeType = (short)BoostedAttributeType.DAMAGE;
                    break;
                case BoosterTypeModule.SHD_B01:
                case BoosterTypeModule.SHD_B02:
                case BoosterTypeModule.SHD_B04:
                    boostedAttributeType = (short)BoostedAttributeType.SHIELD;
                    break;
                case BoosterTypeModule.HP_B01:
                case BoosterTypeModule.HP_B02:
                case BoosterTypeModule.HP_B03:
                case BoosterTypeModule.HP_B04:
                    boostedAttributeType = (short)BoostedAttributeType.MAXHP;
                    break;
                case BoosterTypeModule.REP_B01:
                case BoosterTypeModule.REP_B02:
                case BoosterTypeModule.REP_S01:

                    boostedAttributeType = (short)BoostedAttributeType.REPAIR;
                    break;
                case BoosterTypeModule.HON_B01:
                case BoosterTypeModule.HON_B02:
                case BoosterTypeModule.HON_B03:
                case BoosterTypeModule.HON50:
                case BoosterTypeModule.HON_B04:
                    boostedAttributeType = (short)BoostedAttributeType.HONOUR;
                    break;
                case BoosterTypeModule.EP_B01:
                case BoosterTypeModule.EP_B02:
                case BoosterTypeModule.EP50:
                    boostedAttributeType = (short)BoostedAttributeType.EP;
                    break;
                case BoosterTypeModule.DMG_PVE_B01:
                    boostedAttributeType = (short)BoostedAttributeType.DAMAGE_PVE;
                    break;
            }

            return boostedAttributeType;
        }

        private int GetBoosterPercentage(short boosterTypeModule)
        {
            int percentage = 0;

            switch (boosterTypeModule)
            {
                case BoosterTypeModule.DMG_B01:
                    percentage = 20;
                    break;
                case BoosterTypeModule.SHD_B02:
                    percentage = 50;
                    break;
                case BoosterTypeModule.HON_B02:
                    percentage = 10;
                    break;
                case BoosterTypeModule.HP_B02:
                    percentage = 10;
                    break;
                case BoosterTypeModule.REP_S01:
                    percentage = 20;
                    break;
                case BoosterTypeModule.DMG_B02:
                    percentage = 10;
                    break;
                case BoosterTypeModule.HP_B01:
                    percentage = 25;
                    break;
                case BoosterTypeModule.REP_B01:
                case BoosterTypeModule.HON_B01:
                    percentage = 20;
                    break;
                case BoosterTypeModule.SHD_B01:
                    percentage = 50;
                    break;
                case BoosterTypeModule.EP_B02:
                    percentage = 10;
                    break;
                case BoosterTypeModule.EP_B01:
                    percentage = 20;
                    break;
                case BoosterTypeModule.HON50:
                case BoosterTypeModule.EP50:
                    percentage = 50;
                    break;
                case BoosterTypeModule.DMG_B03:
                case BoosterTypeModule.HP_B03:
                    percentage = 5;
                    break;
                case BoosterTypeModule.HON_B03:
                    percentage = 15;
                    break;
                case BoosterTypeModule.DMG_B04:
                case BoosterTypeModule.REP_B02:
                case BoosterTypeModule.HON_B04:
                case BoosterTypeModule.HP_B04:
                case BoosterTypeModule.SHD_B04:
                    percentage = 5;
                    break;
                case BoosterTypeModule.DMG_PVE_B01:
                    percentage = 12;
                    break;



            }

            return percentage;
        }
    }
}
