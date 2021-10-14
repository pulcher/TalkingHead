using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System;

namespace MrBigHead.Func
{
    public class SayingEntity : ITableEntity
    {
        public string Mood { get; set; }
        public string Phrase { get; set; }
        public string PartitionKey { get => Mood; set => Mood = value; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            Phrase = properties["Phrase"].StringValue;
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = new Dictionary<string, EntityProperty>();

            //This don't work...  for now obvious reasons....
            //results["PartitionKey"].StringValue = PartitionKey;
            //results["Phrase"].StringValue = Phrase;
            //results["RowKey"].StringValue = Guid.NewGuid().ToString();
            //results["TimeStamp"].DateTime = DateTime.Now;

            results.Add("PartitionKey", EntityProperty.GeneratePropertyForString(PartitionKey));
            results.Add("Phrase", EntityProperty.GeneratePropertyForString(Phrase));
            results.Add("RowKey", EntityProperty.GeneratePropertyForString(Guid.NewGuid().ToString()));
            results.Add("TimeStamp", EntityProperty.GeneratePropertyForDateTimeOffset(DateTime.Now));

            Console.WriteLine($"WriteEntity: {results["PartitionKey"]}");

            return results;
        }
    }

}
