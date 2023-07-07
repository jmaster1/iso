using Common.Unity.Util.Math;
using Common.Util.Math;
using Iso.Cells;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("Map abilities editor")]
public class WalkableZoneEditor : EditorTool
{
   
   public Texture2D icon;
   [SerializeField] GameObject buildAble, walkAble, closed;

   private int mapHeigth = 100, mapWidth = 100;
   private GameObject sprite;
   private int colorIndex;
   private Grid grid;
   private IsometricProjector isometricProjector = new();
   private readonly Cells Cells = new();
   private CellType currentType = CellType.Buildable;

    public override GUIContent toolbarIcon =>
       new()
       {
          image = icon,
          text = "MapEditor",
          tooltip = "Edit your map"
       };

    public override void OnToolGUI(EditorWindow window)
   {
      if (grid == null)
      {
         grid = FindObjectOfType<Grid>();
         isometricProjector.halfTileHeight = grid.cellSize.y / 2f;
         isometricProjector.halfTileWidth = grid.cellSize.x / 2f;
      }
      if (sprite == null)
      {
         sprite = Instantiate(buildAble, grid.transform);
      }

      if (Cells.Width == 0)
      {
         Cells.Create(mapWidth, mapHeigth);
      }
      Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
      var isoPos = ray.GetPoint(10f);
      var orthoPos = isometricProjector.v2m(isoPos);
      var orthoPosSnap = orthoPos.Floor();
      var isoPosSnap = isometricProjector.m2v(orthoPosSnap);

      sprite.transform.position = isoPosSnap;
      if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
      {
         colorIndex = colorIndex >= 2 ? 0 : colorIndex + 1;
         DestroyImmediate(sprite);
         switch (colorIndex)
         {
            case 0: sprite = Instantiate(buildAble, grid.transform);
               currentType = CellType.Buildable;
               break;
            case 1: sprite = Instantiate(walkAble, grid.transform);
               currentType = CellType.Traversable;
               break;
            case 2: sprite =  Instantiate(closed, grid.transform);
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
            var newSprite = Instantiate(sprite, isoPosSnap, Quaternion.identity, grid.transform);
            newSprite.name = $"Cell({orthoPosSnap.x}, {orthoPosSnap.y})";
         }
      }
   }
}
