using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MrBigHead.Shared;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace MrBigHead.Func
{
    public static class PostPhrases
    {
        [FunctionName("PostPhrases")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("Sayings")] CloudTable cloudTable,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<List<Saying>>(requestBody);

            await InsertRecordsAsync(cloudTable, data);

            return new OkResult();
        }

        private static async Task InsertRecordsAsync(CloudTable cloudTable, List<Saying> data)
        {
            foreach (var phrase in data)
            {
                var sayingEntity = new SayingEntity
                {
                    PartitionKey = phrase.Mood,
                    Phrase = phrase.Phrase,
                    RowKey = Guid.NewGuid().ToString()
                };

                Console.WriteLine($"{sayingEntity.Phrase}");

                var tableOps = TableOperation.Insert(sayingEntity);

                await cloudTable.ExecuteAsync(tableOps);
            }
        }
    }
}
