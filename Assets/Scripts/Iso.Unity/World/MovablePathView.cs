using System.Collections.Generic;
using Common.Lang.Observable;
using Common.Unity.Bind;
using Common.Unity.Util;
using Common.Unity.Util.Math;
using Iso.Cells;
using Iso.Movables;
using UnityEngine;

namespace Iso.Unity.World
{
    public class MovablePathView : BindableMono<Movable>
    {
        public IsometricProjectorGrid prj;

        public GameObject pathElementViewTemplate;
        
        private ObsListAdapter<Cell, GameObject> cellsAdapter;

        private List<GameObject> pathElements = new();
        
        public override void OnBind()
        {
            base.OnBind();
            BindModelEvents(Model.Events, evt =>
            {
                switch (evt)
                {
                    case MovableEvent.movingChange:
                        updatePath();
                        break;
                }
            });
            if (prj == null)
            {
                prj = UnityHelper.FindComponentInScene<IsometricProjectorGrid>();
            }
        }

        private void updatePath()
        {
            foreach (var pathElement in pathElements)
            {
                DestroyImmediate(pathElement);
            }
            pathElements.Clear();

            foreach (var cell in Model.path)
            {
                var pathElement = Instantiate(pathElementViewTemplate);
                prj.Transform(pathElement, cell.X, cell.Y);
                pathElements.Add(pathElement);
            }
        }
    }
}