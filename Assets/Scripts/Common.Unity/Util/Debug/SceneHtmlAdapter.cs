using System;
using System.Text;
using Common.IO.Streams;
using Common.Lang;
using Common.Lang.Collections;
using Common.Lang.Collections.Tree;
using Common.Lang.Entity;
using Common.Util.Http;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Unity.Util.Debug
{
    /// <summary>
    /// current scene html adapter
    /// </summary>
    public class SceneHtmlAdapter : GenericBean
    {
        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            var objCount = 0;
            var compCount = 0;
            var compTypeCount = new Map<Type, int>();
            var scene = SceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();
            var tree = new TransformTreeApi();
            html.tableHeader("#", "name", "x", "y", "z", "lx", "ly", "lz", "components");
            bool RenderNode(Transform tx)
            {
                objCount++;
                var name = tx.gameObject.name;
                var comps = tx.gameObject.GetComponents<Component>();
                var sbc = new StringBuilder();
                foreach (var c in comps)
                {
                    var ctype = c.GetType();
                    sbc.Append(ctype.Name).Append(", ");
                    compCount++;
                    var n = compTypeCount.Find(ctype);
                    compTypeCount[ctype] = ++n;
                }
                html.tr().tdRowNum().td();
                var depth = tree.GetDepth(tx);
                for (var i = 0; i < depth; i++) html.nbsp();
                html.text(name).endTd();
                var pos = tx.position;
                html.td(pos.x).td(pos.y).td(pos.z);
                var lpos = tx.localPosition;
                html.td(lpos.x).td(lpos.y).td(lpos.z);
                html.td(sbc);
                html.endTr();
                return true;
            }
            
            foreach (var e in roots)
            {
                tree.TraversePreorder(e.transform, RenderNode);
            }
            html.endTable();
            html.propertyTable("objects", objCount,
                "components", compCount);
            html.tableHeader("#", "type", "count");
            foreach (var e in compTypeCount)
            {
                html.tr().tdRowNum().td(e.Key.Name).td(e.Value).endTr();
            }
            html.endTable();
        }
    }
}