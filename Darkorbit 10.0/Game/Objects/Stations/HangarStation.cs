namespace Darkorbit.Game.Objects.Players.Stations
{
    class HangarStation : Activatable
    {
        public HangarStation(Spacemap spacemap, int factionId, Position position, Clan clan) : base(spacemap, factionId, position, clan, AssetTypeModule.HANGAR_HOME) { }

        public override void Click(GameSession gameSession) { }

        public override byte[] GetAssetCreateCommand(short clanRelationModule = ClanRelationModule.NONE, Player p = null)
        {
            bool showIcon = true;
            if (p != null && FactionId != p.FactionId) showIcon = false;

            return AssetCreateCommand.write(GetAssetType(), "Hangar",
                                          FactionId, "", Id, 0, 0,
                                          Position.X, Position.Y, 0, true, true, true, showIcon,
                                          new ClanRelationModule(clanRelationModule),
                                          new List<VisualModifierCommand>());
        }
    }
}
