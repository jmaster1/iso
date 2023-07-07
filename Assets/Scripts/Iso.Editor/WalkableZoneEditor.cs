using Common.Unity.Util.Math;
using Common.Util.Math;
using Iso.Cells;
using Iso.Unity.World;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Iso.Editor
{
   [EditorTool("Map abilities editor")]
   public class WalkableZoneEditor : EditorTool
   {
   
      public Texture2D icon;
      [SerializeField] GameObject buildAble, walkAble, closed;

      private int mapHeigth = 100, mapWidth = 100;
      private GameObject sprite;
      private int colorIndex;
      
      private IsometricProjector isoPrj => gridPrj.Projector;

      private IsometricProjectorGrid gridPrj;
      private readonly Cells.Cells Cells = new();
      private CellType currentType = CellType.Buildable;
      private CellsView cellsView;

      public override GUIContent toolbarIcon =>
         new()
         {
            image = icon,
            text = "MapEditor",
            tooltip = "Edit your map"
         };

      public override void OnToolGUI(EditorWindow window)
      {
         cellsView = FindObjectOfType<CellsView>();
         if(cellsView == null) return;
         
         gridPrj = FindObjectOfType<IsometricProjectorGrid>();
         if(gridPrj == null) return;
         
         if (sprite == null)
         {
            sprite = Instantiate(buildAble, ParentTransform);
         }

         if (Cells.Width == 0)
         {
            Cells.Create(mapWidth, mapHeigth);
         }
         var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
         var isoPos = ray.GetPoint(10f);
         var orthoPos = isoPrj.v2m(isoPos);
         var orthoPosSnap = orthoPos.Floor();
         var isoPosSnap = isoPrj.m2v(orthoPosSnap);
         //Debug.Log($"isoPos={isoPos}, orthoPos={orthoPos}, orthoPosSnap={orthoPosSnap}, isoPosSnap={isoPosSnap}");
         

         sprite.transform.position = isoPosSnap;
         if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
         {
            colorIndex = colorIndex >= 2 ? 0 : colorIndex + 1;
            DestroyImmediate(sprite);
            switch (colorIndex)
            {
               case 0: sprite = Instantiate(buildAble, ParentTransform);
                  currentType = CellType.Buildable;
                  break;
               case 1: sprite = Instantiate(walkAble, ParentTransform);
                  currentType = CellType.Traversable;
                  break;
               case 2: sprite =  Instantiate(closed, ParentTransform);
                  currentType = CellType.Blocked;
                  break;
            }

            sprite.transform.position = isoPosSnap;
         }

         if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
         {
            if (Cells.Find(orthoPosSnap.x, orthoPosSnap.y) == null)
            {
               Cells.Set(orthoPosSnap.x, orthoPosSnap.y, currentType);
               var newSprite = Instantiate(sprite, isoPosSnap, Quaternion.identity, ParentTransform);
               newSprite.name = $"{orthoPosSnap.x} : {orthoPosSnap.y}";
            }
         }
      }

      public Transform ParentTransform => cellsView.transform;
   }
}
