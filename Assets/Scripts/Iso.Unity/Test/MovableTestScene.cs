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
        
        [FormerlySerializedAs("view")] public MovableView movableView;

        public int cellsWidth = 20;
        
        public int cellsHeight = 20;
        
        public int blockedCells = 20;

        private Cells.Cells cells = new();

        private Movables.Movables movables = new();
            
        private Movable movable;

        private Time time = new();
        
        private void Awake()
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
            //movable.MoveTo(cellsWidth - 1, cellsHeight - 1);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                var mousePos = Input.mousePosition;
                var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                var modelPos = movableView.prj.View2Model(worldPos.x, worldPos.y);
                Debug.Log("mousePos: " + mousePos + " > worldPos: " + worldPos + " > modelPos: " + modelPos);
                movable.MoveTo((int)modelPos.x, (int)modelPos.y);
            }
        }

        private void FixedUpdate()
        {
            time.UpdateSec(UnityEngine.Time.fixedDeltaTime);
        }
    }
}
