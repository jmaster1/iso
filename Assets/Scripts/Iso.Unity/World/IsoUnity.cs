using System;
using Common.Unity.Bind;
using Common.Unity.Util;
using Common.Unity.Util.Math;
using Common.Util.Math;
using Iso.Cells;
using UnityEngine;

namespace Iso.Unity.World
{
    public static class IsoUnity
    {
        public static Cell Find(this Cells.Cells cells, Vector2 v)
        {
            return cells.Find((int)v.x, (int)v.y);
        }
        
        public static Cell Find(this Cells.Cells cells, Vector2Int v)
        {
            return cells.Find(v.x, v.y);
        }
        
        public static Cell Set(this Cells.Cells cells, Vector2Int v, CellType type)
        {
            return cells.Set(v.x, v.y, type);
        }

        private static IsometricProjectorGrid _isometricProjectorGrid;
        
        public static IsometricProjectorGrid GetIsoProjector(this BindableMonoRaw mono)
        {
            if (!_isometricProjectorGrid)
            {
                _isometricProjectorGrid = UnityHelper.FindComponentInScene<IsometricProjectorGrid>();
            }
            return _isometricProjectorGrid;
        }
        
        public static void ApplyTransform(this BindableMonoRaw mono, GameObject target, int modelX, int modelY)
        {
            var prj = mono.GetIsoProjector();
            prj.Transform(target, modelX, modelY);
        }

        public static void ApplyTransform(this BindableMonoRaw mono, GameObject target, float modelX, float modelY)
        {
            var prj = mono.GetIsoProjector();
            prj.Transform(target, modelX, modelY);
        }
        
        
        public static void ApplyTransform(this BindableMonoRaw mono, Vector2DFloat modelPos)
        {
            mono.GetIsoProjector().Transform(mono.gameObject, modelPos);
        }
    }
}
