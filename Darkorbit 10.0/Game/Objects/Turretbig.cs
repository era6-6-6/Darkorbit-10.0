namespace Darkorbit.Game.Objects.Stations
{
    class StationBase3
    {
        public int TypeId { get; set; }
        public int FactionId { get; set; }
        public List<int> Position { get; set; }
    }

    class TURRETSGRANDE : Activatable
    {


        public TURRETSGRANDE(Spacemap spacemap, int factionId, Position position, Clan clan) : base(spacemap, factionId, position, clan, AssetTypeModule.STATION_TURRET_LARGE)
        {

        }



        public override void Click(GameSession gameSession)
        {

        }

        public override byte[] GetAssetCreateCommand(short clanRelationModule = ClanRelationModule.NONE, Player p = null)
        {
            return AssetCreateCommand.write(GetAssetType(), "HQ",
                                          FactionId, "", Id, 0, 0,
                                          Position.X, Position.Y, 0, true, true, true, true,
                                          new ClanRelationModule(clanRelationModule),
                                          new List<VisualModifierCommand>());
        }
    }
}