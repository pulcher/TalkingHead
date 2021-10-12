using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System;

namespace MrBigHead.Func
{
    public static partial class GetAllPhrases
    {
        public class SayingEntity: ITableEntity
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

                results["PartitionKey"].StringValue = Mood;
                results["Phrase"].StringValue = Phrase;
                results["RowKey"].StringValue = Guid.NewGuid().ToString();
                results["TimeStamp"].DateTime = DateTime.Now;

                return results;
            }
        }

    }
}
