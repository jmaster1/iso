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
        public GameObject cellBlocked;
            
        public GameObject cellTraversable;

        public GameObject cellBuildable;

        private ObsListAdapter<Cell, GameObject> cellsAdapter;

        /// <summary>
        /// external cloner (optional)
        /// </summary>
        public Func<Cell, GameObject, GameObject> CellPrefabCloner;

        public Action<GameObject> CellViewDestructor;
        
        public override void OnBind()
        {
            base.OnBind();
            var prj = this.GetIsoProjector();
            prj ??= UnityHelper.FindComponentInScene<IsometricProjectorGrid>();
            cellsAdapter ??= new ObsListAdapter<Cell, GameObject>
            {
                CreateView = (cell, _) =>
                {
                    var prefab = GetPrefab(cell.cellType);
                    var cellView = CellPrefabCloner == null ? Instantiate(prefab, transform) : CellPrefabCloner(cell, prefab);
                    cellView.name = $"{cell.X:000} : {cell.Y:000}";
                    prj.Transform(cellView, cell.X, cell.Y);
                    return cellView;
                },
                DisposeView = cellView =>
                {

                    if (CellViewDestructor == null)
                    {
                        DestroyImmediate(cellView);
                    }
                    else
                    {
                        CellViewDestructor(cellView);
                    }
                }
            };
            BindBindable(Model.CellList, cellsAdapter);
            BindEvents(Model.Events, OnCellEvent);
        }

        public GameObject GetPrefab(CellType cellType)
        {
            return cellType switch
            {
                CellType.Blocked => cellBlocked,
                CellType.Buildable => cellBuildable,
                CellType.Traversable => cellTraversable,
                _ => null
            };
        }

        private void OnCellEvent(CellEvent type, Cell cell)
        {
            switch (type)
            {
                case CellEvent.cellTypeChange:
                    cellsAdapter.Recreate(cell);
                    break;
            }
        }
    }
}
