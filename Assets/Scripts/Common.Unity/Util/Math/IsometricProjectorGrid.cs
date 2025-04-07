using Common.Util.Math;
using UnityEngine;

namespace Common.Unity.Util.Math
{
    public class IsometricProjectorGrid : MonoBehaviour
    {
        public Grid grid;

        private IsometricProjector projector;
        
        public IsometricProjector Projector
        {
            get
            {
                return projector ??= new()
                {
                    halfTileWidth = grid.cellSize.x / 2f,
                    halfTileHeight = grid.cellSize.y / 2f
                };
            }
        }

        public void Transform(GameObject view, float mx, float my)
        {
            var vx = Projector.m2vx(mx, my);
            var vy = Projector.m2vy(mx, my);
            var z = view.transform.position.z;
            view.transform.position = new Vector3(vx, vy, z);
        }
        
        public void Transform(GameObject view, Vector2DFloat modelPos)
        {
            Transform(view, modelPos.x, modelPos.y);
        }
        
        public Vector2 View2Model(float vx, float vy)
        {
            var mx = Projector.v2mx(vx, vy);
            var my = Projector.v2my(vx, vy);
            return new Vector2(mx, my);
        }

        public Vector2 Screen2View(Vector3 screenPos, Camera cam)
        {
            return cam.ScreenToWorldPoint(screenPos);
        }
        

        public Vector2 Screen2Model(Vector3 screenPos, Camera cam)
        {
            var viewPos = Screen2View(screenPos, cam);
            return View2Model(viewPos.x, viewPos.y);
        }
    }
}
