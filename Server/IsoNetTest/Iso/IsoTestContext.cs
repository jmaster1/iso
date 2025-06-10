using Common.Api.Info;
using Common.ContextNS;
using Iso.Buildings;

namespace IsoNetTest.Iso;

public class IsoTestContext
{
    public const string BuildingId = "b0";
    
    public static void InitContext()
    {
        var infoApi = Context.Get<InfoApi>();
        infoApi.loaders.Add((_, type) =>
        {
            if (type == typeof(List<BuildingInfo>))
            {
                return new List<BuildingInfo> { 
                    new()
                    {
                        Id = BuildingId,
                        width = 2,
                        height = 2
                    }
                };
            }
            throw new Exception();
        });
    }
}