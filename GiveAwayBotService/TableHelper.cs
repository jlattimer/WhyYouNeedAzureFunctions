using GiveAwayBotService.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace GiveAwayBotService
{
    public static class TableHelper
    {
        public static void AddEntryToTable(string name, string phone, string fromId, string conversationId, string recipientId)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("GiveAwayEntries");

            EntryTableEntity tableEntry = new EntryTableEntity(name, phone)
            {
                FromId = fromId,
                ConversationId = conversationId,
                RecipientId = recipientId
            };

            TableOperation insertOperation = TableOperation.Insert(tableEntry);

            table.Execute(insertOperation);
        }

        public static IEnumerable<EntryTableEntity> GetEntries()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("GiveAwayEntries");

            TableQuery<EntryTableEntity> query = new TableQuery<EntryTableEntity>();

            return table.ExecuteQuery(query);
        }
    }
}