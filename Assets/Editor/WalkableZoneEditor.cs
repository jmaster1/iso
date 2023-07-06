using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("Map abilities editor")]
public class WalkableZoneEditor : EditorTool
{
   
   [SerializeField] Texture2D icon;
   [SerializeField] GameObject spritePrefab;
   [SerializeField] float gridXLength;
   [SerializeField] float gridYLength;
   private List<Color> colors = new(){new Color(0,1,0, 0.3f), new Color(1,1,0,0.3f), new Color(1, 0,0,0.3f)};
   private GameObject sprite;
   private int colorIndex;
   private GameObject grid;
   private bool mousePressed;
   private Dictionary<Vector3, GameObject> tiles = new();

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
      }
      
      if (sprite == null)
      {
         sprite = Instantiate(spritePrefab, grid.transform);
      }

      Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
      Vector3 mousePos = ray.GetPoint(10f);
      var x = Mathf.Round(mousePos.x / gridXLength);
      var y = Mathf.Round(mousePos.y / gridYLength);
      mousePos = new Vector3(gridXLength * x, gridYLength * y, 10);
      sprite.transform.position = mousePos;
      if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
      {
         colorIndex = colorIndex >= colors.Count-1 ? 0 : colorIndex + 1;
         spritePrefab.transform.GetChild(0).GetComponent<SpriteRenderer>().color = colors[colorIndex];
         sprite.transform.GetChild(0).GetComponent<SpriteRenderer>().color = colors[colorIndex];
      }

      if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !tiles.ContainsKey(mousePos))
      {
         var newSprite = Instantiate(spritePrefab, mousePos, quaternion.identity, grid.transform);
         newSprite.name = "C(posX, posY)";
         tiles.Add(mousePos, newSprite);
      }
   }
}
