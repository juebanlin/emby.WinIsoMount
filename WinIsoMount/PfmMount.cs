using System;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;

namespace WinIsoMount
{

    internal class PfmMount : IIsoMount
    {

        #region Private Fields
        private readonly WindowsMounter IsoMounter;
        private readonly ILogger Logger;
        private Pfm.Mount mount;
        private Pfm.FileMount fileMount;
        #endregion

        #region Constructor(s)

        internal PfmMount(WindowsMounter isoMounter,ILogger logger, string isoPath)
        {
            IsoMounter = isoMounter;
            IsoPath = isoPath;
            Logger = logger;
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
                Logger.Info("WindowsMount Unhandled exception removing mount point, exception is [{0}].",ex.Message);
            }
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Interface Implementation for IIsoMount

        public string IsoPath { get; private set; }
        public string MountedPath { get; private set; }

        #endregion

        internal void Mount()
        {
            if(MountedPath!=null)
            {
                return;
            }
            if (Pfm.InstallCheck() != Pfm.instInstalled)
            {
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
                Logger.Error("ERROR: {0} Unable to create mount.\n", error);
                return;
            }
            error = fileMount.WaitReady();
            if (error != Pfm.errorSuccess)
            {
                Logger.Error("ERROR: {0} Unable to create mount.\n", error);
                return;
            }
            mount = fileMount.GetMount();
            MountedPath = mount.GetMountPoint();
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

