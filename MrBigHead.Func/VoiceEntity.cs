using Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Adt = Azure.Data.Tables;

namespace MrBigHead.Func
{
    public class VoiceEntity : Adt.ITableEntity
    {
        public string? Category { get; set; }
        public string? Name { get; set; }
        public string? Language { get; set; }
        public string PartitionKey { get => Category; set => Category = value; }
        public string? RowKey { get; set; }
        DateTimeOffset? Adt.ITableEntity.Timestamp { get; set; }
        ETag Adt.ITableEntity.ETag { get; set; }
        public bool IsDefault { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            Name = properties["Name"].StringValue;
            Language = properties["Language"].StringValue;

            properties.TryGetValue("IsDefault", out var value);
            IsDefault = (bool)value.BooleanValue;
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = new Dictionary<string, EntityProperty>();

            results.Add(nameof(PartitionKey), EntityProperty.GeneratePropertyForString(PartitionKey));
            results.Add(nameof(Name), EntityProperty.GeneratePropertyForString(Name));
            results.Add(nameof(Language), EntityProperty.GeneratePropertyForString(Language));
            results.Add(nameof(IsDefault), EntityProperty.GeneratePropertyForBool(IsDefault));
            results.Add(nameof(RowKey), EntityProperty.GeneratePropertyForString(Guid.NewGuid().ToString()));
            results.Add("Timestamp", EntityProperty.GeneratePropertyForDateTimeOffset(DateTime.Now));

            Console.WriteLine($"WriteEntity: {results[nameof(PartitionKey)]}");

            return results;
        }
    }

}
