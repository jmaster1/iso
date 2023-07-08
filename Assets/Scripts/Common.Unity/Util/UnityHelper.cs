using System;
using System.Collections.Generic;
using System.IO;
using Common.IO.FileSystem;
using Common.Util;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Common.Unity.Util
{
    /// <summary>
    /// unity utility methods
    /// </summary>
    public static class UnityHelper
    {
        public const string MainTex = "_MainTex";
        public static bool IsAndroid => Application.platform == RuntimePlatform.Android;
        
        public static bool IsIos => Application.platform == RuntimePlatform.IPhonePlayer;
        public static bool IsMouseDownLeft => Event.current.type == EventType.MouseDown && Event.current.button == 0;
        public static bool IsMouseDownRight => Event.current.type == EventType.MouseDown && Event.current.button == 1;

        /// <summary>
        /// this should be used to store application specific files
        /// (to prevent mixing with unity and plugins files)
        /// </summary>
        public static readonly string PersistentPrivateDataPath =
            Path.Combine(Application.persistentDataPath, Application.identifier);
        
        /// <summary>
        /// LocalFileSystem mapped to PersistentPrivateDataPath
        /// </summary>
        public static LocalFileSystem PrivateFileSystem = new LocalFileSystem(PersistentPrivateDataPath);
        
        /// <summary>
        /// retrieve application root directory
        /// </summary>
        public static string GetProjectDir()
        {
            return Directory.GetParent(Application.dataPath).FullName;
        }

        /// <summary>
        /// retrieve application Assets directory
        /// </summary>
        public static string GetAssetsDir()
        {
            return Path.GetFullPath(Application.dataPath);
        }

        /// <summary>
        /// convert 0xRRGGBBAA to Color
        /// </summary>
        public static Color RgbaToColor(uint rgba)
        {
            Color c;
            c.a = ((rgba) & 0xFF) / 255f;
            c.b = ((rgba>>8) & 0xFF) / 255f;
            c.g = ((rgba>>16) & 0xFF) / 255f;
            c.r = ((rgba>>24) & 0xFF) / 255f;
            return c;
        }
        
        /// <summary>
        /// convert Texture to png
        /// </summary>
        public static byte[] ToPng(Texture texture)
        {
            switch (texture)
            {
                case Texture2D texture2D:
                    return ToPng(texture2D);
                
                case RenderTexture renderTexture:
                    return ToPng(renderTexture);
                
                default:
                    throw new ArgumentException("Texture type " + texture.GetType() + " not supported.");
            }
        }
        
        /// <summary>
        /// convert Texture2D to png
        /// </summary>
        public static byte[] ToPng(Texture2D texture2d)
        {
            if (texture2d.isReadable)
            {
                return texture2d.EncodeToPNG();
            }
            
            var tmpRenderTexture = RenderTexture.GetTemporary(
                texture2d.width, texture2d.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB
            );
            Graphics.Blit(texture2d, tmpRenderTexture);

            byte[] pngBytes = ToPng(tmpRenderTexture);
            RenderTexture.ReleaseTemporary(tmpRenderTexture);

            return pngBytes;
        }

        public static Texture2D ToTexture2D(byte[] bytes)
        {
            Texture2D t2d = new Texture2D(2, 2);
            t2d.LoadImage(bytes);
            return t2d;
        }

        /// <summary>
        /// convert RenderTexture to png
        /// </summary>
        public static byte[] ToPng(RenderTexture rt)
        {
            RenderTexture active = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D t2d = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            t2d.ReadPixels(new Rect(0,0, rt.width, rt.height), 0,0,false);
            t2d.Apply();
            RenderTexture.active = active;
            return t2d.EncodeToPNG();
        }

        /// <summary>
        /// read resource as TextAsset
        /// </summary>
        public static TextAsset ResourceLoadTextAsset(string path, bool safe = false)
        {
            var asset = Resources.Load<TextAsset>(path);
            if (!safe && asset == null)
            {
                LangHelper.Throw($"Path not found: {path}");
            } 
            return asset;
        }

        /// <summary>
        /// named resource stream factory
        /// </summary>
        public static Stream ResourceStream(string path)
        {
            var asset = ResourceLoadTextAsset(path);
            return new MemoryStream(asset.bytes);
        }

        /// <summary>
        /// shortcut to LayoutRebuilder.MarkLayoutForRebuild
        /// </summary>
        public static void LayoutInvalidate(this GameObject gameObject)
        {
            LayoutRebuilder.MarkLayoutForRebuild((RectTransform) gameObject.transform);
        }
        
        public static void SetActive(this Component comp, bool active)
        {
            comp.gameObject.SetActive(active);
        }
        
        public static void SetParent(this Component comp, Transform parent)
        {
            comp.transform.SetParent(parent);
        }

        public static void DontDestroyOnLoad(this Component comp)
        {
            Object.DontDestroyOnLoad(comp.gameObject);
        }
        
        public static bool IsPrefab(this GameObject gameObject)
        {
            return gameObject.scene.rootCount == 0;
        }
        
        public static bool IsPrefab(this MonoBehaviour behaviour)
        {
            return behaviour.gameObject.scene.rootCount == 0;
        }

        public static bool HasInternetConnection()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        public static Sprite ToSprite(this Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), 
                new Vector2(0.5f, 0.5f), 100.0f);
        } 
        
        public static TextMeshProUGUI SetText(this Button button, string value)
        {
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.text = value;
            return text;
        }
        
        public static T Clone<T>(this T obj) where T : Component
        {
            return Object.Instantiate(obj, obj.transform.parent, true);
        }

        public static T FindComponentInScene<T>() where T : MonoBehaviour
        {
            var scene = SceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();
            foreach (var e in roots)
            {
                var comp = e.GetComponentInChildren<T>();
                if (comp != null)
                {
                    return comp;
                }
            }

            return null;
        }

        public static float SetAlpha(this Graphic graphic, float alpha)
        {
            var color = graphic.color;
            var result = color.a;
            color.a = alpha;
            graphic.color = color;
            return result;
        }
        
        public static Rect ToScreenSpace(this RectTransform transform)
        {
            var size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            return new Rect((Vector2)transform.position - size * 0.5f, size);
        }

        public static void SortChildren(Transform parent, IComparer<GameObject> comparer)
        {
            var list = new List<GameObject>(parent.childCount);
            for(var i = 0; i < parent.childCount; i++)
            {
                var c = parent.GetChild(i).gameObject;
                list.Add(c);
            }
            list.Sort(comparer);
            for (var i = 0; i < list.Count; i++)
            {
                var c = list[i];
                c.transform.SetSiblingIndex(i);
            }
        }
    }
    
    public class GameObjectByNameComparator : IComparer<GameObject>
    {

        public static readonly GameObjectByNameComparator Instance = new();
        
        public int Compare(GameObject x, GameObject y)
        {
            return x.name.CompareTo(y.name);
        }
    }
}
