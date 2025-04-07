using Common.Util;
using Iso.Buildings;
using Iso.Cells;
using Iso.Player;
using Iso.Unity.World;
using UnityEngine;

namespace Iso.Unity.Test
{
    public class PlayerTestScene : MonoBehaviour
    {
        public IsoPlayer Player = new();
        
        public IsoPlayerView PlayerView;
        
        private void Awake()
        {
            var cells = Player.Cells;
            var w = 20;
            var h = 20;
            cells.Create(w, h);
            cells.ForEachPos(0, 0, w, h, (x, y) => cells.Set(x, y, CellType.Buildable));

            var buildings = Player.Buildings;
            var bi = new BuildingInfo();
            bi.Id = "rock1x4";
            bi.width = bi.height = 2;

            for (var i = 0; i < 100; i++)
            {
                var x = Rnd.Instance.RandomIntIncl(0, w - bi.width);
                var y = Rnd.Instance.RandomIntIncl(0, h - bi.height);
                var flip = Rnd.Instance.RandomBool();
                var cell = cells.Get(x, y);
                if (buildings.IsBuildable(bi, cell, flip))
                {
                    buildings.Build(bi, cell, flip);
                }
            }
            PlayerView.Bind(Player);
        }
    }
}
