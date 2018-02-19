using System;

namespace Starship.Azure.Providers.Tables {
    public interface IPartitionedEntity {
        string PartitionKey { get; set; }

        string RowKey { get; set; }

        DateTime Timestamp { get; set; }
    }
}