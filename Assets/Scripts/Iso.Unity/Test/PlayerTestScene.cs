using Iso.Buildings;
using Iso.Cells;
using Iso.Movables;
using Iso.Player;
using Iso.Unity.World;
using UnityEngine;

namespace Iso.Unity.Test
{
    public class PlayerTestScene : MonoBehaviour
    {
        public IsoWorld World = new();
        
        public IsoPlayerView PlayerView;
        BuildingInfo bi = new();
        Cells.Cells Cells => World.Cells;
        Buildings.Buildings Buildings => World.Buildings;
        Movables.Movables Movables => World.Movables;
        
        private void Awake()
        {
            var cells = World.Cells;
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
            // Buildings.Build(bi, 0, 0);
            // Buildings.Build(bi, 1, 1, true);
            PlayerView.Bind(World);
            PlayerView.BindPlayerTime();
        }
        
        private void Update()
        {
            var mpos = this.Screen2Model(Input.mousePosition, Camera.main);
            var viewPos = this.Screen2View(Input.mousePosition, Camera.main);
            var ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (Input.GetMouseButtonDown(0))
            {
                if (Buildings.IsBuildable(bi, (int) mpos.x, (int)mpos.y, shift))
                {
                    Buildings.Build(bi, (int) mpos.x, (int)mpos.y, shift);
                }
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                var hit = PlayerView.movablesView.HitTest(viewPos, view =>
                {
                    view.Model.Select();
                    return false;
                });
                
                if(!hit && ctrl)
                {
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
                }


                var selected = Movables.FindSelected();
                if (!hit && selected != null && !ctrl)
                {
                    selected.MoveTo((int)mpos.x, (int)mpos.y);
                }
            }
        }
    }
}
