#r "Microsoft.WindowsAzure.Storage"
#load "EntryTableEntity.csx"
#load "BotMessage.csx"

using System;
using System.Net;
using System.Runtime.Remoting.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

public static BotMessage Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        System.Environment.GetEnvironmentVariable("AzureTableStorage", EnvironmentVariableTarget.Process));

    var tableClient = storageAccount.CreateCloudTableClient();
    var table = tableClient.GetTableReference("GiveAwayEntries");

    TableQuery<EntryTableEntity> query = new TableQuery<EntryTableEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.NotEqual, "Windows"));
    var results = table.ExecuteQuery(query);

    var count = results.Count();
    log.Info("Entry Count: " + results.Count().ToString());

    BotMessage message = new BotMessage
    {
        Source = "Azure Function: GiveAwayWinnerPicker",
    };

    if (count == 0)
    {
        message.Message = "Winner is: Nobody";
        log.Info("Winner: Nobody");
        return message;
    }

    var random = new Random();
    var i = random.Next(0, count);
    var winner = (EntryTableEntity)results.ToList()[i];
    log.Info("Winner:" + winner.PartitionKey);

    message.Message = "Winner is: " + winner.PartitionKey + " - " + winner.RowKey;
    return message;
}