using System;
using Common.Util;

namespace Common.Lang
{
    /// <summary>
    /// api for navigating tree of elements
    /// </summary>
    public interface ITreeApi<TE>
    {
        /// <summary>
        /// retrieve parent of specified element (must not be null)
        /// </summary>
        TE GetParent(TE e);

        /// <summary>
        /// retrieve count of direct children of specified element (must not be null)
        /// </summary>
        int GetSize(TE e);

        /// <summary>
        /// retrieve child of specified element (must not be null)
        /// </summary>
        TE GetChild(TE e, int index);
    }

    public static class Extension
    {
        /// <summary>
        /// traverse tree in preorder
        /// https://en.wikipedia.org/wiki/Tree_traversal#Pre-order_(NLR)
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="node">a node to start with</param>
        /// <param name="callback">callback receives visited node, return true to proceed with children</param>
        /// <typeparam name="TE">tree element type</typeparam>
        public static void TraversePreorder<TE>(this ITreeApi<TE> tree, TE node, Func<TE, bool> callback)
        {
            LangHelper.Validate(node != null);
            var proceed = callback(node);
            if (!proceed)
            {
                return;
            }
            
            var n = tree.GetSize(node);
            for (var i = 0; i < n; i++)
            {
                var child = tree.GetChild(node, i);
                TraversePreorder(tree, child, callback);
            }
        }

        /// <summary>
        /// retrieve depth of element in the tree
        /// </summary>
        /// <returns>0 for root element</returns>
        public static int GetDepth<TE>(this ITreeApi<TE> tree, TE node)
        {
            var depth = 0;
            while ((node = tree.GetParent(node)) != null)
            {
                depth++;
            }

            return depth;
        }
    }
}