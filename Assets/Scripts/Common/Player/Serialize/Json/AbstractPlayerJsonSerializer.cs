using System;
using System.Collections.Generic;
using System.IO;
using Common.Api.Info;
using Common.IO.FileSystem;
using Common.IO.Serialize;
using Common.IO.Serialize.Newtonsoft.Json.Converter;
using Common.IO.Serialize.Newtonsoft.Json.References;
using Common.Lang.Entity;
using Common.Player;
using Common.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace Iso.Serialize.Json
{
    /// <summary>
    /// responsible for [de]serializing puzzle adapters
    /// </summary>
    public class AbstractPlayerJsonSerializer<T> : GenericBean where T : AbstractPlayer
    {
        protected readonly T player;

        List<AbstractFeature> Adapters => player.Features;

        /// <summary>
        /// filesystem for i/o
        /// </summary>
        //public readonly AbstractFileSystem localFileSystem;
        
        /// <summary>
        /// used in main thread to write persistent data
        /// </summary>
        public readonly MemoryFileSystem MemoryFileSystem = new();

        /// <summary>
        /// transaction manager on top of FileStore, required to achieve file/transaction consistency
        /// </summary>
        public FileSystemTransaction Transaction;
        
        public JsonSerializer serializer;

        public AbstractPlayerJsonSerializer(T player)
        {
            this.player = player;
            var rr = new ReferenceResolverManager();
            // rr.AddConverter(new PuzzleAdapterReferenceConverter(player));
            // rr.AddConverter(new ShopArticleReferenceConverter(player));
            // rr.AddConverter(new OrderReferenceConverter(player));
            // rr.AddConverter(new CollectionReferenceConverter(player));
            // rr.AddConverter(new CollectionItemReferenceConverter(player));
            var settings = new JsonSerializerSettings()
            {
                ReferenceResolverProvider = () => rr,
                Converters = new List<JsonConverter>
                {
                    //
                    // common
                    new StringEnumConverter(),
                    new HolderConverter(),
                    new TimeTaskConverter(),
                    // new ResourcesConverter(),
                    // new ResourcesCheckConverter(),
                    // new PlayerResourcesConverter(),
                    // new ViewManagerConverter<PuzzleViewType, PuzzleViewLayer>(),
                    //
                    // AbstractIdEntity
                    // new AbstractIdEntityConverter<ShopArticleInfo>(player.Shop.ArticleInfoSet),
                    // new AbstractIdEntityConverter<ShopBundleInfo>(player.Shop.BundleInfoSet),
                    // new AbstractIdEntityConverter<ColorInfo>(player.ColorInfoSet),
                    // new AbstractIdEntityConverter<ModelInfo>(player.ModelInfoSet, ModelInfo.UNDEFINED),
                    // new AbstractIdEntityConverter<BrushInfo>(player.BrushInfoSet),
                    // new AbstractIdEntityConverter<MaterialInfo>(player.MaterialInfoSet),
                    // new AbstractIdEntityConverter<CharacterInfo>(player.CharacterInfoSet),
                    // new AbstractIdEntityConverter<CollectionInfo>(player.Collections.CollectionInfoSet),
                    // new AbstractIdGenericEntityConverter<TutorStepInfo, TutorStepType>(player.Tutor.TutorStepInfoSet),
                    // new AbstractIdEntityConverter<BackgroundInfo>(player.BackgroundsInfoSet),
                    // new AbstractIdEntityConverter<OrderSectionInfo>(player.Orders.OrderSectionInfoSet),
                    //
                    // decks
                    // new DeckListConverter<ModelInfo>(),
                    // new DeckListConverter<ColorInfo>(),
                    // new DeckListConverter<BrushInfo>(),
                    // new DeckListConverter<CharacterInfo>(),
                    // new DeckTableConverter<MaterialInfo, string>(),
                    // new DeckTableConverter<OrderMechanicsType, OrderMechanicsType>(),
                    //
                    // adapter specific
                    // new ShopArticleListConverter(player.Shop),
                    // new TutorStepListConverter(player.Tutor),
                    // new CollectionListConverter(),
                    // new CollectionItemListConverter(),
                    // new OrderSectionListConverter(player.Orders),
                    // new OrderListConverter(),
                },
                Formatting = Formatting.Indented,
                Error = delegate(object sender, ErrorEventArgs args)
                {
                    Log.Error(args.ErrorContext.Error);
                    args.ErrorContext.Handled = false;
                },
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            DecorateSettings(settings);
            serializer = JsonSerializer.CreateDefault(settings);
            //
            // setup transaction on top of filesystem, check
            // Transaction = new FileSystemTransaction(this.localFileSystem);
            // Transaction.Check();
        }

        protected virtual void DecorateSettings(JsonSerializerSettings settings)
        {
        }

        public string GetFileName(AbstractFeature adapter)
        {
            return adapter.GetType().Name + "." + JsonObjectSerializer.Format;
        }

        public void Load(AbstractFileSystem fileSystem)
        {
            player.Clear();
            // player.SetLoading(true);
            try
            {
                foreach (var e in Adapters)
                {
                    if (!e.IsPersistent) continue;
                    LoadAdapter(fileSystem, e);
                }
            }
            finally
            {
                // player.SetLoading(false);
            }
        }

        void LoadAdapter(AbstractFileSystem fileSystem, AbstractFeature adapter)
        {
            var name = GetFileName(adapter);
            if (Log.IsDebugEnabled) Log.DebugFormat("{0} <- {1}", adapter.GetType().Name, name);
            using var textReader = fileSystem.TextReader(name);
            try
            {
                serializer.Populate(textReader, adapter);
            }
            catch (Exception ex)
            {
                LangHelper.Handle(ex, $"LoadAdapter({adapter.GetType()}) failed");
            }
        }

        public int Save(AbstractFileSystem fileSystem, bool dirty)
        {
            var saved = 0;
            foreach (var adapter in Adapters)
            {
                if (!adapter.IsPersistent || (dirty && !adapter.Dirty)) continue;
                SaveAdapter(fileSystem, adapter);
                if (dirty) adapter.DirtyReset();
                saved++;
            }

            return saved;
        }
        
        public MemoryFileSystem Save(bool dirty)
        {
            lock (MemoryFileSystem)
            {
                MemoryFileSystem.Clear();
                var saved = Save(MemoryFileSystem, dirty);
                //
                // copy memory > file
                // if (saved > 0)
                // {
                //     if (Log.IsDebugEnabled) Log.DebugFormat("Saved {0} adapters", saved);
                //     Task.Factory.StartNew(FlushSaved);
                // }
            }

            return MemoryFileSystem;
        }
        
        public MemoryFileSystem SaveAll()
        {
            return Save(false);
        }

        /// <summary>
        /// flush data written
        /// </summary>
        void FlushSaved()
        {
            lock (MemoryFileSystem)
            {
                Transaction.Begin();
                try
                {
                    if (MemoryFileSystem.IsEmpty) return;
                    MemoryFileSystem.CopyTo(Transaction);
                    MemoryFileSystem.Clear();
                }
                finally
                {
                    Transaction.End();
                }
            }
        }

        void SaveAdapter(AbstractFileSystem fileSystem, AbstractFeature adapter)
        {
            string name = GetFileName(adapter);
            if (Log.IsDebugEnabled) Log.DebugFormat("{0} -> {1}", adapter.GetType().Name, name);
            using (TextWriter textWriter = fileSystem.TextWriter(name))
            {
                try
                {
                    serializer.Serialize(textWriter, adapter);
                }
                catch (Exception ex)
                {
                    LangHelper.Handle(ex, $"SaveAdapter({adapter.GetType()}) failed");
                }
            }
        }

        public void SaveDirty()
        {
            Save(true);
        }
        
        protected void AddInfoConverter<TE>(JsonSerializerSettings settings,
            InfoSetIdString<TE> infoSet) where TE : AbstractEntityIdString
        {
            settings.Converters.Add(new AbstractEntityIdStringConverter<TE>(infoSet));
        }
        
        public Dictionary<string, byte[]> Export()
        {
            return SaveAll().Export();
        }

        public void Import(Dictionary<string, byte[]> state)
        {
            lock (MemoryFileSystem)
            {
                MemoryFileSystem.Clear();
                Load(MemoryFileSystem.Import(state));
            }
        }
    }
}
