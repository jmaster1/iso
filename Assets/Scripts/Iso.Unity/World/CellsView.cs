using System;
using Common.Lang.Observable;
using Common.Unity.Bind;
using Common.Unity.Util;
using Common.Unity.Util.Math;
using Iso.Cells;
using UnityEngine;

namespace Iso.Unity.World
{
    public class CellsView : BindableMono<Cells.Cells>
    {
        public IsometricProjectorGrid prj;

        public GameObject cellBlocked;
            
        public GameObject cellTraversable;

        public GameObject cellBuildable;

        public ObsListAdapter<Cell, GameObject> cellsAdapter;

        private void Awake()
        {
            prj = UnityHelper.FindComponentInScene<IsometricProjectorGrid>();
            cellsAdapter = new()
            {
                CreateView = (cell, index) =>
                {
                    GameObject prefab = null;
                    switch (cell.cellType) 
                    {
                        case CellType.Blocked:
                            prefab = cellBlocked;
                            break;
                        case CellType.Buildable:
                            prefab = cellBuildable;
                            break;
                        case CellType.Traversable:
                            prefab = cellTraversable;
                            break;
                    }
                    var cellView = Instantiate(prefab, transform);
                    prj.Transform(cellView, cell.X, cell.Y);
                    return cellView;
                },
                DisposeView = cellView => Destroy(cellView)
            };
        }

        public override void OnBind()
        {
            base.OnBind();
            BindBindable(Model.CellList, cellsAdapter);
            BindEvents(Model.Events, OnCellEvent);
        }

        private void OnCellEvent(CellEvent type, Cell cell)
        {
            switch (type)
            {
                case CellEvent.cellTypeChange:
                    cellsAdapter.Recreate(cell);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}