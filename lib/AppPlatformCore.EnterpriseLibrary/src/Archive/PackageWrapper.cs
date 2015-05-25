using System;
using System.IO;
using System.IO.Packaging;

namespace AppPlatform.Core.EnterpriseLibrary.Archive
{
    /// <summary>
    /// Represents a container that can store multiple data objects
    /// </summary>
    public class PackageWrapper : IDisposable
    {
        #region Constructor

        private PackageWrapper(string path, FileMode packageMode, FileAccess packageAccess)
        {
            this.CheckPath(path);
            this.package = Package.Open(path, packageMode, packageAccess);
        }

        #endregion

        #region Private members

        private Package package;
        private bool closed;

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Close();
        }

        #endregion

        #region Public methods

        public static PackageWrapper Open(string path, FileMode packageMode, FileAccess packageAccess)
        {
            return new PackageWrapper(path, packageMode, packageAccess);
        }

        public void AddContentToZipPart(string zipPartUri, string contentType, byte[] content)
        {
            var partUri = new Uri(zipPartUri, UriKind.Relative);
            var newFilePackagePart = this.package.CreatePart(partUri, contentType, CompressionOption.Maximum);
            newFilePackagePart.GetStream().Write(content, 0, content.Length);
            this.package.Flush();
        }

        public void Close()
        {
            if (this.closed)
            {
                return;
            }

            this.package.Close();
            this.closed = true;
        }

        #endregion

        #region Private members

        private void CheckPath(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        #endregion
    }
}