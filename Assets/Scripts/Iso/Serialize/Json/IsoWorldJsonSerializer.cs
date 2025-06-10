using System;
using Iso.Buildings;
using Iso.Cells;
using Iso.Player;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Iso.Serialize.Json
{
    public class OptInOnlyContractResolver : DefaultContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);
            contract.MemberSerialization = MemberSerialization.OptIn;
            return contract;
        }
    }
    
    public class IsoWorldJsonSerializer : AbstractPlayerJsonSerializer<IsoWorld>
    {
        public IsoWorldJsonSerializer(IsoWorld player) : base(player)
        {
        }

        protected override void DecorateSettings(JsonSerializerSettings settings)
        {
            settings.ContractResolver = new OptInOnlyContractResolver();
            settings.Converters.Add(new CellsConverter(player));
            settings.Converters.Add(new CellConverter(player));
            settings.Converters.Add(new BuildingsConverter(player));
            settings.Converters.Add(new BuildingConverter(player));
            AddInfoConverter(settings, player.Buildings.BuildingInfoSet);
        }
    }
}
