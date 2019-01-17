using System;
using MediaBrowser.Controller.MediaEncoding;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.MediaInfo;

namespace IsoMounter
{

    internal class PfmMount : IMediaMount
    {

        #region Private Fields
        private readonly IMediaEncoder mediaEncoder;
        private readonly WindowsMounter isoMounter;
        private readonly ILogger logger;
        private readonly string container;
        private Pfm.Mount mount;
        private Pfm.FileMount fileMount;
        #endregion

        #region Constructor(s)

        internal PfmMount(WindowsMounter isoMounter,IMediaEncoder mediaEncoder,ILogger logger, string isoPath, string container)
        {
            this.isoMounter = isoMounter;
            this.mediaEncoder=mediaEncoder;
            this.logger = logger;
            this.container = container;
            IsoPath = isoPath;
        }

        #endregion

        #region Interface Implementation for IDisposable


        /// <summary>
        ///  Õ∑≈
        /// </summary>
        public void Dispose()
        {
            try
            {
                UnMount();
            }
            catch (Exception ex)
            {
                logger.Info("WindowsMount Unhandled exception removing mount point, exception is [{0}].",ex.Message);
            }
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Interface Implementation for IIsoMount

        public string IsoPath { get; private set; }
        public string MountedPath { get; private set; }
        public string MountedFolderPath { get; private set; }
        public MediaProtocol MountedProtocol { get; set; }

        #endregion

        static internal bool CheckEnvironment()
        {
            /**
             *      public const int instNotInstalled = 3;
                    public const int instOldVersion = 2;
                    public const int instOldBuild = 1;
                    public const int instInstalled = 0;
             */
            if (Pfm.InstallCheck() != Pfm.instInstalled)
            {
                return false;
            }
            return true;
        }

        internal void Mount()
        {
            if(MountedPath!=null)
            {
                return;
            }
            if (!CheckEnvironment())
            {
                logger.Error("ERROR:{0}.\n", "checkEnvironment is false");
                return;
            }
            Pfm.Api api = null;
            Pfm.ApiFactory(out api);
            api.FileMountFactory(out fileMount);
            Pfm.FileMountCreateParams fmp = new Pfm.FileMountCreateParams();
            fmp.mountFileName = IsoPath;
            fmp.fileMountFlags |= Pfm.fileMountFlagConsoleUi;//‘ –Ì÷ÿ∏¥π“‘ÿ
            fmp.fileMountFlags |= Pfm.fileMountFlagMultiMount;//‘ –Ì÷ÿ∏¥π“‘ÿ
            fmp.mountFlags = Pfm.mountFlagReadOnly;//÷ª∂¡
            int error = 0;
            error = fileMount.Start(fmp);
            if (error != Pfm.errorSuccess)
            {
                logger.Error("ERROR: {0} Unable to create mount.\n", error);
                return;
            }
            error = fileMount.WaitReady();
            if (error != Pfm.errorSuccess)
            {
                logger.Error("ERROR: {0} Unable to create mount.\n", error);
                return;
            }
            mount = fileMount.GetMount();
            MountedPath = mount.GetMountPoint();
            //TODO
            var mountFolder = MountedPath;
            MountedProtocol = MediaProtocol.File;
            if (string.Equals(container, MediaContainer.DvdIso, StringComparison.OrdinalIgnoreCase))
            {
                var files = mediaEncoder.GetDvdVobFiles(mountFolder);

                var mountedPath = string.Join("|", files);
            }
            else if (string.Equals(container, MediaContainer.BlurayIso, StringComparison.OrdinalIgnoreCase))
            {
                var files = mediaEncoder.GetBlurayM2tsFiles(mountFolder);

                var mountedPath = string.Join("|", files);
            }
        }

        internal void UnMount()
        {
            if (fileMount != null)
            {
                fileMount.Cancel();
                fileMount.Detach();
                fileMount.Dispose();
            }
        }
    }
}

