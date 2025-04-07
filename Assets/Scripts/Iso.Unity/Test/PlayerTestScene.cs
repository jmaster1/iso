using System.Collections.Generic;
using Common.Unity.Util.Math;
using Common.Util;
using Iso.Buildings;
using Iso.Cells;
using Iso.Player;
using Iso.Unity.World;
using Math;
using UnityEngine;

namespace Iso.Unity.Test
{
    public class PlayerTestScene : MonoBehaviour
    {
        public IsoPlayer Player = new();
        
        public IsoPlayerView PlayerView;
        BuildingInfo bi = new();
        Buildings.Buildings Buildings => Player.Buildings;
        
        private void Awake()
        {
            var cells = Player.Cells;
            var w = 20;
            var h = 20;
            cells.Create(w, h);
            cells.ForEachPos(0, 0, w, h, (x, y) => cells.Set(x, y, CellType.Buildable));
            
            bi.Id = "rock1x4";
            bi.width = 4;
            bi.height = 1;

            // for (var i = 0; i < 100; i++)
            // {
            //     var x = Rnd.Instance.RandomIntIncl(0, w - bi.width);
            //     var y = Rnd.Instance.RandomIntIncl(0, h - bi.height);
            //     var flip = Rnd.Instance.RandomBool();
            //     var cell = cells.Get(x, y);
            //     if (buildings.IsBuildable(bi, cell, flip))
            //     {
            //         buildings.Build(bi, cell, flip);
            //     }
            // }
            Buildings.Build(bi, 0, 0);
            Buildings.Build(bi, 1, 1, true);
            PlayerView.Bind(Player);
        }
        
        private void Update()
        {
            

            if (Input.GetButtonDown("Fire1"))
            {
                var flip = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                var mpos = this.Screen2Model(Input.mousePosition, Camera.main);
                if (Buildings.IsBuildable(bi, (int) mpos.x, (int)mpos.y, flip))
                {
                    Buildings.Build(bi, (int) mpos.x, (int)mpos.y, flip);
                }
                
                var cmp = new IsometricBoundsComparator();
                var list = new List<Building>(Player.Buildings.List.List);
                var b1 = new RectFloat();
                var b2 = new RectFloat();
                var bcmp = Comparer<Building>.Create((a, b) =>
                {
                    a.GetBounds(b1);
                    b.GetBounds(b2);
                    return cmp.Compare(b1, b2);
                });
                list.Sort(bcmp);

                for (var i = 0; i < list.Count; i++)
                {
                    var b = list[i];
                    var bv = PlayerView.buildingsView.buildingListAdapter.Adapter.GetView(b);
                    var pos = bv.transform.position;
                    bv.transform.position = new Vector3(pos.x, pos.y, (float) (i / 10.0));
                }
            }
        }
    }
}
