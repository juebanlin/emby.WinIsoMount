using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Diagnostics;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.System;
using System.Collections.Generic;
using MediaBrowser.Controller.MediaEncoding;
using MediaBrowser.Model.Entities;

namespace IsoMounter
{
    public class WindowsMounter : IMediaMounter
    {
        #region Private Fields

        private readonly ILogger Logger;
        private readonly IEnvironmentInfo EnvironmentInfo;
        private readonly IMediaMountManager IsoManager;
        private readonly IFileSystem FileSystem;
        private readonly IProcessFactory ProcessFactory;
        private readonly IMediaEncoder MediaEncoder;

        #endregion

        #region Constructor(s)

        public WindowsMounter(ILogger logger, IEnvironmentInfo environment, IMediaMountManager isoManager,IProcessFactory processFactory, IFileSystem fileSystem , IMediaEncoder mediaEncoder)
        {
            Logger = logger;
            EnvironmentInfo = environment;
            IsoManager = isoManager;
            FileSystem = fileSystem;
            ProcessFactory = processFactory;
            MediaEncoder = mediaEncoder;
            Logger.Debug("**********************WinIsoMount init start",null);
            test();
        }

        #endregion

        
        private void test()
        {
            if (IsoManager != null)
            {
                /**
                 * 
                */
                List<IMediaMounter> list = new List<IMediaMounter>();
                list.Add(this);
                IsoManager.AddParts(list);
                Logger.Debug("isoManager init:", null);
            }
            Logger.Debug("**********************WinIsoMount inited", null);
            try
            {
                string path = "D:\\test\\test.iso";
                Logger.Debug("**********************WinIsoMount test path:" + path, null);
                PfmMount w = new PfmMount(this, MediaEncoder, Logger, path, MediaContainer.DvdIso);
                w.Mount();
                string mountedPath = w.MountedPath;
                if (mountedPath != null)
                {
                    Logger.Debug("**********************testMountPath success:" + mountedPath, null);
                }
                w.UnMount();
            }
            catch (Exception ex)
            {
                Logger.Info("**********************testMountPath fail,ex:{0}", ex.Message, null);
            }
        }
        
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
        public bool CanMount(string path, string container)
        {
            Logger.Debug("entry is canMount isoPath:[{0}],container:[{1}]", path,container);
            if (EnvironmentInfo.OperatingSystem != MediaBrowser.Model.System.OperatingSystem.Windows)
            {
                Logger.Debug("is canMount not windows",null);
                return false;
            }
            bool isIsoPath = string.Equals(Path.GetExtension(path), ".iso", StringComparison.OrdinalIgnoreCase);
            if (!isIsoPath)
            {
                Logger.Debug("is not isoPath:"+path, null);
                return false;
            }
            if (!PfmMount.CheckEnvironment())
            {
                Logger.Error("ERROR: checkEnvironment is false.\n", null);
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
        public Task<IMediaMount> Mount(string isoPath, string container, CancellationToken cancellationToken)
        {
            Logger.Debug("Mount isoPath:[{0}],container:[{1}]", isoPath, container);
            PfmMount m = new PfmMount(this, MediaEncoder, Logger, isoPath, container);
            try
            {
                m.Mount();
                if (m.MountedPath != null)
                {
                    Logger.Debug("Mount success isoPath:[{0}],container:[{1}]", isoPath, container);
                    return Task.FromResult<IMediaMount>(m);
                }
            }
            catch (Exception ex)
            {
                Logger.Info("WindowsMount Unhandled exception removing mount point, exception is [{0}].", ex.Message);
            }
            throw new IOException(String.Format(
                    "An error occurred trying to mount image [$0].",
                    isoPath
                ));
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
   
        #endregion
    }
}

