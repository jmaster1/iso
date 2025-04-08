using System;
using Common.Unity.Bind;
using UnityEngine;
using UnityEngine.Serialization;

namespace Iso.Unity.World
{
    public class MovablesView : BindableMono<Movables.Movables>
    {
        public MovableListAdapter movableListAdapter;
        
        public override void OnBind()
        {
            base.OnBind();
            BindBindable(Model.List, movableListAdapter);
        }

        /**
         * hit test all the views, report hits to consumer, consumer should return:
         * true - to continue hit test
         * false - to stop
         * @return true if at least one hit reported
         */
        public bool HitTest(Vector3 worldPoint, Func<MovableView, bool> consumer)
        {
            var result = false;
            foreach (var view in movableListAdapter.Views)
            {
                if (!view.HitTest(worldPoint)) continue;
                result = true;
                if (!consumer(view))
                {
                    break;
                }
            }

            return result;
        }
    }
}
