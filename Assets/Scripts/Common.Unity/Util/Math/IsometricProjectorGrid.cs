using Common.Util.Math;
using UnityEngine;

namespace Common.Unity.Util.Math
{
    public class IsometricProjectorGrid : MonoBehaviour
    {
        public Grid grid;

        public IsometricProjector projector = new();

        private void Start()
        {
            var cellSize = grid.cellSize;
            projector.halfTileWidth = cellSize.x / 2f;
            projector.halfTileHeight = cellSize.y / 2f;
        }

        public void Transform(GameObject view, Vector2DFloat modelPos)
        {
            var vx = projector.m2vx(modelPos.x, modelPos.y);
            var vy = projector.m2vy(modelPos.x, modelPos.y);
            view.transform.position = new Vector3(vx, vy, 0);
        }
        
        public Vector2 View2Model(float vx, float vy)
        {
            var mx = projector.v2mx(vx, vy);
            var my = projector.v2my(vx, vy);
            return new Vector2(mx, my);
        }
    }
}
