using System;
using Common.Api;
using Common.IO.Streams;
using Common.Lang;
using Common.Lang.Collections;
using Common.Util;
using Common.Util.Http;
using UnityEngine;

namespace Common.Unity.Api.Sprite
{
    /// <summary>
    /// sprite manager provides cached access to named sprite resources
    /// </summary>
    public class SpriteApi : AbstractApi
    {
        public const char PathSeparator = '/';
        
        Map<int, UnityEngine.Sprite> cache = new();

        /// <summary>
        /// texture path resolver for entity type/id,
        /// param 0: entity type simple name
        /// param 1: entity identifier
        /// result: path to sprite resource
        /// </summary>
        public Func<string, string, string> PathResolver = ResolvePath;

        public static string ResolvePath(string name, string id)
        {
            return $"Textures/{name}/{id}";
        }

        /// <summary>
        /// retrieve sprite for entity type/id
        /// </summary>
        public UnityEngine.Sprite GetSprite(string entityType, string entityId, string suffix = null, bool validate = true)
        {
            var hash = StringHelper.Hash(PathSeparator, entityType, entityId, suffix);
            var ret = cache.Find(hash);
            if (ret != null) return ret;
            
            LangHelper.Validate(PathResolver != null);
            var path = PathResolver(entityType, entityId);
            if (suffix != null) path += suffix;
            if (Log.IsDebugEnabled) Log.DebugFormat("Loading sprite from {0}", path);
            ret = Resources.Load<UnityEngine.Sprite>(path);
            if (ret == null)
            {
                if (validate) LangHelper.Throw($"Sprite not found at {path}");
            }
            cache.Add(hash, ret);
            return ret;
        }

        public UnityEngine.Sprite GetSprite(Type type, string id, string suffix = null)
        {
            var typeName = type.Name;
            return GetSprite(typeName, id, suffix);
        }

        public UnityEngine.Sprite GetSprite(object entity, string id, string suffix = null)
        {
            var type = entity.GetType();
            return GetSprite(type, id, suffix);
        }
        
        /// <summary>
        /// retrieve sprite for specified entity, the path to sprite should be
        /// {PathBase}/{entity.Type.Name}/{entity.id}
        /// </summary>
        public UnityEngine.Sprite GetSprite(IIdAware<string> entity, string suffix = null)
        {
            LangHelper.Validate(entity != null);
            var id = entity.GetId();
            return GetSprite(entity, id, suffix);
        }
        
        /// <summary>
        /// retrieve sprite for specified enum value, the path to sprite should be
        /// {PathBase}/{entity.Type.Name}/{entity.name}
        /// </summary>
        public UnityEngine.Sprite GetSprite(Enum entity, string suffix = null)
        {
            return GetSprite(entity, entity.ToString(), suffix);
        }

        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            html.tableHeader("#", "name");
            foreach (var kv in cache)
            {
                html.tr().tdRowNum().td(kv.Value.name).endTr();
            }
            html.endTable();
        }
    }
}
