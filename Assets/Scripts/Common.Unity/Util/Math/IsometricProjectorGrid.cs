using Common.Util.Math;
using UnityEngine;

namespace Common.Unity.Util.Math
{
    public class IsometricProjectorGrid : MonoBehaviour
    {
        public Grid grid;

        public IsometricProjector projector = new();

        private void Awake()
        {
            var cellSize = grid.cellSize;
            projector.halfTileWidth = cellSize.x / 2f;
            projector.halfTileHeight = cellSize.y / 2f;
        }

        public void Transform(GameObject view, float mx, float my)
        {
            var vx = projector.m2vx(mx, my);
            var vy = projector.m2vy(mx, my);
            view.transform.position = new Vector3(vx, vy, 0);
        }
        
        public void Transform(GameObject view, Vector2DFloat modelPos)
        {
            Transform(view, modelPos.x, modelPos.y);
        }
        
        public Vector2 View2Model(float vx, float vy)
        {
            var mx = projector.v2mx(vx, vy);
            var my = projector.v2my(vx, vy);
            return new Vector2(mx, my);
        }
    }
}
