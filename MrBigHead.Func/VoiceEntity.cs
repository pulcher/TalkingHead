using Azure;
using Azure.Data.Tables;

namespace MrBigHead.Func
{
    public class VoiceEntity : ITableEntity
    {
        public string? Category { get; set; }
        public string? Name { get; set; }
        public string? Language { get; set; }
        public string PartitionKey { get => Category; set => Category = value; }
        public string? RowKey { get; set; }
        DateTimeOffset? ITableEntity.Timestamp { get; set; }
        ETag ITableEntity.ETag { get; set; }
        public bool IsDefault { get; set; }

        public void ReadEntity(TableEntity properties)
        {
            Name = properties["Name"].ToString();
            Language = properties["Language"].ToString();

            properties.TryGetValue("IsDefault", out var value);
            IsDefault = (bool)properties["IsDefault"];
        }

        public IDictionary<string, TableEntity> WriteEntity()
        {
            var results = new Dictionary<string, TableEntity>();

            //results.Add(nameof(PartitionKey), EntityProperty.GeneratePropertyForString(PartitionKey));
            //results.Add(nameof(Name), EntityProperty.GeneratePropertyForString(Name));
            //results.Add(nameof(Language), EntityProperty.GeneratePropertyForString(Language));
            //results.Add(nameof(IsDefault), EntityProperty.GeneratePropertyForBool(IsDefault));
            //results.Add(nameof(RowKey), EntityProperty.GeneratePropertyForString(Guid.NewGuid().ToString()));
            //results.Add("Timestamp", EntityProperty.GeneratePropertyForDateTimeOffset(DateTime.Now));

            //Console.WriteLine($"WriteEntity: {results[nameof(PartitionKey)]}");

            return results;
        }
    }

}
