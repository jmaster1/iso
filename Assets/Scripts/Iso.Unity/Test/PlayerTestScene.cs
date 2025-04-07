using System.Collections.Generic;
using Common.Unity.Bind;
using Common.Unity.Util.Math;
using Iso.Buildings;
using Iso.Cells;
using Iso.Movables;
using Iso.Player;
using Iso.Unity.World;
using Iso.Util;
using Math;
using UnityEngine;

namespace Iso.Unity.Test
{
    public class PlayerTestScene : MonoBehaviour
    {
        public IsoPlayer Player = new();
        
        public IsoPlayerView PlayerView;
        BuildingInfo bi = new();
        Cells.Cells Cells => Player.Cells;
        Buildings.Buildings Buildings => Player.Buildings;
        Movables.Movables Movables => Player.Movables;
        
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

                SortObjs();
            }
            
            if (Input.GetButtonDown("Fire2"))
            {
                var mpos = this.Screen2Model(Input.mousePosition, Camera.main);
                var bi = new MovableInfo
                {
                    Id = "afroman",
                    Velocity = 4
                };
                var c1 = Cells.Find(mpos.x, mpos.y);
                if (c1 != null)
                {
                    var movable = Movables.Add(bi, c1);
                    movable.Select();
                }

                SortObjs();
            }
        }

        private void SortObjs()
        {
            var cmp = new IsometricBoundsComparator();
            var list = new List<IBoundsProvider>(Player.Buildings.List.List);
            list.AddRange(Player.Movables.List.List);
            var b1 = new RectFloat();
            var b2 = new RectFloat();
            var bcmp = Comparer<IBoundsProvider>.Create((a, b) =>
            {
                a.GetBounds(b1);
                b.GetBounds(b2);
                return cmp.Compare(b1, b2);
            });
            list.Sort(bcmp);

            for (var i = 0; i < list.Count; i++)
            {
                var e = list[i];
                BindableMonoRaw view = e switch
                {
                    Building building => PlayerView.buildingsView.buildingListAdapter.Adapter.GetView(building),
                    Movable movable => PlayerView.movablesView.listAdapter.Adapter.GetView(movable),
                    _ => null
                };
                var tx = view!.transform;
                var pos = tx.position;
                tx.position = new Vector3(pos.x, pos.y, (float) (i / 10.0));
            }
        }
    }
}
