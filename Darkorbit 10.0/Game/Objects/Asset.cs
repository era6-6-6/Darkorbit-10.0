using Darkorbit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Game.Movements;
using Darkorbit.Net.netty.commands;
using Darkorbit.Managers;

namespace Darkorbit.Game.Objects
{
    class Asset : Object
    {
        public short AssetTypeId { get; set; }

        public Asset(Spacemap spacemap, Position position, short assetTypeId) : base(Randoms.CreateRandomID(), position, spacemap)
        {
            AssetTypeId = assetTypeId;
        }

        public byte[] GetAssetCreateCommand()
        {
            return AssetCreateCommand.write(new AssetTypeModule(AssetTypeId), "",
                              0, "", Id, 0, 0,
                              Position.X, Position.Y, 0, false, false, false, false,
                              new ClanRelationModule(ClanRelationModule.NONE),
                              new List<VisualModifierCommand>());
        }

        public void Remove()
        {
            Spacemap.Objects.TryRemove(Id, out var asset);
            GameManager.SendCommandToMap(Spacemap.Id, AssetRemoveCommand.write(new AssetTypeModule(AssetTypeId), Id));
        }
    }
}
