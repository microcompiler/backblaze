using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Extensions;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Represents a default implementation of the <see cref="Storage"/> which uses <see cref="ApiClient"/> for making HTTP requests.
    /// </summary>
    public partial class Storage : IStorageDirectories
    {
        /// <summary>
        /// Provides methods to access directory operations.
        /// </summary>
        public IStorageDirectories Directories { get { return this; } }

        #region ApiClient

        #endregion

    }
}
