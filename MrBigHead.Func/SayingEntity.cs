using Azure;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Table;
using Azure.Data.Tables;


namespace MrBigHead.Func
{
    public class SayingEntity : ITableEntity
    {
        public string? Mood { get; set; }
        public string? Phrase { get; set; }
        public string PartitionKey { get => Mood; set => Mood = value; }
        public string? RowKey { get; set; }
        DateTimeOffset? ITableEntity.Timestamp { get; set; }
        ETag ITableEntity.ETag { get; set; }

        public void ReadEntity(IDictionary<string, TableEntity > properties)
        {
            Phrase = properties["Phrase"].ToString();  //.StringValue;
        }

        public IDictionary<string, TableEntity> WriteEntity()
        {
            var results = new Dictionary<string, TableEntity>();

            //results.Add("PartitionKey", EntityProperty.GeneratePropertyForString(PartitionKey));
            //results.Add("Phrase", EntityProperty.GeneratePropertyForString(Phrase));
            //results.Add("RowKey", EntityProperty.GeneratePropertyForString(Guid.NewGuid().ToString()));
            //results.Add("TimeStamp", EntityProperty.GeneratePropertyForDateTimeOffset(DateTime.Now));

            //Console.WriteLine($"WriteEntity: {results["PartitionKey"]}");

            return results;
        }
    }
}
