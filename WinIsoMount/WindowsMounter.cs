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
            try
            {
                string path = "D:\\test\\test.iso";
                Logger.Debug("**********************WinIsoMount test path:"+ path);
                WindowsMount w = new WindowsMount(this, Logger, path);
                w.Mount();
                string testPath = w.MountedPath;
                w.UnMount();
                Logger.Debug("**********************testMountPath success");
            }
            catch (Exception ex)
            {
                Logger.Info( "**********************testMountPath fail,ex:{0}",ex.Message);
            }
        }

        #endregion

        #region Interface Implementation for IIsoMounter

        public string Name
        {
            get { return "WinIsoMount"; }
        }

        /// <summary>
        /// 是否能挂载此文件
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
                Logger.Debug("is not isoPath:"+path);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 异步执行挂载操作
        /// </summary>
        /// <param name="isoPath"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IIsoMount> Mount(string isoPath, CancellationToken cancellationToken)
        {
            return new Task<IIsoMount>(() => { return DoMountISO(isoPath); });
        }
        #endregion

        #region Interface Implementation for IDisposable

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Internal Methods

  

        #endregion



        #region Private Methods

        /// <summary>
        /// 执行挂载
        /// </summary>
        /// <param name="isoPath"></param>
        /// <param name="mountedISO"></param>
        /// <returns></returns>
        private IIsoMount DoMountISO(string isoPath)
        {
            WindowsMount m = new WindowsMount(this, Logger, isoPath);
            try
            {
                m.Mount();
            }
            catch (Exception ex)
            {
                Logger.Info(
                    "[{0}] Unhandled exception removing mount point, exception is [{1}].",
                    Name,
                    ex.Message
                );
            }
            return m;
        }
        #endregion
    }
}

