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
            cells.Create(20, 20);
            cells.ForEachPos(0, 0, 20, 20, (x, y) => cells.Set(x, y, CellType.Traversable));
            PlayerView.Bind(Player);
        }

        
/*
        private void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                var viewPos = this.Screen2View(Input.mousePosition, Camera.main);
                var hit = movableView.HitTest(viewPos);
                Debug.Log("hit=" + hit);

                var modelPos = this.Screen2Model(Input.mousePosition, Camera.main);
                movable.MoveTo((int)modelPos.x, (int)modelPos.y);
            }
        }

        private void FixedUpdate()
        {
            time.UpdateSec(UnityEngine.Time.fixedDeltaTime);
        }
            */
    }
}
