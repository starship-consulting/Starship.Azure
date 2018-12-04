using System;
using Microsoft.WindowsAzure.Storage.Table;
using Starship.Core.Storage;

namespace Starship.Azure.Providers.Storage {
    public class StoredFileReference : TableEntity {

        public StoredFileReference() {
            RowKey = Guid.NewGuid().ToString();
        }

        public StoredFileReference(string partition, string path) : this() {
            PartitionKey = partition;
            Path = path;
        }

        public string Path;

        public bool IsFolder { get; set; }

        public DateTime? ValidUntil { get; set; }
    }
}