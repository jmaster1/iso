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
      private const string CellPrefabPrefix = "cell";
         
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

      private CellType? CurrentType => cellTypes[cellTypeIndex];
      
      private IsometricProjector IsoPrj => gridPrj.Projector;

      private IsometricProjectorGrid gridPrj;
      
      private readonly Cells.Cells cells = new();
      
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
         
         cells.Create(mapWidth, mapHeigth);
         //
         // build cells from game objects
         for (var i = 0; i < ParentTransform.childCount; i++)
         {
            var obj = ParentTransform.GetChild(i);
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);
            if (prefab != null && prefab.name.StartsWith(CellPrefabPrefix) && 
                Enum.TryParse(prefab.name.Substring(CellPrefabPrefix.Length), out CellType cellType))
            {
               var orthoPos = IsoPrj.v2m(obj.transform.position);
               var cell = cells.Set((int)orthoPos.x, (int)orthoPos.y, cellType);
               existingCells[cell] = obj.gameObject;
            }
         }
         Debug.Log($"existingCells={existingCells.Count}");
         
         cellsView.CellPrefabCloner = CreateCell;
         cellsView.Bind(cells);
         UpdatePointer();
         EditorHelper.ShowNotification("Entering Cells Editor");
      }

      private GameObject CreateCell(Cell cell, GameObject cellPrefab)
      {
         var obj = existingCells.Find(cell);
         if(obj == null) obj = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab, ParentTransform);
         return obj;
      }

      // Called before the active tool is changed, or destroyed.
      // The exception to this rule is if you have manually
      // destroyed this tool (ex, calling `Destroy(this)` will skip the OnWillBeDeactivated invocation).
      public override void OnWillBeDeactivated()
      {
         ClearPointer();
         existingCells.Clear();
         //
         // cells have to remain, provide null destructor
         cellsView.CellViewDestructor = _ => {};
         cellsView.Unbind();
         cellsView.CellViewDestructor = null;
         cells.Clear();
         EditorHelper.ShowNotification("Exiting Cells Editor");
      }
      
      public override void OnToolGUI(EditorWindow window)
      {
         if(cellsView == null || gridPrj == null) return;
         
         var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
         var isoPos = ray.GetPoint(10f);
         var orthoPos = IsoPrj.v2m(isoPos);
         var orthoPosSnap = orthoPos.Floor();
         var isoPosSnap = IsoPrj.m2v(orthoPosSnap);
         if (lastOrthoPosSnap != orthoPosSnap)
         {
            lastOrthoPosSnap = orthoPosSnap;
            //Debug.Log($"orthoPosSnap={orthoPosSnap}");
         }

         if (pointer != null)
         {
            pointer.transform.position = isoPosSnap;
         }

         if (UnityHelper.IsMouseDownRight)
         {
            cellTypeIndex = (cellTypeIndex + 1) % cellTypes.Length;
            Debug.Log($"currentType={CurrentType}");
            UpdatePointer();
         }

         if (UnityHelper.IsMouseDownLeft && 
             orthoPosSnap.x > 0 && orthoPosSnap.y >= 0)
         {
            if (CurrentType == null)
            {
               cells.Clear(orthoPosSnap.x, orthoPosSnap.y);
            }
            else
            {
               cells.Set(orthoPosSnap.x, orthoPosSnap.y, CurrentType.Value);
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
         Debug.Log($"UpdatePointer={CurrentType}");
         ClearPointer();
         if (CurrentType == null) return;
         var prefab = cellsView.GetPrefab(CurrentType.Value);
         pointer = Instantiate(prefab, ParentTransform);
         pointer.transform.SetSiblingIndex(0);
         pointer.name = "_cursor";
      }
   }
}
