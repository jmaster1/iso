using Common.Unity.Bind;
using Iso.Player;
using UnityEngine;

namespace Iso.Unity.World
{

    [DisallowMultipleComponent]
    public class IsoPlayerView : BindableMono<IsoPlayer>
    {
        public CellsView cellsView;
        public BuildingsView buildingsView;
        public MovablesView movablesView;

        public override void OnBind()
        {
            BindBindable(Model.Cells, cellsView);
            BindBindable(Model.Buildings, buildingsView);
            BindBindable(Model.Movables, movablesView);
        }
    }

}