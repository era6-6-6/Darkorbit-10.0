global using Darkorbit.Game.Objects.Players.Techs;


namespace Darkorbit.Game.Objects.Players.Managers
{
    internal class TechManager : AbstractManager
    {
        public const string TECH_ENERGY_LEECH = "tech_energy-leech";
        public const string TECH_CHAIN_IMPULSE = "tech_chain-impulse";
        public const string TECH_PRECISION_TARGETER = "tech_precision-targeter";
        public const string TECH_BACKUP_SHIELDS = "tech_backup-shields";
        public const string TECH_BATTLE_REPAIR_BOT = "tech_battle-repair-bot";

        public PrecisionTargeter PrecisionTargeter { get; set; }
        public BackupShields BackupShields { get; set; }
        public BattleRepairBot BattleRepairBot { get; set; }
        public EnergyLeech EnergyLeech { get; set; }
        public ChainImpulse ChainImpulse { get; set; }

        public TechManager(Player player) : base(player) { InitiateTechs(); }

        public void InitiateTechs()
        {
            PrecisionTargeter = new PrecisionTargeter(Player);
            BackupShields = new BackupShields(Player);
            BattleRepairBot = new BattleRepairBot(Player);
            EnergyLeech = new EnergyLeech(Player);
            ChainImpulse = new ChainImpulse(Player);
        }

        public void Tick()
        {
            PrecisionTargeter.Tick();
            BattleRepairBot.Tick();
            EnergyLeech.Tick();
        }

        public void AssembleTechCategoryRequest(string pTechItem)
        {
            switch (pTechItem)
            {
                case TECH_PRECISION_TARGETER:
                    PrecisionTargeter.Send();
                    break;
                case TECH_BACKUP_SHIELDS:
                    BackupShields.Send();
                    break;
                case TECH_BATTLE_REPAIR_BOT:
                    BattleRepairBot.Send();
                    break;
                case TECH_ENERGY_LEECH:
                    EnergyLeech.Send();
                    break;
                case TECH_CHAIN_IMPULSE:
                    ChainImpulse.Send();
                    break;
            }
        }

    }
}
