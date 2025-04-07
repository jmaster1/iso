using Common.Unity.Bind;
using Iso.Player;
using UnityEngine;

namespace Iso.Unity.World
{

    [DisallowMultipleComponent]
    public class IsoPlayerView : BindableMono<IsoPlayer>
    {
        public CellsView cellsView;

        public override void OnBind()
        {
            BindBindable(Model.Cells, cellsView);
        }
    }

}