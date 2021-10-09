using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

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
            public System.DateTimeOffset Timestamp { get; set; }
            public string ETag { get; set; }

            public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
                Phrase = properties["Phrase"].StringValue;
            }

            public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
            {
                throw new System.NotImplementedException();
                return new Dictionary<string, EntityProperty>();
            }
        }

    }
}
