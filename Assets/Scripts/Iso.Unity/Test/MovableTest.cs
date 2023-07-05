using System;
using Iso.Cells;
using Iso.Movables;
using Iso.Unity.World;
using UnityEngine;
using Time = Common.TimeNS.Time;

namespace Iso.Unity.Test
{
    public class MovableTest : MonoBehaviour
    {
        public MovableView view;

        public int cellsWidth = 20;
        
        public int cellsHeight = 20;

        private Cells.Cells cells = new();

        private Movables.Movables movables = new();
            
        private Movable movable;

        private Time time = new();
        
        private void Awake()
        {
            cells.Create(cellsWidth, cellsHeight);
            cells.ForEachPos((x, y) => cells.Set(x, y, CellType.Traversable));
            movables.Cells = cells;
            movables.Time = time;
            movables.Start();
            
            var bi = new MovableInfo
            {
                Id = "M1",
                Velocity = 1
            };
            var c1 = cells.Get(0, 0);
            movable = movables.Add(bi, c1);
            view.Bind(movable);
            movable.MoveTo(cellsWidth - 1, cellsHeight - 1);
        }

        private void FixedUpdate()
        {
            time.UpdateSec(UnityEngine.Time.fixedDeltaTime);
        }
    }
}