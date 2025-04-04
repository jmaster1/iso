using Common.Util;
using Iso.Cells;
using Iso.Movables;
using Iso.Unity.World;
using Math;
using UnityEngine;
using UnityEngine.Serialization;
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

        private Cells.Cells cells = new();

        private Movables.Movables movables = new();
            
        private Movable movable;

        private Time time = new();
        
        private void Awake()
        {
            createCells();
            createMovables();
        }

        private void createMovables()
        {
            movables.Cells = cells;
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

        private void createCells()
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
            var prj = movableView.prj;
            if (Input.GetButtonDown("Fire1"))
            {
                var viewPos = Screen2View(Input.mousePosition, Camera.main);
                var hit = movableView.HitTest(viewPos);
                Debug.Log("hit=" + hit);

                var modelPos = prj.Screen2Model(Input.mousePosition, Camera.main);
                movable.MoveTo((int)modelPos.x, (int)modelPos.y);
            }
        }

        private Vector3 Screen2View(Vector3 mousePosition, Camera main)
        {
            throw new System.NotImplementedException();
        }

        private void FixedUpdate()
        {
            time.UpdateSec(UnityEngine.Time.fixedDeltaTime);
        }
    }
}
