#r "Microsoft.WindowsAzure.Storage"

using Microsoft.WindowsAzure.Storage.Table;

public class EntryTableEntity : TableEntity
{
    public EntryTableEntity(string name, string phone)
    {
        PartitionKey = name;
        RowKey = phone;
    }

    public EntryTableEntity() { }
    public string FromId { get; set; }
    public string ConversationId { get; set; }
    public string RecipientId { get; set; }
}