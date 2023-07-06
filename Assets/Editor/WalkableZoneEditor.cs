using System;
using System.Collections.Generic;
using Common.Unity.Util.Math;
using Common.Util.Math;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("Map abilities editor")]
public class WalkableZoneEditor : EditorTool
{
   
   public Texture2D icon;
   [SerializeField] GameObject buildAble, walkAble, closed;
   [SerializeField] float gridXLength;
   [SerializeField] float gridYLength;
   private GameObject sprite;
   private int colorIndex;
   private GameObject grid;
   private IsometricProjector isometricProjector = new();

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
          grid = FindObjectOfType<Grid>().gameObject;
          gridXLength = grid.GetComponent<Grid>().cellSize.x / 2;
          gridYLength = grid.GetComponent<Grid>().cellSize.y / 2;
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
      Debug.Log("WorldMousePos = " + worldMousePos);
      
      
      
      var xM = isometricProjector.v2mx(worldMousePos.x, worldMousePos.y);
      var yM = isometricProjector.v2my(worldMousePos.x, worldMousePos.y);
      var posFloat = new Vector3(xM, yM, 0);
      Debug.Log("ModelPos = " + posFloat);
      var pos = new Vector3((int) posFloat.x, (int) posFloat.y, 0);
      Debug.Log("PosToInt = " + pos);
      var xV = isometricProjector.m2vx(pos.x, pos.y);
      var yV = isometricProjector.m2vy(pos.x, pos.y);
      worldMousePos = new Vector3(xV, yV, 0);
      Debug.Log("FinallyViewPos = " + worldMousePos);
      sprite.transform.position = worldMousePos;
      
      
      
      
      
      if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
      {
         colorIndex = colorIndex >= 2 ? 0 : colorIndex + 1;
         DestroyImmediate(sprite);
         switch (colorIndex)
         {
            case 0: sprite = Instantiate(buildAble, grid.transform); 
               break;
            case 1: sprite = Instantiate(walkAble, grid.transform);
               break;
            case 2: sprite =  Instantiate(closed, grid.transform);
               break;
         }

         sprite.transform.position = worldMousePos;
      }

      if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
      {
         var newSprite = Instantiate(sprite, worldMousePos, quaternion.identity, grid.transform);
         newSprite.name = "C(posX, posY)";
      }
   }
}
