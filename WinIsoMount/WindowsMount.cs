using System;
using MediaBrowser.Model.IO;

namespace WinIsoMount
{
    internal class WindowsMount : IIsoMount
    {

        #region Private Fields

        private readonly WindowsMounter isoMounter;

        #endregion

        #region Constructor(s)

        internal WindowsMount(WindowsMounter isoMounter, string isoPath, string mountFolder)
        {
            this.isoMounter = isoMounter;
            IsoPath = isoPath;
            MountedPath = mountFolder;
        }

        #endregion

        #region Interface Implementation for IDisposable

        // Flag: Has Dispose already been called?
        private bool disposed = false;

        /// <summary>
        ///  Õ∑≈
        /// </summary>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) {
                return;
            }
            if (disposing) {
                //
                // Free managed objects here.
                //
                isoMounter.OnUnmount(this);
            }
            //
            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        #endregion

        #region Interface Implementation for IIsoMount

        public string IsoPath { get; private set; }
        public string MountedPath { get; private set; }

        #endregion

    }
}

