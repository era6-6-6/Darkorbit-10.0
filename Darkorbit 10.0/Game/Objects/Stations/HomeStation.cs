namespace Darkorbit.Game.Objects.Players.Stations
{
    class StationBase
    {
        public int TypeId { get; set; }
        public int FactionId { get; set; }
        public List<int> Position { get; set; }
    }

    class HomeStation : Activatable
    {
        public static int SECURE_ZONE_RANGE = 
            1700;

        public RepairStation RepairStation { get; set; }
        public HangarStation HangarStation { get; set; }
        public QuestGiverStation QuestGiverStation { get; set; }
        public OreTradeStation OreTradeStation { get; set; }

        public HomeStation(Spacemap spacemap, int factionId, Position position, Clan clan) : base(spacemap, factionId, position, clan, AssetTypeModule.BASE_COMPANY)
        {
            PrepareStations();
        }

        public void PrepareStations()
        {
            var rPosition = new Position(Position.X, Position.Y - 600);
            RepairStation = new RepairStation(Spacemap, FactionId, rPosition, Clan);

            var hPosition = new Position(Position.X + 600, Position.Y);
            HangarStation = new HangarStation(Spacemap, FactionId, hPosition, Clan);

            var qPosition = new Position(Position.X, Position.Y + 600);
            QuestGiverStation = new QuestGiverStation(Spacemap, FactionId, qPosition, Clan);

            var oPosition = new Position(Position.X - 600, Position.Y);
            OreTradeStation = new OreTradeStation(Spacemap, FactionId, oPosition, Clan);
        }

        public override void Click(GameSession gameSession)
        {

        }

        public override byte[] GetAssetCreateCommand(short clanRelationModule = ClanRelationModule.NONE, Player p = null)
        {
            bool showIcon = true;
            if (p != null && FactionId != p.FactionId) showIcon = false;

            return AssetCreateCommand.write(GetAssetType(), "HQ",
                                          FactionId, "", Id, 0, 0,
                                          Position.X, Position.Y, 0, true, true, true, showIcon,
                                          new ClanRelationModule(clanRelationModule),
                                          new List<VisualModifierCommand>());
        }
    }
}
