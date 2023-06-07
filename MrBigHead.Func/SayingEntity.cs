using Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Adt = Azure.Data.Tables;

namespace MrBigHead.Func
{
    public class SayingEntity : Adt.ITableEntity
    {
        public string? Mood { get; set; }
        public string? Phrase { get; set; }
        public string PartitionKey { get => Mood; set => Mood = value; }
        public string? RowKey { get; set; }
        DateTimeOffset? Adt.ITableEntity.Timestamp { get; set; }
        ETag Adt.ITableEntity.ETag { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            Phrase = properties["Phrase"].StringValue;
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = new Dictionary<string, EntityProperty>();

            results.Add("PartitionKey", EntityProperty.GeneratePropertyForString(PartitionKey));
            results.Add("Phrase", EntityProperty.GeneratePropertyForString(Phrase));
            results.Add("RowKey", EntityProperty.GeneratePropertyForString(Guid.NewGuid().ToString()));
            results.Add("TimeStamp", EntityProperty.GeneratePropertyForDateTimeOffset(DateTime.Now));

            Console.WriteLine($"WriteEntity: {results["PartitionKey"]}");

            return results;
        }
    }
}
