using System;
using System.Collections.Generic;
using Common.Unity.Util.Math;
using Common.Util.Math;
using Iso.Cells;
using Unity.Mathematics;
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
   private GameObject grid;
   private IsometricProjector isometricProjector = new();
   private Cells cells;
   private CellType currentType = CellType.Buildable;

   /*public override GUIContent toolbarIcon =>
      new()
      {
         image = icon,
         text = "MapEditor",
         tooltip = "Edit your map"
      };*/
    public override GUIContent toolbarIcon
    {
       get
       {
          if (cells == null)
          {
             cells = new Cells();
             cells.Create(mapWidth, mapHeigth);
          }
          return new GUIContent
          {
             image = icon,
             text = "MapEditor",
             tooltip = "Edit your map"
          };
       }
    }

   public override void OnToolGUI(EditorWindow window)
   {
      if (grid == null)
      {
         grid = FindObjectOfType<Grid>().gameObject;
         isometricProjector.halfTileHeight = 0.8f;
         isometricProjector.halfTileWidth = 1.5f;
      }
      if (sprite == null)
      {
         sprite = Instantiate(buildAble, grid.transform);
      }
      
      Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
      Vector3 worldMousePos = ray.GetPoint(10f);
      
      var xM = isometricProjector.v2mx(worldMousePos.x, worldMousePos.y);
      var yM = isometricProjector.v2my(worldMousePos.x, worldMousePos.y);
      var xV = isometricProjector.m2vx(Mathf.FloorToInt(xM), Mathf.FloorToInt(yM));
      var yV = isometricProjector.m2vy(Mathf.FloorToInt(xM), Mathf.Floor(yM));
      var roundedPos = new Vector3(xV, yV, 0);
      sprite.transform.position = roundedPos;
      
      
      
      
      
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

         sprite.transform.position = roundedPos;
      }

      if (Event.current.type == EventType.KeyDown)
      {
         Debug.Log("KeyUp");
      }
      if (Event.current.type == EventType.KeyUp)
      {
         Debug.Log("KeyUp");
      }
      if (Event.current.type == EventType.MouseDown)
      {
         Debug.Log("KeyUp");
      }
      if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
      {
         if (cells.Get((int) xM, (int) yM) == null)
         {
            cells.Set((int) xM, (int) yM, currentType);
            var newSprite = Instantiate(sprite, roundedPos, quaternion.identity, grid.transform);
            newSprite.name = $"Cell(X:{roundedPos.x}, Y:{roundedPos.y})";
         }
      }
   }
}
