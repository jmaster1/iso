using UnityEngine;

namespace Common.Unity.Util
{
    /// <summary>
    /// animates material properties
    /// </summary>
    public class MaterialAnimator : MonoBehaviour
    {
        public Vector2 textureOffsetSpeed = new Vector2(0.005f, 0.005f);
        
        public MeshRenderer meshRenderer;
        
        private void Update()
        {
            var material = meshRenderer.sharedMaterial;
            var offset = new Vector2(Time.time, Time.time) * textureOffsetSpeed;
            material.SetTextureOffset (UnityHelper.MainTex, offset);
        }
    }
}