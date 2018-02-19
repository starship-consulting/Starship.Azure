using Microsoft.WindowsAzure.Storage.Table;
using Starship.Azure.Extensions;
using Starship.Core.Data;

namespace Starship.Azure.Interfaces {
    public interface IsPartitionedDataModel : IsDataModel {
        string GetPartition();
    }

    public static class IsPartitionedDataModelExtensions {
        public static DynamicTableEntity ToTableEntity(this IsPartitionedDataModel model, string partitionPrefix = "") {
            var entity = new DynamicTableEntity {
                PartitionKey = partitionPrefix + model.GetPartition(),
                RowKey = model.GetId()
            };

            entity.Apply(model);
            return entity;
        }
    }
}