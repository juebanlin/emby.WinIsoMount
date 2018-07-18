using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Diagnostics;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.System;
using System.Collections.Generic;

namespace WinIsoMount
{
    public class WindowsMounter : IIsoMounter
    {
        #region Private Fields

        private readonly ILogger Logger;
        private readonly IEnvironmentInfo EnvironmentInfo;
        private readonly IIsoManager IsoManager;
        private readonly IFileSystem FileSystem;
        private readonly IProcessFactory ProcessFactory;

        #endregion

        #region Constructor(s)

        public WindowsMounter(ILogger logger, IEnvironmentInfo environment, IIsoManager isoManager,IProcessFactory processFactory, IFileSystem fileSystem )
        {
            Logger = logger;
            EnvironmentInfo = environment;
            IsoManager = isoManager;
            FileSystem = fileSystem;
            ProcessFactory = processFactory;
            Logger.Debug("**********************WinIsoMount init start");
            if (isoManager != null)
            {
                /**
                 * 
                */
                List<IIsoMounter> list = new List<IIsoMounter>();
                list.Add(this);
                isoManager.AddParts(list);
                Logger.Debug("isoManager init:");
            }
            Logger.Debug("**********************WinIsoMount inited");
            Logger.Debug(MountUtil.Test());
            Logger.Debug(MountUtil.TestMount());
        }

        #endregion

        #region Interface Implementation for IIsoMounter

        public string Name
        {
            get { return "WinIsoMount"; }
        }

        /// <summary>
        ///  «∑Òƒ‹π“‘ÿ¥ÀŒƒº˛
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool CanMount(string path)
        {
            Logger.Debug("is canMount isoPath:"+ path);
            if (EnvironmentInfo.OperatingSystem != MediaBrowser.Model.System.OperatingSystem.Windows)
            {
                return false;
            }
            bool isIsoPath = string.Equals(Path.GetExtension(path), ".iso", StringComparison.OrdinalIgnoreCase);
            if (!isIsoPath)
            {
                Logger.Debug("is not isoPath:[{1}]", path);
                return false;
            }
            return true;
        }

        /// <summary>
        /// “Ï≤Ω÷¥––π“‘ÿ≤Ÿ◊˜
        /// </summary>
        /// <param name="isoPath"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IIsoMount> Mount(string isoPath, CancellationToken cancellationToken)
        {
            return new Task<IIsoMount>(() => { return doMountISO(isoPath); });
        }
        #endregion

        #region Interface Implementation for IDisposable

        public void Dispose()
        {
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// –∂‘ÿƒ≥∏ˆπ“‘ÿ
        /// </summary>
        /// <param name="mount"></param>
        internal void OnUnmount(WindowsMount mount)
        {
            if (mount != null)
            {
                Logger.Info(
                    "[{0}] Attempting to unmount ISO [{1}] mounted on [{2}].",
                    Name,
                    mount.IsoPath,
                    mount.MountedPath
                );
                try
                {
                    DoUnMountISO(mount.IsoPath);
                }
                catch (Exception ex)
                {
                    Logger.Info(
                        "[{0}] Unhandled exception removing mount point, exception is [{1}].",
                        Name,
                        ex.Message
                    );
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(mount));
            }
        }

        #endregion



        #region Private Methods

        /// <summary>
        /// ÷¥––π“‘ÿ
        /// </summary>
        /// <param name="isoPath"></param>
        /// <param name="mountedISO"></param>
        /// <returns></returns>
        private IIsoMount doMountISO(string isoPath)
        {
            char let = MountUtil.MountDiskImage(isoPath);
            if (!let.Equals("\0"))
            {
                String mountPath = let.ToString() + ":\\";
                IIsoMount m = new WindowsMount(this, isoPath, mountPath);
                return m;
            }
            return null;
        }

        /// <summary>
        /// –∂‘ÿiso
        /// </summary>
        /// <param name="isoPath"></param>
        /// <returns></returns>
        private void DoUnMountISO(string isoPath)
        {
            MountUtil.DismountDiskImage(isoPath);
        }
        #endregion
    }
}

