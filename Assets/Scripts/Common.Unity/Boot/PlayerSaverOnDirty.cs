using System.Threading.Tasks;
using Common.Bind;
using Common.IO.FileSystem;
using Common.Player;
using Common.Unity.Util;

namespace Common.Unity.Boot
{
    /// <summary>
    /// listens to feature dirty and schedules save on next update (to prevent multiple saves in one frame)
    /// use memory FS to write data in main thread, then flush data in background thread on disk
    /// </summary>
    public class PlayerSaverOnDirty<T> : BindableBean<AbstractPlayerJsonSerializer<T>> where T : AbstractPlayer
    {
        /// <summary>
        /// used in main thread to write persistent data
        /// </summary>
        private readonly MemoryFileSystem memoryFileSystem = new MemoryFileSystem();

        /// <summary>
        /// local private file system, we write here in background thread using transaction
        /// </summary>
        private readonly FileSystemTransaction localFileSystem = new FileSystemTransaction(UnityHelper.PrivateFileSystem);

        private bool saveScheduled;
        
        protected override void OnBind()
        {
            Model.Player.OnDirty += OnDirty;
        }

        protected override void OnUnbind()
        {
            Model.Player.OnDirty -= OnDirty;
        }

        private void OnDirty(AbstractFeature feature)
        {
            if(saveScheduled) return;
            Unicom.RunNextTime(SaveDirty);
            saveScheduled = true;
        }

        private void SaveDirty()
        {
            lock (memoryFileSystem)
            {
                Model.SaveDirty(memoryFileSystem);
                saveScheduled = false;
            }
            //
            // copy memory > file async
            Task.Factory.StartNew(FlushSaved);
        }
        
        /// <summary>
        /// flush data written
        /// </summary>
        void FlushSaved()
        {
            lock (memoryFileSystem)
            {
                localFileSystem.Begin();
                try
                {
                    if (memoryFileSystem.IsEmpty) return;
                    memoryFileSystem.CopyTo(localFileSystem);
                    memoryFileSystem.Clear();
                }
                finally
                {
                    localFileSystem.End();
                }
            }
        }
    }
}
