using Common.Util;
using Iso.Cells;
using Iso.Movables;
using Iso.Player;
using Iso.Unity.World;
using Math;
using UnityEngine;
using Time = Common.TimeNS.Time;

namespace Iso.Unity.Test
{
    public class MovableTestScene : MonoBehaviour
    {
        public CellsView cellsView;
        
        public MovableView movableView;

        public int cellsWidth = 20;
        
        public int cellsHeight = 20;
        
        public int blockedCells = 20;

        public readonly IsoPlayer Player = new();
        public Cells.Cells cells => Player.Cells;
        public Buildings.Buildings buildings => Player.Buildings;
        public Movables.Movables movables => Player.Movables;
            
        private Movable movable;

        private Time time = new();
        
        private void Awake()
        {
            CreateCells();
            CreateMovables();
        }

        private void CreateMovables()
        {
            movables.Time = time;
            movables.Start();
            
            var bi = new MovableInfo
            {
                Id = "M1",
                Velocity = 4
            };
            var c1 = cells.Get(0, 0);
            movable = movables.Add(bi, c1);
            
            movableView.Bind(movable);
            movableView.GetComponent<MovablePathView>()?.Bind(movable);
            //movable.MoveTo(cellsWidth - 1, cellsHeight - 1);
        }

        private void CreateCells()
        {
            cells.Create(cellsWidth, cellsHeight);
            var rc = new RectFloat(0, 0, cellsWidth, cellsHeight);
            cells.ForEachPos(rc, (x, y) => cells.Set(x, y, CellType.Traversable));

            for (var i = 0; i < blockedCells; i++)
            {
                var pos = rc.RandomPointInside(Rnd.Instance);
                var cell = cells.Set((int)pos.x, (int)pos.y, CellType.Blocked);
                if (cell.Is(0, 0))
                {
                    cell.Set(CellType.Traversable);
                }
            }
            cellsView.Bind(cells);
        }

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
    }
}
