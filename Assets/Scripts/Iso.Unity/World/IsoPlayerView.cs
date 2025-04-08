using System.Collections.Generic;
using Common.Unity.Bind;
using Common.Unity.Util.Math;
using Iso.Buildings;
using Iso.Movables;
using Iso.Player;
using Iso.Util;
using Math;
using UnityEngine;

namespace Iso.Unity.World
{

    [DisallowMultipleComponent]
    public class IsoPlayerView : BindableMono<IsoPlayer>
    {
        public CellsView cellsView;
        public BuildingsView buildingsView;
        public MovablesView movablesView;
        private Common.TimeNS.Time time = new();
        IsometricBoundsComparator isoBoundsComparator = new();

        public override void OnBind()
        {
            BindBindable(Model.Cells, cellsView);
            BindBindable(Model.Buildings, buildingsView);
            BindBindable(Model.Movables, movablesView);
        }

        public void BindPlayerTime()
        {
            Model.Bind(time);
        }
        
        private void FixedUpdate()
        {
            time.UpdateSec(Time.fixedDeltaTime);
        }

        private void Update()
        {
            SortObjs();
        }
        
        // TODO:
        private void SortObjs()
        {
            var list = new List<IBoundsProvider>(Model.Buildings.List);
            list.AddRange(Model.Movables.List);
            var b1 = new RectFloat();
            var b2 = new RectFloat();
            var bcmp = Comparer<IBoundsProvider>.Create((a, b) =>
            {
                a.GetBounds(b1);
                b.GetBounds(b2);
                return isoBoundsComparator.Compare(b1, b2);
            });
            list.Sort(bcmp);

            for (var i = 0; i < list.Count; i++)
            {
                var e = list[i];
                BindableMonoRaw view = e switch
                {
                    Building building => buildingsView.buildingListAdapter.FindView(building),
                    Movable movable => movablesView.movableListAdapter.FindView(movable),
                    _ => null
                };
                var tx = view!.transform;
                var pos = tx.position;
                tx.position = new Vector3(pos.x, pos.y, (float) (i / 10.0));
            }
        }
    }

}