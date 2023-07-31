using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using MrBigHead.Shared;
using System.Collections.Generic;
//using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Functions.Worker.Extensions.Tables;

namespace MrBigHead.Func
{
    public class PostPhrases
    {
        private readonly ILogger _logger;

        public PostPhrases(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PostPhrases>();
        }

        //[Function("PostPhrases")]
        //public async Task<HttpResponseData> Run(
        //    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        //    [TableInput("Sayings")] CloudTable cloudTable)
        //{
        //    //_logger.LogInformation("C# HTTP trigger function processed a request.");

        //    //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    //var data = JsonSerializer.Deserialize<List<Saying>>(requestBody);

        //    //await InsertRecordsAsync(cloudTable, data);

        //    var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        //    return response;
        //}

        //private static async Task InsertRecordsAsync(CloudTable cloudTable, List<Saying> data)
        //{
        //    //foreach (var phrase in data)
        //    //{
        //    //    var sayingEntity = new SayingEntity
        //    //    {
        //    //        PartitionKey = phrase.Mood,
        //    //        Phrase = phrase.Phrase,
        //    //        RowKey = Guid.NewGuid().ToString()
        //    //    };

        //    //    Console.WriteLine($"{sayingEntity.Phrase}");

        //    //    var tableOps = TableOperation.Insert(sayingEntity);

        //    //    var tableResult = await cloudTable.ExecuteAsync(tableOps);

        //    //    Console.WriteLine($"tableResult: {tableResult.HttpStatusCode}");
        //    //}
        //}
    }
}
