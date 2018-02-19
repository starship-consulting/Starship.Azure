using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Starship.Azure.ErrorHandling {
    public class BatchOperationException : Exception {
        public BatchOperationException(StorageException innerException, TableBatchOperation operation)
            : base("Batch operation exception.", innerException) {
            Operation = operation;
        }

        public int ItemIndex {
            get {
                var storageException = InnerException as StorageException;
                return int.Parse(storageException.Message.Split(' ').Last());
            }
        }

        public object Item {
            get { return Operation[ItemIndex]; }
        }

        public TableBatchOperation Operation { get; set; }
    }
}