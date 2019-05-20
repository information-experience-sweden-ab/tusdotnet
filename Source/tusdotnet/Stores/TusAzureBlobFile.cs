using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using tusdotnet.Interfaces;
using tusdotnet.Models;

namespace tusdotnet.Stores
{
    /// <summary>
    /// Represents a file saved in the TusBlobStore data store
    /// </summary>
    public class TusAzureBlobFile : ITusFile
    {
        readonly string _metadata;
        readonly CloudAppendBlob _appendBlob;

        /// <summary>
        /// A new instance of TusAzureBlobFile
        /// </summary>
        /// <param name="blobId">The file id</param>
        /// <param name="metadata">The upload metadata</param>
        /// <param name="appendBlob">The associated append blob</param>
        internal TusAzureBlobFile(string blobId, string metadata, CloudAppendBlob appendBlob)
        {
            _appendBlob = appendBlob;
            _metadata = metadata;
            Id = blobId;
        }

        /// <Inheritdoc />
        public string Id { get; set; }

        /// <summary>
        /// Returns the blob stream
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Stream> GetContentAsync(CancellationToken cancellationToken)
        {
            return _appendBlob.OpenReadAsync();
        }

        internal Task<bool> Exists()
        {
            return _appendBlob.ExistsAsync();
        }

        /// <inheritdoc />
        public Task<Dictionary<string, Metadata>> GetMetadataAsync(CancellationToken cancellationToken)
        {
            var meta = Metadata.Parse(_metadata);
            return Task.FromResult(meta);
        }

    }
}
