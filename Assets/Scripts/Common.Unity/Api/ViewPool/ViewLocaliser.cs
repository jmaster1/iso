using Common.Api;
using Common.Api.Local;
using Common.ContextNS;
using Common.Lang.Collections.Tree;
using Common.Unity.Util;
using TMPro;
using UnityEngine;

namespace Common.Unity.Api.ViewPool
{
    /// <summary>
    /// used to assign text to 
    /// </summary>
    public class ViewLocaliser : AbstractApi
    {
        /// <summary>
        /// prefix for ui localisation keys
        /// </summary>
        public const string LocalisationPrefix = "ui";
        
        /// <summary>
        /// objects and descendants with this tag set will not be localised
        /// </summary>
        public const string TagSkipLocalisation = "SkipLocalisation";

        private readonly LocalApi localApi = Context.Get<LocalApi>();

        /// <summary>
        /// localise labels for specified root component descendants
        /// </summary>
        public void Localise(MonoBehaviour root)
        {
            TransformTreeApi.Instance.TraversePreorder(root.transform,
                e => ProcessTransform(root, e));
        }

        private bool ProcessTransform(Object root, Component child)
        {
            //
            // localize label text
            if(TagSkipLocalisation.Equals(child.tag)) return false;
            var text = child.gameObject.GetComponent<TMP_Text>();
            if(!text) return true;
            var monoName = root.name;
            var componentName = child.name;
            var msg = localApi.GetMessage(LocalisationPrefix, monoName, componentName);
            if (msg != null)
            {
                text.text = msg;
            }

            return true;
        }
    }
}