using Common.Unity.Bind;

namespace Iso.Unity.World
{
    public class BuildingsView : BindableMono<Buildings.Buildings>
    {

        public BuildingListAdapter buildingListAdapter;
        
        public override void OnBind()
        {
            base.OnBind();
            BindBindable(Model.List, buildingListAdapter);
        }
    }
}
