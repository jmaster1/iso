using System.Collections.Generic;
using Common.Unity.Bind;
using Iso.Movables;
using UnityEngine;

namespace Iso.Unity.World
{
    public class MovablePathView : BindableMono<Movable>
    {
        public GameObject pathElementViewTemplate;
        
        private readonly List<GameObject> pathElements = new();
        
        public override void OnBind()
        {
            base.OnBind();
            BindModelEvents(Model.Events, evt =>
            {
                switch (evt)
                {
                    case MovableEvent.pathChange:
                        UpdatePath();
                        break;
                }
            });
        }

        private void UpdatePath()
        {
            foreach (var pathElement in pathElements)
            {
                DestroyImmediate(pathElement);
            }
            pathElements.Clear();

            foreach (var cell in Model.Path)
            {
                var pathElement = Instantiate(pathElementViewTemplate);
                this.ApplyTransform(pathElement, cell.X, cell.Y);
                pathElements.Add(pathElement);
            }
        }
    }
}
