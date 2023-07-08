using System;
using Common.Editor;
using Common.Lang.Collections;
using Common.Unity.Util;
using Common.Unity.Util.Math;
using Common.Util.Math;
using Iso.Cells;
using Iso.Unity.World;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Iso.Editor
{
   [EditorTool("Cells Editor")]
   public class WalkableZoneEditor : EditorTool
   {
   
      const string CellPrefabPrefix = "cell";
         
      public Texture2D icon;

      private int mapHeigth = 100, mapWidth = 100;
      
      /// <summary>
      /// cell prefab under cursor
      /// </summary>
      private GameObject pointer;
      
      /// <summary>
      /// array of cell type used as brush
      /// </summary>
      private readonly CellType?[] cellTypes = { null, CellType.Buildable, CellType.Traversable, CellType.Blocked };

      /// <summary>
      /// current index of cell type brush element
      /// </summary>
      private int cellTypeIndex = 1;

      private CellType? currentType => cellTypes[cellTypeIndex];
      
      private IsometricProjector isoPrj => gridPrj.Projector;

      private IsometricProjectorGrid gridPrj;
      
      private readonly Cells.Cells Cells = new();
      
      private CellsView cellsView;
      
      public Transform ParentTransform => cellsView.transform;

      private Vector2 lastOrthoPosSnap;

      private readonly Map<Cell, GameObject> existingCells = new();

      public override GUIContent toolbarIcon =>
         new()
         {
            image = icon,
            text = "Cells Editor",
            tooltip = "Cells Editor"
         };
      
      // Called when the active tool is set to this tool instance.
      // Global tools are persisted by the ToolManager,
      // so usually you would use OnEnable and OnDisable to manage native resources,
      // and OnActivated/OnWillBeDeactivated
      // to set up state. See also `EditorTools.{ activeToolChanged, activeToolChanged }` events.
      public override void OnActivated()
      {
         cellsView = FindObjectOfType<CellsView>();
         gridPrj = FindObjectOfType<IsometricProjectorGrid>();
         if(cellsView == null || gridPrj == null) return;
         
         Cells.Create(mapWidth, mapHeigth);
         //
         // build cells from game objects
         for (var i = 0; i < ParentTransform.childCount; i++)
         {
            var obj = ParentTransform.GetChild(i);
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);
            if (prefab != null && prefab.name.StartsWith(CellPrefabPrefix) && 
                Enum.TryParse(prefab.name.Substring(CellPrefabPrefix.Length), out CellType cellType))
            {
               var orthoPos = isoPrj.v2m(obj.transform.position);
               var cell = Cells.Set((int)orthoPos.x, (int)orthoPos.y, cellType);
               existingCells[cell] = obj.gameObject;
            }
         }
         
         cellsView.CellPrefabCloner = CreateCell;
         cellsView.Bind(Cells);
         UpdatePointer();
         EditorHelper.ShowNotification("Entering Cells Editor");
      }

      private GameObject CreateCell(Cell cell, GameObject cellPrefab)
      {
         var obj = existingCells[cell];
         if(obj == null) obj = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab, ParentTransform);
         return obj;
      }

      // Called before the active tool is changed, or destroyed.
      // The exception to this rule is if you have manually
      // destroyed this tool (ex, calling `Destroy(this)` will skip the OnWillBeDeactivated invocation).
      public override void OnWillBeDeactivated()
      {
         ClearPointer();
         EditorHelper.ShowNotification("Exiting Cells Editor");
      }
      
      public override void OnToolGUI(EditorWindow window)
      {
         if(cellsView == null || gridPrj == null) return;
         
         var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
         var isoPos = ray.GetPoint(10f);
         var orthoPos = isoPrj.v2m(isoPos);
         var orthoPosSnap = orthoPos.Floor();
         var isoPosSnap = isoPrj.m2v(orthoPosSnap);
         if (lastOrthoPosSnap != orthoPosSnap)
         {
            lastOrthoPosSnap = orthoPosSnap;
            Debug.Log($"orthoPosSnap={orthoPosSnap}");
         }

         if (pointer != null)
         {
            pointer.transform.position = isoPosSnap;
         }

         if (UnityHelper.IsMouseDownRight)
         {
            cellTypeIndex = (cellTypeIndex + 1) % cellTypes.Length;
            Debug.Log($"currentType={currentType}");
            UpdatePointer();
         }

         if (UnityHelper.IsMouseDownLeft && 
             orthoPosSnap.x > 0 && orthoPosSnap.y >= 0)
         {
            if (currentType == null)
            {
               Cells.Clear(orthoPosSnap.x, orthoPosSnap.y);
            }
            else
            {
               Cells.Set(orthoPosSnap.x, orthoPosSnap.y, currentType.Value);
               UnityHelper.SortChildren(ParentTransform, GameObjectByNameComparator.Instance);
            }
         }
      }

      private void ClearPointer()
      {
         if (pointer != null)
         {
            DestroyImmediate(pointer);
            pointer = null;
         }
      }

      private void UpdatePointer()
      {
         ClearPointer();
         var cellType = cellTypes[cellTypeIndex];
         if (cellType == null) return;
         var prefab = cellsView.GetPrefab(cellType.Value);
         pointer = Instantiate(prefab, ParentTransform);
         pointer.transform.SetSiblingIndex(0);
         pointer.name = "_cursor";
      }
   }
}
