using System;
using System.Collections.Generic;
using Common.IO.FileSystem;
using Common.IO.Serialize;
using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Common.IO.Serialize.Newtonsoft.Json.References;
using Common.Lang.Entity;
using Common.Util;
using Common.Util.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace Common.Player
{
    /// <summary>
    /// responsible for [de]serializing player features
    /// </summary>
    public abstract class AbstractPlayerJsonSerializer<T> : GenericBean where T : AbstractPlayer
    {
        public JsonSerializer serializer;

        public readonly T Player;

        private readonly ReferenceResolverManager referenceResolver = new ReferenceResolverManager();

        public AbstractPlayerJsonSerializer(T player)
        {
            Player = player;
            var settings = new JsonSerializerSettings()
            {
                ReferenceResolverProvider = () => referenceResolver,
                Converters = new List<JsonConverter>()
                {
                    //
                    // common
                    new StringEnumConverter(),
                    new HolderConverter(),
                    new TimeTaskConverter()
                },
                Formatting = Formatting.Indented,
                Error = delegate(object sender, ErrorEventArgs args)
                {
                    Log.Error(args.ErrorContext.Error);
                    args.ErrorContext.Handled = false;
                },
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            UpdateSettings(settings);
            serializer = JsonSerializer.CreateDefault(settings);
        }

        /// <summary>
        /// should be overridden by subclasses in order to customize JsonSerializerSettings
        /// </summary>
        protected virtual void UpdateSettings(JsonSerializerSettings settings)
        {
        }

        private string GetFileName(AbstractFeature feature)
        {
            return feature.Name + "." + JsonObjectSerializer.Format;
        }

        public void Load(AbstractFileSystem fileSystem)
        {
            Player.Load(e => LoadFeature(e, fileSystem));
        }

        public void LoadFeature(AbstractFeature feature, AbstractFileSystem fileSystem)
        {
            var name = GetFileName(feature);
            using var textReader = fileSystem.TextReader(name);
            if(textReader == null) return;
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"Load:{feature} <- {name}");
            }
            try
            {
                serializer.Populate(textReader, feature);
            }
            catch (Exception ex)
            {
                LangHelper.Handle(ex, $"LoadFeature({feature}) failed");
            }
        }

        public void Save(AbstractFileSystem fileSystem, bool dirty = false)
        {
            Player.Save(dirty, e => SaveFeature(e, fileSystem));
        }

        public ZipFileSystem SaveZip()
        {
            var zip = new ZipFileSystem();
            Save(zip);
            return zip;
        }

        [HttpInvoke]
        public void DownloadZip(HttpQuery query)
        {
            var zip = SaveZip();
            query.SetContentType("application/zip");
            query.SetFileName("player.zip");
            using (var zipStream = zip.GetZipStream())
            {
                zipStream.CopyTo(query.OutputStream);
            }
            query.Dispose();
        }
        
        public void SaveDirty(AbstractFileSystem fileSystem)
        {
            Save(fileSystem, true);
        }

        public void SaveFeature(AbstractFeature feature, AbstractFileSystem fileSystem)
        {
            var name = GetFileName(feature);
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"Save: {feature} -> {name}");
            }
            using (var textWriter = fileSystem.TextWriter(name))
            {
                try
                {
                    serializer.Serialize(textWriter, feature);
                }
                catch (Exception ex)
                {
                    LangHelper.Handle(ex, $"SaveFeature({feature}) failed");
                }
            }
        }
    }
}
